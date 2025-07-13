using System;
using System.Text;
using System.IO;

namespace Assembly.Metro.Controls.PageTemplates.Tools
{
    public class GetXmlStringByLine
    {
        private readonly string[] _lines;

        public GetXmlStringByLine(string xmlFilePath)
        {
            if (!File.Exists(xmlFilePath))
                throw new FileNotFoundException("XML file not found.", xmlFilePath);

            _lines = File.ReadAllLines(xmlFilePath);
        }

        public string GetNodeXmlAtLine(int lineNumber)
        {
            if (lineNumber < 1 || lineNumber > _lines.Length)
                throw new ArgumentOutOfRangeException(nameof(lineNumber));

            int idx = lineNumber - 1;
            string line = _lines[idx].Trim();

            // 1. 自闭合标签
            if (IsSelfClosingTag(line))
                return _lines[idx].Trim();

            // 2. 结束标签，向上找到对应起始标签
            if (IsEndTag(line))
            {
                int startIdx = FindStartTagUpwards(idx);
                if (startIdx == -1) return line; // 若找不到则返回自己
                return GetFullNodeXmlFromStart(startIdx);
            }

            // 3. 起始标签，查找配对结束标签
            if (IsStartTag(line))
            {
                return GetFullNodeXmlFromStart(idx);
            }

            // 4. 其他情况（如在节点内部），向上找到最近的起始标签
            int nodeStart = FindStartTagUpwards(idx);
            if (nodeStart != -1)
            {
                return GetFullNodeXmlFromStart(nodeStart);
            }
            // 默认返回当前行
            return _lines[idx].Trim();
        }

        private bool IsSelfClosingTag(string line)
        {
            // <xxx ... /> 并排除注释和声明
            return line.StartsWith("<") && line.EndsWith("/>") && !line.StartsWith("<!--") && !line.StartsWith("<?");
        }

        private bool IsStartTag(string line)
        {
            // <xxx ...> 但不是自闭合、不是声明、不是注释、不是结束标签
            return line.StartsWith("<") && !line.StartsWith("</") && !line.StartsWith("<?") && !line.StartsWith("<!--") && !IsSelfClosingTag(line);
        }

        private bool IsEndTag(string line)
        {
            // </xxx>
            return line.StartsWith("</");
        }

        private string GetTagName(string line)
        {
            // 获取标签名
            if (!line.StartsWith("<")) return "";
            int i = line.StartsWith("</") ? 2 : 1;
            int end = line.IndexOfAny(new[] { ' ', '>', '/' }, i);
            if (end == -1) end = line.Length;
            return line.Substring(i, end - i);
        }

        private int FindStartTagUpwards(int idx)
        {
            int level = 0;
            string goalTag = null;
            for (int i = idx; i >= 0; i--)
            {
                string l = _lines[i].Trim();
                if (IsEndTag(l))
                {
                    if (goalTag == null)
                        goalTag = GetTagName(l);
                    if (GetTagName(l) == goalTag)
                        level++;
                }
                else if (IsStartTag(l))
                {
                    if (goalTag == null || GetTagName(l) == goalTag)
                    {
                        if (level == 0)
                            return i;
                        level--;
                    }
                }
            }
            return -1;
        }

        private int FindMatchingEndTag(int startIdx, string tagName)
        {
            int depth = 0;
            for (int i = startIdx; i < _lines.Length; i++)
            {
                string l = _lines[i].Trim();
                if (IsStartTag(l) && GetTagName(l) == tagName)
                    depth++;
                else if (IsEndTag(l) && GetTagName(l) == tagName)
                {
                    depth--;
                    if (depth == 0) return i;
                }
            }
            return -1;
        }

        private string GetFullNodeXmlFromStart(int startIdx)
        {
            string line = _lines[startIdx].Trim();
            if (IsSelfClosingTag(line))
                return _lines[startIdx].Trim();

            string tagName = GetTagName(line);
            int end = FindMatchingEndTag(startIdx, tagName);
            if (end == -1) return _lines[startIdx].Trim(); // 未找到闭合则返回自身
            StringBuilder sb = new StringBuilder();
            for (int i = startIdx; i <= end; i++)
                sb.AppendLine(_lines[i]);
            return sb.ToString().TrimEnd();
        }
    }
}
