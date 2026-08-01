using System;
using System.Collections.Generic;
using System.Globalization;
using System.Security.Cryptography;
using System.Text;
using System.Web;
using Newtonsoft.Json.Linq;

namespace Assembly.Tool.TranslateService.Providers
{
	internal sealed class AliyunTranslateProvider : ITranslateProvider
	{
		public string Id => "Aliyun";
		public string DisplayName => "Aliyun";
		public string AppIdLabel => "AccessKey ID";
		public string SecretKeyLabel => "AccessKey Secret";
		public string ExtraLabel => "Region (optional, default cn-hangzhou)";
		public string ApiUrlLabel => "API URL";
		public bool UsesAppId => true;
		public bool UsesSecretKey => true;
		public bool UsesExtra => true;
		public bool UsesApiUrl => true;
		public bool RequiresAppId => true;
		public bool RequiresSecretKey => true;
		public bool RequiresExtra => false;
		public bool RequiresApiUrl => false;
		public string DefaultApiUrl => "https://mt.cn-hangzhou.aliyuncs.com/";
		public string HelpText => "Aliyun Machine Translation: https://www.aliyun.com/product/ai/alimt (AccessKey ID + Secret)";

		public string Translate(string text, string targetLanguage, string appId, string secretKey, string extra, string apiUrl)
		{
			string region = string.IsNullOrWhiteSpace(extra) ? "cn-hangzhou" : extra.Trim();
			string endpoint;
			string host;
			if (!string.IsNullOrWhiteSpace(apiUrl))
			{
				endpoint = TranslateEndpoint.Resolve(apiUrl, DefaultApiUrl) + "/";
				host = TranslateEndpoint.ResolveHost(endpoint, "mt." + region + ".aliyuncs.com");
			}
			else
			{
				host = "mt." + region + ".aliyuncs.com";
				endpoint = "https://" + host + "/";
			}

			var parameters = new SortedDictionary<string, string>(StringComparer.Ordinal)
			{
				["AccessKeyId"] = appId,
				["Action"] = "TranslateGeneral",
				["Format"] = "JSON",
				["FormatType"] = "text",
				["SignatureMethod"] = "HMAC-SHA1",
				["SignatureNonce"] = Guid.NewGuid().ToString(),
				["SignatureVersion"] = "1.0",
				["SourceLanguage"] = "en",
				["SourceText"] = text,
				["TargetLanguage"] = TranslateLanguageMapper.Map(Id, targetLanguage),
				["Timestamp"] = DateTime.UtcNow.ToString("yyyy-MM-dd'T'HH:mm:ss'Z'", CultureInfo.InvariantCulture),
				["Version"] = "2018-10-12",
				["Scene"] = "general"
			};

			var canonicalized = new StringBuilder();
			foreach (var pair in parameters)
			{
				if (canonicalized.Length > 0)
					canonicalized.Append("&");
				canonicalized.Append(PercentEncode(pair.Key));
				canonicalized.Append("=");
				canonicalized.Append(PercentEncode(pair.Value));
			}

			string stringToSign = "GET&" + PercentEncode("/") + "&" + PercentEncode(canonicalized.ToString());
			string signature = Sign(stringToSign, secretKey + "&");
			string url = endpoint + "?" + canonicalized + "&Signature=" + PercentEncode(signature);

			JObject json = JObject.Parse(TranslateHttp.Get(url));
			string code = json["Code"]?.ToString();
			if (!string.Equals(code, "200", StringComparison.OrdinalIgnoreCase))
				throw new Exception("Aliyun error " + code + ": " + (json["Message"] ?? json["Code"]));

			return json["Data"]?["Translated"]?.ToString()
				?? throw new Exception("Aliyun: empty translation result");
		}

		private static string Sign(string stringToSign, string key)
		{
			using (var hmac = new HMACSHA1(Encoding.UTF8.GetBytes(key)))
				return Convert.ToBase64String(hmac.ComputeHash(Encoding.UTF8.GetBytes(stringToSign)));
		}

		private static string PercentEncode(string value)
		{
			if (value == null)
				return string.Empty;

			string encoded = HttpUtility.UrlEncode(value, Encoding.UTF8) ?? string.Empty;
			return encoded
				.Replace("+", "%20")
				.Replace("*", "%2A")
				.Replace("%7E", "~");
		}
	}
}
