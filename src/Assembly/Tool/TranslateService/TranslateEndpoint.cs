using System;

namespace Assembly.Tool.TranslateService
{
	internal static class TranslateEndpoint
	{
		public static string Resolve(string apiUrl, string defaultApiUrl)
		{
			string url = string.IsNullOrWhiteSpace(apiUrl) ? defaultApiUrl : apiUrl;
			if (string.IsNullOrWhiteSpace(url))
				throw new Exception("API URL is not configured");
			return url.Trim().TrimEnd('/');
		}

		public static string ResolveHost(string apiUrl, string defaultHost)
		{
			string value = string.IsNullOrWhiteSpace(apiUrl) ? defaultHost : apiUrl.Trim();
			if (value.StartsWith("http://", StringComparison.OrdinalIgnoreCase)
				|| value.StartsWith("https://", StringComparison.OrdinalIgnoreCase))
			{
				var uri = new Uri(value);
				return uri.Host;
			}
			return value.Trim().TrimEnd('/');
		}
	}
}
