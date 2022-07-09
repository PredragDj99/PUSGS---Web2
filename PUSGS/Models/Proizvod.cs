using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
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

        [Required]
        public string ImeProizvoda { get; set; }
        [Required]
        public double Cena { get; set; }
        [Required]
        public string Sastojci { get; set; }
    }
}