using Assembly.Metro.SharedViewModelUntil;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Assembly.Tool.GPTservice
{
    internal class GPTstreamClient
    {
        public static async Task GPT_Async(string name)
        {
            string GptAppId = ConfigManager.GetSetting("Assembly", "GptAppId");
            string GptAppKey = ConfigManager.GetSetting("Assembly", "GptAppKey");
            string GptApp = ConfigManager.GetSetting("Assembly", "GptApp");
            if (GptApp == "Qwen" && GptAppId != "" && GptAppKey != "")
            {
                await QwenGPT_Stream(name, GptAppId, GptAppKey);
            }
            else
            {
                var sharedVM = (SharedViewModel)Application.Current.FindResource("SharedViewModel");
                sharedVM.AiText = "There has been a problem, please try again later";
            }
        }
        public static async Task QwenGPT_NoStream(string name, string GptAppId, string GptAppKey)
        {
            string url = $"https://dashscope.aliyuncs.com/api/v1/apps/{GptAppId}/completion";
            using (HttpClient client = new HttpClient())
            {
                client.DefaultRequestHeaders.Add("Authorization", $"Bearer {GptAppKey}");
                var jsonData = new Dictionary<string, object>
                {
                    ["input"] = new Dictionary<string, string>
                    {
                        ["prompt"] = name
                    },
                    ["parameters"] = new Dictionary<string, string>(),
                    ["debug"] = new Dictionary<string, string>()
                };
                // 将字典序列化为JSON字符串
                string jsonContent = JsonConvert.SerializeObject(jsonData, Formatting.Indented);
                HttpContent content = new StringContent(jsonContent, Encoding.UTF8, "application/json");
                var sharedVM = (SharedViewModel)Application.Current.FindResource("SharedViewModel");
                try
                {
                    HttpResponseMessage response = await client.PostAsync(url, content);

                    if (response.IsSuccessStatusCode)
                    {
                        string responseBody = await response.Content.ReadAsStringAsync();
                        JObject jsonObject = JObject.Parse(responseBody);
                        // 提取 'text' 字段
                        string text = jsonObject["output"]["text"].ToString();
                        Console.WriteLine("Request successful:");
                        sharedVM.AiText = text;
                    }
                    else
                    {
                        Console.WriteLine($"Request failed with status code: {response.StatusCode}");
                        string responseBody = await response.Content.ReadAsStringAsync();
                        Console.WriteLine(responseBody);
                    }
                }
                catch (System.Exception ex)
                {
                    Console.WriteLine($"Error calling DashScope: {ex.Message}");
                }
            }
        }


        public static async Task QwenGPT_Stream(string name,string GptAppId, string GptAppKey)
        {
            string url = $"https://dashscope.aliyuncs.com/api/v1/apps/{GptAppId}/completion";
            using (HttpClient client = new HttpClient())
            {
                client.DefaultRequestHeaders.Add("Authorization", $"Bearer {GptAppKey}");
                client.DefaultRequestHeaders.Add("X-DashScope-SSE", "enable");

                var jsonData = new Dictionary<string, object>
                {
                    ["input"] = new Dictionary<string, string> { ["prompt"] = name },
                    ["parameters"] = new Dictionary<string, bool> { ["incremental_output"] = true },
                    ["debug"] = new Dictionary<string, string>()
                };
                string jsonContent = Newtonsoft.Json.JsonConvert.SerializeObject(jsonData, Newtonsoft.Json.Formatting.Indented);
                HttpContent content = new StringContent(jsonContent, Encoding.UTF8, "application/json");
                var sharedVM = (SharedViewModel)Application.Current.FindResource("SharedViewModel");
                sharedVM.AiText = "Please wait......";
                try
                {
                    HttpResponseMessage response = await client.PostAsync(url, content);
                    if (response.IsSuccessStatusCode)
                    {
                        using (var stream = await response.Content.ReadAsStreamAsync())
                        using (var reader = new StreamReader(stream))
                        {
                            string line;
                            sharedVM.AiText = "";
                            while ((line = await reader.ReadLineAsync()) != null)
                            {
                                if (string.IsNullOrWhiteSpace(line) || line.StartsWith(":"))
                                    continue; // 忽略注释和空行
                                if (line.StartsWith("data:"))
                                {
                                    string data = line.Substring(5).Trim();
                                    JObject jsonObject = JObject.Parse(data);
                                    // 提取 'text' 字段
                                    Console.WriteLine(data);
                                    string text = jsonObject["output"]["text"].ToString();
                                    Console.WriteLine(text);
                                    sharedVM.AiText += text;
                                }
                            }
                        }
                    }
                    else
                    {
                        Console.WriteLine($"Request failed with status code: {response.StatusCode}");
                        string responseBody = await response.Content.ReadAsStringAsync();
                        Console.WriteLine(responseBody);
                    }
                }
                catch (System.Exception ex)
                {
                    Console.WriteLine($"Error calling DashScope: {ex.Message}");
                }
            }
        }
    }
}
