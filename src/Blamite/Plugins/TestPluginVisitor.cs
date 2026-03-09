using System.Diagnostics;
using System.Text;
using Blamite.Blam.Shaders;

namespace Blamite.Plugins
{
	public class TestPluginVisitor : IPluginVisitor
	{
		private int _level = 0;
		private int _indentSize = 4;
		private string Indent()
		{
			var sb = new StringBuilder();
			for (var i = 0; i < (_level * _indentSize); i++)
				sb.Append(' ');

			return sb.ToString();
		}

		public bool EnterPlugin(int baseSize)
		{
			Debug.WriteLine(Indent() + "Plugin, baseSize = 0x{0:X}", baseSize);
			_level++;
			return true;
		}

		public void LeavePlugin()
		{
			_level--;
		}

		public bool EnterRevisions()
		{
			Debug.WriteLine(Indent() + "Version history:");
			_level++;
			return true;
		}

		public void VisitRevision(PluginRevision revision)
		{
			Debug.WriteLine(Indent() + "Plugin version {0} by {1}: {2}", revision.Version, revision.Researcher, revision.Description);
		}

		public void LeaveRevisions()
		{
			_level--;
		}

		public void VisitComment(string title, string text, uint pluginLine)
		{
			Debug.WriteLine(Indent() + "Comment \"{0}\": {1}", title, text);
		}

		public void VisitUInt8(string name, uint offset, bool visible, uint pluginLine, string tooltip, string tra,string tooltipTra)
		{
			PrintBasicValue("UInt8", name, offset, visible, tooltip,tra,tooltipTra);
		}

		public void VisitInt8(string name, uint offset, bool visible, uint pluginLine, string tooltip, string tra,string tooltipTra)
		{
			PrintBasicValue("Int8", name, offset, visible, tooltip, tra, tooltipTra);
		}

		public void VisitUInt16(string name, uint offset, bool visible, uint pluginLine, string tooltip, string tra,string tooltipTra)
		{
			PrintBasicValue("UInt16", name, offset, visible, tooltip, tra, tooltipTra);
		}

		public void VisitInt16(string name, uint offset, bool visible, uint pluginLine, string tooltip, string tra,string tooltipTra)
		{
			PrintBasicValue("Int16", name, offset, visible, tooltip, tra, tooltipTra);
		}

		public void VisitUInt32(string name, uint offset, bool visible, uint pluginLine, string tooltip, string tra,string tooltipTra)
		{
			PrintBasicValue("UInt32", name, offset, visible, tooltip, tra, tooltipTra);
		}

		public void VisitInt32(string name, uint offset, bool visible, uint pluginLine, string tooltip, string tra,string tooltipTra)
		{
			PrintBasicValue("Int32", name, offset, visible, tooltip, tra, tooltipTra);
		}

		public void VisitUInt64(string name, uint offset, bool visible, uint pluginLine, string tooltip, string tra,string tooltipTra)
		{
			PrintBasicValue("UInt64", name, offset, visible, tooltip, tra, tooltipTra);
		}

		public void VisitInt64(string name, uint offset, bool visible, uint pluginLine, string tooltip, string tra,string tooltipTra)
		{
			PrintBasicValue("Int64", name, offset, visible, tooltip, tra, tooltipTra);
		}

		public void VisitFloat32(string name, uint offset, bool visible, uint pluginLine, string tooltip, string tra,string tooltipTra)
		{
			PrintBasicValue("Float32", name, offset, visible, tooltip, tra, tooltipTra);
		}

		public void VisitUndefined(string name, uint offset, bool visible, uint pluginLine, string tooltip, string tra,string tooltipTra)
		{
			PrintBasicValue("Undefined", name, offset, visible, tooltip, tra, tooltipTra);
		}

		public void VisitPoint2(string name, uint offset, bool visible, uint pluginLine, string tooltip, string tra,string tooltipTra)
		{
			PrintBasicValue("Point2", name, offset, visible, tooltip, tra, tooltipTra);
		}

		public void VisitPoint3(string name, uint offset, bool visible, uint pluginLine, string tooltip, string tra,string tooltipTra)
		{
			PrintBasicValue("Point3", name, offset, visible, tooltip, tra, tooltipTra);
		}

		public void VisitVector2(string name, uint offset, bool visible, uint pluginLine, string tooltip, string tra,string tooltipTra)
		{
			PrintBasicValue("Vector2", name, offset, visible, tooltip, tra, tooltipTra);
		}

		public void VisitVector3(string name, uint offset, bool visible, uint pluginLine, string tooltip, string tra,string tooltipTra)
		{
			PrintBasicValue("Vector3", name, offset, visible, tooltip, tra, tooltipTra);
		}

		public void VisitVector4(string name, uint offset, bool visible, uint pluginLine, string tooltip, string tra,string tooltipTra)
		{
			PrintBasicValue("Vector4", name, offset, visible, tooltip, tra, tooltipTra);
		}

