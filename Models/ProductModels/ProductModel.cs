using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MarketPlace.Models.ProductModels
{
    public class ProductModel
    {
        public int Id { get; set; }
        public int IconId { get; set; }
        public string Name { get; set; }
        public string Manufacturer { get; set; }
        public string Description { get; set; }
        //public string PhotoPath { get; set; }
        public double Score { get; set; }
        public int Scored { get; set; }
    }
}
