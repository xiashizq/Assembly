using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Blamite.Util.DictionaryUntil
{
    internal class DictionaryDict
    {
        // 语言字典实例
        private static Dictionary<string, DictionaryDictBase> languageDictionaries = new Dictionary<string, DictionaryDictBase>
        {
            { "zh", new DictionaryDictZH() },
            { "ja", new DictionaryDictJP() }
        };

        // 初始化所有字典
        static DictionaryDict()
        {
            foreach (var dict in languageDictionaries.Values)
            {
                dict.InitializeDictionary();
            }
        }

        // 从配置文件读取设置
        private static string GetLocalLanguageSetting()
        {
            try
            {
                string appDataPath = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
                string configFilePath = Path.Combine(appDataPath, "Assembly", "config.ini");
                if (File.Exists(configFilePath))
                {
                    var lines = File.ReadAllLines(configFilePath);
                    foreach (var line in lines)
                    {
                        var parts = line.Split(new[] { '=' }, 2);
                        if (parts.Length == 2 && parts[0].Trim().Equals("LocalLanguage", StringComparison.OrdinalIgnoreCase))
                        {
                            return parts[1].Trim();
                        }
                    }
                }
            }
            catch { }
            return "zh";
        }

        // 根据语言获取翻译
        public static string GetTranslation(string englishTerm, string language = null)
        {
            if (string.IsNullOrEmpty(language))
            {
                language = GetLocalLanguageSetting();
            }
            
            if (languageDictionaries.TryGetValue(language, out var dictionary))
            {
                string translation = dictionary.GetTranslation(englishTerm);
                if (translation != "Translation not found.")
                {
                    return translation;
                }
            }
            return "Translation not found.";
        }

        // 兼容旧方法
        public static string GetChineseTranslation(string englishTerm)
        {
            return GetTranslation(englishTerm, "zh");
        }
    }
}