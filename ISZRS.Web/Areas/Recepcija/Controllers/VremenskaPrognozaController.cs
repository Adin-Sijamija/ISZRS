using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace ISZRS.Web.Areas.Recepcija.Controllers
{
    public class VremenskaPrognozaController : Controller
    {
        
        [Area("Recepcija")]
        public string WeatherApiJson([FromBody]string data)
        {
            return data;
        }
    }
}