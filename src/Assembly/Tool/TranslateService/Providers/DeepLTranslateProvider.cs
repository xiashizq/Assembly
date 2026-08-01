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
		public string SecretKeyLabel => "";
		public string ExtraLabel => "";
		public string ApiUrlLabel => "API URL";
		public bool UsesAppId => true;
		public bool UsesSecretKey => false;
		public bool UsesExtra => false;
		public bool UsesApiUrl => true;
		public bool RequiresAppId => true;
		public bool RequiresSecretKey => false;
		public bool RequiresExtra => false;
		public bool RequiresApiUrl => false;
		public string DefaultApiUrl => "https://api-free.deepl.com";
		public string HelpText => "DeepL API: https://www.deepl.com/pro-api (Auth Key only; free keys end with :fx ? api-free.deepl.com, pro ? api.deepl.com)";

		public string Translate(string text, string targetLanguage, string appId, string secretKey, string extra, string apiUrl)
		{
			string host = ResolveHost(appId, apiUrl);
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

		private string ResolveHost(string authKey, string apiUrl)
		{
			if (!string.IsNullOrWhiteSpace(apiUrl))
				return TranslateEndpoint.Resolve(apiUrl, DefaultApiUrl);

			// Backward compat: Extra used to store free/pro; still honor via empty apiUrl + key suffix.
			return (authKey ?? string.Empty).EndsWith(":fx", StringComparison.OrdinalIgnoreCase)
				? "https://api-free.deepl.com"
				: "https://api.deepl.com";
		}
	}
}
