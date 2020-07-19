using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using ISZRS.Data;
using ISZRS.Data.Models;
using Microsoft.AspNetCore.Authorization;

namespace ISZRS.Web.Areas.Administracija.Controllers
{
    [Authorize(Roles =("Administrator,Logisticar"))]
    [Area("Administracija")]
    public class TipSobeNamjestajController : Controller
    {
        private readonly MojContext db;

        public TipSobeNamjestajController(MojContext context)
        {
            db = context;
        }
        public async Task<IActionResult> Index(int? TipSobeId)
        {
            if (TipSobeId!=null)
            {

                return View(db.TipSobeNamjestaj.Include(t => t.TipSobe).Where(x=>x.TipSobeID==TipSobeId).OrderBy(x => x.TipSobe).ToList());
            }
            var mojContext = db.TipSobeNamjestaj.Include(t => t.TipSobe);
            return View(await mojContext.ToListAsync());
        }

        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var tipSobeNamjestaj = await db.TipSobeNamjestaj
                .Include(t => t.TipSobe)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (tipSobeNamjestaj == null)
            {
                return NotFound();
            }

            return View(tipSobeNamjestaj);
        }

        public IActionResult Create()
        {
            ViewData["TipSobeID"] = new SelectList(db.TipSobe, "Id", "Naziv");
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,TipSobeID,TipNamjestaja,Kolicina")] TipSobeNamjestaj tipSobeNamjestaj)
        {
            if (ModelState.IsValid)
            {
                TipSobeNamjestaj postoji = db.TipSobeNamjestaj.SingleOrDefault(x => x.TipSobeID == tipSobeNamjestaj.TipSobeID && (int)x.TipNamjestaja == (int)tipSobeNamjestaj.TipNamjestaja);
                if (postoji!=null)
                {
                    postoji.Kolicina = tipSobeNamjestaj.Kolicina;

                    db.TipSobeNamjestaj.Update(postoji);
                    await db.SaveChangesAsync();
                    return RedirectToAction(nameof(Index));
                }


                db.Add(tipSobeNamjestaj);
                await db.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["TipSobeID"] = new SelectList(db.TipSobe, "Id", "Naziv", tipSobeNamjestaj.TipSobeID);
            return View(tipSobeNamjestaj);
        }

        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var tipSobeNamjestaj = await db.TipSobeNamjestaj.FindAsync(id);
            if (tipSobeNamjestaj == null)
            {
                return NotFound();
            }
            ViewData["TipSobeID"] = new SelectList(db.TipSobe, "Id", "Naziv", tipSobeNamjestaj.TipSobeID);
            return View(tipSobeNamjestaj);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,TipSobeID,TipNamjestaja,Kolicina")] TipSobeNamjestaj tipSobeNamjestaj)
        {
            if (id != tipSobeNamjestaj.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    db.Update(tipSobeNamjestaj);
                    await db.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!TipSobeNamjestajExists(tipSobeNamjestaj.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index),new { TipSobeId=tipSobeNamjestaj.TipSobeID });
            }
            ViewData["TipSobeID"] = new SelectList(db.TipSobe, "Id", "Naziv", tipSobeNamjestaj.TipSobeID);
            return View(tipSobeNamjestaj);
        }

        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var tipSobeNamjestaj = await db.TipSobeNamjestaj
                .Include(t => t.TipSobe)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (tipSobeNamjestaj == null)
            {
                return NotFound();
            }

            return View(tipSobeNamjestaj);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var tipSobeNamjestaj = await db.TipSobeNamjestaj.FindAsync(id);
            db.TipSobeNamjestaj.Remove(tipSobeNamjestaj);
            await db.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool TipSobeNamjestajExists(int id)
        {
            return db.TipSobeNamjestaj.Any(e => e.Id == id);
        }
    }
}
