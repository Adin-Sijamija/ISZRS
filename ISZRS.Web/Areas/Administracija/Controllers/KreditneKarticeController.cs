using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using ISZR.Web.Models;
using ISZRS.Data;
using Microsoft.AspNetCore.Authorization;

namespace ISZRS.Web.Areas.Administracija.Controllers
{
    [Area("Administracija")]
    [Authorize(Roles = ("Administrator,Logisticar"))]
    public class KreditneKarticeController : Controller
    {
        private readonly MojContext _context;

        public KreditneKarticeController(MojContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var mojContext = _context.KreditneKartice.Include(k => k.TipKartice);
            return View(await mojContext.ToListAsync());
        }

        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var kreditneKartice = await _context.KreditneKartice
                .Include(k => k.TipKartice)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (kreditneKartice == null)
            {
                return NotFound();
            }

            return View(kreditneKartice);
        }

        public IActionResult Create()
        {
            ViewData["TipKarticeID"] = new SelectList(_context.TipKartice, "Id", "Naziv");
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,VaziDo,BrojKartice,CVV,TipKarticeID")] KreditneKartice kreditneKartice)
        {
            if (ModelState.IsValid)
            {
                _context.Add(kreditneKartice);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["TipKarticeID"] = new SelectList(_context.TipKartice, "Id", "Naziv", kreditneKartice.TipKarticeID);
            return View(kreditneKartice);
        }

        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var kreditneKartice = await _context.KreditneKartice.FindAsync(id);
            if (kreditneKartice == null)
            {
                return NotFound();
            }
            ViewData["TipKarticeID"] = new SelectList(_context.TipKartice, "Id", "Naziv", kreditneKartice.TipKarticeID);
            return View(kreditneKartice);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,VaziDo,BrojKartice,CVV,TipKarticeID")] KreditneKartice kreditneKartice)
        {
            if (id != kreditneKartice.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(kreditneKartice);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!KreditneKarticeExists(kreditneKartice.Id))
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
            ViewData["TipKarticeID"] = new SelectList(_context.TipKartice, "Id", "Naziv", kreditneKartice.TipKarticeID);
            return View(kreditneKartice);
        }

        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var kreditneKartice = await _context.KreditneKartice
                .Include(k => k.TipKartice)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (kreditneKartice == null)
            {
                return NotFound();
            }

            return View(kreditneKartice);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var kreditneKartice = await _context.KreditneKartice.FindAsync(id);
            _context.KreditneKartice.Remove(kreditneKartice);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool KreditneKarticeExists(int id)
        {
            return _context.KreditneKartice.Any(e => e.Id == id);
        }
    }
}
