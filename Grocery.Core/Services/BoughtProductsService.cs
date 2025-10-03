
using Grocery.Core.Interfaces.Repositories;
using Grocery.Core.Interfaces.Services;
using Grocery.Core.Models;

namespace Grocery.Core.Services
{
    public class BoughtProductsService : IBoughtProductsService
    {
        private readonly IGroceryListItemsRepository _groceryListItemsRepository;
        private readonly IClientRepository _clientRepository;
        private readonly IProductRepository _productRepository;
        private readonly IGroceryListRepository _groceryListRepository;
        public BoughtProductsService(IGroceryListItemsRepository groceryListItemsRepository, IGroceryListRepository groceryListRepository, IClientRepository clientRepository, IProductRepository productRepository)
        {
            _groceryListItemsRepository=groceryListItemsRepository;
            _groceryListRepository=groceryListRepository;
            _clientRepository=clientRepository;
            _productRepository=productRepository;
        }

        //UC13: Overzicht gekochte producten per product
        public List<BoughtProducts> Get(int? productId)
        {
            if (productId is null) return new List<BoughtProducts>();
            int pid = productId.Value;

            var product = _productRepository.Get(pid);
            if (product == null) return new List<BoughtProducts>();

            var items = _groceryListItemsRepository.GetAll()
                         .Where(i => i.ProductId == pid && i.Amount > 0);

            var result = new List<BoughtProducts>();
            var seen = new HashSet<(int clientId, int listId)>(); // dedupe per (Client, List)

            foreach (var it in items)
            {
                // 1) Eerst de lijst ophalen → daar zit de ClientId
                var list = _groceryListRepository.Get(it.GroceryListId);
                if (list == null) continue;

                // 2) Dan de client via de lijst
                var client = _clientRepository.Get(list.ClientId);
                if (client == null) continue;

                // 3) Dubbelen voorkomen (zelfde client + lijst voor dit product)
                if (!seen.Add((client.Id, list.Id))) continue;

                // 4) Juiste ctor gebruiken (BoughtProducts(Client, GroceryList, Product))
                result.Add(new BoughtProducts(client, list, product));
            }

            return result;
        }

    }
}
