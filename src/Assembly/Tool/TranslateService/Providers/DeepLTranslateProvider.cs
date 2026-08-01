using System;
using System.Collections.Generic;
using Newtonsoft.Json.Linq;

namespace Assembly.Tool.TranslateService.Providers
{
	internal sealed class DeepLTranslateProvider : ITranslateProvider
	{
		public string Id => "DeepL";
		public string DisplayName => "DeepL";
		public string AppIdLabel => "Auth Key";
		public string SecretKeyLabel => "(不需要)";
		public string ExtraLabel => "Endpoint (可选: free / pro，默认按密钥自动判断)";
		public bool RequiresSecretKey => false;
		public bool RequiresExtra => false;
		public string HelpText => "在 DeepL 申请 API Key：https://www.deepl.com/pro-api 。免费密钥通常以 :fx 结尾。";

		public string Translate(string text, string targetLanguage, string appId, string secretKey, string extra)
		{
			string host = ResolveHost(appId, extra);
			string url = host + "/v2/translate";
			var payload = new JObject
			{
				["text"] = new JArray(text),
				["source_lang"] = "EN",
				["target_lang"] = TranslateLanguageMapper.Map(Id, targetLanguage)
			};

			var headers = new Dictionary<string, string>
			{
				["Authorization"] = "DeepL-Auth-Key " + appId
			};

			JObject json = JObject.Parse(TranslateHttp.PostJson(url, payload.ToString(Newtonsoft.Json.Formatting.None), headers));
			return json["translations"]?[0]?["text"]?.ToString()
				?? throw new Exception("DeepL: empty translation result");
		}

		private static string ResolveHost(string authKey, string extra)
		{
			string mode = (extra ?? string.Empty).Trim().ToLowerInvariant();
			if (mode == "pro" || mode == "api.deepl.com")
				return "https://api.deepl.com";
			if (mode == "free" || mode == "api-free.deepl.com")
				return "https://api-free.deepl.com";

			return (authKey ?? string.Empty).EndsWith(":fx", StringComparison.OrdinalIgnoreCase)
				? "https://api-free.deepl.com"
				: "https://api.deepl.com";
		}
	}
}
