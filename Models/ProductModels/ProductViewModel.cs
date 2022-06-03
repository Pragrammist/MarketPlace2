using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MarketPlace.Services.Products;
using Microsoft.AspNetCore.Http;

namespace MarketPlace.Models.ProductModels
{
    public class ProductViewModel
    {

        public ProductViewModel() { }

        public ProductViewModel(ProductModel model) 
        {
            Name = model.Name;
            Manufacturer = model.Manufacturer;
            Description = model.Description;
            //PhotoPath = model.PhotoPath;
            Score = model.Score;
        }

        public string Name { get; set; }
        public string Manufacturer { get; set; }
        public string Description { get; set; }
        public string PhotoPath { get; set; }
        public double Score { get; set; }
        public IFormFile Image { get; set; }
        public Description ParsedDescription { get; set; }
    }
}
