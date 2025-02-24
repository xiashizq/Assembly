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
    public partial class Gpt
    {
        public Gpt()
        {
            InitializeComponent();
            // 初始化ComboBox选项
            var options = new ObservableCollection<SimpleOption>
            {
                new SimpleOption { Content = "Qwen" },
            // 继续添加更多选项...
            };


            comboBoxOptions.ItemsSource = options;
            // 获取当前配置的翻译应用名称
            string currentTranslationApp = ConfigManager.GetSetting("Assembly", "GptApp", "Qwen");
            // 使用LINQ简化设置选中项的过程
            var selectedOption = options.FirstOrDefault(o => o.Content.Equals(currentTranslationApp));
            if (selectedOption != null)
            {
                comboBoxOptions.SelectedItem = selectedOption;
            }

            // 获取TranslationAppId 和 TranslationSecretKey
            string gptAppId = ConfigManager.GetSetting("Assembly", "GptAppId");
            string gptAppKey = ConfigManager.GetSetting("Assembly", "GptAppKey");
            appIdInput.Text = gptAppId;
            appKeyInput.Text = gptAppKey;
        }

        public class SimpleOption
        {
            public string Content { get; set; }

        }

        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            // 获取当前选中的翻译应用名称
            string selectedGptApp = ((SimpleOption)comboBoxOptions.SelectedItem)?.Content ?? "Qwen";
            // 获取用户输入的AppId和SecretKey
            string gptAppId = appIdInput.Text;
            string gptAppKey = appKeyInput.Text;
            // 使用ConfigManager保存这些设置
            ConfigManager.SetSetting("Assembly", "GptApp", selectedGptApp);
            ConfigManager.SetSetting("Assembly", "GptAppId", gptAppId);
            ConfigManager.SetSetting("Assembly", "GptAppKey", gptAppKey);
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
                    string gptAppId = ConfigManager.GetSetting("Assembly", "GptAppId");
                    string gptAppKey = ConfigManager.GetSetting("Assembly", "GptAppKey");
                    appIdInput.Text = gptAppId;
                    appKeyInput.Text = gptAppKey;
                }
            }
        }
    }
}
