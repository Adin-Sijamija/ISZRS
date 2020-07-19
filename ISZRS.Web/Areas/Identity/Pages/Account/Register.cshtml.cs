using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text.Encodings.Web;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using ISZRS.Web.Areas.Identity.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Http;
using System.IO;
using ISZR.Web.Models;
using ISZRS.Web.Models;

namespace ISZRS.Web.Areas.Identity.Pages.Account
{
    //[AllowAnonymous]
    [Authorize(Roles = "Administrator")]
    public class RegisterModel : PageModel
    {
        private readonly SignInManager<ZaposlenikISZRSWebUser> _signInManager;
        private readonly UserManager<ZaposlenikISZRSWebUser> _userManager;
        private readonly ILogger<RegisterModel> _logger;
        private readonly IEmailSender _emailSender;
        private readonly ISZRSWebContext UserDB;

        public RegisterModel(
            UserManager<ZaposlenikISZRSWebUser> userManager,
            SignInManager<ZaposlenikISZRSWebUser> signInManager,
            ILogger<RegisterModel> logger,
             ISZRSWebContext DB,
            IEmailSender emailSender)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _logger = logger;
            _emailSender = emailSender;
            UserDB = DB;
        }

        [BindProperty]
        public InputModel Input { get; set; }

        [BindProperty]
        [Display(Name = "Profilna slika")]
        public IFormFile SlikaFile { get; set; }

        [BindProperty]
        public Ugovori Ugovor { get; set; }


        [Display(Name = "Tip Korisnika")]
        [BindProperty]
        public int TipKorisnika { get; set; }

        public string ReturnUrl { get; set; }

        public class InputModel
        {
            //[Required]
            //[EmailAddress]
            //[Display(Name = "Email")]
            //public string Email { get; set; }

            [Required(ErrorMessage = "Obavezno Polje")]
            [StringLength(30,MinimumLength =2)]
            public string Ime { get; set; }

            [Required(ErrorMessage = "Obavezno Polje")]
            [StringLength(50,MinimumLength =4)]
            public string Prezime { get; set; }

            [Display(Name ="Korisničko Ime")]
            [StringLength(50, MinimumLength = 4)]
            public string KorisnickoIme { get; set; }


            [StringLength(13)]
            [Required]
            [RegularExpression("[0-9]{13}", ErrorMessage = "JMBG nije validan")]
            public string JMBG { get; set; }


            [Display(Name = "Datum rođenja")]
            [Required(ErrorMessage = "Obavezno Polje")]
            [DataType(DataType.Date)]
            public DateTime DatumRođenja { get; set; }



            [StringLength(100, ErrorMessage = "The {0} must be at least {2} and at max {1} characters long.", MinimumLength = 6)]
            [DataType(DataType.Password)]
            [Required(ErrorMessage = "Obavezno Polje")]
            [Display(Name = "Šifra")]
            public string Password { get; set; }

            [DataType(DataType.Password)]
            [Display(Name = "Potvrdi šifru")]
            [Required(ErrorMessage = "Obavezno Polje")]
            [Compare("Password", ErrorMessage = "Mora da bude ista kao šifra")]
            public string ConfirmPassword { get; set; }

            [Display(Name ="Profilna slika")]
            public byte[] Slika { get; set; }

            [Phone]
            [Display(Name = "Phone number")]
            public string PhoneNumber { get; set; }

        }

        public void OnGet(string returnUrl = null)
        {
            ReturnUrl = returnUrl;
        }

        public async Task<IActionResult> OnPostAsync(string returnUrl = null)
        {
            bool validanUgovor = true;
            returnUrl = returnUrl ?? Url.Content("~/");
            if (ModelState.IsValid)
            {


                var user = new ZaposlenikISZRSWebUser
                {
                    
                    UserName = Input.Ime+Input.Prezime,
                    PhoneNumber=Input.PhoneNumber,
                    Ime=Input.Ime,
                    Prezime=Input.Prezime,
                    DatumRođenja=Input.DatumRođenja,
                    JMBG=Input.JMBG,
                    
                    
                    Email = null
                };

                if (SlikaFile != null)
                {
                    if (SlikaFile.Length > 0)
                    {
                        byte[] SlikaBitovi = null;
                        using (var fs1 = SlikaFile.OpenReadStream())
                        using (var ms1 = new MemoryStream())
                        {
                            fs1.CopyTo(ms1);
                            SlikaBitovi = ms1.ToArray();
                        }

                        user.Slika = SlikaBitovi;

                    }

                }

                if (!String.IsNullOrEmpty(Input.KorisnickoIme))
                {
                    var postojeci = await _userManager.FindByNameAsync(Input.KorisnickoIme);
                    if (postojeci!=null)
                    {
                        user.UserName = Input.KorisnickoIme;

                    }
                }




                if (Ugovor.DatumPocetkaUgovora<DateTime.Now)
                {
                    ModelState.AddModelError(string.Empty, "Datum pocetka mora biti veći od jučernjašeg datuma");
                    validanUgovor = false;
                }
                
                if (Ugovor.DatumPocetkaUgovora > Ugovor.DatumZavrsetkaUgovora)
                {
                    ModelState.AddModelError(string.Empty, "Datum pocetka mora biti veći od datuma završetka ugovora");
                    validanUgovor = false;

                }

                if ((Ugovor.DatumZavrsetkaUgovora - Ugovor.DatumPocetkaUgovora).TotalDays<30)
                {
                    ModelState.AddModelError(string.Empty, "ugovor mora trajati najmanje 1 mjesec");
                    validanUgovor = false;

                }

                if (validanUgovor)
                {
                    var result = await _userManager.CreateAsync(user, Input.Password);
                    if (result.Succeeded )
                    {
                        switch (TipKorisnika)
                        {
                            case 1:
                                await _userManager.AddToRoleAsync(user, "Recepcionar");
                                break;
                            case 2:
                                await _userManager.AddToRoleAsync(user, "Logisticar");
                                break;
                            default:
                                break;
                        }
                        

                        _logger.LogInformation("User created a new account with password.");



                        Ugovor.ZaposlenikKorisnickoIme = user.UserName;
                        await UserDB.UgovoriORadu.AddAsync(Ugovor);
                        await UserDB.SaveChangesAsync();
                   

                        return RedirectToAction("Index", "Zaposlenici", new { area = "Administracija" });
                    }
                    foreach (var error in result.Errors)
                    {
                        ModelState.AddModelError(string.Empty, error.Description);
                    }
                }
               
            }

            // If we got this far, something failed, redisplay form
            return Page();
        }
    }
}
