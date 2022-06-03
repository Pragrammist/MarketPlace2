using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MarketPlace.Models.ImagesModels;
using System.Web;
using System.IO;
using System.Net.Http;
using System.Threading;
using Microsoft.Extensions.Configuration;
using MarketPlace.Models;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Processing;
using SixLabors.ImageSharp.Formats.Jpeg;
using MarketPlace.Services.Validators.ImgValidators;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Hosting;

namespace MarketPlace.Services.Images
{
    public interface IImageManager
    {
        Task<int> UploadImage(ImageModel image);
        Task<bool> DeleteImage(int id);
        Task<ImageModel> GetImage(int id);
        Task<bool> UpdateImageModel(ImageModel model, int id)
        {
            return new Task<bool>(() => false);
        }
        Task<ImageModel> GetResizeImage(ImageModel model);

    }

    public class WebApiImageManager : IImageManager
    {

        IHttpClientFactory _clientFactory;
        IConfiguration _configuration;
        readonly string uploadUrl = "/api/Image/Upload";
        readonly string getUrl = "/api/Image/Get";
        readonly string deleteUrl = "/api/Image/Delete";
        public WebApiImageManager(IHttpClientFactory clientFactory, IConfiguration configuration)
        {
            var baseUrl = configuration.GetValue<string>("ImageApiBaseUrl");

            _clientFactory = clientFactory;
            _configuration = configuration;

            uploadUrl = baseUrl + uploadUrl;
            getUrl = baseUrl + getUrl;
            deleteUrl = baseUrl + deleteUrl;
        }


        

        public async Task<int> UploadImage(ImageModel image)
        {
            var client = _clientFactory.CreateClient();
            bool isExeption = false;
            HttpResponseMessage res = null;
            HttpRequestMessage req = null;
            try
            {

                req = new HttpRequestMessage(HttpMethod.Post, uploadUrl);
                req.Headers.Add("extention", image.Extention);
                //req.Headers.Add("name", image.Name);
                //req.Content = new StreamContent(image.Stream);
                res = await client.SendAsync(req);/*.ContinueWith(async(r, stream) => { ((Stream)stream).Close(); }, image.Stream);*/
                var id = res?.Headers?.GetValues("id")?.FirstOrDefault();
                int resId;
                var isParsed = int.TryParse(id, out resId);

                
                if (isParsed)
                {
                    image.Id = resId;
                    return resId;
                }
            }
            catch
            {
                isExeption = true;
                image.Id = -1;
            }
            finally
            {
                if (!isExeption)
                    //image?.Dispose();
                res?.Dispose();
                req?.Dispose();
                client.Dispose();
            }
            return image.Id;
        }

        public async Task<ImageModel> GetImage(int id)
        {
            var client = _clientFactory.CreateClient();
            HttpRequestMessage req = null;
            HttpResponseMessage resp = null;
            try
            {

                req = new HttpRequestMessage(HttpMethod.Get, getUrl);

                var headers = req.Headers;
                headers.Add("id", id.ToString());
                

                headers.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("image/jpeg"));
                resp = await client.SendAsync(req);


                //var name = resp.Headers.GetValues("name").FirstOrDefault();
                //var size = int.Parse(resp.Headers.GetValues("size").FirstOrDefault());
                //var ext = resp.Headers.GetValues("extention").FirstOrDefault();
                //var sizeX = int.Parse(resp.Headers.GetValues("width").FirstOrDefault());
                //var sizeY = int.Parse(resp.Headers.GetValues("height").FirstOrDefault());


                //ImageModel model = new ImageModel();
                //model.Id = id;
                //model.Name = name;
                //model.Size = size;
                //model.SizeX = sizeX;
                //model.SizeY = sizeY;
                //model.Extention = ext;

                using (var memStream = await resp.Content.ReadAsStreamAsync())
                {

                       
                    //var bs = await resp.Content.ReadAsByteArrayAsync();
                }



                return null;
            }
            catch (Exception ex)
            {
                string m = ex.Message;
                return null;
            }
            finally
            {
                resp?.Dispose();
                req?.Dispose();
                client.Dispose();
            }

        }

        public async Task<bool> DeleteImage(int id)
        {
            var client = _clientFactory.CreateClient();
            HttpRequestMessage req = null;
            HttpResponseMessage resp = null;
            try
            {

                req = new HttpRequestMessage(HttpMethod.Delete, deleteUrl);
                var headers = req.Headers;
                headers.Add("id", id.ToString());
                resp = await client.SendAsync(req);

                if (resp.IsSuccessStatusCode)
                    return true;
                return false;
            }
            catch
            {

            }
            finally
            {
                req?.Dispose();
                client.Dispose();
                resp?.Dispose();
            }
            return false;
        }

