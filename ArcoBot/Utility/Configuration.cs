using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;

namespace ArcoBot
{
    public class Configuration
    { 

        public Configuration()
        {
           
        }
        public static void Set(string key, string value)
        {
            try
            {
                var configFile = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
                var settings = configFile.AppSettings.Settings;
                if (settings[key] == null)
                {
                    settings.Add(key, value);
                }
                else
                {
                    settings[key].Value = value;
                }
                configFile.Save(ConfigurationSaveMode.Modified);
                ConfigurationManager.RefreshSection(configFile.AppSettings.SectionInformation.Name);
            }
            catch (ConfigurationErrorsException)
            {
                Console.WriteLine("Error writing app settings");
            }
        }
        public static string Get(string key)
        {
            if (ConfigurationManager.AppSettings != null)
            {
                if (ConfigurationManager.AppSettings.AllKeys.Contains(key))
                {
                   return ConfigurationManager.AppSettings.Get(key);//Check if value is null or empty
                }
            }
            return string.Empty;
        }
    }
}
