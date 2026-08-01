using System;
using System.Collections.Generic;
using Newtonsoft.Json.Linq;

namespace Assembly.Tool.TranslateService.Providers
{
	internal sealed class CaiyunTranslateProvider : ITranslateProvider
	{
		public string Id => "Caiyun";
		public string DisplayName => "Caiyun";
		public string AppIdLabel => "Token";
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
		public string DefaultApiUrl => "http://api.interpreter.caiyunai.com/v1/translator";
		public string HelpText => "Caiyun Translator: https://fanyi.caiyunapp.com/#/ (Token only, no Secret Key)";

		public string Translate(string text, string targetLanguage, string appId, string secretKey, string extra, string apiUrl)
		{
			string to = TranslateLanguageMapper.Map(Id, targetLanguage);
			string direction = "en2" + to;
			if (to == "zh" || to == "zh-TW")
				direction = "en2zh";
			else if (to == "ja")
				direction = "en2ja";

			var payload = new JObject
			{
				["source"] = text,
				["trans_type"] = direction,
				["request_id"] = "assembly",
				["detect"] = true
			};

			var headers = new Dictionary<string, string>
			{
				["X-Authorization"] = "token " + appId
			};

			JObject json = JObject.Parse(
				TranslateHttp.PostJson(TranslateEndpoint.Resolve(apiUrl, DefaultApiUrl),
					payload.ToString(Newtonsoft.Json.Formatting.None), headers));

			if (json["message"] != null && json["target"] == null)
				throw new Exception("Caiyun error: " + json["message"]);

			return json["target"]?.ToString()
				?? throw new Exception("Caiyun: empty translation result");
		}
	}
}
