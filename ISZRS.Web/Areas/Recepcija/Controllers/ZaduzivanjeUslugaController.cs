using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ISZRS.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ISZRS.Web.Areas.Recepcija.Controllers
{

    [Authorize]
    [Area("Recepcija")]
    public class ZaduzivanjeUslugaController : Controller
    {


        private readonly MojContext db;

        public ZaduzivanjeUslugaController(MojContext context)
        {
            db = context;
        }

        public IActionResult Index()
        {
            return View();
        }


       
    }
}