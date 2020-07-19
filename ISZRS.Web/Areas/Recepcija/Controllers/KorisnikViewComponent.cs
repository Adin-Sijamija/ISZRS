using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using ISZRS.Data;
using ISZRS.Web.Areas.Identity.Data;
using ISZRS.Web.Areas.Identity.Pages.Account;
using ISZRS.Web.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace ISZRS.Web.Areas.Identity
{
    [ViewComponent(Name = "TrenutniKorisnikRecepcija")]
    public class KorisnikViewComponent :  ViewComponent
    {
        private readonly MojContext db;
        private readonly ISZRSWebContext dbUsers;
        private readonly SignInManager<ZaposlenikISZRSWebUser> _signInManager;
        private readonly ILogger<LoginModel> _logger;
        private UserManager<ZaposlenikISZRSWebUser> _userManager;

        public KorisnikViewComponent(MojContext db, ISZRSWebContext dbUsers, SignInManager<ZaposlenikISZRSWebUser> signInManager, ILogger<LoginModel> logger, UserManager<ZaposlenikISZRSWebUser> userManager)
        {
            this.db = db;
            this.dbUsers = dbUsers;
            _signInManager = signInManager;
            _logger = logger;
            _userManager = userManager;
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {


            var user = await _userManager.GetUserAsync(HttpContext.User);
            ZaposlenikISZRSWebUser user1 = await _userManager.GetUserAsync(HttpContext.User);


           IList<string> uloge = await _userManager.GetRolesAsync(user);

            KorisnikInfo korisnikInfo = new KorisnikInfo(user1,uloge);
           
            return View("KorisnikVC", korisnikInfo);
        }


        public class KorisnikInfo
        {
           public KorisnikInfo(ZaposlenikISZRSWebUser korisnik, IList<string> uloge)
            {
                this.korisnik = korisnik;
                 Uloge = string.Join(",", uloge);
            }

            public ZaposlenikISZRSWebUser korisnik { get; }
            public string Uloge { get; set;}
        }



    }
}