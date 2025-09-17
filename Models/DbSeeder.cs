using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace katsuCMS_backend.Models
{
    public  static class DbSeeder
    {
        public static void Seed(ApplicationDbContext context)
        {
            context.Database.EnsureCreated();

            if (!context.Units.Any())
            {
                context.Units.AddRange(
                    new Unit { UnitName = "Piece" },
                    new Unit { UnitName = "Kilogram" },
                    new Unit { UnitName = "Liter" }
                );
                context.SaveChanges();
            }

            if (!context.Categories.Any())
            {
                context.Categories.AddRange(
                    new ProductCategory { CategoryName = "Raw Material" },
                    new ProductCategory { CategoryName = "Frozen" },
                    new ProductCategory { CategoryName = "Fresh Goods" },
                    new ProductCategory { CategoryName = "Beverages" }
                );
                context.SaveChanges();
            }
            if (!context.Suppliers.Any())
            {
                context.Suppliers.AddRange(
                    new Supplier { SupplierName = "Foster Foods DP", Address = "Dressing Plant", ContactNumber = "123-456-7890" },
                    new Supplier { SupplierName = "Foster Foods HO", Address = "Tektitte", ContactNumber = "987-654-3210" }
                );
                context.SaveChanges();
            }

            if (!context.Stores.Any())
            {
                context.Stores.AddRange(
                    new Store { Name = "SM Megamall", Address = "Ortigas", ContactNumber = "123-456-7890" },
                    new Store { Name = "SM North EDSA", Address = "North Ave", ContactNumber = "987-654-3210" },
                    new Store { Name = "SM Fairview", Address = "Fairview", ContactNumber = "555-555-5555" }
                );
                context.SaveChanges();
            }

            if (!context.Products.Any())
            {
                var unitPiece = context.Units.First(u => u.UnitName == "Piece");
                var cat = context.Categories.First(c => c.CategoryName == "Frozen");
                var supplier = context.Suppliers.First(s => s.SupplierName == "Foster Foods DP");
                var store = context.Stores.First(s => s.Name == "SM Megamall");

                context.Products.AddRange(
                    new Product
                    {
                        ProductCode = "A001",
                        ProductName = "Chicken Breast",
                        Quantity = 100,
                        Price = 150.00m,
                        Description = "Fresh chicken breast",
                        CategoryId = cat.Id,
                        UnitId = unitPiece.Id,
                        SupplierId = supplier.Id
                    },
                    new Product
                    {
                        ProductCode = "A002",
                        ProductName = "Chicken Thigh",
                        Quantity = 200,
                        Price = 120.00m,
                        Description = "Fresh chicken thigh",
                        CategoryId = cat.Id,
                        UnitId = unitPiece.Id,
                        SupplierId = supplier.Id  
                    },
                    new Product
                    {
                        ProductCode = "A003",
                        ProductName = "Chicken Neck",
                        Quantity = 50,
                        Price = 70.00m,
                        Description = "Fresh chicken Neck",
                        CategoryId = cat.Id,
                        UnitId = unitPiece.Id,
                        SupplierId = supplier.Id  
                    }
                );
                context.SaveChanges();
            }
        }
    }
}