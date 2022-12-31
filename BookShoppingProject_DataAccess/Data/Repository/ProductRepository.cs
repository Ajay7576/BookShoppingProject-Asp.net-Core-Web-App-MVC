using BookShoppingProject_DataAccess.Data.Repository.IRepository;
using BookShoppingProject_Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BookShoppingProject_DataAccess.Data.Repository
{
   public class ProductRepository:Repository<Product>,IProductRepository
    {
        private readonly ApplicationDbContext _context;
        public ProductRepository(ApplicationDbContext context):base(context)
        {
            _context = context;
        }

        public void Update(Product product)
        {
            var ProductIndb = _context.Products.FirstOrDefault(p => p.Id == product.Id);
            if(ProductIndb!=null)
            {
                if (product.ImageUrl != "")
                    ProductIndb.ImageUrl = product.ImageUrl;
                    ProductIndb.Title = product.Title;
                    ProductIndb.Discription = product.Discription;
                    ProductIndb.ISBN = product.ISBN;
                    ProductIndb.Author = product.Author;
                    ProductIndb.ListPrice = product.ListPrice;
                    ProductIndb.Price50 = product.Price50;
                    ProductIndb.Price100 = product.Price100;
                    ProductIndb.Price = product.Price;
                    ProductIndb.CategoryId = product.CategoryId;
                    ProductIndb.CoverTypeId = product.CoverTypeId;
                
            }
        }
    }
}
