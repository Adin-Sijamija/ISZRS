using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ISZRS.Web.Areas.Administracija.Controllers
{
    [Authorize(Roles =("Administrator,Logisticar"))]
    [Area("Administracija")]
    public class PocetnaController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
        public IActionResult Logistika()
        {
            return View();
        }
        public IActionResult LogistikaSobe()
        {
            return View();
        }
        public IActionResult LogistikaUsluge()
        {
            return View();
        }
    }
    
}