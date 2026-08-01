using System.ComponentModel;

namespace Assembly.Metro.SharedViewModelUntil
{
	public class SharedViewModel : INotifyPropertyChanged
	{
		private string _aiText = string.Empty;
		private string _aiLongText = string.Empty;
		private string _aiStatusText = "Ready";
		private bool _isAiBusy;
		private string _metaEditorXmlPath;
		private int _aiPanelOpenRequest;

		public string AiText
		{
			get { return _aiText; }
			set
			{
				if (_aiText != value)
				{
					_aiText = value ?? string.Empty;
					OnPropertyChanged(nameof(AiText));
					OnPropertyChanged(nameof(HasAiText));
					OnPropertyChanged(nameof(HasAnyAiContent));
					OnPropertyChanged(nameof(ShowEmptyState));
				}
			}
		}

		public string AiLongText
		{
			get { return _aiLongText; }
			set
			{
				if (_aiLongText != value)
				{
					_aiLongText = value ?? string.Empty;
					OnPropertyChanged(nameof(AiLongText));
					OnPropertyChanged(nameof(HasAiLongText));
					OnPropertyChanged(nameof(HasAnyAiContent));
					OnPropertyChanged(nameof(ShowEmptyState));
				}
			}
		}

		public string AiStatusText
		{
			get { return _aiStatusText; }
			set
			{
				if (_aiStatusText != value)
				{
					_aiStatusText = value ?? string.Empty;
					OnPropertyChanged(nameof(AiStatusText));
				}
			}
		}

		public bool IsAiBusy
		{
			get { return _isAiBusy; }
			set
			{
				if (_isAiBusy != value)
				{
					_isAiBusy = value;
					OnPropertyChanged(nameof(IsAiBusy));
					OnPropertyChanged(nameof(ShowEmptyState));
					AiStatusText = value ? "Thinking..." : "Ready";
				}
			}
		}

		/// <summary>
		/// Increment to request MetaEditor open the AI side panel.
		/// </summary>
		public int AiPanelOpenRequest
		{
			get { return _aiPanelOpenRequest; }
			set
			{
				_aiPanelOpenRequest = value;
				OnPropertyChanged(nameof(AiPanelOpenRequest));
			}
		}

		public bool HasAiText => !string.IsNullOrWhiteSpace(_aiText);
		public bool HasAiLongText => !string.IsNullOrWhiteSpace(_aiLongText);
		public bool HasAnyAiContent => HasAiText || HasAiLongText;
		public bool ShowEmptyState => !HasAnyAiContent && !_isAiBusy;

		public string MetaEditorXmlPath
		{
			get { return _metaEditorXmlPath; }
			set
			{
				if (_metaEditorXmlPath != value)
				{
					_metaEditorXmlPath = value;
					OnPropertyChanged(nameof(MetaEditorXmlPath));
				}
			}
		}

		public void RequestOpenAiPanel()
		{
			AiPanelOpenRequest++;
		}

		public void BeginAi()
		{
			IsAiBusy = true;
			RequestOpenAiPanel();
		}

		public void EndAi()
		{
			IsAiBusy = false;
		}

		public event PropertyChangedEventHandler PropertyChanged;

		protected void OnPropertyChanged(string propertyName)
		{
			PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
		}
	}
}
