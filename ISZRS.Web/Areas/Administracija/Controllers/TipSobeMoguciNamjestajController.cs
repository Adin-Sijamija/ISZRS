using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using ISZRS.Data;
using ISZRS.Data.Models;
using ISZRS.Web.Areas.Administracija.Models;
using ISZR.Web.Models;
using Microsoft.AspNetCore.Authorization;

namespace ISZRS.Web.Areas.Administracija.Controllers
{
    [Authorize(Roles =("Administrator,Logisticar"))]
    [Area("Administracija")]
    public class TipSobeMoguciNamjestajController : Controller
    {
        private readonly MojContext _context;

        public TipSobeMoguciNamjestajController(MojContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index(int? TipNamjestajaInt, int? tipSobeID)
        {
            if (TipNamjestajaInt != null && tipSobeID != null)
            {
                TipSobeNamjestajVM VM = new TipSobeNamjestajVM();

                VM.TipSobe = _context.TipSobe.SingleOrDefault(x => x.Id == tipSobeID);

                if (TipNamjestajaInt.HasValue)
                    VM.TipNamjestaja = TipNamjestajaInt.Value;

                List<TipSobeMoguciNamjestaj> VecDodani = _context.TipSobeMoguciNamjestaj.Include(x => x.TipSobe).Include(x => x.Namjestaj)
                    .Where(x => x.TipSobeID == tipSobeID)
                    .ToList();
                VecDodani = VecDodani.Except(VecDodani.Where(x => (int)x.Namjestaj.tipNamjestaja != TipNamjestajaInt)).ToList();
                VM.zauzeti = VecDodani;


                List<Namjestaj> namjestaj = _context.Namjestaj.Where(x => (int)x.tipNamjestaja == TipNamjestajaInt).ToList();
                VM.slobodni = new List<Namjestaj>();
                VM.slobodni.AddRange(namjestaj);

                foreach (var VC in VecDodani)
                {
                    foreach (var n in namjestaj)
                    {
                        if (VC.NamjestajID == n.Id)
                        {
                            VM.slobodni.Remove(n);
                        }
                    }
                }


                return View(VM);

                //var test = _context.TipSobeMoguciNamjestaj.Include(t => t.Namjestaj).Include(t => t.TipSobe);
                //return View(await test.ToListAsync());
            }
            var mojContext = _context.TipSobeMoguciNamjestaj.Include(t => t.Namjestaj).Include(t => t.TipSobe);
            return View(await mojContext.ToListAsync());
        }
        
        public IActionResult Dodaj(int NamjestajID, int TipSobeId )
        {
            TipSobeMoguciNamjestaj novi = new TipSobeMoguciNamjestaj()
            {
                NamjestajID = NamjestajID,
                TipSobeID = TipSobeId
            };
            _context.TipSobeMoguciNamjestaj.Add(novi);
            _context.SaveChanges();

            TipSobeMoguciNamjestaj novi2 = _context.TipSobeMoguciNamjestaj.Include(x=>x.Namjestaj).SingleOrDefault(x => x.Id == novi.Id);

            return RedirectToAction("Index",new { TipNamjestajaInt=(int)novi2.Namjestaj.tipNamjestaja, tipSobeID= TipSobeId });
        }

        public IActionResult Ukloni(int ID)
        {
            TipSobeMoguciNamjestaj zaUkloniz = _context.TipSobeMoguciNamjestaj.Include(x => x.Namjestaj).SingleOrDefault(x => x.Id == ID);
            int tipSobe = zaUkloniz.TipSobeID;
            int TipNamjestajaInt = (int)zaUkloniz.Namjestaj.tipNamjestaja;

            _context.TipSobeMoguciNamjestaj.Remove(zaUkloniz);
            _context.SaveChanges();

            return RedirectToAction("Index", new { TipNamjestajaInt= TipNamjestajaInt, tipSobeID= tipSobe });
        }

        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var tipSobeMoguciNamjestaj = await _context.TipSobeMoguciNamjestaj
                .Include(t => t.Namjestaj)
                .Include(t => t.TipSobe)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (tipSobeMoguciNamjestaj == null)
            {
                return NotFound();
            }

            return View(tipSobeMoguciNamjestaj);
        }

        public IActionResult Create()
        {
            ViewData["NamjestajID"] = new SelectList(_context.Namjestaj, "Id", "Ime");
            ViewData["TipSobeID"] = new SelectList(_context.TipSobe, "Id", "Naziv");
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,NamjestajID,TipSobeID")] TipSobeMoguciNamjestaj tipSobeMoguciNamjestaj)
        {
            if (ModelState.IsValid)
            {
                _context.Add(tipSobeMoguciNamjestaj);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["NamjestajID"] = new SelectList(_context.Namjestaj, "Id", "Ime", tipSobeMoguciNamjestaj.NamjestajID);
            ViewData["TipSobeID"] = new SelectList(_context.TipSobe, "Id", "Naziv", tipSobeMoguciNamjestaj.TipSobeID);
            return View(tipSobeMoguciNamjestaj);
        }

        // GET: Administracija/TipSobeMoguciNamjestaj/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var tipSobeMoguciNamjestaj = await _context.TipSobeMoguciNamjestaj.FindAsync(id);
            if (tipSobeMoguciNamjestaj == null)
            {
                return NotFound();
            }
            ViewData["NamjestajID"] = new SelectList(_context.Namjestaj, "Id", "Ime", tipSobeMoguciNamjestaj.NamjestajID);
            ViewData["TipSobeID"] = new SelectList(_context.TipSobe, "Id", "Naziv", tipSobeMoguciNamjestaj.TipSobeID);
            return View(tipSobeMoguciNamjestaj);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,NamjestajID,TipSobeID")] TipSobeMoguciNamjestaj tipSobeMoguciNamjestaj)
        {
            if (id != tipSobeMoguciNamjestaj.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(tipSobeMoguciNamjestaj);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!TipSobeMoguciNamjestajExists(tipSobeMoguciNamjestaj.Id))
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
            ViewData["NamjestajID"] = new SelectList(_context.Namjestaj, "Id", "Ime", tipSobeMoguciNamjestaj.NamjestajID);
            ViewData["TipSobeID"] = new SelectList(_context.TipSobe, "Id", "Naziv", tipSobeMoguciNamjestaj.TipSobeID);
            return View(tipSobeMoguciNamjestaj);
        }

        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var tipSobeMoguciNamjestaj = await _context.TipSobeMoguciNamjestaj
                .Include(t => t.Namjestaj)
                .Include(t => t.TipSobe)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (tipSobeMoguciNamjestaj == null)
            {
                return NotFound();
            }

            return View(tipSobeMoguciNamjestaj);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var tipSobeMoguciNamjestaj = await _context.TipSobeMoguciNamjestaj.FindAsync(id);
            _context.TipSobeMoguciNamjestaj.Remove(tipSobeMoguciNamjestaj);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool TipSobeMoguciNamjestajExists(int id)
        {
            return _context.TipSobeMoguciNamjestaj.Any(e => e.Id == id);
        }
    }
}
