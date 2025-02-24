using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Threading.Tasks;

namespace Assembly.Tool
{
    internal class ConfigManager
    {
        private static Dictionary<string, string> LoadConfig(string configPath)
        {
            var config = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
            if (File.Exists(configPath))
            {
                var lines = File.ReadAllLines(configPath);
                foreach (var line in lines)
                {
                    var parts = line.Split(new[] { '=' }, 2);
                    if (parts.Length == 2)
                    {
                        config[parts[0].Trim()] = parts[1].Trim();
                    }
                }
            }
            return config;
        }

        public static string GetSetting(string appName, string key, string defaultValue = "")
        {
            string appDataPath = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
            string configFilePath = Path.Combine(appDataPath, appName, "config.ini");
            var config = LoadConfig(configFilePath);
            return config.TryGetValue(key, out var value) ? value : defaultValue;
        }

        public static void SetSetting(string appName, string key, string value)
        {
            string appDataPath = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
            string configFilePath = Path.Combine(appDataPath, appName, "config.ini");
            var config = LoadConfig(configFilePath);
            // 更新或添加键值对
            config[key] = value;
            // 将配置写回到文件
            using (var sw = new StreamWriter(configFilePath))
            {
                foreach (var kvp in config)
                {
                    sw.WriteLine($"{kvp.Key}={kvp.Value}");
                }
            }
        }
    }
}