		public void VisitDegree(string name, uint offset, bool visible, uint pluginLine, string tooltip, string tra,string tooltipTra)
		{
			PrintBasicValue("Degree", name, offset, visible, tooltip, tra, tooltipTra);
		}

		public void VisitDegree2(string name, uint offset, bool visible, uint pluginLine, string tooltip, string tra,string tooltipTra)
		{
			PrintBasicValue("Degree2", name, offset, visible, tooltip, tra, tooltipTra);
		}

		public void VisitDegree3(string name, uint offset, bool visible, uint pluginLine, string tooltip, string tra,string tooltipTra)
		{
			PrintBasicValue("Degree3", name, offset, visible, tooltip, tra, tooltipTra);
		}

		public void VisitPlane2(string name, uint offset, bool visible, uint pluginLine, string tooltip, string tra,string tooltipTra)
		{
			PrintBasicValue("Plane2", name, offset, visible, tooltip, tra, tooltipTra);
		}

		public void VisitPlane3(string name, uint offset, bool visible, uint pluginLine, string tooltip, string tra,string tooltipTra)
		{
			PrintBasicValue("Plane3", name, offset, visible, tooltip, tra, tooltipTra);
		}

		public void VisitRect16(string name, uint offset, bool visible, uint pluginLine, string tooltip, string tra,string tooltipTra)
		{
			PrintBasicValue("Rect16", name, offset, visible, tooltip, tra, tooltipTra);
		}

		public void VisitQuat16(string name, uint offset, bool visible, uint pluginLine, string tooltip, string tra,string tooltipTra)
		{
			PrintBasicValue("Quat16", name, offset, visible, tooltip, tra, tooltipTra);
		}

		public void VisitPoint16(string name, uint offset, bool visible, uint pluginLine, string tooltip, string tra,string tooltipTra)
		{
			PrintBasicValue("Point16", name, offset, visible, tooltip, tra, tooltipTra);
		}

		public void VisitStringID(string name, uint offset, bool visible, uint pluginLine, string tooltip, string tra,string tooltipTra)
		{
			PrintBasicValue("StringID", name, offset, visible, tooltip, tra, tooltipTra);
		}

		public void VisitTagReference(string name, uint offset, bool visible, bool withGroup, uint pluginLine, string tooltip, string tra,string tooltipTra)
		{
			PrintBasicValue("Tag reference", name, offset, visible, tooltip, tra, tooltipTra);
		}

		public bool EnterFlags8(string name, uint offset, bool visible, uint pluginLine, string tooltip, string tra,string tooltipTra)
		{
			PrintBasicValue("Flags8", name, offset, visible, tooltip, tra, tooltipTra);
			_level++;
			return true;
		}

		public bool EnterFlags16(string name, uint offset, bool visible, uint pluginLine, string tooltip, string tra,string tooltipTra)
		{
			PrintBasicValue("Flags16", name, offset, visible, tooltip, tra, tooltipTra);
			_level++;
			return true;
		}

		public bool EnterFlags32(string name, uint offset, bool visible, uint pluginLine, string tooltip, string tra,string tooltipTra)
		{
			PrintBasicValue("Flags32", name, offset, visible, tooltip, tra, tooltipTra);
			_level++;
			return true;
		}

		public bool EnterFlags64(string name, uint offset, bool visible, uint pluginLine, string tooltip, string tra,string tooltipTra)
		{
			PrintBasicValue("Flags64", name, offset, visible, tooltip, tra, tooltipTra);
			_level++;
			return true;
		}

		public void VisitBit(string name, int index, string tooltip,string tra,string tooltipTra)
		{
			Debug.WriteLine(Indent() + "Bit \"{0}\" at position {1}, tooltip = {2}", name, index, tooltip);
		}

		public void LeaveFlags()
		{
			_level--;
		}

		public bool EnterEnum8(string name, uint offset, bool visible, uint pluginLine, string tooltip, string tra,string tooltipTra)
		{
			PrintBasicValue("Enum8", name, offset, visible, tooltip, tra, tooltipTra);
			_level++;
			return true;
		}

		public bool EnterEnum16(string name, uint offset, bool visible, uint pluginLine, string tooltip, string tra,string tooltipTra)
		{
			PrintBasicValue("Enum16", name, offset, visible, tooltip, tra, tooltipTra);
			_level++;
			return true;
		}

		public bool EnterEnum32(string name, uint offset, bool visible, uint pluginLine, string tooltip,string tra,string tooltipTra)
		{
			PrintBasicValue("Enum32", name, offset, visible, tooltip, tra, tooltipTra);
			_level++;
			return true;
		}

		public void VisitOption(string name, int value, string tooltip,string tra,string tooltipTra)
		{
			Debug.WriteLine(Indent() + "{0} = {1}, tooltip = {2}", name, value, tooltip,tra,tooltipTra);
		}

		public void LeaveEnum()
		{
			_level--;
		}

