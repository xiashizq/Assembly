﻿using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using Assembly.Metro.Dialogs;
using Blamite.Blam;
using Blamite.Blam.Localization;
using Blamite.Blam.SecondGen.Localization;
using Blamite.Blam.Util;
using Blamite.IO;
using Blamite.Serialization;
using Microsoft.Win32;

namespace Assembly.Metro.Controls.PageTemplates.Games.Components.Editors
{
	/// <summary>
	///     Interaction logic for StringEditor.xaml
	/// </summary>
	public partial class StringEditor : UserControl
	{
		internal class StringPair
		{
			public string StringID { get; set; }
			public string Value { get; set; }

			public StringPair(string stringid, string value)
			{
				StringID = stringid;
				Value = value;
			}

			public bool Filter(string filter)
			{
				return StringID.ToLowerInvariant().Contains(filter) ||
					Value.ToLowerInvariant().Contains(filter);
			}
		}

		internal class LocalizedStringPairs
		{
			public GameLanguage Langauge { get; private set; }

			public string LanguageName
			{
				get { return GameLanguageTools.GetPrettyName(Langauge); }
			}

			public List<StringPair> Strings { get; private set; }

			public LocalizedStringPairs(GameLanguage language)
			{
				Langauge = language;
				Strings = new List<StringPair>();
			}
		}

		private TagEntry _tag;
		private ICacheFile _cache;
		private EngineDescription _buildInfo;
		private readonly IStreamManager _streamManager;
		private IPointerExpander _expander;
		private CollectionView _stringView;

		private readonly Dictionary<GameLanguage, LocalizedStringPairs> Strings = new Dictionary<GameLanguage, LocalizedStringPairs>();

		private Dictionary<string, string> _escapeChars = new Dictionary<string, string>()
		{
			{ "\"", "\\\"" },

			{ "'",  @"\'" },
			{ "?",  "\\?" },
			{ ";",  "\\;" },
		};

		public StringEditor(EngineDescription buildInfo, ICacheFile cache, TagEntry tag, IStreamManager streamManager)
		{
			InitializeComponent();

			_tag = tag;
			_buildInfo = buildInfo;
			_cache = cache;
			_streamManager = streamManager;
			_expander = _cache.PointerExpander;

			if (tag.GroupName == "hmt ")
				GetHmtStrings(tag);
			else if (tag.GroupName == "str#")
				GetStrStrings(tag, false);
			else if (tag.GroupName == "ustr")
				GetStrStrings(tag, true);
			else if (tag.GroupName == "unic")
				GetUnicStrings(tag);

			if (Strings.Count == 0)
			{
				cbLanguageGroups.IsEnabled = false;
				return;
			}

			cbLanguageGroups.ItemsSource = Strings.Values.ToList();
			cbLanguageGroups.SelectedIndex = 0;
		}

		#region hud message
		internal enum HudMessageType
		{
			Length,
			Character,
		}

		private void GetHmtStrings(TagEntry tag)
		{
			if (!_buildInfo.Layouts.HasLayout("hmt"))
				return;

			GameLanguage defaultLang = GameLanguage.English;
			using (var reader = _streamManager.OpenRead())
			{
				reader.SeekTo(tag.RawTag.MetaLocation.AsOffset());
				StructureValueCollection values = StructureReader.ReadStructure(reader, _buildInfo.Layouts.GetLayout("hmt"));

				byte[] buffer = ReadDataReference(values, reader, "string data size", "string data address");

				StructureValueCollection[] elements = ReadBlock(values, reader, "number of message elements", "message element table address", "hud message element");
				StructureValueCollection[] messages = ReadBlock(values, reader, "number of messages", "message table address", "hud message");

				using (MemoryStream ms = new MemoryStream(buffer))
				{
					using (EndianReader bufferReader = new EndianReader(ms, _buildInfo.Endian))
					{
						Strings[defaultLang] = new LocalizedStringPairs(defaultLang);

						for (int i = 0; i < messages.Length; i++)
						{
							StructureValueCollection msg = messages[i];

							string name = msg.GetString("name");
							int stringOffset = (int)msg.GetInteger("string offset") * 2;
							int firstElement = (int)msg.GetInteger("starting element index");
							int elementCount = (int)msg.GetInteger("element count");

							ms.Position = stringOffset;

							string result = "";

							for (int e = 0; e < elementCount; e++)
							{
								StructureValueCollection elem = elements[firstElement + e];
								HudMessageType type = (HudMessageType)elem.GetInteger("type");
								byte data = (byte)elem.GetInteger("data");

								if (type != HudMessageType.Length)
								{
									if (_buildInfo.HudMessageSymbols != null)
									{
										string symbol = _buildInfo.HudMessageSymbols.GetSymbolName((char)data);
										if (string.IsNullOrEmpty(symbol))
											result += $"<undefined-{data:X2}/>";
										else
											result += symbol;
									}
								}
								else
									result += bufferReader.ReadUTF16();
							}

							Strings[defaultLang].Strings.Add(new StringPair(name, result));
						}
					}
				}
			}
		}
		#endregion

