using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using ISZRS.Web.Models;
using Microsoft.AspNetCore.Identity;
using ISZRS.Web.Areas.Identity.Data;
using Microsoft.Extensions.Logging;
using ISZRS.Web.Areas.Identity.Pages.Account;
using ISZR.Web.Models;

namespace ISZRS.Web.Controllers
{
    public class HomeController : Controller
    {


        private readonly SignInManager<ZaposlenikISZRSWebUser> _signInManager;
        private readonly UserManager<ZaposlenikISZRSWebUser> _userManager;
        private readonly ILogger<RegisterModel> _logger;
        private ISZRSWebContext UserDB;

        public HomeController(ISZRSWebContext IdentityDB, SignInManager<ZaposlenikISZRSWebUser> signInManager, UserManager<ZaposlenikISZRSWebUser> userManager, ILogger<RegisterModel> logger)
        {
            UserDB = IdentityDB;
            _signInManager = signInManager;
            _userManager = userManager;
            _logger = logger;
        }




        public async Task<IActionResult> OdjaviSe()
        {

            ZaposlenikISZRSWebUser zaposlenik = await _userManager.FindByNameAsync(HttpContext.User.Identity.Name);
            if (await _userManager.IsInRoleAsync(zaposlenik, "Administrator"))
            {
                await _signInManager.SignOutAsync();
                return RedirectToAction("Index", "Home", new { area = "" });
            }



            Ugovori ugovor = UserDB.UgovoriORadu.Where(x => x.ZaposlenikKorisnickoIme == HttpContext.User.Identity.Name).SingleOrDefault();

            DolasciNaPosao dolazak = UserDB.DolasciNaPosao.Where(x => x.DatumDolaska.Date == DateTime.Today && x.UgovorOraduId == ugovor.Id).SingleOrDefault();

            dolazak.DatumOdlaska = DateTime.Now;
            dolazak.BrojSati = (dolazak.DatumOdlaska - dolazak.DatumDolaska).Hours;
            UserDB.DolasciNaPosao.Update(dolazak);
            UserDB.SaveChanges();

            await _signInManager.SignOutAsync();
            return RedirectToAction("Index", "Home", new { area = "" });
        }





        #region  Prijava
        [HttpPost]
        public async Task<IActionResult> PrijaviSe(string username, string password)
        {


            if (String.IsNullOrEmpty(username) || String.IsNullOrEmpty(password))
            {
                TempData["Error"] = "Polje KorisnickoIme/Lozinka su prazne";
                return RedirectToAction("Index", "Home");

            }

            var result = await _signInManager.PasswordSignInAsync(username, password, false, false);

            if (result.Succeeded)
            {
                ZaposlenikISZRSWebUser zaposlenik = await _userManager.FindByNameAsync(username);
                if (await _userManager.IsInRoleAsync(zaposlenik, "Administrator"))
                {
                    return RedirectToAction("Index", "Pocetna", new { area = "Administracija" });
                }



                Ugovori ugovor = UserDB.UgovoriORadu.Where(x => x.ZaposlenikKorisnickoIme == username).SingleOrDefault();

                DolasciNaPosao dolazak = UserDB.DolasciNaPosao.Where(x => x.DatumDolaska.Date == DateTime.Today && x.UgovorOraduId == ugovor.Id).SingleOrDefault();

                if (dolazak==null)
                {
                    dolazak = new DolasciNaPosao()
                    {
                        DatumDolaska = DateTime.Now,
                        BrojSati = 0,
                        UgovorOraduId = ugovor.Id

                    };
                    await UserDB.DolasciNaPosao.AddAsync(dolazak);
                    await UserDB.SaveChangesAsync();

                }
                


                bool JeAdmin = await _userManager.IsInRoleAsync(zaposlenik, "Administrator");
                if (JeAdmin)
                {
                    return RedirectToAction("Index", "Pocetna", new { area = "Administracija" });
                }
                bool JeLogisticar = await _userManager.IsInRoleAsync(zaposlenik, "Logisticar");
                if (JeLogisticar)
                {
                    return RedirectToAction("Logistika", "Pocetna", new { area = "Administracija" });
                }
                bool JeRecepcionar = await _userManager.IsInRoleAsync(zaposlenik, "Recepcionar");
                if (JeRecepcionar)
                {
                    return RedirectToAction("Index", "Pocetna", new { area = "Recepcija" });
                }

            }


            TempData["Error"] = "Korisnik sa ovom kombinacijom ne postoji";
            return RedirectToAction("Index", "Home");
        }

        #endregion





        public async Task<IActionResult> KorisnikInfo()
        {

            var Zaposlenik =await _userManager.FindByNameAsync(HttpContext.User.Identity.Name);

            return View("Info");
        }


        public IActionResult Index()
        {

            if (TempData["Error"]!=null)
            {
                ViewBag.Greska = TempData["Error"];
            }
            return View();
        }
  
     

        public IActionResult Error()
        {
            return View(new ISZR.Web.Models.ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }



    }
}
