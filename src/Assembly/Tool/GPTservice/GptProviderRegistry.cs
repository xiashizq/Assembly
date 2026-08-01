using System;
using System.Collections.Generic;
using System.Linq;

namespace Assembly.Tool.GPTservice
{
	internal static class GptProviderRegistry
	{
		private static readonly IGptProvider[] Providers =
		{
			new GptProviderInfo
			{
				Id = "Qwen",
				DisplayName = "Qwen (通义千问)",
				DefaultBaseUrl = "https://dashscope.aliyuncs.com/compatible-mode/v1",
				DefaultModel = "qwen-plus",
				DefaultLongModel = "qwen-long",
				SupportsNativeFileUpload = true,
				HelpText = "阿里云 DashScope：https://dashscope.console.aliyun.com/ 使用 API-KEY。"
			},
			new GptProviderInfo
			{
				Id = "DeepSeek",
				DisplayName = "DeepSeek",
				DefaultBaseUrl = "https://api.deepseek.com",
				DefaultModel = "deepseek-chat",
				DefaultLongModel = "deepseek-chat",
				HelpText = "DeepSeek 开放平台：https://platform.deepseek.com/"
			},
			new GptProviderInfo
			{
				Id = "Moonshot",
				DisplayName = "Moonshot (Kimi)",
				DefaultBaseUrl = "https://api.moonshot.cn/v1",
				DefaultModel = "moonshot-v1-8k",
				DefaultLongModel = "moonshot-v1-128k",
				HelpText = "月之暗面 Kimi：https://platform.moonshot.cn/"
			},
			new GptProviderInfo
			{
				Id = "Zhipu",
				DisplayName = "Zhipu (智谱 GLM)",
				DefaultBaseUrl = "https://open.bigmodel.cn/api/paas/v4",
				DefaultModel = "glm-4-flash",
				DefaultLongModel = "glm-4-long",
				HelpText = "智谱开放平台：https://open.bigmodel.cn/"
			},
			new GptProviderInfo
			{
				Id = "Doubao",
				DisplayName = "Doubao (豆包/火山方舟)",
				DefaultBaseUrl = "https://ark.cn-beijing.volces.com/api/v3",
				DefaultModel = "",
				DefaultLongModel = "",
				ModelLabel = "Endpoint ID",
				HelpText = "火山方舟：https://console.volcengine.com/ark ，Model 请填推理接入点 Endpoint ID。"
			},
			new GptProviderInfo
			{
				Id = "SiliconFlow",
				DisplayName = "SiliconFlow (硅基流动)",
				DefaultBaseUrl = "https://api.siliconflow.cn/v1",
				DefaultModel = "Qwen/Qwen2.5-7B-Instruct",
				DefaultLongModel = "Qwen/Qwen2.5-72B-Instruct",
				HelpText = "硅基流动：https://cloud.siliconflow.cn/ 可选用多种开源模型。"
			},
			new GptProviderInfo
			{
				Id = "Baichuan",
				DisplayName = "Baichuan (百川)",
				DefaultBaseUrl = "https://api.baichuan-ai.com/v1",
				DefaultModel = "Baichuan4-Air",
				DefaultLongModel = "Baichuan4-Turbo",
				HelpText = "百川智能：https://platform.baichuan-ai.com/"
			},
			new GptProviderInfo
			{
				Id = "OpenAI",
				DisplayName = "OpenAI (ChatGPT)",
				DefaultBaseUrl = "https://api.openai.com/v1",
				DefaultModel = "gpt-4o-mini",
				DefaultLongModel = "gpt-4o",
				HelpText = "OpenAI Platform：https://platform.openai.com/"
			},
			new GptProviderInfo
			{
				Id = "Groq",
				DisplayName = "Groq",
				DefaultBaseUrl = "https://api.groq.com/openai/v1",
				DefaultModel = "llama-3.3-70b-versatile",
				DefaultLongModel = "llama-3.3-70b-versatile",
				HelpText = "Groq Cloud：https://console.groq.com/"
			},
			new GptProviderInfo
			{
				Id = "AzureOpenAI",
				DisplayName = "Azure OpenAI",
				ApiStyle = GptApiStyle.AzureOpenAi,
				DefaultBaseUrl = "",
				DefaultModel = "",
				DefaultLongModel = "",
				ExtraLabel = "Endpoint (如 https://xxx.openai.azure.com)",
				RequiresExtra = true,
				ModelLabel = "Deployment Name",
				HelpText = "Azure OpenAI：填 API Key、Deployment Name，以及资源 Endpoint。"
			},
			new GptProviderInfo
			{
				Id = "Custom",
				DisplayName = "Custom / Ollama (OpenAI Compatible)",
				DefaultBaseUrl = "http://localhost:11434/v1",
				DefaultModel = "llama3.2",
				DefaultLongModel = "llama3.2",
				ExtraLabel = "Base URL (OpenAI Compatible)",
				RequiresExtra = true,
				HelpText = "任意 OpenAI 兼容接口，例如 Ollama、OneAPI、LocalAI、自建网关。"
			}
		};

		public static IReadOnlyList<IGptProvider> GetProviders()
		{
			return Providers;
		}

		public static IGptProvider GetProvider(string id)
		{
			return Providers.FirstOrDefault(p =>
				string.Equals(p.Id, id, StringComparison.OrdinalIgnoreCase));
		}

		public static string GetApiKeyKey(string providerId)
		{
			return "GptAppKey_" + providerId;
		}

		public static string GetModelKey(string providerId)
		{
			return "GptModel_" + providerId;
		}

		public static string GetLongModelKey(string providerId)
		{
			return "GptLongModel_" + providerId;
		}

		public static string GetExtraKey(string providerId)
		{
			return "GptExtra_" + providerId;
		}
	}
}
