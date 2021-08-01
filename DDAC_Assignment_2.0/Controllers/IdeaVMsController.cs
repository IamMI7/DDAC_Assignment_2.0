using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DDAC_Assignment_2._0.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Http;
using System.Security.Claims;
using Microsoft.Extensions.Logging;
using System.IO;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace DDAC_Assignment_2._0.Controllers
{
    public class IdeaVMsController : Controller
    {
        //public IActionResult Index()
        //{
        //    return View();
        //}

        private readonly ApplicationDbContext _context;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly ApplicationDbContext _db;
        BlobtheController blob = new BlobtheController();
        private readonly ILogger<IdeaVMsController> _logger;

        //Blob Names:
        string blobIdeasImg = "ideasblob";
        string blobIdeasFile = "ideasfileblob";
        string blobMatImg = "materialsblob";

        public IdeaVMsController(ApplicationDbContext context, ILogger<HomeController> logger, SignInManager<ApplicationUser> signInManager, UserManager<ApplicationUser> userManager
            , RoleManager<IdentityRole> roleManager, ApplicationDbContext db, IHttpContextAccessor httpContextAccessor)
        {
            _context = context;
            //_logger = logger;
            _signInManager = signInManager;
            _userManager = userManager;
            _roleManager = roleManager;
            _db = db;
            _httpContextAccessor = httpContextAccessor;
        }

        // GET: IdeaVMs
        public async Task<IActionResult> Index(string Title, string Material)
        {
            var ideas = from m in _context.Ideas
                         select m;
            if (!String.IsNullOrEmpty(Title))
            {
                ideas = ideas.Where(s => s.Title.Contains(Title));
            }

            //attach flower type values to dropdown list
            IQueryable<string> TypeQuery = from m in _context.Ideas
                                           orderby m.Material
                                           select m.Material;
            IEnumerable<SelectListItem> items = new SelectList(await TypeQuery.Distinct().ToListAsync());
            ViewBag.Material = items;
            //filtering based on flower type
            if (!String.IsNullOrEmpty(Material))
            {
                ideas = ideas.Where(s => s.Material == Material);
            }    
                return View(await ideas.ToListAsync());
        }

        // GET: IdeaVMs/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var ideaVM = await _context.Ideas
                .FirstOrDefaultAsync(m => m.IdeaID == id);
            if (ideaVM == null)
            {
                return NotFound();
            }

            return View(ideaVM);
        }

        // GET: IdeaVMs/Create
        public IActionResult Create()
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

        // POST: IdeaVMs/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(IdeaVM model, IFormFile images, IFormFile document)
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
                        Message = uname + "@" + DateTime.Now.TimeOfDay + "#" + document.FileName
                    };
                    _db.Ideas.Add(data);
                    _db.SaveChanges();
                    blob.CreateBlobContainer(blobIdeasImg);
                    blob.UploadBlob(images, data.Image, blobIdeasImg);
                    blob.CreateBlobContainer(blobIdeasFile);
                    blob.UploadBlob(document, data.Message, blobIdeasFile);
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
            return View();

            //if (ModelState.IsValid)
            //{
            //    _context.Add(ideaVM);
            //    await _context.SaveChangesAsync();
            //    return RedirectToAction(nameof(Index));
            //}
            //return View(ideaVM);
        }

        // GET: IdeaVMs/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var ideaVM = await _context.Ideas.FindAsync(id);
            if (ideaVM == null)
            {
                return NotFound();
            }

            List<MaterialsVM> model = (from x in _db.Materials select x).ToList();

            List<String> y = new List<String>();

            foreach (var z in model)
            {
                y.Add(z.Material);
            }

            ViewBag.Materials = y;

            List<String> models = new List<String>();
            List<String> file = new List<String>();

            List<IdeaVM> ivm = (from x in _db.Ideas select x).ToList();

            foreach (var x in ivm)
            {
                models.Add(x.Image);
                file.Add(x.Message);
            }

            models = blob.getBlobFileLink(models, blobIdeasImg);
            file = blob.getBlobFileLink(file, blobIdeasFile);

            return View(ideaVM);
        }

        // POST: IdeaVMs/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, IdeaVM ideaVM, IFormFile images, IFormFile document)
        {
            List<MaterialsVM> mvm = (from x in _db.Materials select x).ToList();

            List<String> y = new List<String>();

            foreach (var z in mvm)
            {
                y.Add(z.Material);
            }

            ViewBag.Materials = y;

            //List<String> model = new List<String>();
            //List<String> file = new List<String>();

            //List<IdeaVM> ivm = (from x in _db.Ideas select x).ToList();
            
            //string uname = _httpContextAccessor.HttpContext.User.FindFirst(ClaimTypes.Name).Value;
            //string Image = uname + "@" + DateTime.Now.TimeOfDay + "#" + images.FileName;
            //string Message = uname + "@" + DateTime.Now.TimeOfDay + "#" + document.FileName;
            
            //foreach (var x in ivm)
            //{
            //    model.Add(x.Image);
            //    file.Add(x.Message);
            //}

            //model = blob.getBlobFileLink(model, blobIdeasImg);
            //file = blob.getBlobFileLink(file, blobIdeasFile);

            if (id != ideaVM.IdeaID)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(ideaVM);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!IdeaVMExists(ideaVM.IdeaID))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction("Index", "IdeaVMs");
            }
            return View(ideaVM);
        }

        // GET: IdeaVMs/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var ideaVM = await _context.Ideas
                .FirstOrDefaultAsync(m => m.IdeaID == id);
            if (ideaVM == null)
            {
                return NotFound();
            }

            return View(ideaVM);
        }

        // POST: IdeaVMs/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var ideaVM = await _context.Ideas.FindAsync(id);
            _context.Ideas.Remove(ideaVM);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool IdeaVMExists(int id)
        {
            return _context.Ideas.Any(e => e.IdeaID == id);
        }
    }
}
