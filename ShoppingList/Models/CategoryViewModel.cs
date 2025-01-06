using System.Collections.ObjectModel;

namespace ShoppingList.Models
{
    public class CategoryViewModel
    {
        private static CategoryViewModel categoryView;
        public static CategoryViewModel Category
        {
            get
            {
                if (categoryView == null)
                {
                    categoryView = new CategoryViewModel();
                }
                return categoryView;
            }
        }

        public ObservableCollection<CategoryModel> Categories { get; set; }

        private CategoryViewModel()
        {
            Categories = new ObservableCollection<CategoryModel>();
        }
    }
}
