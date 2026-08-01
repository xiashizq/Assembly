namespace Assembly.Tool.GPTservice
{
	internal sealed class GptProviderInfo : IGptProvider
	{
		public string Id { get; set; }
		public string DisplayName { get; set; }
		public string ApiKeyLabel { get; set; } = "API Key";
		public string ModelLabel { get; set; } = "Model";
		public string ExtraLabel { get; set; } = "";
		public bool RequiresExtra { get; set; }
		public bool SupportsNativeFileUpload { get; set; }
		public string DefaultModel { get; set; }
		public string DefaultLongModel { get; set; }
		public string DefaultBaseUrl { get; set; }
		public string HelpText { get; set; }
		public GptApiStyle ApiStyle { get; set; } = GptApiStyle.OpenAiCompatible;
	}
}
