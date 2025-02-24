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
            // 初始化ComboBox选项
            var options = new ObservableCollection<SimpleOption>
            {
                new SimpleOption { Content = "Baidu" },
            // 继续添加更多选项...
            };

            var options_TargetLau = new ObservableCollection<SimpleOption>
            {
                new SimpleOption { Content = "zh" },
                new SimpleOption { Content = "jp" },
            // 继续添加更多选项...
            };

            comboBoxOptions.ItemsSource = options;

            comboBoxOptionsTargetLau.ItemsSource = options_TargetLau;
            // 获取当前配置的翻译应用名称
            string currentTranslationApp = ConfigManager.GetSetting("Assembly", "TranslationApp", "Baidu");
            // 使用LINQ简化设置选中项的过程
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


            // 获取TranslationAppId 和 TranslationSecretKey
            string translationAppId = ConfigManager.GetSetting("Assembly", "TranslationAppId");
            string translationSecretKey = ConfigManager.GetSetting("Assembly", "TranslationSecretKey");
            appIdInput.Text = translationAppId;
            secretKeyInput.Text = translationSecretKey;
        }

        public class SimpleOption
        {
            public string Content { get; set; }

        }

        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            // 获取当前选中的翻译应用名称
            string selectedTranslationApp = ((SimpleOption)comboBoxOptions.SelectedItem)?.Content ?? "Baidu";
            string selectedTranslationTL = ((SimpleOption)comboBoxOptionsTargetLau.SelectedItem)?.Content ?? "zh";
            // 获取用户输入的AppId和SecretKey
            string translationAppId = appIdInput.Text;
            string translationSecretKey = secretKeyInput.Text;
            // 使用ConfigManager保存这些设置
            ConfigManager.SetSetting("Assembly", "TranslationApp", selectedTranslationApp);
            ConfigManager.SetSetting("Assembly", "TranslationTargetlanguage", selectedTranslationTL);
            ConfigManager.SetSetting("Assembly", "TranslationAppId", translationAppId);
            ConfigManager.SetSetting("Assembly", "TranslationSecretKey", translationSecretKey);
            ConfigManager.SetSetting("Assembly", "TranslationSecretKey", translationSecretKey);
            MetroMessageBox.Show("Successfully saved");
        }

        private void comboBoxOptions_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            if (comboBoxOptions.SelectedItem != null)
            {
                SimpleOption selectedOption = comboBoxOptions.SelectedItem as SimpleOption;
                if (selectedOption != null)
                {
                    string selectedValueString = selectedOption.Content;
                    // 根据所选的应用程序名称获取对应的Id和SecretKey
                    string translationAppId = ConfigManager.GetSetting("Assembly", "TranslationAppId");
                    string translationSecretKey = ConfigManager.GetSetting("Assembly", "TranslationSecretKey");
                    appIdInput.Text = translationAppId;
                    secretKeyInput.Text = translationSecretKey;
                }
            }
        }
    }
}
