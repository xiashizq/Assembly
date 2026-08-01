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
		public string SecretKeyLabel => "(optional)";
		public string ExtraLabel => "Endpoint (optional: free / pro)";
		public bool RequiresAppId => true;
		public bool RequiresSecretKey => false;
		public bool RequiresExtra => false;
		public string HelpText => "DeepL API: https://www.deepl.com/pro-api (free keys usually end with :fx)";

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
