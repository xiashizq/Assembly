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
	///     Interaction logic for AsciiValue.xaml
	/// </summary>
	public partial class AsciiValue : UserControl
	{
		public AsciiValue()
		{
			InitializeComponent();
		}

        private async void btnAI_Click(object sender, RoutedEventArgs e)
        {
            string name = lblValueName.Text;
            await GPTstreamClient.GPT_Async(name);
        }

        private void DockPane2_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            // 这里获取绑定的文本值
            var dockPanel = sender as DockPanel;
            var dataContext = dockPanel?.DataContext;
            var name = dataContext?.GetType().GetProperty("Name")?.GetValue(dataContext)?.ToString();
            var tooltip = dataContext?.GetType().GetProperty("ToolTip")?.GetValue(dataContext)?.ToString();
            string result = "";
            if (!string.IsNullOrEmpty(name))
            {
                if (!string.IsNullOrEmpty(tooltip))
                {
                    result = PublicTranslateService.TranslateAsync(name + "（" + tooltip + "）");
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