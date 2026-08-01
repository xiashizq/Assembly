namespace Assembly.Tool.TranslateService
{
	internal interface ITranslateProvider
	{
		string Id { get; }
		string DisplayName { get; }
		string AppIdLabel { get; }
		string SecretKeyLabel { get; }
		string ExtraLabel { get; }
		bool RequiresAppId { get; }
		bool RequiresSecretKey { get; }
		bool RequiresExtra { get; }
		string HelpText { get; }
		string Translate(string text, string targetLanguage, string appId, string secretKey, string extra);
	}
}
