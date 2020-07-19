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

    public class JelaController : Controller
    {
        private readonly MojContext _context;

        public JelaController(MojContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            return View(await _context.Hrana.ToListAsync());
        }

        // GET: Administracija/Hrana/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var jela = await _context.Hrana
                .FirstOrDefaultAsync(m => m.Id == id);
            if (jela == null)
            {
                return NotFound();
            }

            return View(jela);
        }

        // GET: Administracija/Hrana/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Administracija/Hrana/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Naziv,Opis,Cijena,Slika")] Hrana jela, IFormFile Slika)
        {
            if (ModelState.IsValid)
            {

                if (Slika == null)
                {
                    ViewData["INFO"] = "Slija je null";
                }

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

                        jela.Slika = SlikaBitovi;

                    }

                }


                _context.Add(jela);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(jela);
        }

        // GET: Administracija/Hrana/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var jela = await _context.Hrana.FindAsync(id);
            if (jela == null)
            {
                return NotFound();
            }
            return View(jela);
        }

        // POST: Administracija/Hrana/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Ime,Opis,Cijena,Slika")] Hrana jela)
        {
            if (id != jela.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(jela);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!JelaExists(jela.Id))
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
            return View(jela);
        }

        // GET: Administracija/Hrana/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var jela = await _context.Hrana
                .FirstOrDefaultAsync(m => m.Id == id);
            if (jela == null)
            {
                return NotFound();
            }

            return View(jela);
        }

        // POST: Administracija/Hrana/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var jela = await _context.Hrana.FindAsync(id);
            _context.Hrana.Remove(jela);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool JelaExists(int id)
        {
            return _context.Hrana.Any(e => e.Id == id);
        }
    }
}
