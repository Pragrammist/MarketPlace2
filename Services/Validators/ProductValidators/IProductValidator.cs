using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MarketPlace.Models.ProductModels;
using Microsoft.AspNetCore.Identity;

namespace MarketPlace.Services.Validators.ProductValidators
{
    public interface IProductValidator
    {
        public Task<IdentityResult> ValidateAsync(ProductModel model);
    }
    public abstract class BaseProductValidator : IProductValidator
    {
        public async Task<IdentityResult> ValidateAsync(ProductModel model)
        {
            List<IdentityError> errs = new();
            if (model is null || string.IsNullOrEmpty(model.Description) || string.IsNullOrEmpty(model.Manufacturer) || string.IsNullOrEmpty(model.Name))
            {
                errs.Add(new IdentityError { Code = "null", Description = "нет данных" });
            }
            if (model.IconId == -1 || model.IconId == 0)
            {
                errs.Add(new IdentityError {Code = "img", Description = "что-то не так с фотографией" });
            }
            if (!NameIsValid(model.Name) || !DescriptionIsValid(model.Description) || !ManafacturerIsValid(model.Manufacturer) || !ScoreIsValid((int)model.Score))
            {
                errs.Add(new IdentityError { Code = "field", Description = "поле не прошло валидацию" });
            }
            

            return errs.Count == 0 ? IdentityResult.Success : IdentityResult.Failed(errs.ToArray());
        }
        protected virtual bool NameIsValid(string name)
        {
            return true;
        }
        protected virtual bool DescriptionIsValid(string description)
        {
            return true;
        }
        protected virtual bool ManafacturerIsValid(string manafact)
        {
            return true;
        }
        protected virtual bool ScoreIsValid(int score)
        {
            return score >= 0 && score <= 5;
        }
    }

    public class ProductValidator : BaseProductValidator 
    {

    }

}
