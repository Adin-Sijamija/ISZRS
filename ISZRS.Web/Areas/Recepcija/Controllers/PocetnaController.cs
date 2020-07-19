using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ISZR.Web.Models;
using ISZRS.Data;
using ISZRS.Data.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ISZRS.Web.Areas.Recepcija.Controllers
{
    [Authorize]
    [Area("Recepcija")]
    public class PocetnaController : Controller
    {

        private readonly MojContext db;

        public PocetnaController(MojContext context)
        {
            db = context;
        }

        public IActionResult Index()
        {

            return View();
        }

        public JsonResult GostAutocomplete(string term)
        {
           

            string termm = term.ToUpper().Trim();
            List<Gosti> gostis = db.Gosti
                .Include(x => x.Gradovi)
                .ThenInclude(x => x.Drzave)
                .ToList();



            var GostiArray = (from N in gostis
                              where N.PunoIme.Trim().ToUpper().Contains(termm)
                              select new { N.Id, gostInfo = string.Format("{0}, {1} {2} {3}, {4}", N.PunoIme, N.Gradovi.Drzave.Naziv, N.Gradovi.Regija, N.Gradovi.Naziv, N.Adresa) }
                 );

            return Json(GostiArray);

        }









    }
}