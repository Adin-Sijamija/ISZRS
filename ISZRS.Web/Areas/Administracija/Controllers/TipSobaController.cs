using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using ISZR.Web.Models;
using ISZRS.Data;
using ISZRS.Data.Models;
using Microsoft.AspNetCore.Authorization;

namespace ISZRS.Web.Areas.Administracija.Controllers
{
    [Authorize(Roles =("Administrator,Logisticar"))]
    [Area("Administracija")]
    public class TipSobaController : Controller
    {
        private readonly MojContext _context;

        public TipSobaController(MojContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            return View(await _context.TipSobe.ToListAsync());
        }

        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var tipSobe = await _context.TipSobe
                .FirstOrDefaultAsync(m => m.Id == id);
            if (tipSobe == null)
            {
                return NotFound();
            }

            return View(tipSobe);
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Kapacitet,Naziv,Opis")] TipSobe tipSobe)
        {
            if (ModelState.IsValid)
            {
                _context.Add(tipSobe);
                await _context.SaveChangesAsync();

                List<TipSobeNamjestaj> nova = new List<TipSobeNamjestaj>();
                foreach (Namjestaj.TipNamjestaja val in Enum.GetValues(typeof(Namjestaj.TipNamjestaja)))
                {
                    nova.Add(new TipSobeNamjestaj()
                    {
                        TipNamjestaja = val,
                        Kolicina = tipSobe.Kapacitet,
                        TipSobeID = tipSobe.Id
                    });

                }
                _context.TipSobeNamjestaj.AddRange(nova);
                 _context.SaveChanges();


                return RedirectToAction(nameof(Index));
            }
            return View(tipSobe);
        }

        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var tipSobe = await _context.TipSobe.FindAsync(id);
            if (tipSobe == null)
            {
                return NotFound();
            }
            return View(tipSobe);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Naziv,Opis")] TipSobe tipSobe)
        {
            if (id != tipSobe.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(tipSobe);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!TipSobeExists(tipSobe.Id))
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
            return View(tipSobe);
        }

        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var tipSobe = await _context.TipSobe
                .FirstOrDefaultAsync(m => m.Id == id);
            if (tipSobe == null)
            {
                return NotFound();
            }

            return View(tipSobe);
        }

        // POST: Administracija/TipSoba/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var tipSobe = await _context.TipSobe.FindAsync(id);
            _context.TipSobe.Remove(tipSobe);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool TipSobeExists(int id)
        {
            return _context.TipSobe.Any(e => e.Id == id);
        }
    }
}
