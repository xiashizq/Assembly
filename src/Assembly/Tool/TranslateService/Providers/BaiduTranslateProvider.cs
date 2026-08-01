using System;
using System.Security.Cryptography;
using System.Text;
using System.Web;
using Newtonsoft.Json.Linq;

namespace Assembly.Tool.TranslateService.Providers
{
	internal sealed class BaiduTranslateProvider : ITranslateProvider
	{
		public string Id => "Baidu";
		public string DisplayName => "Baidu";
		public string AppIdLabel => "APP ID";
		public string SecretKeyLabel => "Secret Key";
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
		public string DefaultApiUrl => "https://fanyi-api.baidu.com/api/trans/vip/translate";
		public string HelpText => "Baidu Translate API: https://fanyi-api.baidu.com/ (APP ID + Secret Key)";

		public string Translate(string text, string targetLanguage, string appId, string secretKey, string extra, string apiUrl)
		{
			string from = "en";
			string to = TranslateLanguageMapper.Map(Id, targetLanguage);
			string salt = new Random().Next(100000, 999999).ToString();
			string sign = Md5Hex(appId + text + salt + secretKey);

			string url = TranslateEndpoint.Resolve(apiUrl, DefaultApiUrl)
				+ "?q=" + HttpUtility.UrlEncode(text)
				+ "&from=" + from
				+ "&to=" + to
				+ "&appid=" + HttpUtility.UrlEncode(appId)
				+ "&salt=" + salt
				+ "&sign=" + sign;

			JObject json = JObject.Parse(TranslateHttp.Get(url));
			if (json["error_code"] != null)
				throw new Exception("Baidu error " + json["error_code"] + ": " + json["error_msg"]);

			return json["trans_result"]?[0]?["dst"]?.ToString()
				?? throw new Exception("Baidu: empty translation result");
		}

		private static string Md5Hex(string input)
		{
			using (var md5 = MD5.Create())
			{
				byte[] hash = md5.ComputeHash(Encoding.UTF8.GetBytes(input));
				var sb = new StringBuilder(hash.Length * 2);
				foreach (byte b in hash)
					sb.Append(b.ToString("x2"));
				return sb.ToString();
			}
		}
	}
}
