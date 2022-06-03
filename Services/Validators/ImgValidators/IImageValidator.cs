using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MarketPlace.Models.ImagesModels;
using Microsoft.AspNetCore.Identity;

namespace MarketPlace.Services.Validators.ImgValidators
{
    public interface IImageValidator
    {
        Task<IdentityResult> ValidateAsync(ImageModel model);
    }

    public abstract class BaseImageValidator : IImageValidator
    {
        protected BaseImageValidator() { }
        public readonly string[] exts = new string[] {"jpeg", "jpg" };
        public const int maxValue = 1048576;
        public int minSize = 20;
        

        public virtual async Task<IdentityResult> ValidateAsync(ImageModel model)
        {


            List<IdentityError> errors = new List<IdentityError>();

            if (model is null || Array.Empty<byte>() == model.Bytes || model.Bytes is null || string.IsNullOrEmpty(model.Extention))
            {
                errors.Add(new IdentityError() { Code = "null", Description = "image is null" });
            }
            if (!exts.Contains(model.Extention.Trim('.').ToLower()))
            {
                errors.Add(new IdentityError() { Code = "ext", Description = "используйте другое расширение" });
            }
            if (model.Bytes.Length > maxValue)
            {
                errors.Add(new IdentityError() { Code = "maxValue", Description = $"слишком большой размер фотографии ({model.Bytes.Length} из {maxValue})" });
            }

            if (model.SizeX != default(int) && (model.SizeX < 20 || model.SizeY < 20))
            {
                errors.Add(new IdentityError() { Code = "minValue", Description = $"слишком маленький размер фотографии"});
            }
            
            return errors.Count == 0 ? IdentityResult.Success : IdentityResult.Failed(errors.ToArray());
        }

        
    }

    public class ImageValidator : BaseImageValidator
    {

    }
}
