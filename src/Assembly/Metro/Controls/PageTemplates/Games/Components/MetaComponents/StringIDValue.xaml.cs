using System.Windows;
using System.Windows.Controls;
using Blamite.Util;
using Assembly.Metro.Dialogs;
using Assembly.Tool.TranslateService;
using System.Windows.Input;
using Assembly.Tool.GPTservice;

namespace Assembly.Metro.Controls.PageTemplates.Games.Components.MetaComponents
{
	/// <summary>
	///     Interaction logic for StringIDValue.xaml
	/// </summary>
	public partial class StringIDValue : UserControl
	{
		public static readonly DependencyProperty SearchTrieProperty = DependencyProperty.Register("SearchTrie", typeof (Trie),
			typeof (StringIDValue));

		public StringIDValue()
		{
			InitializeComponent();
		}
        private async void btnAI_Click(object sender, RoutedEventArgs e)
        {
            string name = lblValueName.Text;
            await GPTstreamClient.GPT_Async(name);
        }
        public Trie SearchTrie
		{
			get { return (Trie) GetValue(SearchTrieProperty); }
			set { SetValue(SearchTrieProperty, value); }
		}

        private void DockPanel_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
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