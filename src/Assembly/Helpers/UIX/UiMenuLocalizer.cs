using System.Windows;
using System.Windows.Controls;
using Assembly.Tool;
using Blamite.Util.DictionaryUntil;

namespace Assembly.Helpers.UIX
{
	/// <summary>
	/// Applies offline dictionary translations to WPF menus based on LocalLanguage.
	/// English Header/ToolTip strings are kept as lookup keys.
	/// </summary>
	public static class UiMenuLocalizer
	{
		private sealed class EnglishTexts
		{
			public string Header;
			public string ToolTip;
		}

		public static void Apply(ItemsControl root)
		{
			if (root == null)
				return;

			string language = ConfigManager.GetSetting("Assembly", "LocalLanguage", "zh");
			ApplyItems(root.Items, language);
		}

		public static void ApplyToHomeMenus()
		{
			var home = App.AssemblyStorage?.AssemblySettings?.HomeWindow;
			home?.ApplyMenuLocalization();
		}

		private static void ApplyItems(ItemCollection items, string language)
		{
			foreach (object item in items)
			{
				var menuItem = item as MenuItem;
				if (menuItem == null)
					continue;

				var stored = menuItem.Tag as EnglishTexts;
				if (stored == null)
				{
					stored = new EnglishTexts
					{
						Header = menuItem.Header as string,
						ToolTip = menuItem.ToolTip as string
					};
					menuItem.Tag = stored;
				}

				if (!string.IsNullOrEmpty(stored.Header))
					menuItem.Header = DictionaryDict.GetUiTranslation(stored.Header, language);

				if (!string.IsNullOrEmpty(stored.ToolTip))
					menuItem.ToolTip = DictionaryDict.GetUiTranslation(stored.ToolTip, language);

				if (menuItem.HasItems)
					ApplyItems(menuItem.Items, language);
			}
		}
	}
}
