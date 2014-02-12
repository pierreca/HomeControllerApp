using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.IO.IsolatedStorage;
using System.Runtime.CompilerServices;

namespace HomeController
{
    public class SettingsHelper
    {
        private const string DefaultServerIP = "192.168.1.111";
        private const string DefaultPandoraUser = "pierreca@live.com";

        public static string ServerIP
        {
            get 
            { 
                var result = SettingsHelper.GetSettingsValue();

                if (string.IsNullOrWhiteSpace(result))
                    return SettingsHelper.DefaultServerIP;
                else
                    return result;
            }

            set 
            { 
                SettingsHelper.SetSettingsValue(value); 
            }
        }

        public static string PandoraUser
        {
            get
            {
                var result = SettingsHelper.GetSettingsValue();

                if (string.IsNullOrWhiteSpace(result))
                    return SettingsHelper.DefaultPandoraUser;
                else
                    return result;
            }
            
            set { SettingsHelper.SetSettingsValue(value); }
        }

        private static string GetSettingsValue([CallerMemberName] string key = "")
        {
            var results = from r in IsolatedStorageSettings.ApplicationSettings
                          where r.Key == key
                          select r;

            if (results.Count() != 1)
                return string.Empty;
            else
                return results.First().Value as string;        
        }

        private static void SetSettingsValue(string value, [CallerMemberName] string key = "")
        {
            if (IsolatedStorageSettings.ApplicationSettings.Contains(key))
                IsolatedStorageSettings.ApplicationSettings[key] = value;
            else
                IsolatedStorageSettings.ApplicationSettings.Add(key, value);
        }
    }
}
