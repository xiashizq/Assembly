using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assembly.Metro.SharedViewModelUntil
{
    public class SharedViewModel : INotifyPropertyChanged
    {
        private string _aiText;

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

        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
