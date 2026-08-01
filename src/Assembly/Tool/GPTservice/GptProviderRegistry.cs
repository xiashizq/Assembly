using System;
using System.Collections.Generic;
using System.Linq;

namespace Assembly.Tool.GPTservice
{
	internal static class GptProviderRegistry
	{
		private static readonly IGptProvider[] Providers =
		{
			// ---- Domestic ----
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

			// ---- International ----
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
				Id = "Anthropic",
				DisplayName = "Anthropic (Claude)",
				ApiStyle = GptApiStyle.Anthropic,
				DefaultBaseUrl = "https://api.anthropic.com",
				DefaultModel = "claude-sonnet-4-20250514",
				DefaultLongModel = "claude-sonnet-4-20250514",
				HelpText = "Anthropic Console：https://console.anthropic.com/ 使用 x-api-key。"
			},
			new GptProviderInfo
			{
				Id = "Gemini",
				DisplayName = "Google Gemini",
				DefaultBaseUrl = "https://generativelanguage.googleapis.com/v1beta/openai",
				DefaultModel = "gemini-2.0-flash",
				DefaultLongModel = "gemini-2.0-flash",
				HelpText = "Google AI Studio：https://aistudio.google.com/apikey （OpenAI 兼容接口）。"
			},
			new GptProviderInfo
			{
				Id = "xAI",
				DisplayName = "xAI (Grok)",
				DefaultBaseUrl = "https://api.x.ai/v1",
				DefaultModel = "grok-2-latest",
				DefaultLongModel = "grok-2-latest",
				HelpText = "xAI Console：https://console.x.ai/"
			},
			new GptProviderInfo
			{
				Id = "Mistral",
				DisplayName = "Mistral AI",
				DefaultBaseUrl = "https://api.mistral.ai/v1",
				DefaultModel = "mistral-small-latest",
				DefaultLongModel = "mistral-large-latest",
				HelpText = "Mistral Console：https://console.mistral.ai/"
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
				Id = "OpenRouter",
				DisplayName = "OpenRouter",
				DefaultBaseUrl = "https://openrouter.ai/api/v1",
				DefaultModel = "openai/gpt-4o-mini",
				DefaultLongModel = "anthropic/claude-sonnet-4",
				HelpText = "OpenRouter：https://openrouter.ai/ 可统一调用多家国际模型。"
			},
			new GptProviderInfo
			{
				Id = "Together",
				DisplayName = "Together AI",
				DefaultBaseUrl = "https://api.together.xyz/v1",
				DefaultModel = "meta-llama/Meta-Llama-3.1-8B-Instruct-Turbo",
				DefaultLongModel = "meta-llama/Meta-Llama-3.1-70B-Instruct-Turbo",
				HelpText = "Together AI：https://api.together.xyz/"
			},
			new GptProviderInfo
			{
				Id = "Fireworks",
				DisplayName = "Fireworks AI",
				DefaultBaseUrl = "https://api.fireworks.ai/inference/v1",
				DefaultModel = "accounts/fireworks/models/llama-v3p1-8b-instruct",
				DefaultLongModel = "accounts/fireworks/models/llama-v3p1-70b-instruct",
				HelpText = "Fireworks AI：https://fireworks.ai/"
			},
			new GptProviderInfo
			{
				Id = "Perplexity",
				DisplayName = "Perplexity",
				DefaultBaseUrl = "https://api.perplexity.ai",
				DefaultModel = "sonar",
				DefaultLongModel = "sonar-pro",
				HelpText = "Perplexity API：https://www.perplexity.ai/settings/api"
			},
			new GptProviderInfo
			{
				Id = "Cerebras",
				DisplayName = "Cerebras",
				DefaultBaseUrl = "https://api.cerebras.ai/v1",
				DefaultModel = "llama3.1-8b",
				DefaultLongModel = "llama3.1-70b",
				HelpText = "Cerebras Cloud：https://cloud.cerebras.ai/"
			},
			new GptProviderInfo
			{
				Id = "DeepInfra",
				DisplayName = "DeepInfra",
				DefaultBaseUrl = "https://api.deepinfra.com/v1/openai",
				DefaultModel = "meta-llama/Meta-Llama-3.1-8B-Instruct",
				DefaultLongModel = "meta-llama/Meta-Llama-3.1-70B-Instruct",
				HelpText = "DeepInfra：https://deepinfra.com/"
			},
			new GptProviderInfo
			{
				Id = "HuggingFace",
				DisplayName = "Hugging Face",
				DefaultBaseUrl = "https://router.huggingface.co/v1",
				DefaultModel = "meta-llama/Meta-Llama-3.1-8B-Instruct",
				DefaultLongModel = "meta-llama/Meta-Llama-3.1-70B-Instruct",
				HelpText = "Hugging Face Inference：https://huggingface.co/settings/tokens"
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
				HelpText = "任意 OpenAI 兼容接口，例如 Ollama、OneAPI、LocalAI、LiteLLM、自建网关。"
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
