using System;
using System.Collections.Generic;
using System.Globalization;
using System.Security.Cryptography;
using System.Text;
using Newtonsoft.Json.Linq;

namespace Assembly.Tool.TranslateService.Providers
{
	internal sealed class AmazonTranslateProvider : ITranslateProvider
	{
		public string Id => "Amazon";
		public string DisplayName => "Amazon Translate";
		public string AppIdLabel => "Access Key ID";
		public string SecretKeyLabel => "Secret Access Key";
		public string ExtraLabel => "Region (default us-east-1)";
		public bool RequiresAppId => true;
		public bool RequiresSecretKey => true;
		public bool RequiresExtra => false;
		public string HelpText => "AWS Translate: https://aws.amazon.com/translate/ (IAM Access Key)";

		public string Translate(string text, string targetLanguage, string appId, string secretKey, string extra)
		{
			string region = string.IsNullOrWhiteSpace(extra) ? "us-east-1" : extra.Trim();
			string service = "translate";
			string host = "translate." + region + ".amazonaws.com";
			string endpoint = "https://" + host + "/";
			string amzTarget = "AWSShineFrontendService_20170701.TranslateText";
			string contentType = "application/x-amz-json-1.1";

			var payloadObj = new JObject
			{
				["Text"] = text,
				["SourceLanguageCode"] = "en",
				["TargetLanguageCode"] = TranslateLanguageMapper.Map(Id, targetLanguage)
			};
			string payload = payloadObj.ToString(Newtonsoft.Json.Formatting.None);

			DateTime now = DateTime.UtcNow;
			string amzDate = now.ToString("yyyyMMdd'T'HHmmss'Z'", CultureInfo.InvariantCulture);
			string dateStamp = now.ToString("yyyyMMdd", CultureInfo.InvariantCulture);

			string canonicalHeaders =
				"content-type:" + contentType + "\n"
				+ "host:" + host + "\n"
				+ "x-amz-date:" + amzDate + "\n"
				+ "x-amz-target:" + amzTarget + "\n";
			string signedHeaders = "content-type;host;x-amz-date;x-amz-target";
			string payloadHash = Sha256Hex(payload);
			string canonicalRequest = "POST\n/\n\n" + canonicalHeaders + "\n" + signedHeaders + "\n" + payloadHash;

			string credentialScope = dateStamp + "/" + region + "/" + service + "/aws4_request";
			string stringToSign = "AWS4-HMAC-SHA256\n" + amzDate + "\n" + credentialScope + "\n" + Sha256Hex(canonicalRequest);
			byte[] signingKey = GetSignatureKey(secretKey, dateStamp, region, service);
			string signature = ToHex(HmacSha256(signingKey, stringToSign));

			string authorization = "AWS4-HMAC-SHA256 Credential=" + appId + "/" + credentialScope
				+ ", SignedHeaders=" + signedHeaders
				+ ", Signature=" + signature;

			var headers = new Dictionary<string, string>
			{
				["Authorization"] = authorization,
				["Host"] = host,
				["X-Amz-Date"] = amzDate,
				["X-Amz-Target"] = amzTarget
			};

			JObject json = JObject.Parse(TranslateHttp.Post(endpoint, payload, contentType, headers));
			if (json["__type"] != null || (json["message"] != null && json["TranslatedText"] == null))
				throw new Exception("Amazon error: " + (json["message"] ?? json["__type"]));

			return json["TranslatedText"]?.ToString()
				?? throw new Exception("Amazon: empty translation result");
		}

		private static byte[] GetSignatureKey(string key, string dateStamp, string regionName, string serviceName)
		{
			byte[] kDate = HmacSha256(Encoding.UTF8.GetBytes("AWS4" + key), dateStamp);
			byte[] kRegion = HmacSha256(kDate, regionName);
			byte[] kService = HmacSha256(kRegion, serviceName);
			return HmacSha256(kService, "aws4_request");
		}

		private static byte[] HmacSha256(byte[] key, string data)
		{
			using (var hmac = new HMACSHA256(key))
				return hmac.ComputeHash(Encoding.UTF8.GetBytes(data));
		}

		private static string Sha256Hex(string data)
		{
			using (var sha = SHA256.Create())
				return ToHex(sha.ComputeHash(Encoding.UTF8.GetBytes(data)));
		}

		private static string ToHex(byte[] bytes)
		{
			var sb = new StringBuilder(bytes.Length * 2);
			foreach (byte b in bytes)
				sb.Append(b.ToString("x2"));
			return sb.ToString();
		}
	}
}
