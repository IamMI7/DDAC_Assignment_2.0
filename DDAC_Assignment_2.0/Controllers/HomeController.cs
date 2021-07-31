using DDAC_Assignment_2._0.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace DDAC_Assignment_2._0.Controllers
{
    public class HomeController : Controller
    {
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly ApplicationDbContext _db;
        BlobtheController blob = new BlobtheController();
        private readonly ILogger<HomeController> _logger;

        //Blob Names:
        string blobIdeasImg = "ideasblob";
        string blobIdeasFile = "ideasfileblob";
        string blobMatImg = "materialsblob";

        public HomeController(ILogger<HomeController> logger, SignInManager<ApplicationUser> signInManager, UserManager<ApplicationUser> userManager
            , RoleManager<IdentityRole> roleManager, ApplicationDbContext db, IHttpContextAccessor httpContextAccessor)
        {
            _logger = logger;
            _signInManager = signInManager;
            _userManager = userManager;
            _roleManager = roleManager;
            _db = db;
            _httpContextAccessor = httpContextAccessor;
        }

        public IActionResult Index()
        {
            return View();
        }


        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(LoginVM model) 
        {
            if (ModelState.IsValid) 
            {
                var result = await _signInManager.PasswordSignInAsync(model.Username, model.password,model.rememberMe, false);
                if (result.Succeeded) 
                {
                    return RedirectToAction("Index", "Home");
                }
                ModelState.AddModelError("", "Invalid login attempts.");
            }
            return View();
        }

        [HttpGet]
        public async Task<IActionResult> Register() 
        {
            if (!_roleManager.RoleExistsAsync("Customer").GetAwaiter().GetResult()) 
            {
                await _roleManager.CreateAsync(new IdentityRole("Admin"));
                await _roleManager.CreateAsync(new IdentityRole("Staff"));
                await _roleManager.CreateAsync(new IdentityRole("Customer"));
            } 
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Register(RegisterVM model) 
        {
            if (ModelState.IsValid) 
            {
                var user = new ApplicationUser
                {
                    Email = model.email,
                    UserName = model.userName,
                    PhoneNumber = model.contactNumber,
                    roleName = "Customer"
                }; 

                var result = await _userManager.CreateAsync(user, model.password);
                if (result.Succeeded) 
                {
                    
                    await _userManager.AddToRoleAsync(user, model.roleName);
                    await _signInManager.SignInAsync(user, isPersistent: false);
                    return RedirectToAction("Index", "Home");
                } 
            }
            return View();
        }

        [HttpGet]
        public IActionResult SubmitIdea()
        {
            List<MaterialsVM> model = (from x in _db.Materials select x).ToList();

            List<String> y = new List<String>();

            foreach (var z in model)
            {
                y.Add(z.Material);
            }

            ViewBag.Materials = y;
            return View();
        }
        public IActionResult SubmitIdea(IdeaVM model, IFormFile images, IFormFile document)
        {
            List<MaterialsVM> mvm = (from x in _db.Materials select x).ToList();

            List<String> y = new List<String>();

            foreach (var z in mvm)
            {
                y.Add(z.Material);
            }

            ViewBag.Materials = y;

            
            if (_signInManager.IsSignedIn(User))
            {
                string uname = _httpContextAccessor.HttpContext.User.FindFirst(ClaimTypes.Name).Value;
                if (ModelState.IsValid)
                {

                    IdeaVM data = new IdeaVM()
                    {
                        Title = model.Title,
                        Curator = uname,
                        DatePublish = DateTime.Now.Date,
                        Material = model.Material,
                        Image = uname + "@" + DateTime.Now.TimeOfDay + "#" + images.FileName,
                        Message = uname + "@" + DateTime.Now.TimeOfDay + "#" + document.FileName,
                        Status = false
                    };
                    _db.Ideas.Add(data);
                    _db.SaveChanges();
                    blob.CreateBlobContainer(blobIdeasImg);
                    blob.UploadBlob(images, data.Image, blobIdeasImg);
                    blob.CreateBlobContainer(blobIdeasFile);
                    blob.UploadBlob(document, data.Message, blobIdeasFile);

                    ViewData["SignState"] = "Submitted";
                    return View();
                }
                else
                {
                    return View();
                }
            }
            else
            {
                ViewData["SignState"] = "NotSigned";
                return View();
            }
            
        }

        [HttpGet]
        public IActionResult MaterialsPage()
        {
            List<string> ImageNames = new List<string>();
            List<MaterialsVM> materialsList = (from list in _db.Materials select list).ToList();
            foreach(var x in materialsList)
            {
                ImageNames.Add(x.ImageName);
            }
            List<String> imagesource = blob.GetMaterialsImage(ImageNames, blobMatImg);

            ViewData["MaterialsList"] = imagesource;

            return View();
        }
        [HttpPost]
        public IActionResult MaterialsPage(string material)
        {
            TempData["materialselection"] = material;
            return RedirectToAction("IdeasPage");
        }

        [HttpGet]
        public IActionResult IdeasPage()
        {
            string Smaterial = (string)TempData["materialselection"];
            List<String> model = new List<String>();
            List<String> file = new List<String>();

            List<IdeaVM> ivm = (from x in _db.Ideas where x.Material == Smaterial && x.Status == true select x).ToList();

            foreach(var y in ivm)
            {
                model.Add(y.Image);
                file.Add(y.Message);
            }

            model = blob.getBlobFileLink(model, blobIdeasImg);
            file = blob.getBlobFileLink(file, blobIdeasFile);

            for (int i = 0; i < ivm.Count; i++)
            {
                ivm[i].Image = model[i];
                ivm[i].Message = file[i];
            }

            ViewData["IdeasListVM"] = ivm;

            return View();
        }

        [HttpPost]
        public IActionResult IdeasInner(IdeaVM model)
        {
            return View(model);
        }

        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            return RedirectToAction("Index");
        }



        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }   
}
