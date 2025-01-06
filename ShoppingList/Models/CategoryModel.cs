using System.Collections.ObjectModel;
using System.ComponentModel;

namespace ShoppingList.Models
{
    public class CategoryModel : INotifyPropertyChanged
    {
        private string _categoryName;

        public string CategoryName
        {
            get => _categoryName;
            set
            {
                if (_categoryName != value)
                {
                    _categoryName = value;
                    OnPropertyChanged(nameof(CategoryName));
                }
            }
        }

        public ObservableCollection<ProductModel> Products { get; set; } = new ObservableCollection<ProductModel>(); 

        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}