using System;
using System.Collections.Generic;
using System.Web;
using Newtonsoft.Json.Linq;

namespace Assembly.Tool.TranslateService.Providers
{
	internal sealed class ModernMTTranslateProvider : ITranslateProvider
	{
		public string Id => "ModernMT";
		public string DisplayName => "ModernMT";
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
		public string DefaultApiUrl => "https://api.modernmt.com/translate";
		public string HelpText => "ModernMT API: https://www.modernmt.com/api/ (API Key only)";

		public string Translate(string text, string targetLanguage, string appId, string secretKey, string extra, string apiUrl)
		{
			string url = TranslateEndpoint.Resolve(apiUrl, DefaultApiUrl)
				+ "?q=" + HttpUtility.UrlEncode(text)
				+ "&source=en"
				+ "&target=" + HttpUtility.UrlEncode(TranslateLanguageMapper.Map(Id, targetLanguage));

			var headers = new Dictionary<string, string>
			{
				["MM-Api-Key"] = appId
			};

			JObject json = JObject.Parse(TranslateHttp.Get(url, headers));
			int? status = json["status"]?.Value<int>();
			if (status.HasValue && status.Value >= 400)
				throw new Exception("ModernMT error: " + (json["error"]?["message"] ?? json["error"]));

			return json["data"]?["translation"]?.ToString()
				?? throw new Exception("ModernMT: empty translation result");
		}
	}
}
