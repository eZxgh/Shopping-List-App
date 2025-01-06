using System.ComponentModel;
using ShoppingList.Models;

namespace ShoppingList.Models
{
    public class ProductModel : INotifyPropertyChanged
    {
        public string Name { get; set; }
        public string Unit { get; set; }

        public string CategoryName { get; set; }  

        private bool _isPurchased;
        public bool IsPurchased
        {
            get => _isPurchased;
            set
            {
                if (_isPurchased != value)
                {
                    _isPurchased = value;
                    OnPropertyChanged(nameof(IsPurchased));
                }
            }
        }

        private int _Quantity;
        public int Quantity
        {
            get => _Quantity;
            set
            {
                if (_Quantity != value)
                {
                    _Quantity = value;
                    OnPropertyChanged(nameof(Quantity));
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
