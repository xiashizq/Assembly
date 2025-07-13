using System.Windows.Controls;
using System.Windows.Input;
using Assembly.Metro.Dialogs;
using Assembly.Tool.TranslateService;

namespace Assembly.Metro.Controls.PageTemplates.Games.Components.MetaComponents
{
	/// <summary>
	///     Interaction logic for Plane2Value.xaml
	/// </summary>
	public partial class Plane2Value : UserControl
	{
		public Plane2Value()
		{
			InitializeComponent();
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