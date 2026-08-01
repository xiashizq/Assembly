using System;
using System.Collections.Generic;
using Newtonsoft.Json.Linq;

namespace Assembly.Tool.TranslateService.Providers
{
	internal sealed class YandexTranslateProvider : ITranslateProvider
	{
		public string Id => "Yandex";
		public string DisplayName => "Yandex Translate";
		public string AppIdLabel => "API Key";
		public string SecretKeyLabel => "(optional)";
		public string ExtraLabel => "Folder ID (optional)";
		public bool RequiresAppId => true;
		public bool RequiresSecretKey => false;
		public bool RequiresExtra => false;
		public string HelpText => "Yandex Cloud Translate: https://cloud.yandex.com/services/translate";

		public string Translate(string text, string targetLanguage, string appId, string secretKey, string extra)
		{
			var payload = new JObject
			{
				["texts"] = new JArray(text),
				["sourceLanguageCode"] = "en",
				["targetLanguageCode"] = TranslateLanguageMapper.Map(Id, targetLanguage)
			};
			if (!string.IsNullOrWhiteSpace(extra))
				payload["folderId"] = extra.Trim();

			var headers = new Dictionary<string, string>
			{
				["Authorization"] = "Api-Key " + appId
			};

			JObject json = JObject.Parse(TranslateHttp.PostJson(
				"https://translate.api.cloud.yandex.net/translate/v2/translate",
				payload.ToString(Newtonsoft.Json.Formatting.None),
				headers));

			if (json["code"] != null || json["error"] != null)
				throw new Exception("Yandex error: " + (json["message"] ?? json["error"]));

			return json["translations"]?[0]?["text"]?.ToString()
				?? throw new Exception("Yandex: empty translation result");
		}
	}
}
