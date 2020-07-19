using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ISZRS.Web.Areas.Recepcija.Data
{
    public class Pagination
    {


        //Automatski pagenation za bilokakav Model u MVC modelu
        private ArrayList ArrayList {  get;  set; }
        private int PageSize { get; set; }

        public Pagination(ArrayList arrayList, int pageSize)
        {
            ArrayList = arrayList;
            PageSize = pageSize;
        }

        public Tuple< ArrayList,bool> GetStranicu(int BrojStranice)
        {
            ArrayList StranicaValues = new ArrayList();
            int pocetniIndex = 0;

            if (BrojStranice == 1)
            {

                if (ArrayList.Count > PageSize)
                {
                    StranicaValues = ArrayList.GetRange(pocetniIndex, PageSize);
                    return Tuple.Create(StranicaValues,false);

                }
                else
                {
                    StranicaValues = ArrayList;
                    //TempData["ZadnjaStranica"] = true;

                    return Tuple.Create(StranicaValues, true);

                }

            }
            else
            {

                //5 po starnici
                //1 str 0-4 pocetni index 0
                //2 str 5-9 pocedtni index 5 (broj stranice -1)*po stranici =(2-1)*5=1*5=5
                //3 str 10-14 ocedtni index 10 (3-1)*5=2*5=10
                //4 str 15-19 poc ind       15 (4-1)*5=3*5=15
                //5 str 20-24 ind           20 
                int max = ArrayList.Count;
                pocetniIndex = (BrojStranice - 1) * PageSize;

                if (pocetniIndex>=max)
                {
                    return Tuple.Create( StranicaValues, true);//salje null-true
                }

                if (max < pocetniIndex + PageSize)
                {
                    int zadnjaStranicaBrojRedova = ( max-pocetniIndex);
                    StranicaValues = ArrayList.GetRange(pocetniIndex, zadnjaStranicaBrojRedova);
                    //TempData["ZadnjaStranica"] = true;
                    return Tuple.Create(StranicaValues, true);


                }


                StranicaValues = ArrayList.GetRange(pocetniIndex, PageSize);

            }

            return Tuple.Create(StranicaValues, false);



            
        }
    }
}