		public bool EnterTagBlock(string name, uint offset, bool visible, uint elementSize, int align, bool sort, uint pluginLine, string tooltip,string tra,string tooltipTra)
		{
			Debug.WriteLine(Indent() + "Tag block \"{0}\" at {1}, visible = {2}, elementSize = {3}, align = {4}, sort = {5}, tooltip = {6}", name, offset, visible, elementSize, align, sort, tooltip);
			_level++;
			return true;
		}

		public void LeaveTagBlock()
		{
			_level--;
		}

		public void VisitDataReference(string name, uint offset, string format, bool visible, int align, uint pluginLine, string tooltip,string tra,string tooltipTra)
		{
			Debug.WriteLine(Indent() + "Data reference \"{0}\" at {1}, format = {2}, visible = {3}, align = {4}, tooltip = {5}", name, offset, format, visible, align, tooltip);
		}

		public void VisitAscii(string name, uint offset, bool visible, int size, uint pluginLine, string tooltip,string tra,string tooltipTra)
		{
			Debug.WriteLine(Indent() + "Ascii string \"{0}\" at {1}, visible = {2}, size = {3}, tooltip = {4}", name, offset, visible, size, tooltip);
		}

		public void VisitUtf16(string name, uint offset, bool visible, int size, uint pluginLine, string tooltip,string tra,string tooltipTra)
		{
			Debug.WriteLine(Indent() + "Utf16 string \"{0}\" at {1}, visible = {2}, size = {3}, tooltip = {4}", name, offset, visible, size, tooltip);
		}

		public void VisitHexString(string name, uint offset, bool visible, int size, uint pluginLine, string tooltip, string tra,string tooltipTra)
		{
			Debug.WriteLine(Indent() + "Hex string \"{0}\" at {1}, visible = {2}, size = {3}, tooltip = {4}", name, offset, visible, size, tooltip);
		}

		public void VisitColorInt(string name, uint offset, bool visible, bool alpha, uint pluginLine, string tooltip,string tra,string tooltipTra)
		{
			Debug.WriteLine(Indent() + "Color32 \"{0}\" at {1}, visible = {2}, alpha = {3}, tooltip = {4}", name, offset, visible, alpha, tooltip);
		}

		public void VisitColorF(string name, uint offset, bool visible, bool alpha, bool basic, uint pluginLine, string tooltip, string tra,string tooltipTra)
		{
			Debug.WriteLine(Indent() + "ColorF \"{0}\" at {1}, visible = {2}, alpha = {3}, basic = {4}, tooltip = {5}", name, offset, visible, alpha, basic, tooltip);
		}

		public void VisitRawData(string name, uint offset, bool visible, int size, uint pluginLine, string tooltip,string tra,string tooltipTra)
		{
			Debug.WriteLine(Indent() + "Raw data block \"{0}\" at {1}, visible = {2}, size = {3}, tooltip = {4}", name, offset, visible, size, tooltip,tra,tooltipTra);
		}

		private static void PrintBasicValue(string type, string name, uint offset, bool visible, string tooltip, string tra,string tooltipTra)
		{
			Debug.WriteLine("{0} \"{1}\" at {2}, visible = {3}, tooltip = {4}", type, name, offset, visible, tooltip,tra,tooltipTra);
		}

		public void VisitShader(string name, uint offset, bool visible, ShaderType type, uint pluginLine, string tooltip, string tra,string tooltipTra)
		{
			Debug.WriteLine(Indent() + "Shader \"{0}\" at {1}, visible = {2}, type = {3}, tooltip = {4}", name, offset, visible, type, tooltip);
		}

		public void VisitRangeInt16(string name, uint offset, bool visible, uint pluginLine, string tooltip, string tra,string tooltipTra)
		{
			PrintBasicValue("Range16", name, offset, visible, tooltip, tra, tooltipTra);
		}

		public void VisitRangeFloat32(string name, uint offset, bool visible, uint pluginLine, string tooltip, string tra,string tooltipTra)
		{
			PrintBasicValue("RangeF", name, offset, visible, tooltip, tra, tooltipTra);
		}

		public void VisitRangeDegree(string name, uint offset, bool visible, uint pluginLine, string tooltip, string tra,string tooltipTra)
		{
			PrintBasicValue("RangeD", name, offset, visible, tooltip, tra, tooltipTra);
		}

		public void VisitUnicList(string name, uint offset, bool visible, int languages, uint pluginLine, string tooltip,string tra,string tooltipTra)
		{
			Debug.WriteLine(Indent() + "Unicode string list \"{0}\" at {1}, visible = {2}, languages = {3}, tooltip = {4}", name, offset, visible, languages, tooltip);
		}

		public void VisitDatum(string name, uint offset, bool visible, uint pluginLine, string tooltip, string tra,string tooltipTra)
		{
			PrintBasicValue("Datum", name, offset, visible, tooltip, tra, tooltipTra);
		}

		public void VisitOldStringID(string name, uint offset, bool visible, uint pluginLine, string tooltip, string tra,string tooltipTra)
		{
			PrintBasicValue("OldStringID", name, offset, visible, tooltip, tra, tooltipTra);
		}
	}
}
