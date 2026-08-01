namespace Assembly.Tool.GPTservice
{
	internal enum GptApiStyle
	{
		OpenAiCompatible,
		AzureOpenAi
	}

	internal interface IGptProvider
	{
		string Id { get; }
		string DisplayName { get; }
		string ApiKeyLabel { get; }
		string ModelLabel { get; }
		string ExtraLabel { get; }
		bool RequiresExtra { get; }
		bool SupportsNativeFileUpload { get; }
		string DefaultModel { get; }
		string DefaultLongModel { get; }
		string DefaultBaseUrl { get; }
		string HelpText { get; }
		GptApiStyle ApiStyle { get; }
	}
}
