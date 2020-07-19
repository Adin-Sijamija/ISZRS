using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using ISZR.Web.Models;
using ISZRS.Data;
using System.IO;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Authorization;

namespace ISZRS.Web.Areas.Administracija.Controllers
{
    [Area("Administracija")]
    [Authorize(Roles = ("Administrator,Logisticar"))]

    public class DodaciUslugeController : Controller
    {
        private readonly MojContext _context;

        public DodaciUslugeController(MojContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            return View(await _context.Dodaci.Include(x=>x.Usluga).ToListAsync());
        }

        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var dodaci = await _context.Dodaci
                .FirstOrDefaultAsync(m => m.Id == id);
            if (dodaci == null)
            {
                return NotFound();
            }

            return View(dodaci);
        }

        public IActionResult Create()
        {
            ViewData["UslugeID"] = new SelectList(_context.Usluge, "Id", "Naziv");

            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Ime,Opis,Cijena,JeUkljucen,Slika,UslugeID")] Dodaci dodaci ,IFormFile Slika)
        {
            if (ModelState.IsValid)
            {
                if (Slika != null)
                {
                    if (Slika.Length > 0)
                    {
                        byte[] SlikaBitovi = null;
                        using (var fs1 = Slika.OpenReadStream())
                        using (var ms1 = new MemoryStream())
                        {
                            fs1.CopyTo(ms1);
                            SlikaBitovi = ms1.ToArray();
                        }

                        dodaci.Slika = SlikaBitovi;

                    }

                }

                if (dodaci.JeUkljucen)
                {
                    dodaci.Cijena = 0;
                }

                _context.Add(dodaci);
                await _context.SaveChangesAsync();
                return RedirectToAction("Details", "DodaciUsluge", new { id = dodaci.Id });
            }
            ViewData["UslugeID"] = new SelectList(_context.Usluge, "Id", "Naziv", dodaci.UslugeID);
            return View(dodaci);
        }
    

        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var dodaci = await _context.Dodaci.FindAsync(id);
            if (dodaci == null)
            {
                return NotFound();
            }
            return View(dodaci);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Ime,Opis,Cijena,JeUkljucen,Slika")] Dodaci dodaci)
        {
            if (id != dodaci.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(dodaci);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!DodaciExists(dodaci.Id))
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
            return View(dodaci);
        }

        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var dodaci = await _context.Dodaci
                .FirstOrDefaultAsync(m => m.Id == id);
            if (dodaci == null)
            {
                return NotFound();
            }

            return View(dodaci);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {

            List<UslugaDodaciZaduzenje> zaduzenjadadaci = _context.UslugaDodaciZaduzenje.Where(x => x.DodaciID == id).ToList();
            _context.UslugaDodaciZaduzenje.RemoveRange(zaduzenjadadaci);


            var dodaci = await _context.Dodaci.FindAsync(id);
            _context.Dodaci.Remove(dodaci);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool DodaciExists(int id)
        {
            return _context.Dodaci.Any(e => e.Id == id);
        }
    }
}
