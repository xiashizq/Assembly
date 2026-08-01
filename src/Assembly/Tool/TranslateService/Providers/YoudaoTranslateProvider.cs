using System;
using System.Security.Cryptography;
using System.Text;
using System.Web;
using Newtonsoft.Json.Linq;

namespace Assembly.Tool.TranslateService.Providers
{
	internal sealed class YoudaoTranslateProvider : ITranslateProvider
	{
		public string Id => "Youdao";
		public string DisplayName => "Youdao";
		public string AppIdLabel => "App Key";
		public string SecretKeyLabel => "App Secret";
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
		public string DefaultApiUrl => "https://openapi.youdao.com/api";
		public string HelpText => "Youdao AI Cloud: https://ai.youdao.com/ (App Key + App Secret)";

		public string Translate(string text, string targetLanguage, string appId, string secretKey, string extra, string apiUrl)
		{
			string from = "en";
			string to = TranslateLanguageMapper.Map(Id, targetLanguage);
			string salt = Guid.NewGuid().ToString();
			string curtime = DateTimeOffset.UtcNow.ToUnixTimeSeconds().ToString();
			string input = TruncateForSign(text);
			string sign = Sha256Hex(appId + input + salt + curtime + secretKey);

			string body =
				"q=" + HttpUtility.UrlEncode(text)
				+ "&from=" + from
				+ "&to=" + to
				+ "&appKey=" + HttpUtility.UrlEncode(appId)
				+ "&salt=" + HttpUtility.UrlEncode(salt)
				+ "&sign=" + sign
				+ "&signType=v3"
				+ "&curtime=" + curtime;

			JObject json = JObject.Parse(TranslateHttp.PostForm(TranslateEndpoint.Resolve(apiUrl, DefaultApiUrl), body));
			string errorCode = json["errorCode"]?.ToString();
			if (!string.IsNullOrEmpty(errorCode) && errorCode != "0")
				throw new Exception("Youdao error " + errorCode);

			JToken translation = json["translation"];
			if (translation is JArray arr && arr.Count > 0)
				return arr[0].ToString();

			throw new Exception("Youdao: empty translation result");
		}

		private static string TruncateForSign(string q)
		{
			if (string.IsNullOrEmpty(q))
				return string.Empty;
			return q.Length <= 20
				? q
				: q.Substring(0, 10) + q.Length + q.Substring(q.Length - 10);
		}

		private static string Sha256Hex(string input)
		{
			using (var sha = SHA256.Create())
			{
				byte[] hash = sha.ComputeHash(Encoding.UTF8.GetBytes(input));
				var sb = new StringBuilder(hash.Length * 2);
				foreach (byte b in hash)
					sb.Append(b.ToString("x2"));
				return sb.ToString();
			}
		}
	}
}
