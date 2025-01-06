using System.Text;
using System.Text.Json;
using CommunityToolkit.Maui.Storage;
using ShoppingList.Models;

namespace ShoppingList.Views;

public partial class MainListPage : ContentPage
{
    private static readonly FilePickerFileType JsonFileType = new FilePickerFileType(new Dictionary<DevicePlatform, IEnumerable<string>>
    {
        { DevicePlatform.WinUI, new[] { ".json" } },
    });

    private readonly string _filePath = Path.Combine(FileSystem.AppDataDirectory, "products.json");

    public MainListPage()
    {
        BindingContext = ProductRepository.SharedProduct;
        InitializeComponent();
        LoadProducts();
    }

    private void onAddProductClicked(object sender, EventArgs e)
    {
        string name = productTitle.Text;
        string unit = productUnit.Text;
        string quantityText = productQuantity.Text;
        string categoryName = productCategory.Text;

        if (string.IsNullOrWhiteSpace(name) || string.IsNullOrWhiteSpace(unit) || string.IsNullOrWhiteSpace(quantityText) || string.IsNullOrWhiteSpace(categoryName))
        {
            DisplayAlert("Błąd", "Wszystkie pola muszą być wypełnione.", "OK");
            return;
        }

        if (!int.TryParse(quantityText, out int quantity))
        {
            DisplayAlert("Błąd", "Ilość musi być liczbą całkowitą.", "OK");
            return;
        }

        if (quantity <= 0)
        {
            DisplayAlert("Błąd", "Ilość nie może być mniejsza lub rowna 0.", "OK");
            return;
        }

        if (int.TryParse(name, out _) || int.TryParse(unit, out _) || int.TryParse(categoryName, out int _))
        {
            DisplayAlert("Błąd", "Nazwa, jednostka i kategoria nie mogą być liczbami.", "OK");
            return;
        }

        if (BindingContext is Product product)
        {
            var category = CategoryViewModel.Category.Categories
                .FirstOrDefault(c => c.CategoryName.Equals(categoryName, StringComparison.OrdinalIgnoreCase));

            if (category == null)
            {
                category = new CategoryModel { CategoryName = categoryName };
                CategoryViewModel.Category.Categories.Add(category);
            }

            var newProduct = new ProductModel
            {
                Name = name,
                Unit = unit,
                Quantity = quantity,
                CategoryName = category.CategoryName 
            };

            category.Products.Add(newProduct); 
            product.Products.Add(newProduct);  

            productTitle.Text = string.Empty;
            productUnit.Text = string.Empty;
            productQuantity.Text = string.Empty;
            productCategory.Text = string.Empty;

            saveProducts(); 
            RefreshProductsView();
        }
    }

    private void saveProducts()
    {
        try
        {
            if (BindingContext is Product product)
            {
                string json = JsonSerializer.Serialize(product.Products.ToList());
                File.WriteAllText(_filePath, json);
            }
        }
        catch (Exception ex)
        {
            DisplayAlert("Błąd", $"Nie udało się zapisać produktów: {ex.Message}", "OK");
        }
    }
    private void LoadProducts()
    {
        if (BindingContext is Product product)
        {
            product.Products.Clear();

            if (File.Exists(_filePath))
            {
                try
                {
                    string json = File.ReadAllText(_filePath);
                    var products = JsonSerializer.Deserialize<List<ProductModel>>(json);

                    if (products != null)
                    {
                        foreach (var loadedProduct in products)
                        {
                            product.Products.Add(loadedProduct);
                        }
                    }
                }
                catch (Exception ex)
                {
                    DisplayAlert("Błąd", $"Nie udało się załadować produktów: {ex.Message}", "OK");
                }
            }

            RefreshProductsView();
        }
    }

    private void RefreshProductsView()
    {
        if (BindingContext is Product product)
        {
            ProductsContainer.Children.Clear();

            foreach (var productModel in product.Products)
            {
                var productView = new ProductView(productModel, OnIncrement_Clicked, OnDecrement_Clicked, OnPurchased_Clicked, OnDelete_Clicked);
                ProductsContainer.Children.Add(productView);
            }
        }
    }
    private void OnIncrement_Clicked(ProductModel model)
    {
        model.Quantity += 1;
        saveProducts();
    }
    private void OnDecrement_Clicked(ProductModel model)
    {
        model.Quantity -= 1;
        if (model.Quantity < 0)
        {
            model.Quantity = 0;
        }
        saveProducts();
    }
    private void OnDelete_Clicked(ProductModel productModel)
    {
        if (BindingContext is Product product)
        {
            product.Products.Remove(productModel);
            saveProducts();
            RefreshProductsView();
        }
    }
    private void OnPurchased_Clicked(ProductModel productModel)
    {
        if (BindingContext is Product product)
        {
            int oldIndex = product.Products.IndexOf(productModel);

            if (oldIndex == -1) return;

            product.Products.RemoveAt(oldIndex);

            productModel.IsPurchased = !productModel.IsPurchased;

            product.Products.Add(productModel);

            saveProducts();
            RefreshProductsView();
        }
    }
    private async void onImportProductsClicked(object sender, EventArgs e)
    {
        try
        {
            var result = await FilePicker.PickAsync(new PickOptions
            {
                FileTypes = JsonFileType
            });

            if (result != null)
            {
                string json = File.ReadAllText(result.FullPath);

                var importedProducts = JsonSerializer.Deserialize<List<ProductModel>>(json);

                if (importedProducts != null && BindingContext is Product product)
                {
                    foreach (var newProduct in importedProducts)
                    {
                        product.Products.Add(newProduct);
                    }

                    saveProducts();
                    RefreshProductsView();
                }
                else
                {
                    await DisplayAlert("Błąd", "Nie udało się odczytać pliku lub plik jest pusty.", "OK");
                }
            }
        }
        catch (Exception ex)
        {
            await DisplayAlert("Błąd", $"Wystąpił problem podczas importowania: {ex.Message}", "OK");
        }
    }
    private async void onExportProductsClicked(object sender, EventArgs e)
    {
        try
        {
            if (BindingContext is Product product)
            {
                string json = JsonSerializer.Serialize(product.Products.ToList());

                var stream = new MemoryStream(Encoding.UTF8.GetBytes(json));

                var result = await FileSaver.SaveAsync("products.json", stream, new CancellationToken());

                if (result.IsSuccessful)
                {
                    await DisplayAlert("Sukces", "Plik został zapisany.", "OK");
                }
                else
                {
                    await DisplayAlert("Błąd", $"Nie udało się zapisać pliku: {result.Exception?.Message}", "OK");
                }
            }
        }
        catch (Exception ex)
        {
            await DisplayAlert("Błąd", $"Wystąpił problem podczas exportowania: {ex.Message}", "OK");
        }
    }
}
