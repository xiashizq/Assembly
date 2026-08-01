using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using Assembly.Metro.Controls.PageTemplates.Tools;
using Assembly.Metro.SharedViewModelUntil;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Assembly.Tool.GPTservice
{
	internal class GPTstreamClient
	{
		public static async Task GPT_Async(string name)
		{
			var sharedVm = GetSharedVm();
			sharedVm.BeginAi();
			sharedVm.AiText = string.Empty;

			IGptProvider provider = ResolveProvider();
			if (provider == null)
			{
				await SetAiTextAsync("Unsupported GPT provider");
				sharedVm.EndAi();
				return;
			}

			string apiKey = ResolveApiKey(provider.Id);
			string model = ResolveModel(provider, false);
			string extra = ResolveExtra(provider.Id);

			var messages = new List<Dictionary<string, string>>
			{
				new Dictionary<string, string> { ["role"] = "system", ["content"] = OpenAiCompatibleClient.ShortSystemPrompt },
				new Dictionary<string, string> { ["role"] = "user", ["content"] = name }
			};

			try
			{
				await OpenAiCompatibleClient.StreamChatAsync(
					provider,
					apiKey,
					model,
					ResolveBaseUrl(provider, extra),
					messages,
					status =>
					{
						if (!string.IsNullOrEmpty(status))
							SetAiText(status);
						else
							SetAiText(string.Empty);
					},
					delta =>
					{
						Application.Current.Dispatcher.BeginInvoke(new Action(() =>
						{
							GetSharedVm().AiText += delta;
						}));
					},
					error => { SetAiText(error); });
			}
			finally
			{
				GetSharedVm().EndAi();
			}
		}

		public static async Task UploadFileAndQueryAsync(string filePath, string questInfo)
		{
			var sharedVm = GetSharedVm();
			sharedVm.BeginAi();
			sharedVm.AiLongText = string.Empty;

			try
			{
				IGptProvider provider = ResolveProvider();
				if (provider == null)
				{
					await SetAiLongTextAsync("Unsupported GPT provider");
					return;
				}

				string apiKey = ResolveApiKey(provider.Id);
				if (string.IsNullOrWhiteSpace(apiKey))
				{
					await SetAiLongTextAsync("API Key is not configured");
					return;
				}

				if (provider.SupportsNativeFileUpload)
				{
					await UploadFileAndQueryQwenAsync(filePath, questInfo, apiKey, provider);
					return;
				}

				string fileText;
				try
				{
					fileText = File.ReadAllText(filePath);
				}
				catch (Exception ex)
				{
					await SetAiLongTextAsync("Failed to read file: " + ex.Message);
					return;
				}

				await LongTextQueryCoreAsync(
					"以下是文档内容：\n" + fileText,
					string.IsNullOrWhiteSpace(questInfo)
						? "请根据文档内容进行解析与翻译说明。"
						: questInfo,
					manageBusyState: false);
			}
			finally
			{
				GetSharedVm().EndAi();
			}
		}

		public static async Task QwenLongTextQueryAsync(string xmlLongText)
		{
			await LongTextQueryAsync(
				xmlLongText,
				"请根据XML的内容，为我介绍一下这个节点是什么内容，对于游戏有什么影响？");
		}

		public static async Task LongTextQueryAsync(string contextText, string userQuestion)
		{
			await LongTextQueryCoreAsync(contextText, userQuestion, manageBusyState: true);
		}

		private static async Task LongTextQueryCoreAsync(string contextText, string userQuestion, bool manageBusyState)
		{
			var sharedVm = GetSharedVm();
			if (manageBusyState)
			{
				sharedVm.BeginAi();
				sharedVm.AiLongText = string.Empty;
			}

			try
			{
				IGptProvider provider = ResolveProvider();
				if (provider == null)
				{
					await SetAiLongTextAsync("Unsupported GPT provider");
					return;
				}

				string apiKey = ResolveApiKey(provider.Id);
				string model = ResolveModel(provider, true);
				string extra = ResolveExtra(provider.Id);

				var messages = new List<Dictionary<string, string>>
				{
					new Dictionary<string, string> { ["role"] = "system", ["content"] = OpenAiCompatibleClient.LongSystemPrompt },
					new Dictionary<string, string> { ["role"] = "system", ["content"] = contextText ?? string.Empty },
					new Dictionary<string, string> { ["role"] = "user", ["content"] = userQuestion ?? string.Empty }
				};

				await OpenAiCompatibleClient.StreamChatAsync(
					provider,
					apiKey,
					model,
					ResolveBaseUrl(provider, extra),
					messages,
					status =>
					{
						if (!string.IsNullOrEmpty(status))
							SetAiLongText(status);
						else
							SetAiLongText(string.Empty);
					},
					delta =>
					{
						Application.Current.Dispatcher.BeginInvoke(new Action(() =>
						{
							GetSharedVm().AiLongText += delta;
						}));
					},
					error => { SetAiLongText(error); });
			}
			finally
			{
				if (manageBusyState)
					GetSharedVm().EndAi();
			}
		}

		private static async Task UploadFileAndQueryQwenAsync(string filePath, string questInfo, string apiKey, IGptProvider provider)
		{
			using (var client = new HttpClient())
			{
				string baseUrl = ResolveBaseUrl(provider, ResolveExtra(provider.Id));
				client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", apiKey);
				client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

				string fileIdTemp = getFileid(filePath);
				if (fileIdTemp == "")
					fileIdTemp = await UploadFileAndGetFileIdAsync(filePath, client, baseUrl);
				else
				{
					string resCode = QueryFile(apiKey, fileIdTemp, baseUrl);
					if (resCode == "ERROR")
						fileIdTemp = await UploadFileAndGetFileIdAsync(filePath, client, baseUrl);
				}

				if (string.IsNullOrEmpty(fileIdTemp))
				{
					await SetAiLongTextAsync("File upload failed");
					return;
				}

				string model = ResolveModel(provider, true);
				var sharedVm = (SharedViewModel)Application.Current.FindResource("SharedViewModel");
				sharedVm.AiLongText = "模型正在处理中，请稍候";

				var jsonData = new Dictionary<string, object>
				{
					["model"] = model,
					["stream"] = true,
					["output_format"] = "text",
					["messages"] = new List<Dictionary<string, string>>
					{
						new Dictionary<string, string>
						{
							["role"] = "system",
							["content"] = OpenAiCompatibleClient.LongSystemPrompt
						},
						new Dictionary<string, string>
						{
							["role"] = "system",
							["content"] = "fileid://" + fileIdTemp
						},
						new Dictionary<string, string>
						{
							["role"] = "user",
							["content"] = questInfo
						}
					},
					["stream_options"] = new Dictionary<string, bool>
					{
						["include_usage"] = true
					}
				};

				string jsonContent = JsonConvert.SerializeObject(jsonData, Formatting.Indented);
				HttpContent content = new StringContent(jsonContent, Encoding.UTF8, "application/json");
				HttpResponseMessage queryResponse = await client.PostAsync(baseUrl + "/chat/completions", content);

				if (!queryResponse.IsSuccessStatusCode)
				{
					string responseBody = await queryResponse.Content.ReadAsStringAsync();
					string message = responseBody;
					try
					{
						message = JObject.Parse(responseBody)["error"]?["message"]?.ToString() ?? responseBody;
					}
					catch
					{
					}
					await SetAiLongTextAsync("请求发生错误：" + message);
					return;
				}

				using (var stream = await queryResponse.Content.ReadAsStreamAsync())
				using (var reader = new StreamReader(stream))
				{
					string line;
					sharedVm.AiLongText = "";
					while ((line = await reader.ReadLineAsync()) != null)
					{
						if (string.IsNullOrWhiteSpace(line) || line.StartsWith(":"))
							continue;
						if (!line.StartsWith("data:"))
							continue;

						string data = line.Substring(5).Trim();
						if (data == "[DONE]")
							break;

						try
						{
							var jsonObject = JsonConvert.DeserializeObject<dynamic>(data);
							string contentText = jsonObject?.choices?[0]?.delta?.content;
							if (string.IsNullOrEmpty(contentText))
								continue;

							await Application.Current.Dispatcher.BeginInvoke(new Action(() =>
							{
								sharedVm.AiLongText += contentText;
							}));
						}
						catch
						{
						}
					}
				}
			}
		}

		public static async Task<string> UploadFileAndGetFileIdAsync(string filePath, HttpClient client, string baseUrl = null)
		{
			if (string.IsNullOrWhiteSpace(baseUrl))
				baseUrl = "https://dashscope.aliyuncs.com/compatible-mode/v1";

			try
			{
				var fileContent = new StreamContent(File.OpenRead(filePath));
				fileContent.Headers.ContentType = new MediaTypeHeaderValue("application/octet-stream");
				var uploadRequest = new MultipartFormDataContent();
				uploadRequest.Add(fileContent, "file", Path.GetFileName(filePath));
				uploadRequest.Add(new StringContent("file-extract"), "purpose");
				HttpResponseMessage uploadResponse = await client.PostAsync(baseUrl.TrimEnd('/') + "/files", uploadRequest);
				string uploadResponseBody = await uploadResponse.Content.ReadAsStringAsync();
				if (!uploadResponse.IsSuccessStatusCode)
					return null;

				var fileResponse = JsonConvert.DeserializeObject<FileCreateResponse>(uploadResponseBody);
				string fileIdTemp = fileResponse?.Id;
				saveFileid(filePath, fileIdTemp);
				return fileIdTemp;
			}
			catch
			{
				return null;
			}
		}

		public static string QueryFile(string gptAppKey, string fileId, string baseUrl = null)
		{
			if (string.IsNullOrWhiteSpace(baseUrl))
				baseUrl = "https://dashscope.aliyuncs.com/compatible-mode/v1";

			string url = baseUrl.TrimEnd('/') + "/files/" + fileId;
			var request = (HttpWebRequest)WebRequest.Create(url);
			request.Method = "GET";
			request.Headers["Authorization"] = "Bearer " + gptAppKey;
			request.ContentType = "application/json";
			try
			{
				using (var response = (HttpWebResponse)request.GetResponse())
				{
					if (response.StatusCode == HttpStatusCode.NotFound)
						return "ERROR";
					using (var reader = new StreamReader(response.GetResponseStream()))
						return reader.ReadToEnd();
				}
			}
			catch (WebException ex)
			{
				if (ex.Response is HttpWebResponse webResponse && webResponse.StatusCode == HttpStatusCode.NotFound)
					return "ERROR";
				return "ERROR";
			}
		}

		public class FileCreateResponse
		{
			public string Id { get; set; }
			public string Purpose { get; set; }
		}

		public static string RemoveMarkdown(string text)
		{
			if (string.IsNullOrEmpty(text))
				return text;
			text = Regex.Replace(text, @"([*_]{1,3})(.+?)\1", "$2");
			text = Regex.Replace(text, @"^#{1,6}\s*", "");
			text = Regex.Replace(text, @"^[\*\+\-]\s+", "", RegexOptions.Multiline);
			text = Regex.Replace(text, @"`(.+?)`", "$1");
			return text.Trim();
		}

		public static string getFileid(string filePath)
		{
			var manager = new JsonMapFileManager();
			return manager.GetFileIdByPath(filePath) ?? "";
		}

		public static void saveFileid(string filePath, string fileId)
		{
			var manager = new JsonMapFileManager();
			manager.AddFileRecord(filePath, fileId);
		}

		private static IGptProvider ResolveProvider()
		{
			string gptApp = ConfigManager.GetSetting("Assembly", "GptApp", "Qwen");
			return GptProviderRegistry.GetProvider(gptApp) ?? GptProviderRegistry.GetProvider("Qwen");
		}

		private static string ResolveApiKey(string providerId)
		{
			string key = ConfigManager.GetSetting("Assembly", GptProviderRegistry.GetApiKeyKey(providerId));
			if (string.IsNullOrWhiteSpace(key))
				key = ConfigManager.GetSetting("Assembly", "GptAppKey");
			return key ?? string.Empty;
		}

		private static string ResolveModel(IGptProvider provider, bool longContext)
		{
			string key = longContext
				? GptProviderRegistry.GetLongModelKey(provider.Id)
				: GptProviderRegistry.GetModelKey(provider.Id);
			string model = ConfigManager.GetSetting("Assembly", key);
			if (string.IsNullOrWhiteSpace(model))
				model = ConfigManager.GetSetting("Assembly", GptProviderRegistry.GetModelKey(provider.Id));
			if (string.IsNullOrWhiteSpace(model))
			{
				// Doubao uses Extra/Model field interchangeably for endpoint id.
				model = ConfigManager.GetSetting("Assembly", GptProviderRegistry.GetExtraKey(provider.Id));
			}
			if (string.IsNullOrWhiteSpace(model))
				model = longContext ? provider.DefaultLongModel : provider.DefaultModel;
			return model ?? string.Empty;
		}

		private static string ResolveExtra(string providerId)
		{
			return ConfigManager.GetSetting("Assembly", GptProviderRegistry.GetExtraKey(providerId)) ?? string.Empty;
		}

		private static string ResolveBaseUrl(IGptProvider provider, string extra)
		{
			if (provider.ApiStyle == GptApiStyle.AzureOpenAi || provider.Id == "Custom")
				return extra;
			if (!string.IsNullOrWhiteSpace(extra) &&
				(extra.StartsWith("http://", StringComparison.OrdinalIgnoreCase)
				 || extra.StartsWith("https://", StringComparison.OrdinalIgnoreCase)))
				return extra;
			return provider.DefaultBaseUrl;
		}

		private static SharedViewModel GetSharedVm()
		{
			return (SharedViewModel)Application.Current.FindResource("SharedViewModel");
		}

		private static void SetAiText(string text)
		{
			GetSharedVm().AiText = text ?? string.Empty;
		}

		private static void SetAiLongText(string text)
		{
			GetSharedVm().AiLongText = text ?? string.Empty;
		}

		private static Task SetAiTextAsync(string text)
		{
			SetAiText(text);
			return Task.CompletedTask;
		}

		private static Task SetAiLongTextAsync(string text)
		{
			SetAiLongText(text);
			return Task.CompletedTask;
		}
	}
}
