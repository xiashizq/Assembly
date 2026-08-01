using System;
using System.Collections.Generic;
using System.Linq;
using Assembly.Tool.TranslateService.Providers;

namespace Assembly.Tool.TranslateService
{
	public class PublicTranslateService
	{
		private static readonly ITranslateProvider[] Providers =
		{
			// Domestic
			new BaiduTranslateProvider(),
			new YoudaoTranslateProvider(),
			new TencentTranslateProvider(),
			new AliyunTranslateProvider(),
			new CaiyunTranslateProvider(),

			// International
			new GoogleTranslateProvider(),
			new MicrosoftTranslateProvider(),
			new DeepLTranslateProvider(),
			new AmazonTranslateProvider(),
			new YandexTranslateProvider(),
			new PapagoTranslateProvider(),
			new ModernMTTranslateProvider(),
			new IbmWatsonTranslateProvider(),
			new LibreTranslateProvider(),
			new MyMemoryTranslateProvider()
		};

		internal static IReadOnlyList<ITranslateProvider> GetProviders()
		{
			return Providers;
		}

		internal static ITranslateProvider GetProvider(string id)
		{
			return Providers.FirstOrDefault(p =>
				string.Equals(p.Id, id, StringComparison.OrdinalIgnoreCase));
		}

		public static string TranslateAsync(string q)
		{
			if (string.IsNullOrWhiteSpace(q))
				return string.Empty;

			string translationApp = ConfigManager.GetSetting("Assembly", "TranslationApp", "Baidu");
			ITranslateProvider provider = GetProvider(translationApp);
			if (provider == null)
				return "Unsupported translation provider: " + translationApp;

			string appId = ConfigManager.GetSetting("Assembly", GetAppIdKey(provider.Id));
			string secretKey = ConfigManager.GetSetting("Assembly", GetSecretKeyKey(provider.Id));
			string extra = ConfigManager.GetSetting("Assembly", GetExtraKey(provider.Id));

			// Backward compatible fallback for older Baidu-only config.
			if (string.IsNullOrWhiteSpace(appId))
				appId = ConfigManager.GetSetting("Assembly", "TranslationAppId");
			if (string.IsNullOrWhiteSpace(secretKey))
				secretKey = ConfigManager.GetSetting("Assembly", "TranslationSecretKey");

			if (provider.RequiresAppId && string.IsNullOrWhiteSpace(appId))
				return "Translation credentials not configured";
			if (provider.RequiresSecretKey && string.IsNullOrWhiteSpace(secretKey))
				return "Translation secret key not configured";
			if (provider.RequiresExtra && string.IsNullOrWhiteSpace(extra))
				return "Translation extra setting not configured";

			string targetLanguage = ConfigManager.GetSetting("Assembly", "TranslationTargetlanguage", "zh");

			try
			{
				return provider.Translate(q, targetLanguage, (appId ?? string.Empty).Trim(),
					(secretKey ?? string.Empty).Trim(), (extra ?? string.Empty).Trim());
			}
			catch (Exception ex)
			{
				return "Translation failed: " + ex.Message;
			}
		}

		internal static string GetAppIdKey(string providerId)
		{
			return "TranslationAppId_" + providerId;
		}

		internal static string GetSecretKeyKey(string providerId)
		{
			return "TranslationSecretKey_" + providerId;
		}

		internal static string GetExtraKey(string providerId)
		{
			return "TranslationExtra_" + providerId;
		}
	}
}
