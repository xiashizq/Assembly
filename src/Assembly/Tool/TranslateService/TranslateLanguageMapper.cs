using System;

namespace Assembly.Tool.TranslateService
{
	internal static class TranslateLanguageMapper
	{
		public static string Map(string providerId, string uiCode)
		{
			string code = (uiCode ?? "zh").Trim().ToLowerInvariant();
			switch ((providerId ?? string.Empty).Trim())
			{
				case "Baidu":
					if (code == "ja" || code == "jp")
						return "jp";
					if (code == "zh" || code == "zh-cn" || code == "zh-hans")
						return "zh";
					return code;

				case "Youdao":
					if (code == "zh" || code == "zh-cn" || code == "zh-hans")
						return "zh-CHS";
					if (code == "ja" || code == "jp")
						return "ja";
					return code;

				case "Google":
				case "Papago":
				case "MyMemory":
					if (code == "zh" || code == "zh-hans")
						return "zh-CN";
					if (code == "ja" || code == "jp")
						return "ja";
					return code;

				case "Microsoft":
					if (code == "zh" || code == "zh-cn")
						return "zh-Hans";
					if (code == "ja" || code == "jp")
						return "ja";
					return code;

				case "DeepL":
					if (code == "zh" || code == "zh-cn" || code == "zh-hans")
						return "ZH";
					if (code == "ja" || code == "jp")
						return "JA";
					return code.ToUpperInvariant();

				case "Tencent":
				case "Aliyun":
				case "Caiyun":
				case "Yandex":
				case "Amazon":
				case "LibreTranslate":
				case "ModernMT":
				case "IBMWatson":
					if (code == "zh" || code == "zh-cn" || code == "zh-hans")
						return "zh";
					if (code == "ja" || code == "jp")
						return "ja";
					return code;

				default:
					return code;
			}
		}
	}
}
