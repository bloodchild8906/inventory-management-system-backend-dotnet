using Persistence.GenericRepository.Base;

namespace Inventory.Application.Domain
{
    public class Product : BaseEntity<int>
    {
        // Required constructor
        private Product()
        {
            Id = 0;
            Name = default!;
            Price = 0;

        }

        public Product(string name, decimal price)
        {
            Name = name;
            Price = price;
        }

        public string Name { get; private set; }
        public decimal Price { get; private set; }

        public void Update(string name, decimal price)
        {
            Name = name;
            Price = price;
        }
    }
}
