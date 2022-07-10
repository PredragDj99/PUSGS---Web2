using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace PUSGS.Models
{
    public class Porudzbina
    {
        public Porudzbina(string staPorucuje, string kolicina, string adresa, string komentar, double cena, string status=null)
        {
            StaPorucuje = staPorucuje;
            Kolicina = kolicina;
            Adresa = adresa;
            Komentar = komentar;
            Cena = cena;
            Status = status;
        }

        public Porudzbina()
        {

        }

        [Required]
        public string StaPorucuje { get; set; }
        [Required]
        public string Kolicina { get; set; }
        [Required]
        public string Adresa { get; set; }
        public string Komentar { get; set; }
        [Required]
        public double Cena { get; set; }
        public string Status { get; set; }
    }
}