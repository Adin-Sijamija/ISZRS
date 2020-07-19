using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using ISZR.Web.Models;
using ISZRS.Data;
using Microsoft.AspNetCore.Http;
using System.IO;
using Microsoft.AspNetCore.Authorization;

namespace ISZRS.Web.Areas.Administracija.Controllers
{
    [Authorize(Roles =("Administrator,Logisticar"))]
    [Area("Administracija")]
    public class UslugeController : Controller
    {
        private readonly MojContext _context;

        public UslugeController(MojContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            return View(await _context.Usluge.ToListAsync());
        }

        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var usluge = await _context.Usluge
                .FirstOrDefaultAsync(m => m.Id == id);
            if (usluge == null)
            {
                return NotFound();
            }

            return View(usluge);
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Naziv,Opis,Cijena,TipCijene,TipUsluge")] Usluge usluge, IFormFile Slika)
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

                        usluge.Slika = SlikaBitovi;

                    }

                }

                if (usluge.TipCijene==0)
                {
                    usluge.Cijena = 0;
                }

                _context.Add(usluge);
                await _context.SaveChangesAsync();
                return RedirectToAction("Details", "Usluge", new { id = usluge.Id });
            }
            return View(usluge);
        }

        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var usluge = await _context.Usluge.FindAsync(id);
            if (usluge == null)
            {
                return NotFound();
            }
            return View(usluge);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Naziv,Opis,Cijena,TipCijene,TipUsluge,Slika")] Usluge usluge)
        {
            if (id != usluge.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(usluge);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!UslugeExists(usluge.Id))
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
            return View(usluge);
        }
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var usluge = await _context.Usluge
                .FirstOrDefaultAsync(m => m.Id == id);
            if (usluge == null)
            {
                return NotFound();
            }

            return View(usluge);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {

            List<Dodaci> dodaci = _context.Dodaci.Where(x => x.UslugeID == id).ToList();
            List<UslugaDodaciZaduzenje> zaduzenjadadaci = _context.UslugaDodaciZaduzenje.Where(x => x.UslugaID == id).ToList();

            _context.UslugaDodaciZaduzenje.RemoveRange(zaduzenjadadaci);
            _context.Dodaci.RemoveRange(dodaci);

            var usluge = await _context.Usluge.FindAsync(id);
            _context.Usluge.Remove(usluge);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool UslugeExists(int id)
        {
            return _context.Usluge.Any(e => e.Id == id);
        }
    }
}
