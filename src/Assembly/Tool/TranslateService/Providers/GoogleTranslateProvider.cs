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
		public string SecretKeyLabel => "(optional)";
		public string ExtraLabel => "";
		public bool RequiresAppId => true;
		public bool RequiresSecretKey => false;
		public bool RequiresExtra => false;
		public string HelpText => "Google Cloud Translation API: https://cloud.google.com/translate";

		public string Translate(string text, string targetLanguage, string appId, string secretKey, string extra)
		{
			string url = "https://translation.googleapis.com/language/translate/v2?key=" + HttpUtility.UrlEncode(appId);
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
