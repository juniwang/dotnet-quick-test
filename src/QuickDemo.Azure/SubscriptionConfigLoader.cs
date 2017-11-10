using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuickDemo.Azure
{
    /// <summary>
    /// Load subscriptions together with service principals
    /// 
    /// Put a json file under directory `Sub`. The format looks like:
    /// 
    /// {
    ///     "name": "sub 1",
    ///     "Description": "sub 1",
    ///     "SubscriptionId": "11111111-1111-1111-1111-111111111111",
    ///     "CloudEnvironment": "Azure",
    ///     "ClientId": "11111111-1111-1111-1111-111111111111",
    ///     "ClientSecret": "Dummy Secret",
    ///     "TenantId": "11111111-1111-1111-1111-111111111111"
    /// }
    /// </summary>
    public class SubscriptionConfigLoader
    {
        private static string ConfigDir()
        {
            return Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Sub");
        }

        public static SubscriptionConfig LoadFromFile()
        {
            var dir = new DirectoryInfo(ConfigDir());
            if (!dir.Exists)
            {
                throw new Exception("cannot load subscription config for config dir doesn't exist.");
            }

            var files = dir.GetFiles("*.json");
            foreach (var file in dir.GetFiles("*.json"))
            {
                using (StreamReader reader = new StreamReader(file.OpenRead()))
                {
                    JsonSerializer serializer = new JsonSerializer();
                    try
                    {
                        return (SubscriptionConfig)serializer.Deserialize(reader, typeof(SubscriptionConfig));
                    }
                    catch { }
                }
            }

            throw new Exception("Cannot load subscription config since neither config is valid.");
        }
    }
}
