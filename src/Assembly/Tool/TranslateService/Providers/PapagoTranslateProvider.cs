using System;
using System.Collections.Generic;
using System.Web;
using Newtonsoft.Json.Linq;

namespace Assembly.Tool.TranslateService.Providers
{
	internal sealed class PapagoTranslateProvider : ITranslateProvider
	{
		public string Id => "Papago";
		public string DisplayName => "Naver Papago";
		public string AppIdLabel => "Client ID";
		public string SecretKeyLabel => "Client Secret";
		public string ExtraLabel => "";
		public string ApiUrlLabel => "API URL";
		public bool UsesAppId => true;
		public bool UsesSecretKey => true;
		public bool UsesExtra => false;
		public bool UsesApiUrl => true;
		public bool RequiresAppId => true;
		public bool RequiresSecretKey => true;
		public bool RequiresExtra => false;
		public bool RequiresApiUrl => false;
		public string DefaultApiUrl => "https://openapi.naver.com/v1/papago/n2mt";
		public string HelpText => "Naver Papago API: https://developers.naver.com/products/papago/ (Client ID + Client Secret)";

		public string Translate(string text, string targetLanguage, string appId, string secretKey, string extra, string apiUrl)
		{
			string body =
				"source=en"
				+ "&target=" + HttpUtility.UrlEncode(TranslateLanguageMapper.Map(Id, targetLanguage))
				+ "&text=" + HttpUtility.UrlEncode(text);

			var headers = new Dictionary<string, string>
			{
				["X-Naver-Client-Id"] = appId,
				["X-Naver-Client-Secret"] = secretKey
			};

			JObject json = JObject.Parse(TranslateHttp.PostForm(
				TranslateEndpoint.Resolve(apiUrl, DefaultApiUrl), body, headers));

			if (json["errorCode"] != null)
				throw new Exception("Papago error " + json["errorCode"] + ": " + json["errorMessage"]);

			return json["message"]?["result"]?["translatedText"]?.ToString()
				?? throw new Exception("Papago: empty translation result");
		}
	}
}
