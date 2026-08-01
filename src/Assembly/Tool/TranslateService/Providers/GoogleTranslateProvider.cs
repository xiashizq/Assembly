using System;
using System.Web;
using Newtonsoft.Json.Linq;

namespace Assembly.Tool.TranslateService.Providers
{
	internal sealed class GoogleTranslateProvider : ITranslateProvider
	{
		public string Id => "Google";
		public string DisplayName => "Google Cloud Translation";
		public string AppIdLabel => "API Key";
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
		public string DefaultApiUrl => "https://translation.googleapis.com/language/translate/v2";
		public string HelpText => "Google Cloud Translation API: https://cloud.google.com/translate (API Key only)";

		public string Translate(string text, string targetLanguage, string appId, string secretKey, string extra, string apiUrl)
		{
			string url = TranslateEndpoint.Resolve(apiUrl, DefaultApiUrl) + "?key=" + HttpUtility.UrlEncode(appId);
			var payload = new JObject
			{
				["q"] = text,
				["source"] = "en",
				["target"] = TranslateLanguageMapper.Map(Id, targetLanguage),
				["format"] = "text"
			};

			JObject json = JObject.Parse(TranslateHttp.PostJson(url, payload.ToString(Newtonsoft.Json.Formatting.None)));
			JToken error = json["error"];
			if (error != null)
				throw new Exception("Google error " + error["code"] + ": " + error["message"]);

			return json["data"]?["translations"]?[0]?["translatedText"]?.ToString()
				?? throw new Exception("Google: empty translation result");
		}
	}
}
