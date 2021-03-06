using System.Collections.Generic;
using FooBar.Domain.Entities;
using FooBar.Infrastructure.Data;

namespace FooBar.Api.IntegrationTests.Abstract
{
    public static class Utilities
    {
        public static class CatalogItemNames
        {
            public const string RoslynRedSheet = "Roslyn Red Sheet";
        }
        
        public static void InitializeDbForTests(CatalogContext db)
        {
            db.CatalogBrands.AddRange(GetPreconfiguredCatalogBrands());
            db.CatalogTypes.AddRange(GetPreconfiguredCatalogTypes());
            db.CatalogItems.AddRange(GetPreconfiguredItems());
            db.SaveChanges();
        }

        public static void ReinitializeDbForTests(CatalogContext db)
        {
            db.CatalogBrands.RemoveRange(db.CatalogBrands);
            db.CatalogTypes.RemoveRange(db.CatalogTypes);
            db.CatalogItems.RemoveRange(db.CatalogItems);
            InitializeDbForTests(db);
        }

        internal static IEnumerable<CatalogBrand> GetPreconfiguredCatalogBrands()
        {
            return new List<CatalogBrand>()
            {
                new CatalogBrand("Azure"),
                new CatalogBrand(".NET"),
                new CatalogBrand("Visual Studio"),
                new CatalogBrand("SQL Server"),
                new CatalogBrand("Other")
            };
        }

        internal static IEnumerable<CatalogType> GetPreconfiguredCatalogTypes()
        {
            return new List<CatalogType>()
            {
                new CatalogType("Mug"),
                new CatalogType("T-Shirt"),
                new CatalogType("Sheet"),
                new CatalogType("USB Memory Stick")
            };
        }

        internal static IEnumerable<CatalogItem> GetPreconfiguredItems()
        {
            return new List<CatalogItem>()
            {
                new CatalogItem(2,2, ".NET Bot Black Sweatshirt", ".NET Bot Black Sweatshirt", 19.5M,  "http://catalogbaseurltobereplaced/images/products/1.png"),
                new CatalogItem(1,2, ".NET Black & White Mug", ".NET Black & White Mug", 8.50M, "http://catalogbaseurltobereplaced/images/products/2.png"),
                new CatalogItem(2,5, "Prism White T-Shirt", "Prism White T-Shirt", 12,  "http://catalogbaseurltobereplaced/images/products/3.png"),
                new CatalogItem(2,2, ".NET Foundation Sweatshirt", ".NET Foundation Sweatshirt", 12, "http://catalogbaseurltobereplaced/images/products/4.png"),
                new CatalogItem(3,5, "Roslyn Red Sheet", CatalogItemNames.RoslynRedSheet, 8.5M, "http://catalogbaseurltobereplaced/images/products/5.png"),
                new CatalogItem(2,2, ".NET Blue Sweatshirt", ".NET Blue Sweatshirt", 12, "http://catalogbaseurltobereplaced/images/products/6.png"),
                new CatalogItem(2,5, "Roslyn Red T-Shirt", "Roslyn Red T-Shirt",  12, "http://catalogbaseurltobereplaced/images/products/7.png"),
                new CatalogItem(2,5, "Kudu Purple Sweatshirt", "Kudu Purple Sweatshirt", 8.5M, "http://catalogbaseurltobereplaced/images/products/8.png"),
                new CatalogItem(1,5, "Cup<T> White Mug", "Cup<T> White Mug", 12, "http://catalogbaseurltobereplaced/images/products/9.png"),
                new CatalogItem(3,2, ".NET Foundation Sheet", ".NET Foundation Sheet", 12, "http://catalogbaseurltobereplaced/images/products/10.png"),
                new CatalogItem(3,2, "Cup<T> Sheet", "Cup<T> Sheet", 8.5M, "http://catalogbaseurltobereplaced/images/products/11.png"),
                new CatalogItem(2,5, "Prism White TShirt", "Prism White TShirt", 12, "http://catalogbaseurltobereplaced/images/products/12.png")
            };
        }
    }
}