using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FarmEstimatorMVC
{
    public class LanguageDetection
    {
        static LanguageSetting DetectLanguage(string data)
        {
            LanguageSetting setting = new LanguageSetting();
            if(data.Contains("last") && 
               data.Contains("Tilbagevenden")  &&
               data.Contains("Ankomst")){

            } 
            return setting;
        }

    }
}