		#region string list
		private void GetStrStrings(TagEntry tag, bool unicode)
		{
			if (!_buildInfo.Layouts.HasLayout("str"))
				return;

			GameLanguage defaultLang = GameLanguage.English;
			using (var reader = _streamManager.OpenRead())
			{
				reader.SeekTo(tag.RawTag.MetaLocation.AsOffset());
				StructureValueCollection values = StructureReader.ReadStructure(reader, _buildInfo.Layouts.GetLayout("str"));

				StructureValueCollection[] strings = ReadBlock(values, reader, "number of strings", "string table address", "string list element");

				Strings[defaultLang] = new LocalizedStringPairs(defaultLang);

				for (int i = 0; i < strings.Length; i++)
				{
					StructureValueCollection str = strings[i];
					byte[] buffer = ReadDataReference(str, reader, "string data size", "string data address");

					string locale = unicode ? Encoding.Unicode.GetString(buffer) : Encoding.UTF8.GetString(buffer);

					Strings[defaultLang].Strings.Add(new StringPair(i.ToString(), locale));
				}
			}
		}
		#endregion

		#region unicode
		private void GetUnicStrings(TagEntry tag)
		{
			using (var reader = _streamManager.OpenRead())
			{
				reader.SeekTo(tag.RawTag.MetaLocation.AsOffset());

				if (_buildInfo.Layouts.HasLayout("unic split"))
					GetUnicSplitStrings(tag, reader);
				else if (_buildInfo.Layouts.HasLayout("unic table"))
					GetUnicTableStrings(tag, reader);
			}
		}

		private void GetUnicSplitStrings(TagEntry tag, IReader reader)
		{
			StructureValueCollection values = StructureReader.ReadStructure(reader, _buildInfo.Layouts.GetLayout("unic split"));
			StructureValueCollection[] languages = ReadBlock(values, reader, "number of languages", "language table address", "unic split language");

			for (int i = 0; i < languages.Length; i++)
			{
				StructureValueCollection lang = languages[i];
				int langIndex = (int)lang.GetInteger("language index");
				StructureValueCollection[] strings = ReadBlock(lang, reader, "number of strings", "string table address", "unic split string element");

				GameLanguage language = SecondGenLanguageGlobals.LanguageRemaps.Keys.ElementAt(langIndex);
				Strings[language] = new LocalizedStringPairs(language);

				for (int j = 0; j < strings.Length; j++)
				{
					StructureValueCollection str = strings[j];

					StringID nameSID = new StringID((uint)str.GetInteger("name stringid"));
					string name = _cache.StringIDs.GetString(nameSID);

					byte[] buffer = ReadDataReference(str, reader, "string data size", "string data address");

					string locale = Encoding.Unicode.GetString(buffer);
					if (_buildInfo.LocaleSymbols != null)
						locale = _buildInfo.LocaleSymbols.ReplaceSymbols(locale);

					Strings[language].Strings.Add(new StringPair(name, locale));
				}
			}
		}

		private void GetUnicTableStrings(TagEntry tag, IReader reader)
		{
			StructureValueCollection values = StructureReader.ReadStructure(reader, _buildInfo.Layouts.GetLayout("unic table"));
			byte[] buffer = ReadDataReference(values, reader, "string data size", "string data address");
			StructureValueCollection[] strings = ReadBlock(values, reader, "number of strings", "string table address", "unic table string reference");

			using (MemoryStream ms = new MemoryStream(buffer))
			{
				using (EndianReader bufferReader = new EndianReader(ms, _buildInfo.Endian))
				{
					for (int i = 0; i < strings.Length; i++)
					{
						StructureValueCollection str = strings[i];

						StringID nameSID = new StringID((uint)str.GetInteger("name stringid"));
						string name = _cache.StringIDs.GetString(nameSID);

						StructureValueCollection[] languageSet = str.GetArray("language offsets");

						for (int j = 0; j < languageSet.Length; j++)
						{
							StructureValueCollection lang = languageSet[j];
							int startOffset = (int)lang.GetInteger("start offset");
							if (startOffset == -1)
								continue;

							GameLanguage language = _cache.Engine == EngineType.SecondGeneration ?
								SecondGenLanguageGlobals.LanguageRemaps.Keys.ElementAt(j) : (GameLanguage)j;
							if (!Strings.ContainsKey(language))
								Strings[language] = new LocalizedStringPairs(language);

							bufferReader.SeekTo(startOffset);

							string locale = bufferReader.ReadUTF8();
							if (_buildInfo.LocaleSymbols != null)
								locale = _buildInfo.LocaleSymbols.ReplaceSymbols(locale);

							Strings[language].Strings.Add(new StringPair(name, locale));
						}
					}
				}
			}
		}
		#endregion

