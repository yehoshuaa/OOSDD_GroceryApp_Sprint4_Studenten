using Grocery.Core.Interfaces.Repositories;
using Grocery.Core.Interfaces.Services;
using Grocery.Core.Models;

namespace Grocery.Core.Services
{
    public class GroceryListItemsService : IGroceryListItemsService
    {
        private readonly IGroceryListItemsRepository _groceriesRepository;
        private readonly IProductRepository _productRepository;

        public GroceryListItemsService(IGroceryListItemsRepository groceriesRepository, IProductRepository productRepository)
        {
            _groceriesRepository = groceriesRepository;
            _productRepository = productRepository;
        }

        public List<GroceryListItem> GetAll()
        {
            List<GroceryListItem> groceryListItems = _groceriesRepository.GetAll();
            FillService(groceryListItems);
            return groceryListItems;
        }

        public List<GroceryListItem> GetAllOnGroceryListId(int groceryListId)
        {
            List<GroceryListItem> groceryListItems = _groceriesRepository.GetAll().Where(g => g.GroceryListId == groceryListId).ToList();
            FillService(groceryListItems);
            return groceryListItems;
        }

        public GroceryListItem Add(GroceryListItem item)
        {
            return _groceriesRepository.Add(item);
        }

        public GroceryListItem? Delete(GroceryListItem item)
        {
            throw new NotImplementedException();
        }

        public GroceryListItem? Get(int id)
        {
            return _groceriesRepository.Get(id);
        }

        public GroceryListItem? Update(GroceryListItem item)
        {
            return _groceriesRepository.Update(item);
        }


        
        public List<BestSellingProducts> GetBestSellingProducts(int topX = 5)
        {
            if (topX <= 0) topX = 5;

            var items = _groceriesRepository.GetAll();
            var grouped = GroupProducts(items);

            // 1x alle producten ophalen i.p.v. per item get
            var products = _productRepository.GetAll()
                            .ToDictionary(p => p.Id); 

            return MapToBestSellingProducts(grouped, topX, products);
        }

        // tellen per product.
        private static IEnumerable<(int ProductId, int NrOfSells)> GroupProducts(List<GroceryListItem> items)
        {
            return items
                .Where(i => i.Amount > 0) // filter 
                .GroupBy(i => i.ProductId)
                .Select(g => (ProductId: g.Key, NrOfSells: g.Sum(i => i.Amount)));
        }

        // Objecten maken voor de ViewModel
        private static List<BestSellingProducts> MapToBestSellingProducts(
            IEnumerable<(int ProductId, int NrOfSells)> grouped,
            int topX,
            IDictionary<int, Product> productsById)
        {
            var result = new List<BestSellingProducts>();
            int rank = 1;

            foreach (var g in grouped
                .OrderByDescending(x => x.NrOfSells)
                .ThenBy(x => productsById.TryGetValue(x.ProductId, out var p1) ? p1.Name : string.Empty) // stabiele tiebreaker
                .Take(topX))
            {
                productsById.TryGetValue(g.ProductId, out var product);

                result.Add(new BestSellingProducts(
                    g.ProductId,
                    product?.Name ?? "Onbekend",
                    product?.Stock ?? 0,
                    g.NrOfSells,
                    rank++
                ));
            }

            return result;
        }

        private void FillService(List<GroceryListItem> groceryListItems)
        {
            foreach (GroceryListItem g in groceryListItems)
            {
                g.Product = _productRepository.Get(g.ProductId) ?? new(0, "", 0);
            }
        }
    }
}
