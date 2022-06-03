using MarketPlace.Models;
using MarketPlace.Models.UsersModels;
using MarketPlace.Services;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using MarketPlace.Services.Images;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication.Cookies;
using MarketPlace.Services.Validators.ImgValidators;
using MarketPlace.Services.Validators.ProductValidators;
using MarketPlace.Services.Validators;
using MarketPlace.Services.Products;

namespace MarketPlace
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddHttpClient();
            string connection = Configuration.GetConnectionString("DefaultConnection");
            services.AddTransient<IDescriptionParser, DescriptionParserR>();
            services.AddTransient<DescriptionParserManager, DescriptionParserManagerRealize>();

            services.AddTransient<IPasswordValidator<UserModel>, OwnPasswordValidator>();
            services.AddTransient<IImageValidator, ImageValidator>();
            services.AddTransient<IProductValidator, ProductValidator>();

            services.AddTransient<IImageManager, DbImageManager>();
            services.AddTransient<ImageManager, CertainImageManager>();

            services.AddTransient<IProductManager, DbProductManager>();
            services.AddTransient<ProductManager, CertainProductManager>();


            services.AddControllersWithViews(); 
            services.AddDbContext<MarkePlacetDb>(options =>
                options.UseSqlServer(connection));
            services.AddScoped<DbProductManager>();
            services.AddIdentity<UserModel, UserRole>(opt => 
            {
                
            }).AddEntityFrameworkStores<MarkePlacetDb>();
            //var externalCookie = "idsrv.external";
            //services.AddAuthentication().AddCookie(CookieAuthenticationDefaults.AuthenticationScheme);
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }
            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();
            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Home}/{action=Index}/{id?}");
            });
        }
    }
}
