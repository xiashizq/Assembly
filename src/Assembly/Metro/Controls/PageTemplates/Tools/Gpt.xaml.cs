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
            // ��ʼ��ComboBoxѡ��
            var options = new ObservableCollection<SimpleOption>
            {
                new SimpleOption { Content = "Qwen" },
            // ������Ӹ���ѡ��...
            };


            comboBoxOptions.ItemsSource = options;
            // ��ȡ��ǰ���õķ���Ӧ������
            string currentTranslationApp = ConfigManager.GetSetting("Assembly", "GptApp", "Qwen");
            // ʹ��LINQ������ѡ����Ĺ���
            var selectedOption = options.FirstOrDefault(o => o.Content.Equals(currentTranslationApp));
            if (selectedOption != null)
            {
                comboBoxOptions.SelectedItem = selectedOption;
            }

            // ��ȡTranslationAppId �� TranslationSecretKey
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
            // ��ȡ��ǰѡ�еķ���Ӧ������
            string selectedGptApp = ((SimpleOption)comboBoxOptions.SelectedItem)?.Content ?? "Qwen";
            // ��ȡ�û������AppId��SecretKey
            string gptAppId = appIdInput.Text;
            string gptAppKey = appKeyInput.Text;
            // ʹ��ConfigManager������Щ����
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
                    // ������ѡ��Ӧ�ó������ƻ�ȡ��Ӧ��Id��SecretKey
                    string gptAppId = ConfigManager.GetSetting("Assembly", "GptAppId");
                    string gptAppKey = ConfigManager.GetSetting("Assembly", "GptAppKey");
                    appIdInput.Text = gptAppId;
                    appKeyInput.Text = gptAppKey;
                }
            }
        }
    }
}
