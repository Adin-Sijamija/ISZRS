using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using ISZR.Web.Models;
using ISZRS.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ISZRS.Web.Areas.Administracija.Controllers
{
    [Area("Administracija")]
    [Authorize(Roles = ("Administrator,Logisticar"))]
    public class SlikaController : Controller
    {
        private readonly MojContext db;

        public SlikaController(MojContext context)
        {
            db = context;
        }

   
        public async Task<ActionResult> GetNamjestajSliku(int id)
        {
            Namjestaj namjestaj = await db.Namjestaj.SingleOrDefaultAsync(x => x.Id == id);


            byte[] photoBack = namjestaj.Slika;

            return File(photoBack, "image/png");
        }

        public async Task<ActionResult> GetJelaSliku(int id)
        {
            Hrana jela = await db.Hrana.SingleOrDefaultAsync(x => x.Id == id);
             

            byte[] photoBack = jela.Slika;

            return File(photoBack, "image/png");
        }



    }
}