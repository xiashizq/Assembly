using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using Assembly.Metro.Dialogs;
using Assembly.Tool;
using Assembly.Tool.GPTservice;

namespace Assembly.Metro.Controls.PageTemplates.Tools
{
	public partial class Gpt
	{
		public Gpt()
		{
			InitializeComponent();

			var options = new ObservableCollection<SimpleOption>(
				GptProviderRegistry.GetProviders().Select(p => new SimpleOption
				{
					Content = p.DisplayName,
					Id = p.Id
				}));

			comboBoxOptions.ItemsSource = options;

			string currentGptApp = ConfigManager.GetSetting("Assembly", "GptApp", "Qwen");
			var selectedOption = options.FirstOrDefault(o => o.Id.Equals(currentGptApp))
				?? options.FirstOrDefault();
			if (selectedOption != null)
				comboBoxOptions.SelectedItem = selectedOption;

			LoadProviderCredentials(selectedOption?.Id ?? "Qwen");
		}

		public class SimpleOption
		{
			public string Content { get; set; }
			public string Id { get; set; }
		}

		private void btnSave_Click(object sender, RoutedEventArgs e)
		{
			if (!SaveCurrentProvider(true))
				return;
			MetroMessageBox.Show("Successfully saved");
		}

		private async void btnTest_Click(object sender, RoutedEventArgs e)
		{
			if (!SaveCurrentProvider(false))
				return;

			await GPTstreamClient.GPT_Async("Flag");
			var sharedVm = (Metro.SharedViewModelUntil.SharedViewModel)Application.Current.FindResource("SharedViewModel");
			MetroMessageBox.Show(string.IsNullOrWhiteSpace(sharedVm.AiText) ? "(empty response)" : sharedVm.AiText);
		}

		private void comboBoxOptions_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
		{
			var selectedOption = comboBoxOptions.SelectedItem as SimpleOption;
			if (selectedOption == null)
				return;
			LoadProviderCredentials(selectedOption.Id);
		}

		private bool SaveCurrentProvider(bool validate)
		{
			var selected = comboBoxOptions.SelectedItem as SimpleOption;
			string providerId = selected?.Id ?? "Qwen";
			var provider = GptProviderRegistry.GetProvider(providerId);
			string apiKey = appKeyInput.Text?.Trim() ?? string.Empty;
			string model = modelInput.Text?.Trim() ?? string.Empty;
			string longModel = longModelInput.Text?.Trim() ?? string.Empty;
			string extra = extraInput.Text?.Trim() ?? string.Empty;

			if (validate && provider != null)
			{
				if (string.IsNullOrWhiteSpace(apiKey))
				{
					MetroMessageBox.Show("Please fill in " + provider.ApiKeyLabel);
					return false;
				}
				if (provider.RequiresExtra && string.IsNullOrWhiteSpace(extra) && string.IsNullOrWhiteSpace(model))
				{
					MetroMessageBox.Show("Please fill in " + (string.IsNullOrWhiteSpace(provider.ExtraLabel) ? provider.ModelLabel : provider.ExtraLabel));
					return false;
				}
			}

			if (string.IsNullOrWhiteSpace(model) && provider != null)
				model = provider.DefaultModel;
			if (string.IsNullOrWhiteSpace(longModel) && provider != null)
				longModel = string.IsNullOrWhiteSpace(provider.DefaultLongModel) ? model : provider.DefaultLongModel;

			ConfigManager.SetSetting("Assembly", "GptApp", providerId);
			ConfigManager.SetSetting("Assembly", GptProviderRegistry.GetApiKeyKey(providerId), apiKey);
			ConfigManager.SetSetting("Assembly", GptProviderRegistry.GetModelKey(providerId), model);
			ConfigManager.SetSetting("Assembly", GptProviderRegistry.GetLongModelKey(providerId), longModel);
			ConfigManager.SetSetting("Assembly", GptProviderRegistry.GetExtraKey(providerId), extra);
			ConfigManager.SetSetting("Assembly", "GptAppKey", apiKey);
			return true;
		}

		private void LoadProviderCredentials(string providerId)
		{
			var provider = GptProviderRegistry.GetProvider(providerId);
			if (provider == null)
				return;

			apiKeyLabel.Text = provider.ApiKeyLabel + ":";
			modelLabel.Text = provider.ModelLabel + ":";
			bool showExtra = !string.IsNullOrWhiteSpace(provider.ExtraLabel);
			extraRow.Visibility = showExtra ? Visibility.Visible : Visibility.Collapsed;
			extraLabel.Text = string.IsNullOrWhiteSpace(provider.ExtraLabel) ? "Extra:" : provider.ExtraLabel + ":";
			providerHelpText.Text = provider.HelpText;

			string apiKey = ConfigManager.GetSetting("Assembly", GptProviderRegistry.GetApiKeyKey(providerId));
			string model = ConfigManager.GetSetting("Assembly", GptProviderRegistry.GetModelKey(providerId));
			string longModel = ConfigManager.GetSetting("Assembly", GptProviderRegistry.GetLongModelKey(providerId));
			string extra = ConfigManager.GetSetting("Assembly", GptProviderRegistry.GetExtraKey(providerId));

			if (string.IsNullOrWhiteSpace(apiKey) && providerId == "Qwen")
				apiKey = ConfigManager.GetSetting("Assembly", "GptAppKey");
			if (string.IsNullOrWhiteSpace(model))
				model = provider.DefaultModel;
			if (string.IsNullOrWhiteSpace(longModel))
				longModel = provider.DefaultLongModel;
			if (string.IsNullOrWhiteSpace(extra) && provider.Id == "Custom")
				extra = provider.DefaultBaseUrl;

			appKeyInput.Text = apiKey;
			modelInput.Text = model;
			longModelInput.Text = longModel;
			extraInput.Text = extra;
		}
	}
}