        public Task<ImageModel> GetResizeImage(ImageModel model)
        {
            throw new NotImplementedException();
        }
    }

    public class DbImageManager : IImageManager
    {
        public static readonly string[] exts = new string[] {"jpg", "jpeg" };

        MarkePlacetDb _db;

        public DbImageManager(MarkePlacetDb db)
        {
            _db = db;
        }

        public async Task<bool> DeleteImage(int id)
        {
            var model = await _db.Images.FindAsync(id);

            if (model != null)
            {
                _db.Images.Remove(model);
                await _db.SaveChangesAsync();
                return true;
            }
            return false;
        }

        public async Task<ImageModel> GetImage(int id)
        {
            return await _db.Images.FindAsync(id);
        }

        public async Task<int> UploadImage(ImageModel image)
        {
            image.Name = Path.GetRandomFileName();

            var tImg = DetermineRez(image);
            tImg.Dispose();

            var addRes = await _db.Images.AddAsync(image);
            await _db.SaveChangesAsync();
            var id = addRes.Entity.Id;
            return id;
        }

        public async Task<bool> UpdateImageModel(ImageModel model, int id)
        {
            var finded = await _db.Images.FindAsync(id);
            if (finded is null)
                return false;


            var gBytes = model.Bytes;
            var fBytes = finded.Bytes;


            if (!fBytes.SequenceEqual(gBytes))
            {
                Array.Clear(fBytes, 0, fBytes.Length);
                finded.Bytes = gBytes;
                var tImg = DetermineRez(finded);
                tImg.Dispose();
                if (model.Extention.Trim('.').ToLower() != finded.Extention.Trim('.').ToLower())
                    finded.Extention = model.Extention;
            }

            _db.Images.Update(finded);
            await _db.SaveChangesAsync();

            return true;
        }

        private Image DetermineRez(ImageModel image)
        {
            var tImg = GetTempImg(image.Bytes);
            image = DetermineRez(tImg, image);
            return tImg;
        }

        private Image GetTempImg(byte[] bImg, bool isReize = false, int height = default(int), int width = default(int))
        {
            var img = Image.Load(bImg);

            if (isReize && height != default(int) && width != default(int))
                img.Mutate(x => x.Resize(height, width));


            return img;
        }

        private ImageModel DetermineRez(Image image, ImageModel model)
        {
            model.SizeX = image.Width;
            model.SizeY = image.Height;
            model.Size = model.SizeX * model.SizeY;
            return model;
        }

        public async Task<ImageModel> GetResizeImage(ImageModel model)
        {
            var img = GetTempImg(model.Bytes, true, model.SizeY, model.SizeX);
            model = DetermineRez(img , model);
            return model;
        }
    }

    public abstract class ImageManager
    {
        public IImageManager Manager { private get; set; }
        public IImageValidator Validator { private get; set; }
        public IdentityResult ValidateResult { get; private set; }
        IWebHostEnvironment _appEnvironment;
        protected ImageManager(IImageManager imageManager, IImageValidator validator, IWebHostEnvironment appEnvironment)
        {
            Manager = imageManager;
            Validator = validator;
            _appEnvironment = appEnvironment;
        }

        public virtual async Task<bool> DeleteImage(int id)
        {
            return await Manager.DeleteImage(id);
        }
        public virtual async Task<ImageModel> GetImage(int id)
        {
            return await Manager.GetImage(id);
        }
        public virtual async Task<int> UploadImage(ImageModel image)
        {
            ValidateResult = await Validator.ValidateAsync(image);

            if (ValidateResult.Errors.Count() > 0)
            {
                return -1;
            }

            return await Manager.UploadImage(image);
        }
        public virtual async Task<bool> UpdateImage(ImageModel model, int id)
        {
            ValidateResult = await Validator.ValidateAsync(model);

            if (ValidateResult.Errors.Count() > 0)
            {
                return false;
            }

            return await Manager.UpdateImageModel(model, id);
        }
        public virtual async Task<ImageModel> ResizeImage(ImageModel model)
        {
            ValidateResult = await Validator.ValidateAsync(model);

            if (ValidateResult.Errors.Count() > 0)
            {
                return model;
            }

            return await Manager.GetResizeImage(model);
        }
        public virtual async Task<string> UploadToFolder(ImageModel model)
        {
            ValidateResult = await Validator.ValidateAsync(model);
            if (ValidateResult.Errors.Count() > 0)
            {
                return string.Empty;
            }
            var name = model.Name + "." + model.Extention;// name + extention
            var photoFolder = @"\productsImages\";// folder in wwwroot where exists files
            string folderPath = _appEnvironment.WebRootPath + photoFolder; // its path to folder for filestream
            string photoPathToUpl = Path.Combine(folderPath, name); // photo path for filestream
            var returnPath = Path.Combine(photoFolder, name); //return path for html/css
            if (File.Exists(photoPathToUpl))
                return returnPath;

            try
            {
                using (FileStream fileStream = new FileStream(photoPathToUpl, FileMode.OpenOrCreate))
                {
                    await fileStream.WriteAsync(model.Bytes);
                }
            }
            catch(Exception ex)
            {

                return string.Empty;
            }
            return returnPath;
        }
    }

    public class CertainImageManager : ImageManager
    {
        public CertainImageManager(IImageManager imageManager, IImageValidator validator, IWebHostEnvironment appEnvironment) : base(imageManager, validator, appEnvironment)
        {

        }

        
    }


}
