using Assembly.Metro.SharedViewModelUntil;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Net.Http.Headers;
using System.Text.RegularExpressions;
using Assembly.Metro.Controls.PageTemplates.Tools;
using System.Net;
using System.Diagnostics;

namespace Assembly.Tool.GPTservice
{
    internal class GPTstreamClient
    {

        public static async Task GPT_Async(string name)
        {
            //string GptAppId = ConfigManager.GetSetting("Assembly", "GptAppId");
            string GptAppKey = ConfigManager.GetSetting("Assembly", "GptAppKey");
            string GptApp = ConfigManager.GetSetting("Assembly", "GptApp");
            if (GptApp == "Qwen" && GptAppKey != "")
            {
                await QwenGPT_Stream(name, GptAppKey);
            }
            else
            {
                var sharedVM = (SharedViewModel)Application.Current.FindResource("SharedViewModel");
                sharedVM.AiText = "There has been a problem, please try again later";
            }
        }
        public static async Task QwenGPT_NoStream(string name, string GptAppKey)
        {
            string url = $"https://dashscope.aliyuncs.com/compatible-mode/v1/chat/completions";
            using (HttpClient client = new HttpClient())
            {
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", GptAppKey);
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                var jsonData = new Dictionary<string, object>
                {
                    ["model"] = "qwen-plus",
                    ["stream"] = true,
                    ["messages"] = new List<Dictionary<string, string>>
                    {
                        new Dictionary<string, string>
                        {
                            ["role"] = "system",
                            ["content"] = "你只会接受到英文字符，你需要结合Blamite游戏引擎和微软旗下的《光环》游戏，或者是根据常规的游戏引擎知识，返回一段中文翻译及解释，翻译只需要对接受到英文字符中英对照，翻译与解释中间用逗号隔开，格式例如（Flag旗帜），XXXXXXXX。尽量不要使用换行符，可以使用各类括号。"
                        },
                        new Dictionary<string, string>
                        {
                            ["role"] = "user",
                            ["content"] = name  // 这里可以替换为变量
                        }
                    }
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


        public static async Task QwenGPT_Stream(string name, string GptAppKey)
        {
            string url = $"https://dashscope.aliyuncs.com/compatible-mode/v1/chat/completions";
            using (HttpClient client = new HttpClient())
            {
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", GptAppKey);
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                var jsonData = new Dictionary<string, object>
                {
                    ["model"] = "qwen-plus",
                    ["stream"] = true,
                    ["messages"] = new List<Dictionary<string, string>>
                    {
                        new Dictionary<string, string>
                        {
                            ["role"] = "system",
                            ["content"] = "你只会接受到英文字符，你需要结合Blamite游戏引擎和微软旗下的《光环》游戏，或者是根据常规的游戏引擎知识，返回一段中文翻译及解释，翻译只需要对接受到英文字符中英对照，翻译与解释中间用逗号隔开，格式例如（Flag旗帜），XXXXXXXX。尽量不要使用换行符，可以使用各类括号。"
                        },
                        new Dictionary<string, string>
                        {
                            ["role"] = "user",
                            ["content"] = name  // 这里可以替换为变量
                        }
                    }
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
                                    try
                                    {
                                        JObject jsonObject = JObject.Parse(data);
                                        // 提取 content 字段
                                        string content_text = jsonObject["choices"][0]["delta"]["content"]?.ToString();
                                        if (!string.IsNullOrEmpty(content_text))
                                        {
                                            Console.WriteLine(content_text); // 输出调试用
                                            await Task.Delay(50); // 可选延迟
                                            // 更新 UI
                                            await Application.Current.Dispatcher.BeginInvoke(new Action(() =>
                                            {
                                                sharedVM.AiText += content_text;
                                            }));
                                        }
                                    }
                                    catch (Exception ex)
                                    {
                                        Console.WriteLine("解析 JSON 失败: " + ex.Message);
                                    }
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


        public static async Task QwenGPT_Stream_AppId(string name, string GptAppId, string GptAppKey)
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
                sharedVM.AiText = "模型正在处理中，请稍候";
                try
                {
                    var stopwatch = new Stopwatch();
                    stopwatch.Start();
                    HttpResponseMessage response = await client.PostAsync(url, content);
                    stopwatch.Stop();
                    Console.WriteLine($"PostAsync 耗时：{stopwatch.Elapsed.TotalMilliseconds} 毫秒");
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

        public class FileCreateResponse
        {
            public string Id { get; set; }
            public string Purpose { get; set; }
        }

        public static async Task UploadFileAndQueryAsync(string FilePath,string questInfo)
        {

            using (var client = new HttpClient())
            {
                string GptAppKey = ConfigManager.GetSetting("Assembly", "GptAppKey");
                string GptApp = ConfigManager.GetSetting("Assembly", "GptApp");
                string BaseUrl = "https://dashscope.aliyuncs.com/compatible-mode/v1";
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", GptAppKey);
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                string fileidTemp = getFileid(FilePath);
                if(fileidTemp == "")
                {
                    fileidTemp = await UploadFileAndGetFileIdAsync(FilePath, client);
                }
                else
                {
                    string resCode = QueryFile(GptAppKey, fileidTemp);
                    if(resCode == "ERROR")
                    {
                        fileidTemp = await UploadFileAndGetFileIdAsync(FilePath,client);
                        
                    }
                }
                
                var sharedVM = (SharedViewModel)Application.Current.FindResource("SharedViewModel");
                sharedVM.AiLongText = "模型正在处理中，请稍候";
                Console.WriteLine($"文件ID: {fileidTemp}");

                // Step 2: Query with file ID
                var jsonData = new Dictionary<string, object>
                {
                    ["model"] = "qwen-long",
                    ["stream"] = true,
                    ["output_format"] = "text",
                    ["messages"] = new List<Dictionary<string, string>>
                {
                    new Dictionary<string, string>
                    {
                        ["role"] = "system",
                        ["content"] = "根据文档的XML节点进行解析包括翻译，回答要与文档中的内容一致且完整，回答中不要出现任何代码和文件名，不要输出markdown。"
                    },
                    new Dictionary<string, string>
                    {
                        ["role"] = "system",
                        ["content"] = $"fileid://{fileidTemp}"
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

                HttpResponseMessage queryResponse = await client.PostAsync($"{BaseUrl}/chat/completions", content);

                if (queryResponse.IsSuccessStatusCode)
                {
                    using (var stream = await queryResponse.Content.ReadAsStreamAsync())
                    using (var reader = new StreamReader(stream))
                    {
                        string line;
                        sharedVM.AiLongText = "";
                        while ((line = await reader.ReadLineAsync()) != null)
                        {
                            if (string.IsNullOrWhiteSpace(line) || line.StartsWith(":"))
                                continue;

                            if (line.StartsWith("data:"))
                            {
                                string data = line.Substring(5).Trim();
                                Console.Write(data);
                                try
                                {
                                    var jsonObject = JsonConvert.DeserializeObject<dynamic>(data);
                                    string contentText = jsonObject?.choices?[0]?.delta?.content;
                                    await Task.Delay(50); // 可选延迟
                                                            // 更新 UI
                                    await Application.Current.Dispatcher.BeginInvoke(new Action(() =>
                                    {
                                        //contentText = RemoveMarkdown(contentText);
                                        sharedVM.AiLongText += contentText;

                                    }));
                                    Console.Write(contentText); // 实时输出到控制台
                                }
                                catch (Exception ex)
                                {
                                    Console.WriteLine("解析 JSON 失败: " + ex.Message);
                                }
                            }
                        }
                    }
                }
                else
                {
                    string responseBody = await queryResponse.Content.ReadAsStringAsync();
                    dynamic json = JObject.Parse(responseBody);
                    string message = json.error.message;
                    sharedVM.AiLongText = "";
                    sharedVM.AiLongText = "请求发生错误：" + message;
                    Console.WriteLine($"请求失败: {responseBody}");

                }
            }
        }

        public static async Task QwenLongTextQueryAsync(string xmlLongText)
        {
            using (var client = new HttpClient())
            {
                string GptAppKey = ConfigManager.GetSetting("Assembly", "GptAppKey");
                string GptApp = ConfigManager.GetSetting("Assembly", "GptApp");
                string BaseUrl = "https://dashscope.aliyuncs.com/compatible-mode/v1";
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", GptAppKey);
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                var sharedVM = (SharedViewModel)Application.Current.FindResource("SharedViewModel");
                sharedVM.AiLongText = "模型正在处理中，请稍候";
                // Step 2: Query with file ID
                var jsonData = new Dictionary<string, object>
                {
                    ["model"] = "qwen-long",
                    ["stream"] = true,
                    ["output_format"] = "text",
                    ["messages"] = new List<Dictionary<string, string>>
                    {
                        new Dictionary<string, string>
                        {
                            ["role"] = "system",
                            ["content"] = "根据文档的XML节点进行解析包括翻译，回答要与文档中的内容一致且完整，回答中不要出现任何代码和文件名，不要输出markdown。"
                        },
                        new Dictionary<string, string>
                        {
                            ["role"] = "system",
                            ["content"] = xmlLongText
                        },
                        new Dictionary<string, string>
                        {
                            ["role"] = "user",
                            ["content"] = "请根据XML的内容，为我介绍一下这个节点是什么内容，对于游戏有什么影响？"
                        }
                    },
                    ["stream_options"] = new Dictionary<string, bool>
                    {
                        ["include_usage"] = true
                    }
                };
                string jsonContent = JsonConvert.SerializeObject(jsonData, Formatting.Indented);
                HttpContent content = new StringContent(jsonContent, Encoding.UTF8, "application/json");
                HttpResponseMessage queryResponse = await client.PostAsync($"{BaseUrl}/chat/completions", content);
                if (queryResponse.IsSuccessStatusCode)
                {
                    using (var stream = await queryResponse.Content.ReadAsStreamAsync())
                    using (var reader = new StreamReader(stream))
                    {
                        string line;
                        sharedVM.AiLongText = "";
                        while ((line = await reader.ReadLineAsync()) != null)
                        {
                            if (string.IsNullOrWhiteSpace(line) || line.StartsWith(":"))
                                continue;

                            if (line.StartsWith("data:"))
                            {
                                string data = line.Substring(5).Trim();
                                Console.Write(data);
                                try
                                {
                                    var jsonObject = JsonConvert.DeserializeObject<dynamic>(data);
                                    string contentText = jsonObject?.choices?[0]?.delta?.content;
                                    await Task.Delay(50); // 可选延迟
                                                          // 更新 UI
                                    await Application.Current.Dispatcher.BeginInvoke(new Action(() =>
                                    {
                                        //contentText = RemoveMarkdown(contentText);
                                        sharedVM.AiLongText += contentText;

                                    }));
                                    Console.Write(contentText); // 实时输出到控制台
                                }
                                catch (Exception ex)
                                {
                                    Console.WriteLine("解析 JSON 失败: " + ex.Message);
                                }
                            }
                        }
                    }
                }
                else
                {
                    string responseBody = await queryResponse.Content.ReadAsStringAsync();
                    dynamic json = JObject.Parse(responseBody);
                    string message = json.error.message;
                    sharedVM.AiLongText = "";
                    sharedVM.AiLongText = "请求发生错误：" + message;
                    Console.WriteLine($"请求失败: {responseBody}");
                }
            }
        }


        public static async Task<string> UploadFileAndGetFileIdAsync(string filePath, HttpClient client)
        {
            string BaseUrl = "https://dashscope.aliyuncs.com/compatible-mode/v1";
            try
            {
                var fileContent = new StreamContent(File.OpenRead(filePath));
                fileContent.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("application/octet-stream");
                var uploadRequest = new MultipartFormDataContent();
                uploadRequest.Add(fileContent, "file", Path.GetFileName(filePath));
                uploadRequest.Add(new StringContent("file-extract"), "purpose");
                HttpResponseMessage uploadResponse = await client.PostAsync($"{BaseUrl}/files", uploadRequest);
                string uploadResponseBody = await uploadResponse.Content.ReadAsStringAsync();
                if (!uploadResponse.IsSuccessStatusCode)
                {
                    Console.WriteLine($"文件上传失败：{uploadResponseBody}");
                    return null;
                }
                var fileResponse = JsonConvert.DeserializeObject<FileCreateResponse>(uploadResponseBody);
                string fileidTemp = fileResponse?.Id;
                saveFileid(filePath, fileidTemp);
                return fileidTemp;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"上传过程中发生异常：{ex.Message}");
                return null;
            }
        }

        public static string QueryFile(string GptAppKey,string fileId)
        {
            string url = $"https://dashscope.aliyuncs.com/compatible-mode/v1/files/{fileId}";
            var request = (HttpWebRequest)WebRequest.Create(url);
            request.Method = "GET";
            request.Headers["Authorization"] = $"Bearer {GptAppKey}";
            request.ContentType = "application/json";
            try
            {
                using (var response = (HttpWebResponse)request.GetResponse())
                {
                    if (response.StatusCode == HttpStatusCode.NotFound)
                    {
                        return "ERROR";
                    }
                    if (response.StatusCode != HttpStatusCode.OK)
                    {
                        return response.StatusCode.ToString();
                    }
                    using (var reader = new StreamReader(response.GetResponseStream()))
                    {
                        string responseContent = reader.ReadToEnd();
                        return responseContent;
                    }
                }
            }
            catch (WebException ex)
            {
                if (ex.Response is HttpWebResponse webResponse && webResponse.StatusCode == HttpStatusCode.NotFound)
                {
                    return "ERROR";
                }
                return ((HttpWebResponse)ex.Response).StatusCode.ToString();
            }
        }

        public static string RemoveMarkdown(string text)
        {
            if (string.IsNullOrEmpty(text)) return text;
            // 去除加粗/斜体
            text = Regex.Replace(text, @"([*_]{1,3})(.+?)\1", "$2");
            // 去除标题 #
            text = Regex.Replace(text, @"^#{1,6}\s*", "");
            // 去除列表符号
            text = Regex.Replace(text, @"^[\*\+\-]\s+", "", RegexOptions.Multiline);
            // 去除行内代码符号 `
            text = Regex.Replace(text, @"`(.+?)`", "$1");
            // 去除多余换行
            //text = Regex.Replace(text, @"\n{2,}", "\n\n");
            return text.Trim();
        }

        public static string getFileid(String filePath)
        {
            JsonMapFileManager manager = new JsonMapFileManager();
            string fileId = manager.GetFileIdByPath(filePath);
            if (fileId == null)
            {
                return "";
            }
            else
            {
                return fileId;
            }
        }

        public static void saveFileid(String filePath, String fileId)
        {
            JsonMapFileManager manager = new JsonMapFileManager();
            manager.AddFileRecord(filePath, fileId);
        }

    }
}
