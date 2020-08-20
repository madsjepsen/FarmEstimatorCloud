using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using FarmEstimatorMVC.Models;
using System.Globalization;
using System.Text;

namespace FarmEstimatorMVC.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        public IActionResult Index()
        {
            var model = new FarmEstimatorModel();
            return View(model);

        }

        [HttpPost]
        public IActionResult Index(FarmEstimatorModel model)
        {
            return View(model);
        }

        [HttpPost]
        public ActionResult Calculate(FarmEstimatorModel model)
        {
            if (ModelState.IsValid)
            {
                //split the model into line
                string[] lines = model.Data.Split("\n");
                int count = 0;
                double total = 0;
                int last_arrival = 0;
                foreach (string line_unknow in lines)
                {
                    // Use a tab to indent each line of the file.
                    var utf8 = Encoding.UTF8;
                    var ascil = Encoding.ASCII;
                    byte[] utfBytes = ascil.GetBytes(line_unknow);
                    var line = utf8.GetString(utfBytes, 0, utfBytes.Length);
                    if (line.Contains("last"))
                    {
                        line = line.Replace('?',' ');
                        var no_last = line.Substring(5);
                        int index = no_last.IndexOf("/");
                        var cleared_string = no_last.Remove(index).Trim();
                        int parsed_string = -1;
                        if (int.TryParse(cleared_string, out parsed_string))
                        {
                            Console.WriteLine("\t" + cleared_string);
                            total += parsed_string;
                            ++count;
                        }
                        else
                        {
                            Console.WriteLine("Failed to parse " + cleared_string);

                        }
                    }
                    if (line.Contains("Ankomst"))
                    {
                        var no_ankomst = line.Substring(11);
                        Console.WriteLine("\t" + no_ankomst);
                        List<int> time_list = new List<int>();
                        for (int i = 0; i < 3; ++i)
                        {
                            int index = no_ankomst.IndexOf(":");
                            if (index > 2)
                            {
                                index = 2;
                            }
                            var cleared_string = no_ankomst.Remove(index);
                            int parsed_string = -1;
                            if (int.TryParse(cleared_string, out parsed_string))
                            {
                                time_list.Add(parsed_string);
                            }
                            no_ankomst = no_ankomst.Substring(index + 1);
                        }
                        if (time_list.Count == 3)
                        {
                            int arrival_in_secs = time_list[0] * 3600 + time_list[1] * 60 + time_list[2];
                            Console.WriteLine("Arrival in" + arrival_in_secs);
                            last_arrival = arrival_in_secs;
                        }
                    }
                }
                double per_sec_farming = total / (double)last_arrival;
                double per_hour_farming = per_sec_farming * 3600;
                model.PerHourFarming = per_hour_farming / 4.0;

            }

            // If we got this far, something failed, redisplay form
            return View("Index",model);
        }
        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
