using System;

namespace Assembly.Tool.TranslateService
{
	internal static class TranslateLanguageMapper
	{
		public static string Map(string providerId, string uiCode)
		{
			string code = Normalize(uiCode);
			switch ((providerId ?? string.Empty).Trim())
			{
				case "Baidu":
					return MapBaidu(code);
				case "Youdao":
					return MapYoudao(code);
				case "Google":
				case "Papago":
				case "MyMemory":
				case "LibreTranslate":
				case "ModernMT":
					return MapGoogleLike(code);
				case "Microsoft":
					return MapMicrosoft(code);
				case "DeepL":
					return MapDeepL(code);
				case "Tencent":
				case "Aliyun":
				case "Caiyun":
				case "Yandex":
				case "Amazon":
				case "IBMWatson":
					return MapIsoLike(code);
				default:
					return code;
			}
		}

		private static string Normalize(string uiCode)
		{
			string code = (uiCode ?? "zh").Trim().ToLowerInvariant().Replace('_', '-');
			if (code == "jp")
				return "ja";
			if (code == "zh-cn" || code == "zh-hans" || code == "cn")
				return "zh";
			if (code == "zh-tw" || code == "zh-hant" || code == "cht")
				return "zh-tw";
			if (code == "pt-br")
				return "pt-br";
			if (code == "nb-no" || code == "no")
				return "nb";
			if (code == "iw")
				return "he";
			if (code == "fil")
				return "tl";
			return code;
		}

		private static string MapBaidu(string code)
		{
			switch (code)
			{
				case "zh": return "zh";
				case "zh-tw": return "cht";
				case "en": return "en";
				case "ja": return "jp";
				case "ko": return "kor";
				case "fr": return "fra";
				case "es": return "spa";
				case "ar": return "ara";
				case "bg": return "bul";
				case "et": return "est";
				case "da": return "dan";
				case "fi": return "fin";
				case "ro": return "rom";
				case "sl": return "slo";
				case "sv": return "swe";
				case "vi": return "vie";
				case "zh-yue": return "yue";
				default: return code;
			}
		}

		private static string MapYoudao(string code)
		{
			switch (code)
			{
				case "zh": return "zh-CHS";
				case "zh-tw": return "zh-CHT";
				case "pt-br": return "pt";
				default: return code;
			}
		}

		private static string MapGoogleLike(string code)
		{
			switch (code)
			{
				case "zh": return "zh-CN";
				case "zh-tw": return "zh-TW";
				case "pt-br": return "pt";
				case "nb": return "no";
				default: return code;
			}
		}

		private static string MapMicrosoft(string code)
		{
			switch (code)
			{
				case "zh": return "zh-Hans";
				case "zh-tw": return "zh-Hant";
				case "pt-br": return "pt";
				case "nb": return "nb";
				case "he": return "he";
				default: return code;
			}
		}

		private static string MapDeepL(string code)
		{
			switch (code)
			{
				case "zh":
				case "zh-tw":
					return "ZH";
				case "en": return "EN";
				case "ja": return "JA";
				case "ko": return "KO";
				case "pt": return "PT-PT";
				case "pt-br": return "PT-BR";
				case "nb": return "NB";
				default: return code.ToUpperInvariant();
			}
		}

		private static string MapIsoLike(string code)
		{
			switch (code)
			{
				case "zh-tw": return "zh-TW";
				case "pt-br": return "pt";
				default: return code;
			}
		}
	}
}
