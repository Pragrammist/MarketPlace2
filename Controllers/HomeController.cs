using MarketPlace.Models;
using MarketPlace.Models.UsersModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using MarketPlace.Services.Products;



namespace MarketPlace.Controllers
{
    public class HomeController : Controller
    {
        UserManager<UserModel> _uM; 
        RoleManager<UserRole> _rM; 
        SignInManager<UserModel> _sM;
        DescriptionParserManager _parser;
        public HomeController(UserManager<UserModel> uM, RoleManager<UserRole> rM, SignInManager<UserModel> sM, DescriptionParserManager parser)
        {
            _uM = uM;
            _rM = rM;
            _sM = sM;
            _parser = parser;
        }

        public async Task<IActionResult> Index()
        {
            string descrip =
                @"
                {
                    'main': 'Продукт создан для того, чтобы удовлетворить ваши потребности',
                    'props': 
                        {
                            'общее': 
                            {
                                'вес': '10',
                                'длина':'100',
                                'подпункт': 
                                {
                                    'qwe':'qwe',
                                    'q':'qwe',
                                    'подпункт': 
                                    {
                                        'qwe':'qwe',
                                        'q':'qwe',
                                    },
                                },
                            },
                            'Wiw':'1-rp',
                        },
                }
                ";
            
            var parsed = _parser.ParseDescription(descrip);

            //FileStream stream = new FileStream(@"C:\Users\F\Desktop\Стол\nature.jpg", FileMode.Open);
            //ImageModel image = new ImageModel();
            //image.Extention = Path.GetExtension(stream.Name);

            //var buffer = new byte[1048576];
            //var read = stream.Read(buffer);
            //var bImg = buffer[..read];
            
            //image.Bytes = bImg;

            
            
            ////var id = await _imgMananger.UploadImage(image);
            ////var img = await _imgMananger.GetImage(1);
            ////await _imgMananger.DeleteImage(id);
            //await _imgMananger.UpdateImage(image, 1);
            return View();
        }

        public IActionResult Privacy()
        {
            

            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
