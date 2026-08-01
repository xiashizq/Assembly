using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using Assembly.Metro.SharedViewModelUntil;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Assembly.Tool.GPTservice
{
	internal static class OpenAiCompatibleClient
	{
		public const string ShortSystemPrompt =
			"你只会接受到英文字符，你需要结合Blamite游戏引擎和微软旗下的《光环》游戏，或者是根据常规的游戏引擎知识，返回一段中文翻译及解释，翻译只需要对接受到英文字符中英对照，翻译与解释中间用逗号隔开，格式例如（Flag旗帜），XXXXXXXX。尽量不要使用换行符，可以使用各类括号。";

		public const string LongSystemPrompt =
			"根据文档的XML节点进行解析包括翻译，回答要与文档中的内容一致且完整，回答中不要出现任何代码和文件名，不要输出markdown。";

		public static async Task StreamChatAsync(
			IGptProvider provider,
			string apiKey,
			string model,
			string baseUrlOrEndpoint,
			IList<Dictionary<string, string>> messages,
			Action<string> onStatus,
			Action<string> onDelta,
			Action<string> onError)
		{
			if (string.IsNullOrWhiteSpace(apiKey))
			{
				onError?.Invoke("API Key is not configured");
				return;
			}

			if (string.IsNullOrWhiteSpace(model))
			{
				onError?.Invoke("Model is not configured");
				return;
			}

			string url;
			bool azure = provider.ApiStyle == GptApiStyle.AzureOpenAi;
			if (azure)
			{
				string endpoint = (baseUrlOrEndpoint ?? string.Empty).Trim().TrimEnd('/');
				if (string.IsNullOrWhiteSpace(endpoint))
				{
					onError?.Invoke("Azure endpoint is not configured");
					return;
				}
				url = endpoint + "/openai/deployments/" + Uri.EscapeDataString(model)
					+ "/chat/completions?api-version=2024-02-15-preview";
			}
			else
			{
				string baseUrl = string.IsNullOrWhiteSpace(baseUrlOrEndpoint)
					? provider.DefaultBaseUrl
					: baseUrlOrEndpoint.Trim().TrimEnd('/');
				if (string.IsNullOrWhiteSpace(baseUrl))
				{
					onError?.Invoke("Base URL is not configured");
					return;
				}
				url = baseUrl + "/chat/completions";
			}

			using (var client = new HttpClient())
			{
				client.Timeout = TimeSpan.FromMinutes(10);
				if (azure)
					client.DefaultRequestHeaders.Add("api-key", apiKey);
				else
					client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", apiKey);

				client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

				var payload = new Dictionary<string, object>
				{
					["model"] = model,
					["stream"] = true,
					["messages"] = messages
				};

				string json = JsonConvert.SerializeObject(payload);
				HttpContent content = new StringContent(json, Encoding.UTF8, "application/json");
				onStatus?.Invoke("Please wait......");

				try
				{
					HttpResponseMessage response = await client.PostAsync(url, content);
					if (!response.IsSuccessStatusCode)
					{
						string body = await response.Content.ReadAsStringAsync();
						string message = TryExtractError(body) ?? body;
						onError?.Invoke("Request failed (" + (int)response.StatusCode + "): " + message);
						return;
					}

					onStatus?.Invoke(string.Empty);
					using (Stream stream = await response.Content.ReadAsStreamAsync())
					using (var reader = new StreamReader(stream))
					{
						string line;
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
								JObject jsonObject = JObject.Parse(data);
								string delta = jsonObject["choices"]?[0]?["delta"]?["content"]?.ToString();
								if (!string.IsNullOrEmpty(delta))
									onDelta?.Invoke(delta);
							}
							catch
							{
								// Ignore malformed SSE chunks.
							}
						}
					}
				}
				catch (Exception ex)
				{
					onError?.Invoke("Request error: " + ex.Message);
				}
			}
		}

		public static async Task UpdateUiTextAsync(Action<SharedViewModel> update)
		{
			await Application.Current.Dispatcher.BeginInvoke(new Action(() =>
			{
				var sharedVm = (SharedViewModel)Application.Current.FindResource("SharedViewModel");
				update(sharedVm);
			}));
		}

		private static string TryExtractError(string body)
		{
			try
			{
				JObject json = JObject.Parse(body);
				return json["error"]?["message"]?.ToString()
					?? json["message"]?.ToString();
			}
			catch
			{
				return null;
			}
		}
	}
}
