using System;
using System.Collections.Generic;
using System.IO;

namespace Blamite.Util.DictionaryUntil
{
	public class DictionaryDict
	{
		private const string NotFound = "Translation not found.";

		// 语言字典实例
		private static readonly Dictionary<string, DictionaryDictBase> languageDictionaries =
			new Dictionary<string, DictionaryDictBase>(StringComparer.OrdinalIgnoreCase)
			{
				{ "zh", new DictionaryDictZH() },
				{ "ja", new DictionaryDictJP() }
			};

		static DictionaryDict()
		{
			foreach (var dict in languageDictionaries.Values)
				dict.InitializeDictionary();
		}

		public static IEnumerable<string> GetAvailableDictionaryLanguages()
		{
			return languageDictionaries.Keys;
		}

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
							return parts[1].Trim();
					}
				}
			}
			catch { }
			return "zh";
		}

		private static string NormalizeLanguage(string language)
		{
			string code = (language ?? "zh").Trim().ToLowerInvariant().Replace('_', '-');
			if (code == "zh-cn" || code == "zh-hans" || code == "cn" || code == "zh-tw" || code == "zh-hant" || code == "cht")
				return "zh";
			if (code == "jp")
				return "ja";
			return code;
		}

		/// <summary>
		/// Plugin-field style lookup. Returns "Translation not found." when missing.
		/// </summary>
		public static string GetTranslation(string englishTerm, string language = null)
		{
			if (string.IsNullOrEmpty(englishTerm))
				return string.Empty;

			if (string.IsNullOrEmpty(language))
				language = GetLocalLanguageSetting();

			string normalized = NormalizeLanguage(language);

			if (normalized == "en" || !languageDictionaries.ContainsKey(normalized))
				return englishTerm;

			if (languageDictionaries.TryGetValue(normalized, out var dictionary))
			{
				string translation = dictionary.GetTranslation(englishTerm);
				if (translation != NotFound)
					return translation;
			}

			return NotFound;
		}

		/// <summary>
		/// UI localization lookup. Falls back to the English key when not in dictionary.
		/// </summary>
		public static string GetUiTranslation(string englishTerm, string language = null)
		{
			if (string.IsNullOrEmpty(englishTerm))
				return string.Empty;

			string translation = GetTranslation(englishTerm, language);
			if (string.IsNullOrEmpty(translation) || translation == NotFound)
				return englishTerm;
			return translation;
		}

		public static string GetChineseTranslation(string englishTerm)
		{
			return GetTranslation(englishTerm, "zh");
		}
	}
}
