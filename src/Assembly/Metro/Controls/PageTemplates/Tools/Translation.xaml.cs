using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Forms;
using Assembly.Helpers;
using Assembly.Metro.Dialogs;
using Assembly.Tool;
using Blamite.Compression;

namespace Assembly.Metro.Controls.PageTemplates.Tools
{
    /// <summary>
    /// Interaction logic for MapCompressor.xaml
    /// </summary>
    public partial class Translation
    {
        public Translation()
        {
            InitializeComponent();
            var options = new ObservableCollection<SimpleOption>
            {
                new SimpleOption { Content = "Baidu" },
            };

            var options_TargetLau = new ObservableCollection<SimpleOption>
            {
                new SimpleOption { Content = "zh" },
                new SimpleOption { Content = "ja" },
            };

            var options_LocalLanguage = new ObservableCollection<SimpleOption>
            {
                new SimpleOption { Content = "zh" },
                new SimpleOption { Content = "ja" },
            };

            comboBoxOptions.ItemsSource = options;
            comboBoxOptionsTargetLau.ItemsSource = options_TargetLau;
            comboBoxLocalLanguage.ItemsSource = options_LocalLanguage;
            
            string currentTranslationApp = ConfigManager.GetSetting("Assembly", "TranslationApp", "Baidu");
            var selectedOption = options.FirstOrDefault(o => o.Content.Equals(currentTranslationApp));
            if (selectedOption != null)
            {
                comboBoxOptions.SelectedItem = selectedOption;
            }

            string currentTranslationTl = ConfigManager.GetSetting("Assembly", "TranslationTargetlanguage");
            var selectedOptionTl = options_TargetLau.FirstOrDefault(o => o.Content.Equals(currentTranslationTl));
            if (selectedOptionTl != null)
            {
                comboBoxOptionsTargetLau.SelectedItem = selectedOptionTl;
            }

            string currentLocalLanguage = ConfigManager.GetSetting("Assembly", "LocalLanguage", "zh");
            var selectedOptionLocal = options_LocalLanguage.FirstOrDefault(o => o.Content.Equals(currentLocalLanguage));
            if (selectedOptionLocal != null)
            {
                comboBoxLocalLanguage.SelectedItem = selectedOptionLocal;
            }
            string translationAppId = ConfigManager.GetSetting("Assembly", "TranslationAppId");
            string translationSecretKey = ConfigManager.GetSetting("Assembly", "TranslationSecretKey");
            appIdInput.Text = translationAppId;
            secretKeyInput.Text = translationSecretKey;
        }

        public class SimpleOption
        {
            public string Content { get; set; }

        }

        private void btnSaveAPI_Click(object sender, RoutedEventArgs e)
        {
            string selectedTranslationApp = ((SimpleOption)comboBoxOptions.SelectedItem)?.Content ?? "Baidu";
            string selectedTranslationTL = ((SimpleOption)comboBoxOptionsTargetLau.SelectedItem)?.Content ?? "zh";
            string translationAppId = appIdInput.Text;
            string translationSecretKey = secretKeyInput.Text;
            ConfigManager.SetSetting("Assembly", "TranslationApp", selectedTranslationApp);
            ConfigManager.SetSetting("Assembly", "TranslationTargetlanguage", selectedTranslationTL);
            ConfigManager.SetSetting("Assembly", "TranslationAppId", translationAppId);
            ConfigManager.SetSetting("Assembly", "TranslationSecretKey", translationSecretKey);
            MetroMessageBox.Show("API settings saved successfully");
        }

        private void btnSaveLocal_Click(object sender, RoutedEventArgs e)
        {
            string selectedLocalLanguage = ((SimpleOption)comboBoxLocalLanguage.SelectedItem)?.Content ?? "zh";
            ConfigManager.SetSetting("Assembly", "LocalLanguage", selectedLocalLanguage);
            MetroMessageBox.Show("Local language setting saved successfully. Please restart the application for the changes to take effect.");
        }

        private void comboBoxOptions_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            if (comboBoxOptions.SelectedItem != null)
            {
                SimpleOption selectedOption = comboBoxOptions.SelectedItem as SimpleOption;
                if (selectedOption != null)
                {
                    string selectedValueString = selectedOption.Content;
                    string translationAppId = ConfigManager.GetSetting("Assembly", "TranslationAppId");
                    string translationSecretKey = ConfigManager.GetSetting("Assembly", "TranslationSecretKey");
                    appIdInput.Text = translationAppId;
                    secretKeyInput.Text = translationSecretKey;
                }
            }
        }
    }
}
