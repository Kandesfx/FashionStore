using FashionStore.Models.Entities;
using System.Collections.Generic;

namespace FashionStore.Models.ViewModels
{
    public class HomeViewModel
    {
        public IEnumerable<Product> FeaturedProducts { get; set; }
        public IEnumerable<Product> LatestProducts { get; set; }
        public IEnumerable<Category> Categories { get; set; }
    }
}

