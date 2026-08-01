using System;
using System.Web;
using Newtonsoft.Json.Linq;

namespace Assembly.Tool.TranslateService.Providers
{
	internal sealed class MyMemoryTranslateProvider : ITranslateProvider
	{
		public string Id => "MyMemory";
		public string DisplayName => "MyMemory";
		public string AppIdLabel => "Email / API Key (optional)";
		public string SecretKeyLabel => "(optional)";
		public string ExtraLabel => "";
		public bool RequiresAppId => false;
		public bool RequiresSecretKey => false;
		public bool RequiresExtra => false;
		public string HelpText => "MyMemory free API: https://mymemory.translated.net/doc/spec.php";

		public string Translate(string text, string targetLanguage, string appId, string secretKey, string extra)
		{
			string to = TranslateLanguageMapper.Map(Id, targetLanguage);
			string url = "https://api.mymemory.translated.net/get"
				+ "?q=" + HttpUtility.UrlEncode(text)
				+ "&langpair=en|" + HttpUtility.UrlEncode(to);

			if (!string.IsNullOrWhiteSpace(appId))
			{
				if (appId.Contains("@"))
					url += "&de=" + HttpUtility.UrlEncode(appId);
				else
					url += "&key=" + HttpUtility.UrlEncode(appId);
			}

			JObject json = JObject.Parse(TranslateHttp.Get(url));
			int status = json["responseStatus"]?.Value<int>() ?? 0;
			if (status != 200)
				throw new Exception("MyMemory error: " + (json["responseDetails"] ?? status));

			return json["responseData"]?["translatedText"]?.ToString()
				?? throw new Exception("MyMemory: empty translation result");
		}
	}
}
