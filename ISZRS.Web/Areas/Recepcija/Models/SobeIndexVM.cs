using ISZR.Web.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ISZRS.Web.Areas.Recepcija.Models
{
    public class SobeIndexVM
    {

        public List<Sobe> Sobe { get; set; }

        public class SobeRed
        {
            public Sobe soba { get; set; }
            

        }

    }
}
