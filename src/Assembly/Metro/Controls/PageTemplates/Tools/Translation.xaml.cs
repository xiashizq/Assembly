using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using Assembly.Metro.Dialogs;
using Assembly.Tool;
using Assembly.Tool.TranslateService;

namespace Assembly.Metro.Controls.PageTemplates.Tools
{
	public partial class Translation
	{
		public Translation()
		{
			InitializeComponent();

			var options = new ObservableCollection<SimpleOption>(
				PublicTranslateService.GetProviders().Select(p => new SimpleOption
				{
					Content = p.DisplayName,
					Id = p.Id
				}));

			var optionsTargetLau = new ObservableCollection<SimpleOption>(
				TranslateLanguageCatalog.TargetLanguages.Select(l => new SimpleOption
				{
					Content = l.DisplayName,
					Id = l.Code
				}));

			var optionsLocalLanguage = new ObservableCollection<SimpleOption>(
				TranslateLanguageCatalog.LocalLanguages.Select(l => new SimpleOption
				{
					Content = l.DisplayName,
					Id = l.Code
				}));

			comboBoxOptions.ItemsSource = options;
			comboBoxOptionsTargetLau.ItemsSource = optionsTargetLau;
			comboBoxLocalLanguage.ItemsSource = optionsLocalLanguage;

			string currentTranslationApp = ConfigManager.GetSetting("Assembly", "TranslationApp", "Baidu");
			var selectedOption = options.FirstOrDefault(o => o.Id.Equals(currentTranslationApp, StringComparison.OrdinalIgnoreCase))
				?? options.FirstOrDefault();
			if (selectedOption != null)
				comboBoxOptions.SelectedItem = selectedOption;

			string currentTranslationTl = ConfigManager.GetSetting("Assembly", "TranslationTargetlanguage", "zh");
			var selectedOptionTl = optionsTargetLau.FirstOrDefault(o =>
					o.Id.Equals(currentTranslationTl, StringComparison.OrdinalIgnoreCase)
					|| o.Content.Equals(currentTranslationTl, StringComparison.OrdinalIgnoreCase))
				?? optionsTargetLau.FirstOrDefault(o => o.Id.Equals("zh", StringComparison.OrdinalIgnoreCase));
			if (selectedOptionTl != null)
				comboBoxOptionsTargetLau.SelectedItem = selectedOptionTl;

			string currentLocalLanguage = ConfigManager.GetSetting("Assembly", "LocalLanguage", "zh");
			var selectedOptionLocal = optionsLocalLanguage.FirstOrDefault(o =>
					o.Id.Equals(currentLocalLanguage, StringComparison.OrdinalIgnoreCase)
					|| o.Content.Equals(currentLocalLanguage, StringComparison.OrdinalIgnoreCase))
				?? optionsLocalLanguage.FirstOrDefault(o => o.Id.Equals("zh", StringComparison.OrdinalIgnoreCase));
			if (selectedOptionLocal != null)
				comboBoxLocalLanguage.SelectedItem = selectedOptionLocal;

			LoadProviderCredentials(selectedOption?.Id ?? "Baidu");
		}

		public class SimpleOption
		{
			public string Content { get; set; }
			public string Id { get; set; }
		}

		private void btnSaveAPI_Click(object sender, RoutedEventArgs e)
		{
			if (!TryCollectAndValidate(out string providerId, out string selectedTranslationTl,
				out string translationAppId, out string translationSecretKey, out string translationExtra, out string translationApiUrl))
				return;

			PersistApiSettings(providerId, selectedTranslationTl, translationAppId, translationSecretKey, translationExtra, translationApiUrl);
			MetroMessageBox.Show("API settings saved successfully");
		}

		private void btnTestAPI_Click(object sender, RoutedEventArgs e)
		{
			if (!TryCollectAndValidate(out string providerId, out string selectedTranslationTl,
				out string translationAppId, out string translationSecretKey, out string translationExtra, out string translationApiUrl))
				return;

			PersistApiSettings(providerId, selectedTranslationTl, translationAppId, translationSecretKey, translationExtra, translationApiUrl);
			string result = PublicTranslateService.TranslateAsync("Flag");
			MetroMessageBox.Show(result);
		}

		private bool TryCollectAndValidate(
			out string providerId,
			out string selectedTranslationTl,
			out string translationAppId,
			out string translationSecretKey,
			out string translationExtra,
			out string translationApiUrl)
		{
			var selected = comboBoxOptions.SelectedItem as SimpleOption;
			providerId = selected?.Id ?? "Baidu";
			selectedTranslationTl = ((SimpleOption)comboBoxOptionsTargetLau.SelectedItem)?.Id ?? "zh";
			translationAppId = appIdInput.Text?.Trim() ?? string.Empty;
			translationSecretKey = secretKeyInput.Text?.Trim() ?? string.Empty;
			translationExtra = extraInput.Text?.Trim() ?? string.Empty;
			translationApiUrl = apiUrlInput.Text?.Trim() ?? string.Empty;

			var provider = PublicTranslateService.GetProvider(providerId);
			if (provider != null)
			{
				if (provider.RequiresAppId && string.IsNullOrWhiteSpace(translationAppId))
				{
					MetroMessageBox.Show("Please fill in " + provider.AppIdLabel);
					return false;
				}
				if (provider.RequiresSecretKey && string.IsNullOrWhiteSpace(translationSecretKey))
				{
					MetroMessageBox.Show("Please fill in " + provider.SecretKeyLabel);
					return false;
				}
				if (provider.RequiresExtra && string.IsNullOrWhiteSpace(translationExtra))
				{
					MetroMessageBox.Show("Please fill in " + provider.ExtraLabel);
					return false;
				}
				if (provider.RequiresApiUrl && string.IsNullOrWhiteSpace(translationApiUrl))
				{
					MetroMessageBox.Show("Please fill in " + provider.ApiUrlLabel);
					return false;
				}
			}

			return true;
		}

