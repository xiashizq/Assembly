using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;

namespace Assembly.Tool.TranslateService
{
	internal static class TranslateHttp
	{
		private const int TimeoutMs = 8000;

		public static string Get(string url, IDictionary<string, string> headers = null)
		{
			var request = (HttpWebRequest)WebRequest.Create(url);
			request.Method = "GET";
			request.Timeout = TimeoutMs;
			request.ReadWriteTimeout = TimeoutMs;
			ApplyHeaders(request, headers);
			return ReadResponse(request);
		}

		public static string PostForm(string url, string formBody, IDictionary<string, string> headers = null)
		{
			return Post(url, formBody, "application/x-www-form-urlencoded; charset=UTF-8", headers);
		}

		public static string PostJson(string url, string jsonBody, IDictionary<string, string> headers = null)
		{
			return Post(url, jsonBody, "application/json; charset=UTF-8", headers);
		}

		public static string Post(string url, string body, string contentType, IDictionary<string, string> headers = null)
		{
			var request = (HttpWebRequest)WebRequest.Create(url);
			request.Method = "POST";
			request.Timeout = TimeoutMs;
			request.ReadWriteTimeout = TimeoutMs;
			request.ContentType = contentType;
			ApplyHeaders(request, headers);

			byte[] data = Encoding.UTF8.GetBytes(body ?? string.Empty);
			request.ContentLength = data.Length;
			using (Stream stream = request.GetRequestStream())
			{
				stream.Write(data, 0, data.Length);
			}

			return ReadResponse(request);
		}

		private static void ApplyHeaders(HttpWebRequest request, IDictionary<string, string> headers)
		{
			if (headers == null)
				return;

			foreach (var pair in headers)
			{
				if (string.Equals(pair.Key, "Content-Type", StringComparison.OrdinalIgnoreCase))
				{
					request.ContentType = pair.Value;
					continue;
				}

				if (string.Equals(pair.Key, "Accept", StringComparison.OrdinalIgnoreCase))
				{
					request.Accept = pair.Value;
					continue;
				}

				if (string.Equals(pair.Key, "User-Agent", StringComparison.OrdinalIgnoreCase))
				{
					request.UserAgent = pair.Value;
					continue;
				}

				if (string.Equals(pair.Key, "Host", StringComparison.OrdinalIgnoreCase))
				{
					request.Host = pair.Value;
					continue;
				}

				request.Headers[pair.Key] = pair.Value;
			}
		}

		private static string ReadResponse(HttpWebRequest request)
		{
			try
			{
				using (var response = (HttpWebResponse)request.GetResponse())
				using (var stream = response.GetResponseStream())
				using (var reader = new StreamReader(stream, Encoding.UTF8))
				{
					return reader.ReadToEnd();
				}
			}
			catch (WebException ex)
			{
				if (ex.Response == null)
					throw;

				using (var response = (HttpWebResponse)ex.Response)
				using (var stream = response.GetResponseStream())
				using (var reader = new StreamReader(stream ?? Stream.Null, Encoding.UTF8))
				{
					string body = reader.ReadToEnd();
					throw new Exception($"HTTP {(int)response.StatusCode}: {body}", ex);
				}
			}
		}
	}
}
