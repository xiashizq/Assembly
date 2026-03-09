namespace Assembly.Metro.Controls.PageTemplates.Games.Components.MetaData
{
	public class CommentData : MetaField
	{
		private string _name, _text, _translatedText;

		public CommentData(string name, string text, uint pluginLine)
		{
			_name = name;
			_text = text;
			_translatedText = null;
			PluginLine = pluginLine;
		}

		public string Name
		{
			get { return _name; }
			set
			{
				_name = value;
				NotifyPropertyChanged("Name");
			}
		}

		public string Text
		{
			get { return _text; }
			set
			{
				_text = value;
				NotifyPropertyChanged("Text");
			}
		}

		public string TranslatedText
		{
			get { return _translatedText; }
			set
			{
				_translatedText = value;
				NotifyPropertyChanged("TranslatedText");
				NotifyPropertyChanged("HasTranslatedText");
			}
		}

		public bool TextExists
		{
			get { return !string.IsNullOrEmpty(_text); }
		}

		public bool HasTranslatedText
		{
			get { return !string.IsNullOrEmpty(_translatedText); }
		}

		public override void Accept(IMetaFieldVisitor visitor)
		{
			visitor.VisitComment(this);
		}

		public override MetaField CloneValue()
		{
			CommentData cloned = new CommentData(_name, _text, PluginLine);
			cloned.TranslatedText = _translatedText;
			return cloned;
		}

		public override string AsString()
		{
			return string.Format("comment | {0} ", Name);
		}
	}
}