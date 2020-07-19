using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using ISZR.Web.Models;
using ISZRS.Web.Areas.Identity.Data;
using ISZRS.Web.Areas.Identity.Pages.Account;
using ISZRS.Web.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.Extensions.Logging;
using static ISZRS.Web.Areas.Administracija.Controllers.KorisnikViewComponent;

namespace ISZRS.Web.Areas.Administracija.Controllers
{
    [Area("Administracija")]
    [Authorize(Roles = ("Administrator"))]
    public class ZaposleniciController : Controller
    {

        private readonly SignInManager<ZaposlenikISZRSWebUser> _signInManager;
        private readonly UserManager<ZaposlenikISZRSWebUser> _userManager;
        private readonly ILogger<ZaposlenikISZRSWebUser> _logger;
        private readonly ISZRSWebContext UserDB;


        public ZaposleniciController(ISZRSWebContext DB, SignInManager<ZaposlenikISZRSWebUser> signInManager, UserManager<ZaposlenikISZRSWebUser> userManager, ILogger<ZaposlenikISZRSWebUser> logger)
        {
            _signInManager = signInManager;
            _userManager = userManager;
            _logger = logger;
            UserDB = DB;
        }





        public async Task<IActionResult> Index()
        {

            List<KorisnikViewComponent.KorisnikInfo> korisnici = new List<KorisnikViewComponent.KorisnikInfo>();

            List<ZaposlenikISZRSWebUser> users = await _userManager.Users.ToListAsync();

            users.Remove(await _userManager.FindByNameAsync(HttpContext.User.Identity.Name));

            foreach (var v in users)
            {
                KorisnikInfo korisnik = new KorisnikInfo
                {
                    korisnik = v
                };

                IList<string> uloge = await _userManager.GetRolesAsync(v);
                korisnik.Uloge = string.Join(",", uloge);

                korisnici.Add(korisnik);
            }

            return View(korisnici);
        }




        public IActionResult Isplate(string KorisnickoIme)
        {

            if (TempData["Error"] != null)
            {
                ViewBag.Greska = TempData["Error"];
            }

            Ugovori ugovor = UserDB.UgovoriORadu.Where(x => x.ZaposlenikKorisnickoIme == KorisnickoIme).SingleOrDefault();

            List<Isplata> Isplate = UserDB.Isplata.Where(x => x.UgovoriID == ugovor.Id).ToList();

            return View(Isplate);
        }

        public IActionResult NovaIsplata(int Mjesec, int Godina, string KorisnickoIme)
        {

            if (Mjesec < 1 || Mjesec > 12 || Godina > DateTime.Now.Year)
            {
                TempData["Error"] = "Datum nije validan";
                return RedirectToAction("Isplate", new { KorisnickoIme });
            }

            TempData["Error"] = "Polje KorisnickoIme/Lozinka su prazne";

            Ugovori ugovor = UserDB.UgovoriORadu.Where(x => x.ZaposlenikKorisnickoIme == KorisnickoIme).SingleOrDefault();

            List<Isplata> Isplate = UserDB.Isplata.Where(x => x.UgovoriID == ugovor.Id && x.Mjesec == Mjesec && x.Godina == Godina).ToList();

            if (Isplate.Count() > 0)
            {
                TempData["Error"] = "Isplata za taj mjesec već postoji";
                return RedirectToAction("Isplate", new { KorisnickoIme });
            }


            List<DolasciNaPosao> dolasci = UserDB.DolasciNaPosao.Where(x => x.UgovorOraduId == ugovor.Id && x.DatumDolaska.Month == Mjesec && x.DatumDolaska.Year == Godina).ToList();

            if (Isplate.Count() > 0)
            {
                TempData["Error"] = "Zaposlenik nema dolazaka na posao u navedenom datumu";
                return RedirectToAction("Isplate", new { KorisnickoIme });
            }

            Isplata isplata = new Isplata()
            {
                Godina = Godina,
                Mjesec = Mjesec,
                UgovoriID = ugovor.Id,
                Iznos = dolasci.Sum(x => x.BrojSati) * ugovor.Satnica

            };

            UserDB.Isplata.Add(isplata);
            UserDB.SaveChanges();
            






            return RedirectToAction("Index");
    }

    public async Task<IActionResult> UkoniKorisnika(string KorisnickoIme)
    {
        if (String.IsNullOrEmpty(KorisnickoIme))
        {
            return RedirectToAction("Index");

        }


        Ugovori ugovor = UserDB.UgovoriORadu.Where(x => x.ZaposlenikKorisnickoIme == KorisnickoIme).SingleOrDefault();

        List<DolasciNaPosao> dolasci = UserDB.DolasciNaPosao.Where(x => x.UgovorOraduId == ugovor.Id).ToList();

        UserDB.RemoveRange(dolasci);
        UserDB.Remove(ugovor);
        UserDB.SaveChanges();

        var Korisnik = await _userManager.FindByNameAsync(KorisnickoIme);

        if (Korisnik != null)
        {

            IList<string> Uloge = await _userManager.GetRolesAsync(Korisnik);

            foreach (var item in Uloge)
            {
                await _userManager.RemoveFromRoleAsync(Korisnik, item);
            }

            await _userManager.DeleteAsync(Korisnik);


        }



        return RedirectToAction("Index");

    }

    public async Task<IActionResult> DolasciNaPosao(string KorisnickoIme)
    {
        ZaposlenikISZRSWebUser zaposlenik = await _userManager.FindByNameAsync(KorisnickoIme);

        Ugovori ugovor = UserDB.UgovoriORadu.Where(x => x.ZaposlenikKorisnickoIme == KorisnickoIme).SingleOrDefault();

        List<DolasciNaPosao> dolasci = UserDB.DolasciNaPosao.Where(x => x.UgovorOraduId == ugovor.Id).ToList();

        return View(dolasci);
    }


    public ActionResult Details(int id)
    {
        return View();
    }

    public ActionResult Create()
    {
        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public ActionResult Create(RegisterModel webUser, int Uloga, IFormFile Slika)
    {


        try
        {
            if (ModelState.IsValid)
            {
                ZaposlenikISZRSWebUser novi = new ZaposlenikISZRSWebUser();

                if (Slika.Length > 0)
                {
                    byte[] SlikaBitovi = null;
                    using (var fs1 = Slika.OpenReadStream())
                    using (var ms1 = new MemoryStream())
                    {
                        fs1.CopyTo(ms1);
                        SlikaBitovi = ms1.ToArray();
                    }

                    novi.Slika = SlikaBitovi;

                }
                return RedirectToAction(nameof(Index));

            }
            return View();

        }
        catch
        {
            return View();
        }

    }

    public ActionResult Edit(int id)
    {
        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public ActionResult Edit(int id, IFormCollection collection)
    {
        try
        {

            return RedirectToAction(nameof(Index));
        }
        catch
        {
            return View();
        }
    }

    public ActionResult Delete(int id)
    {
        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public ActionResult Delete(int id, IFormCollection collection)
    {
        try
        {

            return RedirectToAction(nameof(Index));
        }
        catch
        {
            return View();
        }
    }


}
}