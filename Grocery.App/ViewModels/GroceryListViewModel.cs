using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Grocery.Core.Interfaces.Services;
using Grocery.Core.Models;
using System.Collections.ObjectModel;

namespace Grocery.App.ViewModels
{
    public partial class GroceryListViewModel : BaseViewModel
    {
        public ObservableCollection<GroceryList> GroceryLists { get; set; }
        private readonly IGroceryListService _groceryListService;
        private readonly IClientService _clientService;   
        public Client Client { get; }                     //toolbar binding
        //UC13: admin view
        public GroceryListViewModel(IGroceryListService groceryListService,
                                    IClientService clientService)            // ← DI aanvullen
        {
            Title = "Boodschappenlijst";
            _groceryListService = groceryListService;
            _clientService = clientService;

            GroceryLists = new(_groceryListService.GetAll());

            // UC13: user3 is admin;
            Client = _clientService.Get(3)!;             
        }

        [RelayCommand]
        public async Task SelectGroceryList(GroceryList groceryList)
        {
            Dictionary<string, object> paramater = new() { { nameof(GroceryList), groceryList } };
            await Shell.Current.GoToAsync($"{nameof(Views.GroceryListItemsView)}?Titel={groceryList.Name}", true, paramater);
        }

        public override void OnAppearing()
        {
            base.OnAppearing();
            GroceryLists = new(_groceryListService.GetAll());
        }

        public override void OnDisappearing()
        {
            base.OnDisappearing();
            GroceryLists.Clear();
        }

        // ⬇️ UC13: alleen admins mogen naar BoughtProductsView
        [RelayCommand]
        public async Task ShowBoughtProducts()
        {
            if (Client.Role == Role.admin)
            {
                await Shell.Current.GoToAsync(nameof(Views.BoughtProductsView));
            }
        }
    }
}