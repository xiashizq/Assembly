using System;
using System.Collections.Generic;
using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using Assembly.Metro.Dialogs.ControlDialogs;
using Assembly.Metro.Controls.PageTemplates.Games.Components.MetaData;
using Assembly.Metro.Dialogs;
using Assembly.Tool.TranslateService;
using Assembly.Tool.GPTservice;

namespace Assembly.Metro.Controls.PageTemplates.Games.Components.MetaComponents
{
	/// <summary>
	///     Interaction logic for intValue.xaml
	/// </summary>
	public partial class intValue : UserControl
	{
		public intValue()
		{
			InitializeComponent();
		}

        /*private void viewValueAs_Click(object sender, RoutedEventArgs e)
        {
            MetaData.ValueField value = this.DataContext as MetaData.ValueField;
            MetroViewValueAs.Show(value.MemoryAddress, value.CacheOffset);
        }*/

        public async void btnAI_Click(object sender, RoutedEventArgs e)
        {
            string name = lblValueName.Text;
            await GPTstreamClient.GPT_Async(name);
        }

        private void DockPanel_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            // �����ȡ�󶨵��ı�ֵ
            var dockPanel = sender as DockPanel;
            var dataContext = dockPanel?.DataContext;
            var name = dataContext?.GetType().GetProperty("Name")?.GetValue(dataContext)?.ToString();
            var tooltip = dataContext?.GetType().GetProperty("ToolTip")?.GetValue(dataContext)?.ToString();
            string result = "";
            if (!string.IsNullOrEmpty(name))
            {
                if (!string.IsNullOrEmpty(tooltip))
                {
                    result = PublicTranslateService.TranslateAsync(name + "��" + tooltip + "��");
                }
                else
                {
                    result = PublicTranslateService.TranslateAsync(name);
                }

                MetroMessageBox.Show($"{result}");
            }
        }
    }
}