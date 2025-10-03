using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Grocery.Core.Interfaces.Services;
using Grocery.Core.Models;
using System.Collections.ObjectModel;


namespace Grocery.App.ViewModels
{
    public partial class BoughtProductsViewModel : BaseViewModel
    {
        private readonly IBoughtProductsService _boughtProductsService;

        [ObservableProperty]
        Product selectedProduct;
        public ObservableCollection<BoughtProducts> BoughtProductsList { get; set; } = new();
        public ObservableCollection<Product> Products { get; set; }

        public BoughtProductsViewModel(IBoughtProductsService boughtProductsService, IProductService productService)
        {
            _boughtProductsService = boughtProductsService;
            Products = new(productService.GetAll());
        }

        //UC13: Overzicht gekochte producten per product
        partial void OnSelectedProductChanged(Product? oldValue, Product newValue)
        {
            BoughtProductsList.Clear();

            // Haal de regels op voor dit product via de service (join-resultaat client+lijst+product)
            var rows = _boughtProductsService.Get(newValue.Id); 

            // Vul de lijst die aan de CollectionView is gebonden [databinding]
            foreach (var r in rows)
                BoughtProductsList.Add(r);
        }

        [RelayCommand]
        public void NewSelectedProduct(Product product)
        {
            SelectedProduct = product;
        }
    }
}
