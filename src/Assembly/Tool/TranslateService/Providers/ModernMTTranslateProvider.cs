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
		public string SecretKeyLabel => "(optional)";
		public string ExtraLabel => "";
		public bool RequiresAppId => true;
		public bool RequiresSecretKey => false;
		public bool RequiresExtra => false;
		public string HelpText => "ModernMT API: https://www.modernmt.com/api/";

		public string Translate(string text, string targetLanguage, string appId, string secretKey, string extra)
		{
			string url = "https://api.modernmt.com/translate"
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
