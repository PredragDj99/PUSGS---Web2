using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PUSGS.Models
{
    public class Proizvod
    {
        public Proizvod(string imeProizvoda, double cena, string sastojci)
        {
            ImeProizvoda = imeProizvoda;
            Cena = cena;
            Sastojci = sastojci;
        }

        public Proizvod()
        {

        }

        public string ImeProizvoda { get; set; }
        public double Cena { get; set; }
        public string Sastojci { get; set; }
    }
}