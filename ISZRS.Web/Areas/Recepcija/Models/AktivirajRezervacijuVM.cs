﻿using ISZR.Web.Models;
using ISZRS.Data.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ISZRS.Web.Areas.Recepcija.Models
{
    public class AktivirajRezervacijuVM
    {
        public Rezervacije Rezervacija { get; set; }
        public List<ZaduzeneSobe> Sobe { get; set; }

    }
}
