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

			var optionsTargetLau = new ObservableCollection<SimpleOption>
			{
				new SimpleOption { Content = "zh" },
				new SimpleOption { Content = "ja" },
			};

			var optionsLocalLanguage = new ObservableCollection<SimpleOption>
			{
				new SimpleOption { Content = "zh" },
				new SimpleOption { Content = "ja" },
			};

			comboBoxOptions.ItemsSource = options;
			comboBoxOptionsTargetLau.ItemsSource = optionsTargetLau;
			comboBoxLocalLanguage.ItemsSource = optionsLocalLanguage;

			string currentTranslationApp = ConfigManager.GetSetting("Assembly", "TranslationApp", "Baidu");
			var selectedOption = options.FirstOrDefault(o => o.Id.Equals(currentTranslationApp))
				?? options.FirstOrDefault();
			if (selectedOption != null)
				comboBoxOptions.SelectedItem = selectedOption;

			string currentTranslationTl = ConfigManager.GetSetting("Assembly", "TranslationTargetlanguage", "zh");
			var selectedOptionTl = optionsTargetLau.FirstOrDefault(o => o.Content.Equals(currentTranslationTl));
			if (selectedOptionTl != null)
				comboBoxOptionsTargetLau.SelectedItem = selectedOptionTl;

			string currentLocalLanguage = ConfigManager.GetSetting("Assembly", "LocalLanguage", "zh");
			var selectedOptionLocal = optionsLocalLanguage.FirstOrDefault(o => o.Content.Equals(currentLocalLanguage));
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
			var selected = comboBoxOptions.SelectedItem as SimpleOption;
			string providerId = selected?.Id ?? "Baidu";
			string selectedTranslationTl = ((SimpleOption)comboBoxOptionsTargetLau.SelectedItem)?.Content ?? "zh";
			string translationAppId = appIdInput.Text?.Trim() ?? string.Empty;
			string translationSecretKey = secretKeyInput.Text?.Trim() ?? string.Empty;
			string translationExtra = extraInput.Text?.Trim() ?? string.Empty;

			var provider = PublicTranslateService.GetProvider(providerId);
			if (provider != null)
			{
				if (provider.RequiresAppId && string.IsNullOrWhiteSpace(translationAppId))
				{
					MetroMessageBox.Show("Please fill in " + provider.AppIdLabel);
					return;
				}
				if (provider.RequiresSecretKey && string.IsNullOrWhiteSpace(translationSecretKey))
				{
					MetroMessageBox.Show("Please fill in " + provider.SecretKeyLabel);
					return;
				}
				if (provider.RequiresExtra && string.IsNullOrWhiteSpace(translationExtra))
				{
					MetroMessageBox.Show("Please fill in " + provider.ExtraLabel);
					return;
				}
			}

			ConfigManager.SetSetting("Assembly", "TranslationApp", providerId);
			ConfigManager.SetSetting("Assembly", "TranslationTargetlanguage", selectedTranslationTl);
			ConfigManager.SetSetting("Assembly", PublicTranslateService.GetAppIdKey(providerId), translationAppId);
			ConfigManager.SetSetting("Assembly", PublicTranslateService.GetSecretKeyKey(providerId), translationSecretKey);
			ConfigManager.SetSetting("Assembly", PublicTranslateService.GetExtraKey(providerId), translationExtra);

			// Keep legacy keys in sync for older configs / tools.
			ConfigManager.SetSetting("Assembly", "TranslationAppId", translationAppId);
			ConfigManager.SetSetting("Assembly", "TranslationSecretKey", translationSecretKey);

			MetroMessageBox.Show("API settings saved successfully");
		}

		private void btnTestAPI_Click(object sender, RoutedEventArgs e)
		{
			var selected = comboBoxOptions.SelectedItem as SimpleOption;
			string providerId = selected?.Id ?? "Baidu";
			string selectedTranslationTl = ((SimpleOption)comboBoxOptionsTargetLau.SelectedItem)?.Content ?? "zh";
			string translationAppId = appIdInput.Text?.Trim() ?? string.Empty;
			string translationSecretKey = secretKeyInput.Text?.Trim() ?? string.Empty;
			string translationExtra = extraInput.Text?.Trim() ?? string.Empty;

			ConfigManager.SetSetting("Assembly", "TranslationApp", providerId);
			ConfigManager.SetSetting("Assembly", "TranslationTargetlanguage", selectedTranslationTl);
			ConfigManager.SetSetting("Assembly", PublicTranslateService.GetAppIdKey(providerId), translationAppId);
			ConfigManager.SetSetting("Assembly", PublicTranslateService.GetSecretKeyKey(providerId), translationSecretKey);
			ConfigManager.SetSetting("Assembly", PublicTranslateService.GetExtraKey(providerId), translationExtra);
			ConfigManager.SetSetting("Assembly", "TranslationAppId", translationAppId);
			ConfigManager.SetSetting("Assembly", "TranslationSecretKey", translationSecretKey);

			string result = PublicTranslateService.TranslateAsync("Flag");
			MetroMessageBox.Show(result);
		}

		private void btnSaveLocal_Click(object sender, RoutedEventArgs e)
		{
			string selectedLocalLanguage = ((SimpleOption)comboBoxLocalLanguage.SelectedItem)?.Content ?? "zh";
			ConfigManager.SetSetting("Assembly", "LocalLanguage", selectedLocalLanguage);
			MetroMessageBox.Show("Local language setting saved successfully. Please restart the application for the changes to take effect.");
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

			appIdLabel.Text = provider.AppIdLabel + ":";
			secretKeyLabel.Text = provider.SecretKeyLabel + ":";
			secretKeyRow.Visibility = provider.RequiresSecretKey ? Visibility.Visible : Visibility.Collapsed;

			bool showExtra = !string.IsNullOrWhiteSpace(provider.ExtraLabel);
			extraRow.Visibility = showExtra ? Visibility.Visible : Visibility.Collapsed;
			extraLabel.Text = string.IsNullOrWhiteSpace(provider.ExtraLabel) ? "extra:" : provider.ExtraLabel + ":";
			providerHelpText.Text = provider.HelpText;

			string appId = ConfigManager.GetSetting("Assembly", PublicTranslateService.GetAppIdKey(providerId));
			string secretKey = ConfigManager.GetSetting("Assembly", PublicTranslateService.GetSecretKeyKey(providerId));
			string extra = ConfigManager.GetSetting("Assembly", PublicTranslateService.GetExtraKey(providerId));

			if (string.IsNullOrWhiteSpace(appId) && providerId == "Baidu")
				appId = ConfigManager.GetSetting("Assembly", "TranslationAppId");
			if (string.IsNullOrWhiteSpace(secretKey) && providerId == "Baidu")
				secretKey = ConfigManager.GetSetting("Assembly", "TranslationSecretKey");

			appIdInput.Text = appId;
			secretKeyInput.Text = secretKey;
			extraInput.Text = extra;
		}
	}
}
