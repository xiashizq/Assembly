using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace Assembly.Tool.TranslateService
{
    public class PublicTranslateService
    {
        public static string TranslateAsync(string q)
        {
            string translationAppId = ConfigManager.GetSetting("Assembly", "TranslationAppId");
            string translationSecretKey = ConfigManager.GetSetting("Assembly", "TranslationSecretKey");
            string translationApp = ConfigManager.GetSetting("Assembly", "TranslationApp");
            string targetlanguage = ConfigManager.GetSetting("Assembly", "TranslationTargetlanguage");
            if (translationApp == "Baidu" && translationAppId!= "" && translationSecretKey!= "") 
            {
                try
                {
                    return BaiduTranslateAsync(q, targetlanguage, translationAppId, translationSecretKey);
                }catch (Exception e)
                {
                    return e.ToString();
                }
            }
            else
            {
                return "Translation not found";
            }
        }
        internal static string BaiduTranslateAsync(string q, string targetlanguage, string appId, string secretKey)
        {
            // 源语言
            string from = "en";
            // 目标语言
            string to = targetlanguage;
            Random rd = new Random();
            string salt = rd.Next(100000).ToString();
            string sign = EncryptString(appId + q + salt + secretKey);
            string url = "http://api.fanyi.baidu.com/api/trans/vip/translate?";
            url += "q=" + HttpUtility.UrlEncode(q);
            url += "&from=" + from;
            url += "&to=" + to;
            url += "&appid=" + appId;
            url += "&salt=" + salt;
            url += "&sign=" + sign;
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
            request.Method = "GET";
            request.ContentType = "text/html;charset=UTF-8";
            request.UserAgent = null;
            request.Timeout = 2000;
            HttpWebResponse response = (HttpWebResponse)request.GetResponse();
            Stream myResponseStream = response.GetResponseStream();
            StreamReader myStreamReader = new StreamReader(myResponseStream, Encoding.GetEncoding("utf-8"));
            string retString = myStreamReader.ReadToEnd();
            // 解析 JSON
            JObject jsonObject = JObject.Parse(retString);
            // 提取 dst 的值
            string dstValue = jsonObject["trans_result"][0]["dst"].ToString();
            Console.WriteLine("dst: " + dstValue); // 输出: dst: 苹果
            return dstValue;
        }

        public static string EncryptString(string str)
        {
            MD5 md5 = MD5.Create();
            // 将字符串转换成字节数组
            byte[] byteOld = Encoding.UTF8.GetBytes(str);
            // 调用加密方法
            byte[] byteNew = md5.ComputeHash(byteOld);
            // 将加密结果转换为字符串
            StringBuilder sb = new StringBuilder();
            foreach (byte b in byteNew)
            {
                // 将字节转换成16进制表示的字符串，
                sb.Append(b.ToString("x2"));
            }
            // 返回加密的字符串
            return sb.ToString();
        }
    }
}
