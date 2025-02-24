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
            // ��ʼ��ComboBoxѡ��
            var options = new ObservableCollection<SimpleOption>
            {
                new SimpleOption { Content = "Baidu" },
            // ������Ӹ���ѡ��...
            };

            var options_TargetLau = new ObservableCollection<SimpleOption>
            {
                new SimpleOption { Content = "zh" },
                new SimpleOption { Content = "jp" },
            // ������Ӹ���ѡ��...
            };

            comboBoxOptions.ItemsSource = options;

            comboBoxOptionsTargetLau.ItemsSource = options_TargetLau;
            // ��ȡ��ǰ���õķ���Ӧ������
            string currentTranslationApp = ConfigManager.GetSetting("Assembly", "TranslationApp", "Baidu");
            // ʹ��LINQ������ѡ����Ĺ���
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


            // ��ȡTranslationAppId �� TranslationSecretKey
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
            // ��ȡ��ǰѡ�еķ���Ӧ������
            string selectedTranslationApp = ((SimpleOption)comboBoxOptions.SelectedItem)?.Content ?? "Baidu";
            string selectedTranslationTL = ((SimpleOption)comboBoxOptionsTargetLau.SelectedItem)?.Content ?? "zh";
            // ��ȡ�û������AppId��SecretKey
            string translationAppId = appIdInput.Text;
            string translationSecretKey = secretKeyInput.Text;
            // ʹ��ConfigManager������Щ����
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
                    // ������ѡ��Ӧ�ó������ƻ�ȡ��Ӧ��Id��SecretKey
                    string translationAppId = ConfigManager.GetSetting("Assembly", "TranslationAppId");
                    string translationSecretKey = ConfigManager.GetSetting("Assembly", "TranslationSecretKey");
                    appIdInput.Text = translationAppId;
                    secretKeyInput.Text = translationSecretKey;
                }
            }
        }
    }
}
