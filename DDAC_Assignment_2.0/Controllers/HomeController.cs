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

        public IActionResult ViewSupportTicket()
        {
            string statusPending = "Pending";
            List<SupportTicketVM> ticketList = (from list in _db.SupportTickets
                                                where list.status == statusPending
                                                select new SupportTicketVM()
                                                {
                                                    ID = list.ID,
                                                    name = list.name,
                                                    email = list.email,
                                                    subject = list.subject,
                                                    message = list.message,
                                                    status = list.status,
                                                }).ToList();
            return View(ticketList);
        }

        [HttpPost]
        public async Task<IActionResult> SubmitSupportTicket(SupportTicketVM model)
        {
            int id = 0;
            List<SupportTicketVM> tickerList = _db.SupportTickets.ToList();
            int length = tickerList.Count();

            if (length == 0)
            {
                id = 1;
                model.ID = "ST" + id;
            }
            else
            {
                model.ID = "ST" + (length + 1);
            }

            model.status = "Pending";

            if (ModelState.IsValid)
            {
                var ticket = new SupportTicketVM
                {
                    ID = model.ID,
                    email = model.email,
                    name = model.name,
                    subject = model.subject,
                    message = model.message,
                    status = model.status
                };
                await _db.SupportTickets.AddAsync(ticket);
                var created = await _db.SaveChangesAsync();
                TempData["result"] = created;
                return View();
            }
            else
            {
                var errors = ModelState.Select(x => x.Value.Errors)
                    .Where(y => y.Count > 0)
                    .ToList();
            }
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Accept(string id)
        {
            var ticket = new SupportTicketVM() { ID = id, status = "Accepted" };
            _db.SupportTickets.Attach(ticket);
            _db.Entry(ticket).Property(x => x.status).IsModified = true;
            await _db.SaveChangesAsync();
            return RedirectToAction("ViewSubmitTicket", "Home");
        }

        [HttpGet]
        public async Task<IActionResult> ViewProfile()
        {
            var userId = _userManager.GetUserId(HttpContext.User);
            ApplicationUser user = await _userManager.FindByIdAsync(userId);
            if (user != null)
            {
                UserVM userVM = new UserVM()
                {
                    id = user.Id,
                    email = user.Email,
                    name = user.UserName,
                    roleName = user.roleName,
                    contactNumber = user.PhoneNumber
                };
                return View(userVM);
            }
            return RedirectToAction("Index", "Home");
        }

        [HttpPost]
        public async Task<IActionResult> EditDetails(UserVM model)
        {
            if (ModelState.IsValid)
            {
                ApplicationUser user = await _userManager.FindByIdAsync(model.id);
                if (user != null)
                {
                    user.Id = model.id;
                    user.UserName = model.name;
                    user.Email = model.email;
                    user.PhoneNumber = model.contactNumber;
                    user.roleName = model.roleName;
                }
                IdentityResult result = await _userManager.UpdateAsync(user);
                if (result.Succeeded)
                {
                    return RedirectToAction("ViewProfile", "Home");
                }
                foreach (var errors in result.Errors)
                {
                    ModelState.AddModelError("", errors.Description);
                }
            }
            return RedirectToAction("ViewProfile", "Home");
        }

        [HttpGet]
        public IActionResult ChangePassword()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> ChangePassword(ChangePasswordVM model)
        {
            if (ModelState.IsValid)
            {
                var user = await _userManager.GetUserAsync(User);
                if (user == null)
                {
                    return View();
                }

                var result = await _userManager.ChangePasswordAsync(user, model.Password, model.newPassword);

                if (!result.Succeeded)
                {
                    foreach (var error in result.Errors)
                    {
                        ModelState.AddModelError("", error.Description);
                    }
                }
                await _signInManager.RefreshSignInAsync(user);
                TempData["Message"] = "Your password has been updated.";
                return View();
                ;
            }
            TempData["Message"] = "There is something wrong with your password.";
            return View();
        }

        [HttpGet]
        public IActionResult AdminGetStaffList()
        {
            List<UserVM> staffList = (from list in _db.Users
                                      where list.roleName == "Staff"
                                      select new UserVM()
                                      {
                                          id = list.Id,
                                          name = list.UserName,
                                          email = list.UserName,
                                          contactNumber = list.UserName,
                                          roleName = list.roleName
                                      }).ToList();
            return View(staffList);
        }

        [HttpGet]
        public IActionResult AdminGetCustomerList()
        {
            List<UserVM> customerList = (from list in _db.Users
                                         where list.roleName == "Customer"
                                         select new UserVM()
                                         {
                                             id = list.Id,
                                             name = list.UserName,
                                             email = list.UserName,
                                             contactNumber = list.UserName,
                                             roleName = list.roleName
                                         }).ToList();
            return View(customerList);
        }

        [HttpGet]
        public async Task<IActionResult> ViewAccountById(string id)
        {
            if (!string.IsNullOrEmpty(id))
            {
                ApplicationUser user = await _userManager.FindByIdAsync(id);
                if (user != null)
                {
                    UserVM userVM = new UserVM()
                    {
                        id = user.Id,
                        email = user.Email,
                        name = user.UserName,
                        contactNumber = user.PhoneNumber,
                        roleName = user.roleName
                    };
                    return View(userVM);
                }
            }
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> EditAccountById(UserVM model)
        {
            if (ModelState.IsValid)
            {
                ApplicationUser user = await _db.Users.FindAsync(model.id);
                if (user == null)
                {
                    return View("Not found", $"User with Id = {model.id} cannot be found!");
                }
                user.UserName = model.name;
                var updated = await _db.SaveChangesAsync();
                return RedirectToAction("ViewAccountById", new { id = model.id });
            }
            return RedirectToAction("ViewAccountById", new { id = model.id });
        }






        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }   
}
