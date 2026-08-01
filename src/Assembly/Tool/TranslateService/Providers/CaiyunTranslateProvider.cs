using System;
using System.Collections.Generic;
using Newtonsoft.Json.Linq;

namespace Assembly.Tool.TranslateService.Providers
{
	internal sealed class CaiyunTranslateProvider : ITranslateProvider
	{
		public string Id => "Caiyun";
		public string DisplayName => "Caiyun (彩云小译)";
		public string AppIdLabel => "Token";
		public string SecretKeyLabel => "(不需要)";
		public string ExtraLabel => "";
		public bool RequiresSecretKey => false;
		public bool RequiresExtra => false;
		public string HelpText => "在彩云小译开放平台申请 Token：https://fanyi.caiyunapp.com/#/";

		public string Translate(string text, string targetLanguage, string appId, string secretKey, string extra)
		{
			string to = TranslateLanguageMapper.Map(Id, targetLanguage);
			string direction = "en2" + to;
			if (to == "zh")
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
				TranslateHttp.PostJson("http://api.interpreter.caiyunai.com/v1/translator",
					payload.ToString(Newtonsoft.Json.Formatting.None), headers));

			if (json["message"] != null && json["target"] == null)
				throw new Exception("Caiyun error: " + json["message"]);

			return json["target"]?.ToString()
				?? throw new Exception("Caiyun: empty translation result");
		}
	}
}
