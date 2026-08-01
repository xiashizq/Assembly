using System;
using System.Collections.Generic;
using System.Text;
using Newtonsoft.Json.Linq;

namespace Assembly.Tool.TranslateService.Providers
{
	internal sealed class IbmWatsonTranslateProvider : ITranslateProvider
	{
		public string Id => "IBMWatson";
		public string DisplayName => "IBM Watson Language Translator";
		public string AppIdLabel => "API Key";
		public string SecretKeyLabel => "";
		public string ExtraLabel => "";
		public string ApiUrlLabel => "Service URL";
		public bool UsesAppId => true;
		public bool UsesSecretKey => false;
		public bool UsesExtra => false;
		public bool UsesApiUrl => true;
		public bool RequiresAppId => true;
		public bool RequiresSecretKey => false;
		public bool RequiresExtra => false;
		public bool RequiresApiUrl => true;
		public string DefaultApiUrl => "";
		public string HelpText => "IBM Language Translator: https://cloud.ibm.com/catalog/services/language-translator (API Key + Service URL from IBM Cloud, e.g. https://api.us-south.language-translator.watson.cloud.ibm.com)";

		public string Translate(string text, string targetLanguage, string appId, string secretKey, string extra, string apiUrl)
		{
			// Migrate old Extra-based service URL.
			string serviceUrl = !string.IsNullOrWhiteSpace(apiUrl) ? apiUrl : extra;
			if (string.IsNullOrWhiteSpace(serviceUrl))
				throw new Exception("IBM Watson Service URL is required");

			string endpoint = TranslateEndpoint.Resolve(serviceUrl, serviceUrl);
			string url = endpoint + "/v3/translate?version=2018-05-01";
			string basic = Convert.ToBase64String(Encoding.ASCII.GetBytes("apikey:" + appId));

			var payload = new JObject
			{
				["text"] = new JArray(text),
				["source"] = "en",
				["target"] = TranslateLanguageMapper.Map(Id, targetLanguage)
			};

			var headers = new Dictionary<string, string>
			{
				["Authorization"] = "Basic " + basic
			};

			JObject json = JObject.Parse(TranslateHttp.PostJson(url, payload.ToString(Newtonsoft.Json.Formatting.None), headers));
			if (json["error"] != null || json["code"] != null)
				throw new Exception("IBM Watson error: " + (json["error"] ?? json["message"] ?? json["code"]));

			return json["translations"]?[0]?["translation"]?.ToString()
				?? throw new Exception("IBM Watson: empty translation result");
		}
	}
}
