using System;
using System.Collections.Generic;
using System.Web;
using Newtonsoft.Json.Linq;

namespace Assembly.Tool.TranslateService.Providers
{
	internal sealed class MicrosoftTranslateProvider : ITranslateProvider
	{
		public string Id => "Microsoft";
		public string DisplayName => "Microsoft Translator (Azure)";
		public string AppIdLabel => "Subscription Key";
		public string SecretKeyLabel => "(不需要)";
		public string ExtraLabel => "Region (如 global / eastasia，多服务资源必填)";
		public bool RequiresSecretKey => false;
		public bool RequiresExtra => false;
		public string HelpText => "在 Azure 创建 Translator 资源：https://portal.azure.com/ ，填入密钥与区域。";

		public string Translate(string text, string targetLanguage, string appId, string secretKey, string extra)
		{
			string to = TranslateLanguageMapper.Map(Id, targetLanguage);
			string url = "https://api.cognitive.microsofttranslator.com/translate?api-version=3.0"
				+ "&from=en"
				+ "&to=" + HttpUtility.UrlEncode(to);

			var headers = new Dictionary<string, string>
			{
				["Ocp-Apim-Subscription-Key"] = appId
			};

			if (!string.IsNullOrWhiteSpace(extra))
				headers["Ocp-Apim-Subscription-Region"] = extra.Trim();

			string body = new JArray(new JObject { ["Text"] = text }).ToString(Newtonsoft.Json.Formatting.None);
			JArray json = JArray.Parse(TranslateHttp.PostJson(url, body, headers));
			return json[0]?["translations"]?[0]?["text"]?.ToString()
				?? throw new Exception("Microsoft: empty translation result");
		}
	}
}
