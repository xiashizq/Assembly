using System.Diagnostics;
using System.IO;
using System.Windows.Controls;
using Assembly.Metro.Controls.PageTemplates.Games.Components.MetaData;
using Assembly.Metro.Dialogs;
using Blamite.Blam.Shaders;
using Blamite.Plugins;
using Assembly.Tool.TranslateService;
using System.Windows.Input;

namespace Assembly.Metro.Controls.PageTemplates.Games.Components.MetaComponents
{
	/// <summary>
	///     Interaction logic for Shader.xaml
	/// </summary>
	public partial class Shader : UserControl
	{
		public Shader()
		{
			InitializeComponent();
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

        private void btnDisassemble_Click(object sender, System.Windows.RoutedEventArgs e)
		{
			var shaderRef = (ShaderRef)DataContext;
			var shader = shaderRef.Shader;
			if (shader == null)
				return;

			var xsdPath = App.AssemblyStorage.AssemblySettings.XsdPath;
			if (string.IsNullOrWhiteSpace(xsdPath) || !File.Exists(xsdPath))
			{
				MetroMessageBox.Show("xsd.exe (from the XDK) is required in order to disassemble shaders.\r\nYou can set a path to it in Settings under Map Editor.");
				return;
			}

			var microcodePath = Path.GetTempFileName();
			try
			{
				// Write the microcode to a file so XSD can use it
				File.WriteAllBytes(microcodePath, shader.Microcode);

				// Start XSD.exe with one of the /raw switches (depending upon shader type)
				// and the microcode file
				var startInfo = new ProcessStartInfo(xsdPath);
				if (shaderRef.Type == ShaderType.Pixel)
					startInfo.Arguments = "/rawps";
				else
					startInfo.Arguments = "/rawvs";

				// Add the path to the microcode file
				startInfo.Arguments += " \"" + microcodePath + "\"";
				startInfo.CreateNoWindow = true;
				startInfo.RedirectStandardOutput = true;
				startInfo.UseShellExecute = false;

				// Run it and capture the output
				var process = Process.Start(startInfo);
				var output = process.StandardOutput.ReadToEnd();
				process.WaitForExit();

				// Display it
				MetroMessageBoxCode.Show("Shader Disassembly", output);
			}
			finally
			{
				File.Delete(microcodePath);
			}
		}
	}
}