		private static void PersistApiSettings(
			string providerId,
			string selectedTranslationTl,
			string translationAppId,
			string translationSecretKey,
			string translationExtra,
			string translationApiUrl)
		{
			ConfigManager.SetSetting("Assembly", "TranslationApp", providerId);
			ConfigManager.SetSetting("Assembly", "TranslationTargetlanguage", selectedTranslationTl);
			ConfigManager.SetSetting("Assembly", PublicTranslateService.GetAppIdKey(providerId), translationAppId);
			ConfigManager.SetSetting("Assembly", PublicTranslateService.GetSecretKeyKey(providerId), translationSecretKey);
			ConfigManager.SetSetting("Assembly", PublicTranslateService.GetExtraKey(providerId), translationExtra);
			ConfigManager.SetSetting("Assembly", PublicTranslateService.GetApiUrlKey(providerId), translationApiUrl);

			// Keep legacy keys in sync for older configs / tools.
			ConfigManager.SetSetting("Assembly", "TranslationAppId", translationAppId);
			ConfigManager.SetSetting("Assembly", "TranslationSecretKey", translationSecretKey);
		}

		private void btnSaveLocal_Click(object sender, RoutedEventArgs e)
		{
			string selectedLocalLanguage = ((SimpleOption)comboBoxLocalLanguage.SelectedItem)?.Id ?? "zh";
			ConfigManager.SetSetting("Assembly", "LocalLanguage", selectedLocalLanguage);
			Helpers.UIX.UiMenuLocalizer.ApplyToHomeMenus();
			MetroMessageBox.Show(
				"Local language saved. Menu bar updated immediately.\n" +
				"Plugin-field offline dictionary requires reopening the tag/map (or restart) to fully refresh.");
		}

		private void comboBoxOptions_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
		{
			var selectedOption = comboBoxOptions.SelectedItem as SimpleOption;
			if (selectedOption == null)
				return;

			LoadProviderCredentials(selectedOption.Id);
		}

		private void LoadProviderCredentials(string providerId)
		{
			var provider = PublicTranslateService.GetProvider(providerId);
			if (provider == null)
				return;

			appIdRow.Visibility = provider.UsesAppId ? Visibility.Visible : Visibility.Collapsed;
			secretKeyRow.Visibility = provider.UsesSecretKey ? Visibility.Visible : Visibility.Collapsed;
			extraRow.Visibility = provider.UsesExtra ? Visibility.Visible : Visibility.Collapsed;
			apiUrlRow.Visibility = provider.UsesApiUrl ? Visibility.Visible : Visibility.Collapsed;

			appIdLabel.Text = provider.AppIdLabel + ":";
			secretKeyLabel.Text = string.IsNullOrWhiteSpace(provider.SecretKeyLabel)
				? "Secret Key:"
				: provider.SecretKeyLabel + ":";
			extraLabel.Text = string.IsNullOrWhiteSpace(provider.ExtraLabel) ? "Extra:" : provider.ExtraLabel + ":";
			apiUrlLabel.Text = string.IsNullOrWhiteSpace(provider.ApiUrlLabel) ? "API URL:" : provider.ApiUrlLabel + ":";
			providerHelpText.Text = provider.HelpText;

			string appId = ConfigManager.GetSetting("Assembly", PublicTranslateService.GetAppIdKey(providerId));
			string secretKey = ConfigManager.GetSetting("Assembly", PublicTranslateService.GetSecretKeyKey(providerId));
			string extra = ConfigManager.GetSetting("Assembly", PublicTranslateService.GetExtraKey(providerId));
			string apiUrl = ConfigManager.GetSetting("Assembly", PublicTranslateService.GetApiUrlKey(providerId));

			if (string.IsNullOrWhiteSpace(appId) && providerId == "Baidu")
				appId = ConfigManager.GetSetting("Assembly", "TranslationAppId");
			if (string.IsNullOrWhiteSpace(secretKey) && providerId == "Baidu")
				secretKey = ConfigManager.GetSetting("Assembly", "TranslationSecretKey");

			// Migrate values previously stored in Extra (endpoint / service URL / free|pro).
			if (string.IsNullOrWhiteSpace(apiUrl))
			{
				if (providerId == "LibreTranslate" || providerId == "IBMWatson")
				{
					apiUrl = extra;
					extra = string.Empty;
				}
				else if (providerId == "DeepL")
				{
					string mode = (extra ?? string.Empty).Trim().ToLowerInvariant();
					if (mode == "pro" || mode == "api.deepl.com")
						apiUrl = "https://api.deepl.com";
					else if (mode == "free" || mode == "api-free.deepl.com")
						apiUrl = "https://api-free.deepl.com";
					else if (mode.StartsWith("http", StringComparison.OrdinalIgnoreCase))
						apiUrl = extra;
					extra = string.Empty;
				}
			}

			if (string.IsNullOrWhiteSpace(apiUrl))
				apiUrl = provider.DefaultApiUrl ?? string.Empty;

			appIdInput.Text = appId;
			secretKeyInput.Text = secretKey;
			extraInput.Text = extra;
			apiUrlInput.Text = apiUrl;
		}
	}
}
