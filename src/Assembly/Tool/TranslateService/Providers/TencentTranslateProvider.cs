using System;
using System.Collections.Generic;
using System.Globalization;
using System.Security.Cryptography;
using System.Text;
using Newtonsoft.Json.Linq;

namespace Assembly.Tool.TranslateService.Providers
{
	internal sealed class TencentTranslateProvider : ITranslateProvider
	{
		public string Id => "Tencent";
		public string DisplayName => "Tencent (腾讯翻译君)";
		public string AppIdLabel => "SecretId";
		public string SecretKeyLabel => "SecretKey";
		public string ExtraLabel => "Region (可选, 默认 ap-guangzhou)";
		public bool RequiresSecretKey => true;
		public bool RequiresExtra => false;
		public string HelpText => "在腾讯云开通机器翻译并创建 API 密钥：https://cloud.tencent.com/product/tmt";

		public string Translate(string text, string targetLanguage, string appId, string secretKey, string extra)
		{
			string host = "tmt.tencentcloudapi.com";
			string service = "tmt";
			string action = "TextTranslate";
			string version = "2018-03-21";
			string region = string.IsNullOrWhiteSpace(extra) ? "ap-guangzhou" : extra.Trim();
			string timestamp = DateTimeOffset.UtcNow.ToUnixTimeSeconds().ToString(CultureInfo.InvariantCulture);
			string date = DateTimeOffset.UtcNow.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture);

			var payloadObj = new JObject
			{
				["SourceText"] = text,
				["Source"] = "en",
				["Target"] = TranslateLanguageMapper.Map(Id, targetLanguage),
				["ProjectId"] = 0
			};
			string payload = payloadObj.ToString(Newtonsoft.Json.Formatting.None);

			string canonicalHeaders = "content-type:application/json; charset=utf-8\nhost:" + host + "\n";
			string signedHeaders = "content-type;host";
			string hashedRequestPayload = Sha256Hex(payload);
			string canonicalRequest = "POST\n/\n\n" + canonicalHeaders + "\n" + signedHeaders + "\n" + hashedRequestPayload;

			string credentialScope = date + "/" + service + "/tc3_request";
			string stringToSign = "TC3-HMAC-SHA256\n" + timestamp + "\n" + credentialScope + "\n" + Sha256Hex(canonicalRequest);

			byte[] secretDate = HmacSha256(Encoding.UTF8.GetBytes("TC3" + secretKey), date);
			byte[] secretService = HmacSha256(secretDate, service);
			byte[] secretSigning = HmacSha256(secretService, "tc3_request");
			string signature = ToHex(HmacSha256(secretSigning, stringToSign));

			string authorization = "TC3-HMAC-SHA256 Credential=" + appId + "/" + credentialScope
				+ ", SignedHeaders=" + signedHeaders
				+ ", Signature=" + signature;

			var headers = new Dictionary<string, string>
			{
				["Authorization"] = authorization,
				["Host"] = host,
				["X-TC-Action"] = action,
				["X-TC-Timestamp"] = timestamp,
				["X-TC-Version"] = version,
				["X-TC-Region"] = region
			};

			JObject json = JObject.Parse(TranslateHttp.PostJson("https://" + host, payload, headers));
			JToken error = json["Response"]?["Error"];
			if (error != null)
				throw new Exception("Tencent error " + error["Code"] + ": " + error["Message"]);

			return json["Response"]?["TargetText"]?.ToString()
				?? throw new Exception("Tencent: empty translation result");
		}

		private static string Sha256Hex(string input)
		{
			using (var sha = SHA256.Create())
			{
				return ToHex(sha.ComputeHash(Encoding.UTF8.GetBytes(input)));
			}
		}

		private static byte[] HmacSha256(byte[] key, string data)
		{
			using (var hmac = new HMACSHA256(key))
			{
				return hmac.ComputeHash(Encoding.UTF8.GetBytes(data));
			}
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
