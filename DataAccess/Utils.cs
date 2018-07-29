using System;
using System.Configuration;
using System.Text.RegularExpressions;

namespace DataAccess
{
    public class Utils
    {        
        /// <summary>
        /// Perform regular expression replacement
        /// </summary>
        /// <param name="pattern">Regular expression describing what is to be replaced</param>
        /// <param name="replacement"></param>
        /// <param name="input">String on which replacement is performed</param>
        /// <returns></returns>
        public static string Replace(string pattern, string replacement, string input)
        {
            var regex = new Regex(pattern);
            return regex.Replace(input, replacement);
        }

        /// <summary>
        /// Get and Set values to LinkTekTest.exe.config
        /// </summary>
        public class AppSettings
        {
            private readonly Configuration _configuration;

            public AppSettings()
            {
                _configuration = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
            }

            /// <summary>
            /// Get value from LinkTekTest.exe.config
            /// </summary>
            /// <param name="key">Key that identifies the value</param>
            /// <returns>Value</returns>
            public string GetSetting(string key)
            {
                var setting = _configuration.AppSettings.Settings[key];

                if (setting == null)
                    throw new ArgumentException($"The key '{key}' does not exist in the config file.");

                return setting.Value;
            }

            /// <summary>
            /// Update key-value pair that exists in LinkTekTest.exe.config
            /// </summary>
            /// <param name="key">Key that currently identifies a value</param>
            /// <param name="value">New value</param>
            public void UpdateSetting(string key, string value)
            {
                var settings = _configuration.AppSettings.Settings;

                if (settings[key] == null)
                    throw new ArgumentException($"The key \"{key}\" is not associated with any setting.");

                settings[key].Value = value;
                Save();
            }

            /// <summary>
            /// Add key-value pair to LinkTekTest.exe.config
            /// </summary>
            /// <param name="key"></param>
            /// <param name="value"></param>
            public void AddSetting(string key, string value)
            {
                var settings = _configuration.AppSettings.Settings;

                if (settings[key] != null)
                {
                    UpdateSetting(key, value);
                    return;
                    //throw new ArgumentException($"The key \"{key}\" is already associated with a setting.");
                }

                settings.Add(key, value);
                Save();
            }

            /// <summary>
            /// Save LinkTekTest.exe.config
            /// </summary>
            private void Save()
            {
                _configuration.Save(ConfigurationSaveMode.Modified);
                ConfigurationManager.RefreshSection(_configuration.AppSettings.SectionInformation.Name);
            }
        }
    }
}
