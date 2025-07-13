#if !DEBUG
using Assembly.Metro.Dialogs;
#endif
using System;
using System.Collections.Generic;
using System.IO;
using System.Windows;
using Assembly.Helpers;
using Blamite.RTE.Console;
using Microsoft.Shell;

namespace Assembly
{
	/// <summary>
	///     Interaction logic for App.xaml
	/// </summary>
	public partial class App : ISingleInstanceApp
	{
		#region ISingleInstanceApp Members

		public bool SignalExternalCommandLineArgs(IList<string> args)
		{
			return AssemblyStorage.AssemblySettings.HomeWindow == null ||
			       AssemblyStorage.AssemblySettings.HomeWindow.ProcessCommandLineArgs(args);
		}

		#endregion

		public static Storage AssemblyStorage { get; set; }

		[STAThread]
		public static void Main()
		{
			if (!SingleInstance<App>.InitializeAsFirstInstance("RecivedCommand")) return;

			var application = new App();

            application.InitializeComponent();
            application.Run();
			// Allow single instance code to perform cleanup operations
			SingleInstance<App>.Cleanup();
            Application_Config();
        }


        private static void Application_Config()
        {
            // 应用程序名称
            string appName = "Assembly";
            // 获取AppData路径
            string appDataPath = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
            // 构建应用特定的文件夹路径
            string appSpecificFolderPath = Path.Combine(appDataPath, appName);
            // 检查文件夹是否存在，如果不存在则创建
            if (!Directory.Exists(appSpecificFolderPath))
            {
                Directory.CreateDirectory(appSpecificFolderPath);
            }
            // 配置文件路径
            string configFilePath = Path.Combine(appSpecificFolderPath, "config.ini");
            // 检查配置文件是否存在，若不存在则创建并写入默认内容
            if (!File.Exists(configFilePath))
            {
                using (StreamWriter sw = File.CreateText(configFilePath))
                {
                    sw.WriteLine("[Settings]");
                    sw.WriteLine("TranslationApp=Baidu");
                    sw.WriteLine("TranslationAppId=");
                    sw.WriteLine("TranslationSecretKey=");
                    sw.WriteLine("TranslationTargetlanguage=zh");
                    sw.WriteLine("GptApp=Qwen");
                    sw.WriteLine("GptAppId=");
                    sw.WriteLine("GptAppKey=");
                }
            }
        }


        protected override void OnStartup(StartupEventArgs e)
		{
			base.OnStartup(e);

#if !DEBUG
			Current.DispatcherUnhandledException += (o, args) =>
			{
				MetroException.Show(args.Exception);

				args.Handled = true;
			};
#endif
			// Reset Directory
			System.IO.Directory.SetCurrentDirectory(System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location));

			// Load version info
			VersionInfo.Load("version.json");

			// Create Assembly Storage
#if !DEBUG
			try
			{
				App.AssemblyStorage = new Storage();
			}
			catch (Exception ex)
			{
				Metro.Dialogs.MetroException.Show(ex);
			}
#endif
#if DEBUG
			App.AssemblyStorage = new Storage();
#endif

			// Update Assembly Protocol
			AssemblyProtocol.UpdateProtocol();

			// Create jumplist
			JumpLists.UpdateJumplists();

			// Create XBDM Instances
			AssemblyStorage.AssemblySettings.XenonConsole = new XeConsole(AssemblyStorage.AssemblySettings.XeConsoleNameIp);
			AssemblyStorage.AssemblySettings.XboxConsole = new XbConsole(AssemblyStorage.AssemblySettings.XbConsoleNameIp);
			AssemblyStorage.AssemblySettings.XenonFusionConsole = new XeConsole(AssemblyStorage.AssemblySettings.XeConsoleNameIp, true);

			// Try and delete all temp data
			VariousFunctions.EmptyUpdaterLocations();

			// Dubs, checkem
			_0xabad1dea.CheckServerStatus();

			// Update File Defaults
			FileDefaults.UpdateFileDefaults();

			// Set closing method
			Current.Exit += (o, args) =>
			{
				// Update Settings with Window Width/Height
				AssemblyStorage.AssemblySettings.ApplicationSizeMaximize =
					(AssemblyStorage.AssemblySettings.HomeWindow.WindowState == WindowState.Maximized);
				if (AssemblyStorage.AssemblySettings.ApplicationSizeMaximize) return;

				AssemblyStorage.AssemblySettings.ApplicationSizeWidth = AssemblyStorage.AssemblySettings.HomeWindow.Width;
				AssemblyStorage.AssemblySettings.ApplicationSizeHeight = AssemblyStorage.AssemblySettings.HomeWindow.Height;
			};

		}
	}
}