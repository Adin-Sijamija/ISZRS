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
    [Area("Administracija")]
    [Authorize(Roles = ("Administrator,Logisticar"))]

    public class NamjestajController : Controller
    {
        private readonly MojContext _context;

        public NamjestajController(MojContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            return View(await _context.Namjestaj.ToListAsync());
        }

        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var namjestaj = await _context.Namjestaj
                .FirstOrDefaultAsync(m => m.Id == id);
            if (namjestaj == null)
            {
                return NotFound();
            }

            return View(namjestaj);
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Ime,Opis,Cijena,JeOsnovniNamjestaj,tipNamjestaja,Slika")] Namjestaj namjestaj, IFormFile Slika)
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

                        namjestaj.Slika = SlikaBitovi;

                    }

                }

                if (namjestaj.JeOsnovniNamjestaj)
                {
                    namjestaj.Cijena = 0;
                }

                _context.Add(namjestaj);
                await _context.SaveChangesAsync();
                return RedirectToAction("Details", "Namjestaj", new { id = namjestaj.Id });
            }
            return View(namjestaj);
        }

        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var namjestaj = await _context.Namjestaj.FindAsync(id);
            if (namjestaj == null)
            {
                return NotFound();
            }
            return View(namjestaj);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Ime,Opis,Cijena,tipNamjestaja,Slika")] Namjestaj namjestaj)
        {
            if (id != namjestaj.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(namjestaj);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!NamjestajExists(namjestaj.Id))
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
            return View(namjestaj);
        }

        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var namjestaj = await _context.Namjestaj
                .FirstOrDefaultAsync(m => m.Id == id);
            if (namjestaj == null)
            {
                return NotFound();
            }

            return View(namjestaj);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var namjestaj = await _context.Namjestaj.FindAsync(id);
            _context.Namjestaj.Remove(namjestaj);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool NamjestajExists(int id)
        {
            return _context.Namjestaj.Any(e => e.Id == id);
        }
    }
}
