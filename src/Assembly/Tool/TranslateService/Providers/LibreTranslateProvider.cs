using System;
using Newtonsoft.Json.Linq;

namespace Assembly.Tool.TranslateService.Providers
{
	internal sealed class LibreTranslateProvider : ITranslateProvider
	{
		public string Id => "LibreTranslate";
		public string DisplayName => "LibreTranslate";
		public string AppIdLabel => "API Key (optional)";
		public string SecretKeyLabel => "(optional)";
		public string ExtraLabel => "Endpoint URL (default https://libretranslate.com)";
		public bool RequiresAppId => false;
		public bool RequiresSecretKey => false;
		public bool RequiresExtra => false;
		public string HelpText => "LibreTranslate public/self-hosted: https://libretranslate.com/";

		public string Translate(string text, string targetLanguage, string appId, string secretKey, string extra)
		{
			string endpoint = string.IsNullOrWhiteSpace(extra)
				? "https://libretranslate.com"
				: extra.Trim().TrimEnd('/');
			string url = endpoint + "/translate";

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
