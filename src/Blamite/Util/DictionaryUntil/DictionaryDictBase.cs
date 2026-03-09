using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Blamite.Util.DictionaryUntil
{
    internal abstract class DictionaryDictBase
    {
        // 大字典
        protected abstract Dictionary<string, string> Dictionary { get; }

        // 拆分后的字典Parts（每个Part最多300条数据）
        private List<Dictionary<string, string>> dictionaryPartList = new List<Dictionary<string, string>>();

        // 初始化标志
        private bool isInitialized = false;

        // 初始化字典拆分操作
        public void InitializeDictionary()
        {
            // 确保字典只初始化一次
            if (isInitialized)
                return;
            // 将完整字典拆分成多个Part，每个Part最多300条数据
            SplitDictionaryIntoParts(Dictionary, 400);
            // 标记初始化完成
            isInitialized = true;
        }

        // 拆分字典
        private void SplitDictionaryIntoParts(Dictionary<string, string> completeDictionary, int partSize)
        {
            var keys = completeDictionary.Keys.ToList();
            int totalParts = (int)Math.Ceiling(keys.Count / (double)partSize);

            // 拆分字典，确保线程安全
            lock (dictionaryPartList)
            {
                for (int i = 0; i < totalParts; i++)
                {
                    var part = new Dictionary<string, string>();

                    // 获取当前Part的条目
                    var partKeys = keys.Skip(i * partSize).Take(partSize);

                    foreach (var key in partKeys)
                    {
                        part.Add(key, completeDictionary[key]);
                    }

                    dictionaryPartList.Add(part);
                }
            }
        }

        // 查询翻译
        public string GetTranslation(string englishTerm)
        {
            // 确保字典已初始化
            InitializeDictionary();
            string result = "Translation not found.";
            object lockObj = new object();
            // 并行查询各个Part
            Parallel.ForEach(dictionaryPartList, part =>
            {
                CheckDictionary(part, englishTerm, ref result, lockObj);
            });
            return result;
        }

        // 检查每个字典Part
        private void CheckDictionary(Dictionary<string, string> dictionary, string key, ref string result, object lockObj)
        {
            if (dictionary.TryGetValue(key, out string value))
            {
                lock (lockObj)
                {
                    result = value;
                }
            }
        }
    }
}