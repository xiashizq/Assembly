using System.ComponentModel;

namespace Assembly.Metro.SharedViewModelUntil
{
    public class SharedViewModel : INotifyPropertyChanged
    {
        private string _aiText;
        private string _aiLongText;

        private string _metaEditorXmlPath;

        public string AiText
        {
            get { return _aiText; }
            set
            {
                if (_aiText != value)
                {
                    _aiText = value;
                    OnPropertyChanged(nameof(AiText));
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
                    _aiLongText = value;
                    OnPropertyChanged(nameof(AiLongText));
                }
            }
        }

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

        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