		/// <summary>
		///     Filter function for the string view.
		/// </summary>
		/// <param name="item">The item to filter.</param>
		/// <returns><c>true</c> if the item should be made visible.</returns>
		private bool FilterString(object item)
		{
			var entry = (StringPair)item;
			if (!string.IsNullOrEmpty(txtFilter.Text))
			{
				string lowerFilter = txtFilter.Text.ToLowerInvariant();
				return entry.Filter(lowerFilter);
			}
			return true;
		}

		#region Event Handlers
		private void cbLanguageGroups_SelectionChanged(object sender, SelectionChangedEventArgs e)
		{
			var pairs = cbLanguageGroups.SelectedItem as LocalizedStringPairs;
			if (pairs != null && pairs.Strings != null)
			{
				lvLocales.ItemsSource = null;
				lvLocales.ItemsSource = pairs.Strings;

				_stringView = CollectionViewSource.GetDefaultView(lvLocales.ItemsSource) as CollectionView;
				_stringView.Filter = FilterString;
			}
		}

		private void txtFilter_TextChanged(object sender, TextChangedEventArgs e)
		{
			if (lvLocales.ItemsSource != null)
				CollectionViewSource.GetDefaultView(lvLocales.ItemsSource).Refresh();
		}

		private void Cell_MouseDown(object sender, MouseButtonEventArgs e)
		{
			if (e.ClickCount != 2)
				return;

			var cell = sender as TextBlock;
			Clipboard.SetText(cell.Text);
		}

		private void btnExport_Click(object sender, RoutedEventArgs e)
		{
			string tagname = _tag.TagFileName != null
				? _tag.TagFileName : "0x" + _tag.RawTag.Index.Value.ToString("X");

			var invalids = Path.GetInvalidFileNameChars();

			tagname = string.Join("_", tagname.Split(invalids));
			tagname = Path.GetFileName(tagname);

			var sfd = new SaveFileDialog();
			sfd.Title = "Save Strings As";
			sfd.FileName = tagname;

			string filter = txtFilter.Text;
			txtFilter.Text = "";

			using (StringWriter sw = new StringWriter())
			{
				if (_tag.GroupName == "hmt ")
				{
					sfd.Filter = "HUD Message File|*.hmt|Text File|*.txt";
					foreach (StringPair pair in lvLocales.Items)
						sw.WriteLine($"{pair.StringID}={pair.Value}");
				}
				else if (_tag.GroupName == "unic")
				{
					sfd.Filter = "Text File|*.txt";
					sw.WriteLine("[Strings]");
					foreach (StringPair pair in lvLocales.Items)
						sw.WriteLine($"{pair.StringID} = \"{EscapeString(pair.Value)}\"");
				}
				else
				{
					sfd.Filter = "Text File|*.txt";
					foreach (StringPair pair in lvLocales.Items)
					{
						sw.WriteLine($"{pair.Value}");
						sw.WriteLine("###END-STRING###");
					}
				}

				if (!(bool)sfd.ShowDialog())
					return;

				File.WriteAllText(sfd.FileName, sw.ToString());
				MetroMessageBox.Show("Strings Exported", "Strings exported successfully.");
			}

			txtFilter.Text = filter;
		}

		private void btnReset_Click(object sender, RoutedEventArgs e)
		{
			txtFilter.Text = "";
		}
		#endregion

		private byte[] ReadDataReference(StructureValueCollection values, IReader reader, string lengthName, string addressName)
		{
			var size = (int)values.GetInteger(lengthName);
			uint address = (uint)values.GetInteger(addressName);

			long expand = _expander.Expand(address);

			if (size <= 0 || address == 0)
				return new byte[0];

			uint offset = _tag.RawTag.MetaLocation.BaseGroup.PointerToOffset(expand);
			reader.SeekTo(offset);
			return reader.ReadBlock(size);
		}

		private StructureValueCollection[] ReadBlock(StructureValueCollection values, IReader reader, string countName, string addressName, string layoutName)
		{
			int count = (int)values.GetInteger(countName);
			uint address = (uint)values.GetInteger(addressName);

			long expand = _expander.Expand(address);

			var layout = _buildInfo.Layouts.GetLayout(layoutName);
			return TagBlockReader.ReadTagBlock(reader, count, expand, layout, _tag.RawTag.MetaLocation.BaseGroup);
		}

		private string EscapeString(string value)
		{
			string result = value;
			foreach (string k in _escapeChars.Keys)
			{
				if (result.Contains(k))
					result.Replace(k, _escapeChars[k]);
			}

			return result;
		}

	}
}