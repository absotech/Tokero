using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace Tokero.Models
{
    public class CryptoCurrency : INotifyPropertyChanged
    {
        public string Symbol { get; set; }
        public string Name { get; set; }

        private bool isSelected;
        public bool IsSelected
        {
            get => isSelected;
            set
            {
                if (isSelected != value)
                {
                    isSelected = value;
                    OnPropertyChanged();
                }
            }
        }
        public event PropertyChangedEventHandler? PropertyChanged;
        protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
