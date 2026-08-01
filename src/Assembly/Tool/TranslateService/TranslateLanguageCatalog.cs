using System.Collections.Generic;

namespace Assembly.Tool.TranslateService
{
	internal sealed class TranslateLanguageOption
	{
		public string Code { get; set; }
		public string DisplayName { get; set; }
	}

	internal static class TranslateLanguageCatalog
	{
		/// <summary>Online translation target languages (ISO-ish UI codes).</summary>
		public static IReadOnlyList<TranslateLanguageOption> TargetLanguages { get; } = new[]
		{
			new TranslateLanguageOption { Code = "zh", DisplayName = "中文简体 (zh)" },
			new TranslateLanguageOption { Code = "zh-TW", DisplayName = "中文繁體 (zh-TW)" },
			new TranslateLanguageOption { Code = "en", DisplayName = "English (en)" },
			new TranslateLanguageOption { Code = "ja", DisplayName = "日本語 (ja)" },
			new TranslateLanguageOption { Code = "ko", DisplayName = "한국어 (ko)" },
			new TranslateLanguageOption { Code = "fr", DisplayName = "Français (fr)" },
			new TranslateLanguageOption { Code = "de", DisplayName = "Deutsch (de)" },
			new TranslateLanguageOption { Code = "es", DisplayName = "Español (es)" },
			new TranslateLanguageOption { Code = "pt", DisplayName = "Português (pt)" },
			new TranslateLanguageOption { Code = "pt-BR", DisplayName = "Português Brasil (pt-BR)" },
			new TranslateLanguageOption { Code = "it", DisplayName = "Italiano (it)" },
			new TranslateLanguageOption { Code = "ru", DisplayName = "Русский (ru)" },
			new TranslateLanguageOption { Code = "uk", DisplayName = "Українська (uk)" },
			new TranslateLanguageOption { Code = "ar", DisplayName = "العربية (ar)" },
			new TranslateLanguageOption { Code = "th", DisplayName = "ไทย (th)" },
			new TranslateLanguageOption { Code = "vi", DisplayName = "Tiếng Việt (vi)" },
			new TranslateLanguageOption { Code = "id", DisplayName = "Bahasa Indonesia (id)" },
			new TranslateLanguageOption { Code = "ms", DisplayName = "Bahasa Melayu (ms)" },
			new TranslateLanguageOption { Code = "hi", DisplayName = "हिन्दी (hi)" },
			new TranslateLanguageOption { Code = "tr", DisplayName = "Türkçe (tr)" },
			new TranslateLanguageOption { Code = "pl", DisplayName = "Polski (pl)" },
			new TranslateLanguageOption { Code = "nl", DisplayName = "Nederlands (nl)" },
			new TranslateLanguageOption { Code = "sv", DisplayName = "Svenska (sv)" },
			new TranslateLanguageOption { Code = "da", DisplayName = "Dansk (da)" },
			new TranslateLanguageOption { Code = "nb", DisplayName = "Norsk (nb)" },
			new TranslateLanguageOption { Code = "fi", DisplayName = "Suomi (fi)" },
			new TranslateLanguageOption { Code = "cs", DisplayName = "Čeština (cs)" },
			new TranslateLanguageOption { Code = "sk", DisplayName = "Slovenčina (sk)" },
			new TranslateLanguageOption { Code = "ro", DisplayName = "Română (ro)" },
			new TranslateLanguageOption { Code = "hu", DisplayName = "Magyar (hu)" },
			new TranslateLanguageOption { Code = "el", DisplayName = "Ελληνικά (el)" },
			new TranslateLanguageOption { Code = "bg", DisplayName = "Български (bg)" },
			new TranslateLanguageOption { Code = "hr", DisplayName = "Hrvatski (hr)" },
			new TranslateLanguageOption { Code = "sl", DisplayName = "Slovenščina (sl)" },
			new TranslateLanguageOption { Code = "lt", DisplayName = "Lietuvių (lt)" },
			new TranslateLanguageOption { Code = "lv", DisplayName = "Latviešu (lv)" },
			new TranslateLanguageOption { Code = "et", DisplayName = "Eesti (et)" },
			new TranslateLanguageOption { Code = "he", DisplayName = "עברית (he)" },
			new TranslateLanguageOption { Code = "fa", DisplayName = "فارسی (fa)" },
			new TranslateLanguageOption { Code = "tl", DisplayName = "Filipino (tl)" },
		};

		/// <summary>
		/// Offline dictionary languages. Codes without a built-in dictionary keep English terms.
		/// </summary>
		public static IReadOnlyList<TranslateLanguageOption> LocalLanguages { get; } = new[]
		{
			new TranslateLanguageOption { Code = "zh", DisplayName = "中文简体 (zh) — 内置词典" },
			new TranslateLanguageOption { Code = "zh-TW", DisplayName = "中文繁體 (zh-TW)" },
			new TranslateLanguageOption { Code = "en", DisplayName = "English (en) — 原文" },
			new TranslateLanguageOption { Code = "ja", DisplayName = "日本語 (ja) — 内置词典" },
			new TranslateLanguageOption { Code = "ko", DisplayName = "한국어 (ko)" },
			new TranslateLanguageOption { Code = "fr", DisplayName = "Français (fr)" },
			new TranslateLanguageOption { Code = "de", DisplayName = "Deutsch (de)" },
			new TranslateLanguageOption { Code = "es", DisplayName = "Español (es)" },
			new TranslateLanguageOption { Code = "pt", DisplayName = "Português (pt)" },
			new TranslateLanguageOption { Code = "it", DisplayName = "Italiano (it)" },
			new TranslateLanguageOption { Code = "ru", DisplayName = "Русский (ru)" },
			new TranslateLanguageOption { Code = "ar", DisplayName = "العربية (ar)" },
			new TranslateLanguageOption { Code = "th", DisplayName = "ไทย (th)" },
			new TranslateLanguageOption { Code = "vi", DisplayName = "Tiếng Việt (vi)" },
			new TranslateLanguageOption { Code = "id", DisplayName = "Bahasa Indonesia (id)" },
			new TranslateLanguageOption { Code = "hi", DisplayName = "हिन्दी (hi)" },
			new TranslateLanguageOption { Code = "tr", DisplayName = "Türkçe (tr)" },
			new TranslateLanguageOption { Code = "pl", DisplayName = "Polski (pl)" },
			new TranslateLanguageOption { Code = "nl", DisplayName = "Nederlands (nl)" },
		};
	}
}
