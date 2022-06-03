using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using MarketPlace.Models.ProductModels;
using MarketPlace.Models.UsersModels;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
//using Microsoft.AspNetCore.Identity;

namespace MarketPlace.Models
{
    public class MarkePlacetDb : IdentityDbContext<UserModel, UserRole, string>
    {
        public DbSet<ProductModel> Products { get; set; }
        public DbSet<ImagesModels.ImageModel> Images { get; set; }
        public MarkePlacetDb(DbContextOptions<MarkePlacetDb>options) :base(options)
        {
            //Database.EnsureDeleted();
            Database.EnsureCreated();
        }
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            //optionsBuilder.UseSqlServer(@"Server=(localdb)\\mssqllocaldb;Database=MarketPlaceDb;Trusted_Connection=True;");
        }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            
            

            
        }
    }
}
