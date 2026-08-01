using System;
using Newtonsoft.Json.Linq;

namespace Assembly.Tool.TranslateService.Providers
{
	internal sealed class LibreTranslateProvider : ITranslateProvider
	{
		public string Id => "LibreTranslate";
		public string DisplayName => "LibreTranslate";
		public string AppIdLabel => "API Key (optional)";
		public string SecretKeyLabel => "";
		public string ExtraLabel => "";
		public string ApiUrlLabel => "API URL";
		public bool UsesAppId => true;
		public bool UsesSecretKey => false;
		public bool UsesExtra => false;
		public bool UsesApiUrl => true;
		public bool RequiresAppId => false;
		public bool RequiresSecretKey => false;
		public bool RequiresExtra => false;
		public bool RequiresApiUrl => false;
		public string DefaultApiUrl => "https://libretranslate.com";
		public string HelpText => "LibreTranslate public/self-hosted: https://libretranslate.com/ (optional API Key; set API URL for self-hosted)";

		public string Translate(string text, string targetLanguage, string appId, string secretKey, string extra, string apiUrl)
		{
			// Migrate old Extra-based endpoint.
			string baseUrl = !string.IsNullOrWhiteSpace(apiUrl)
				? apiUrl
				: (!string.IsNullOrWhiteSpace(extra) ? extra : DefaultApiUrl);
			string url = TranslateEndpoint.Resolve(baseUrl, DefaultApiUrl) + "/translate";

			var payload = new JObject
			{
				["q"] = text,
				["source"] = "en",
				["target"] = TranslateLanguageMapper.Map(Id, targetLanguage),
				["format"] = "text"
			};
			if (!string.IsNullOrWhiteSpace(appId))
				payload["api_key"] = appId;

			JObject json = JObject.Parse(TranslateHttp.PostJson(url, payload.ToString(Newtonsoft.Json.Formatting.None)));
			if (json["error"] != null)
				throw new Exception("LibreTranslate error: " + json["error"]);

			return json["translatedText"]?.ToString()
				?? throw new Exception("LibreTranslate: empty translation result");
		}
	}
}
