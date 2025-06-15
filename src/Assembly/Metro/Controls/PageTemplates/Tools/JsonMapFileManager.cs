using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace Assembly.Metro.Controls.PageTemplates.Tools
{
    public class JsonMapFileManager
    {
        private readonly string _filePath;

        // 构造函数初始化文件路径
        public JsonMapFileManager()
        {
            string appDir = Path.GetDirectoryName(typeof(App).Assembly.Location);
            string folderPath = Path.Combine(appDir, "mmyj");
            _filePath = Path.Combine(folderPath, "data.json");

            if (!Directory.Exists(folderPath))
            {
                Directory.CreateDirectory(folderPath);
            }
        }

        // 方法1：根据 filePath 查询 fileID（查不到返回 null）
        public string GetFileIdByPath(string filePath)
        {
            if (!File.Exists(_filePath))
            {
                return null;
            }
            string json = File.ReadAllText(_filePath);
            List<MapFileRecord> records = JsonSerializer.Deserialize<List<MapFileRecord>>(json);
            var record = records?.FirstOrDefault(r => r.FilePath == filePath);
            return record?.FileID;
        }

        // 方法2：写入新的 filePath 和 fileID，若已存在则先删除再新增
        public void AddFileRecord(string filePath, string fileID)
        {
            List<MapFileRecord> records = new List<MapFileRecord>();
            if (File.Exists(_filePath))
            {
                string json = File.ReadAllText(_filePath);
                records = JsonSerializer.Deserialize<List<MapFileRecord>>(json) ?? records;
            }
            // 如果已存在相同的 FilePath，移除它
            var existing = records.FirstOrDefault(r => r.FilePath == filePath);
            if (existing != null)
            {
                records.Remove(existing);
            }
            // 添加新记录
            records.Add(new MapFileRecord { FilePath = filePath, FileID = fileID });
            // 序列化并保存到文件
            string output = JsonSerializer.Serialize(records, new JsonSerializerOptions { WriteIndented = true });
            File.WriteAllText(_filePath, output);
        }
    }
}
