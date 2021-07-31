using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DDAC_Assignment_2._0.Models;
using Microsoft.EntityFrameworkCore;

namespace DDAC_Assignment_2._0.Controllers
{
    public class IdeaVMsController : Controller
    {
        //public IActionResult Index()
        //{
        //    return View();
        //}

        private readonly ApplicationDbContext _context;

        public IdeaVMsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: IdeaVMs
        public async Task<IActionResult> Index()
        {
            return View(await _context.Ideas.ToListAsync());
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
            return View();
        }

        // POST: IdeaVMs/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("IdeaID,Image,Title,Curator,DatePublish,Message,Material")] IdeaVM ideaVM)
        {
            if (ModelState.IsValid)
            {
                _context.Add(ideaVM);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(ideaVM);
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
            return View(ideaVM);
        }

        // POST: IdeaVMs/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("IdeaID,Image,Title,Curator,DatePublish,Message,Material,Status")] IdeaVM ideaVM)
        {
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
                return RedirectToAction(nameof(Index));
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
