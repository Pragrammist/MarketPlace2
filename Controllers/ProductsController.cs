using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MarketPlace.Models.ProductModels;
using MarketPlace.Services;
using MarketPlace.Models.ImagesModels;
using System.IO;
using MarketPlace.Services.Images;
//using static Microsoft.AspNetCore.Http.IFormFile;
using MarketPlace.Services.Products;

namespace MarketPlace.Controllers
{
    public class ProductsController : Controller
    {

        ImageManager _imgManager;
        ProductManager _prManager;
        DescriptionParserManager _parser;
        public ProductsController(ProductManager manager, ImageManager imgManager, DescriptionParserManager parser) 
        {
            _imgManager = imgManager;
            _prManager = manager;
            _parser = parser;
        }

        
        public IActionResult AddProduct()
        {
            return View();
        }

        public async Task<IActionResult> ShowProducts(int page = 1)
        {
            page = page < 1 ? 1 : page;
            
            var prods = await _prManager.GetProducts(page);
            var imgs = (await GetImgs(prods.Select(t => t.IconId).ToArray())).ToArray();
            var paths = (await GetPaths(imgs)).ToArray();
            var prodsVm = prods.Select((t,i) =>
            {

                var path = paths[i];
                
                

                var prod = new ProductViewModel(t)
                {
                    PhotoPath = path
                    
                };
                var descr = _parser.ParseDescription(t.Description);
                prod.ParsedDescription = descr;
                prod.ParsedDescription.Properties = descr.Properties.OrderBy(prop => prop.Type.Count(symb => symb == '.')).AsEnumerable().ToList();
                return prod;
            });

            
            

            return View(prodsVm);
        }

        private async Task<IEnumerable<string>> GetPaths(ImageModel[] imgs)
        {
            var paths = new string[imgs.Length];
            
            for (var i = 0; i < imgs.Length; i++)
            {
                var img = imgs[i];
                var path = await _imgManager.UploadToFolder(img);
                paths[i] = path;
            }
            return paths;
        }

        private async Task<IEnumerable<ImageModel>> GetImgs(int[] ids)
        {
            var imgs = new ImageModel[ids.Length];
            for (var i = 0; i < ids.Length; i++)
            {
                var id = ids[i];
                var timg = await _imgManager.GetImage(id);
                imgs[i] = timg;
            }
            return imgs;
        }



        [HttpPost]
        public async Task<IActionResult> AddProduct(ProductViewModel model)
        {
            if (model is null || model.Image is null)
            {
                ModelState.AddModelError("", "is null");
            }
            if (ModelState.IsValid)
            {
                var file = model.Image;
                var product = await initproduct(file, model);// initialize product
                var id = await _prManager.AddProduct(product); 
                if(id == -1)
                {
                    ModelState.AddModelError("", "не прошло валидацию");
                    return View(model);
                }

            }

            return View(model);
        }

        async Task<ProductModel> initproduct (Microsoft.AspNetCore.Http.IFormFile file, ProductViewModel model)
        {
            var img = await initimg(file);
            var idImg = await _imgManager.UploadImage(img);
            if (idImg == -1)
            {
                return null;
            }

            ProductModel product = new()
            {
                Description = model.Description,
                Manufacturer = model.Manufacturer,
                Name = model.Name,
                //PhotoPath = photoPath,
                IconId = idImg,
                Scored = 0,

            };// initialize product
            return product;
        }

        async Task<ImageModel> initimg(Microsoft.AspNetCore.Http.IFormFile file)
        {
            ImageModel img = new ImageModel();

            //img.Name = file.FileName;
            img.Bytes = new byte[1048576];
            img.Extention = Path.GetExtension(file.FileName).ToLower().Trim('.');
            var stream = file.OpenReadStream();
            var read = await stream.ReadAsync(img.Bytes);
            img.Bytes = img.Bytes[..read];
            stream.Close();

            return img;
        }
        



        [ResponseCache(Location = ResponseCacheLocation.Any, Duration = 300)]
        public async Task<IActionResult> GetProducts(int page = 1)
        {
            var prds = await _prManager.GetProducts(page);

            return View(prds);
        }

        [HttpPost]
        public async Task<IActionResult> AddScore(int id, int score)
        {
            var prod = await _prManager.AddScore(id, score);
            if (prod is null)
            {
                return NotFound();
            }
            return Ok(prod);
        }

        [HttpDelete]
        public async Task<IActionResult> DeleteProduct(int id)
        {
            var del = await _prManager.DeleteProduct(id);
            
            if (del)
            {
                return Ok();
            }

            return NotFound();
        }


        [HttpGet]
        public async Task<IActionResult> GetProduct(int id)
        {
            var pr = await _prManager.GetProduct(id);
            if (pr is null)
            {
                return NotFound();
            }
            return Ok(pr);
        }

        [HttpPost]
        public async Task<IActionResult> UpdateProduct(ProductModel product, int id)
        {
            var prod = await _prManager.UpdateProduct(product, id);
            if (prod)
            {
                return Ok();
            }
            return NotFound();
        }
    }
}
