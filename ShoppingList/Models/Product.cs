using System.Collections.ObjectModel;
using System.ComponentModel;

namespace ShoppingList.Models
{
    public class Product : INotifyPropertyChanged
    {
        public ObservableCollection<ProductModel> Products { get; set; } = new();

        public Product()
        {
            Products.CollectionChanged += (s, e) => OnPropertyChanged(nameof(FilteredProducts));
        }

        public IEnumerable<ProductModel> FilteredProducts
        {
            get => Products.Where(product => !product.IsPurchased);
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }

    public static class ProductRepository
    {
        public static Product SharedProduct { get; } = new Product();
    }
}



