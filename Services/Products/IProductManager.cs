using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MarketPlace.Models.ProductModels;
using MarketPlace.Models;
using MarketPlace.Services.Validators.ProductValidators;

namespace MarketPlace.Services
{



    public interface IProductManager
    {
        Task<int> AddProduct(ProductModel product);
        Task<bool> UpdateProduct(ProductModel product, int id);
        Task<bool> DeleteProduct(int id);
        Task<IEnumerable<ProductModel>> GetProducts(int page = 1);
        Task<ProductModel> AddScore(int id, int score);
        Task<ProductModel> GetProduct(int id);
    }

    


    public class DbProductManager : IProductManager
    {
        const int numOnePageProd = 6;
        readonly MarkePlacetDb _db;
        public DbProductManager(MarkePlacetDb db)
        {
            _db = db;
        }

        public async Task<int> AddProduct(ProductModel product)
        {
            var prod = await _db.Products.AddAsync(product);
            await _db.SaveChangesAsync();
            return prod.Entity.Id;
        }

        public async Task<ProductModel> AddScore(int id, int score)
        {
            var prod = await _db.Products.FindAsync(id);

            if (prod == null)
                return null; 

            var sumScore = prod.Score * prod.Scored;
            sumScore += score;
            var numScored = ++prod.Scored;
            prod.Score = sumScore / numScored;

            _db.Products.Update(prod);
            await _db.SaveChangesAsync();
            return prod;
        }

        public async Task<bool> DeleteProduct(int id)
        {
            var prod = await _db.Products.FindAsync(id);

            if (prod == null)
                return false;

            _db.Products.Remove(prod);
            await _db.SaveChangesAsync();
            return true;
        }

        public async Task<ProductModel> GetProduct(int id)
        {
            return await _db.Products.FindAsync(id);
        }

        public async Task<IEnumerable<ProductModel>> GetProducts(int page = 1)
        {
            page = page < 1 ? 1 : page;
            var prds = await Task<IEnumerable<ProductModel>>.Factory.StartNew(()=> _db.Products.Skip((page - 1) * numOnePageProd).Take(page * numOnePageProd).AsEnumerable());
            return prds;
        }

        public async Task<bool> UpdateProduct(ProductModel product, int id)
        {
            var fProd = await _db.Products.FindAsync(product);
            if (fProd is null)
                return false;

            fProd = makechange(fProd, product);

            _db.Update(fProd);

            await _db.SaveChangesAsync();

            return true;
        }

        ProductModel makechange(ProductModel p1, ProductModel p2)
        {
            p1.Description = p1.Description != p2.Description ? p2.Description : p1.Description;
            p1.IconId = p1.IconId != p2.IconId ? p2.IconId : p1.IconId;
            p1.Manufacturer = p1.Manufacturer != p2.Manufacturer ? p2.Manufacturer : p1.Manufacturer;
            p1.Name = p1.Name != p2.Name ? p2.Name : p1.Name;
            return p1;
        }
    }
    public abstract class ProductManager
    {
        public IProductManager Manager { private get; set; }
        public IProductValidator Validator { private get; set; }
        protected ProductManager(IProductManager manager, IProductValidator validator)
        {
            Manager = manager;
            Validator = validator;
        }

        public async Task<int> AddProduct(ProductModel product)
        {
            var res = await Validator.ValidateAsync(product);

            if (!res.Succeeded)
            {
                return -1;
            }

            return await Manager.AddProduct(product);
        }
        public async Task<bool> UpdateProduct(ProductModel product, int id) 
        {
            var res = await Validator.ValidateAsync(product);

            if (!res.Succeeded)
            {
                return false;
            }
            return await Manager.UpdateProduct(product, id);
        }
        public async Task<bool> DeleteProduct(int id) 
        {
            return await Manager.DeleteProduct(id);
        }
        public async Task<IEnumerable<ProductModel>> GetProducts(int page = 1)
        {
            return await Manager.GetProducts(page);
        }
        public async Task<ProductModel> AddScore(int id, int score)
        {
            var prod = new ProductModel { Score = score, Description = "SomeDescription", IconId = 1, Manufacturer = "randName", Name = "randName", Scored = 1 };
            var res = await Validator.ValidateAsync(prod);

            if (!res.Succeeded)
            {
                return prod;
            }

            return await Manager.AddScore(id, score);
        }
        public async Task<ProductModel> GetProduct(int id)
        {
            return await Manager.GetProduct(id);
        }
    }
    public class CertainProductManager : ProductManager
    {
        public CertainProductManager(IProductManager manager, IProductValidator validator) : base(manager, validator)
        {
        }
    }
}
