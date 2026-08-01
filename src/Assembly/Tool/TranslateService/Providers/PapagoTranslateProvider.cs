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
		public bool RequiresAppId => true;
		public bool RequiresSecretKey => true;
		public bool RequiresExtra => false;
		public string HelpText => "Naver Papago API: https://developers.naver.com/products/papago/";

		public string Translate(string text, string targetLanguage, string appId, string secretKey, string extra)
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
				"https://openapi.naver.com/v1/papago/n2mt", body, headers));

			if (json["errorCode"] != null)
				throw new Exception("Papago error " + json["errorCode"] + ": " + json["errorMessage"]);

			return json["message"]?["result"]?["translatedText"]?.ToString()
				?? throw new Exception("Papago: empty translation result");
		}
	}
}
