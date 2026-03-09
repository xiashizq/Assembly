using System.Windows.Controls;
using System.Windows.Input;
using Assembly.Tool.TranslateService;
using Assembly.Metro.Controls.PageTemplates.Games.Components.MetaData;
using System;

namespace Assembly.Metro.Controls.PageTemplates.Games.Components.MetaComponents
{
	/// <summary>
	///     Interaction logic for Comment.xaml
	/// </summary>
	public partial class Comment : UserControl
	{
		public Comment()
		{
			InitializeComponent();
		}

		private void lblComment_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
		{
			if (DataContext is CommentData commentData && !string.IsNullOrEmpty(commentData.Text))
			{
				// 提取原始文本（移除已有的翻译内容）
				string originalText = commentData.Text;
				int startIndex = originalText.IndexOf("【");
				int endIndex = originalText.LastIndexOf("】");
				if (startIndex >= 0 && endIndex > startIndex)
				{
					originalText = originalText.Substring(0, startIndex).TrimEnd();
				}
                Console.WriteLine(originalText);
                // 翻译原始文本
                string translatedText = PublicTranslateService.TranslateAsync(originalText);
				if (!string.IsNullOrEmpty(translatedText) && translatedText != "Translation not found")
				{
					// 用【】包围翻译文本并追加到原始文本
					commentData.Text = originalText + "\n【" + translatedText + "】";
				}
			}
		}
	}
}