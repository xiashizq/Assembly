namespace Assembly.Tool.TranslateService
{
	internal interface ITranslateProvider
	{
		string Id { get; }
		string DisplayName { get; }

		/// <summary>Primary credential label (APP ID / API Key / Token / etc.).</summary>
		string AppIdLabel { get; }
		/// <summary>Secondary credential label (Secret Key). Empty when unused.</summary>
		string SecretKeyLabel { get; }
		/// <summary>Extra setting label (Region / Folder ID). Empty when unused.</summary>
		string ExtraLabel { get; }
		/// <summary>API endpoint field label.</summary>
		string ApiUrlLabel { get; }

		bool UsesAppId { get; }
		bool UsesSecretKey { get; }
		bool UsesExtra { get; }
		bool UsesApiUrl { get; }

		bool RequiresAppId { get; }
		bool RequiresSecretKey { get; }
		bool RequiresExtra { get; }
		bool RequiresApiUrl { get; }

		string DefaultApiUrl { get; }
		string HelpText { get; }

		string Translate(string text, string targetLanguage, string appId, string secretKey, string extra, string apiUrl);
	}
}
