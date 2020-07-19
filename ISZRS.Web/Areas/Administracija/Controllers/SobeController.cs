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

    public class SobeController : Controller
    {
        private readonly MojContext _context;

        public SobeController(MojContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var mojContext = _context.Sobe.Include(s => s.TipSobe);
            return View(await mojContext.ToListAsync());
        }

        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var sobe = await _context.Sobe
                
                .Include(s => s.TipSobe)
                .SingleOrDefaultAsync(m => m.Id == id);
            if (sobe == null)
            {
                return NotFound();
            }

            return View(sobe);
        }

        public IActionResult Create()
        {
           // ViewData["PopustID"] = new SelectList(_context.Popusti, "Id", "Id");
            ViewData["TipSobeID"] = new SelectList(_context.TipSobe, "Id", "Naziv");
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,BrojSobe,BrojSprata,Cijena,TipSobeID")] Sobe sobe)
        {
            if (ModelState.IsValid)
            {
                _context.Add(sobe);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["TipSobeID"] = new SelectList(_context.TipSobe, "Id", "Naziv", sobe.TipSobeID);
            return View(sobe);
        }

        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var sobe = await _context.Sobe.SingleOrDefaultAsync(m => m.Id == id);
            if (sobe == null)
            {
                return NotFound();
            }
            ViewData["TipSobeID"] = new SelectList(_context.TipSobe, "Id", "Naziv", sobe.TipSobeID);
            return View(sobe);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,BrojSobe,BrojSprata,Cijena,TipSobeID,")] Sobe sobe)
        {
            if (id != sobe.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(sobe);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!SobeExists(sobe.Id))
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
            ViewData["TipSobeID"] = new SelectList(_context.TipSobe, "Id", "Naziv", sobe.TipSobeID);
            return View(sobe);
        }

        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var sobe = await _context.Sobe
                //.Include(s => s.Popusti)
                .Include(s => s.TipSobe)
                .SingleOrDefaultAsync(m => m.Id == id);
            if (sobe == null)
            {
                return NotFound();
            }

            return View(sobe);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var sobe = await _context.Sobe.SingleOrDefaultAsync(m => m.Id == id);
            _context.Sobe.Remove(sobe);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool SobeExists(int id)
        {
            return _context.Sobe.Any(e => e.Id == id);
        }
    }
}
