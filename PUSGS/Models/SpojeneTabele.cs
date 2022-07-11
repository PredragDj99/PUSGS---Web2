using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace PUSGS.Models
{
    public class SpojeneTabele
    {
        public SpojeneTabele(string staPorucuje, string proizvod, string kolicina, string adresa, string komentar, double cena, string statusPor, string email,Int32 dostavljacID)
        {
            StaPorucuje = staPorucuje;
            Proizvod = proizvod;
            Kolicina = kolicina;
            Adresa = adresa;
            Komentar = komentar;
            Cena = cena;
            StatusPor = statusPor;
            Email = email;
            DostavljacID = dostavljacID;
        }

        public SpojeneTabele() { }

        [Required]
        public string StaPorucuje { get; set; }
        [Required]
        public string Proizvod { get; set; }
        [Required]
        public string Kolicina { get; set; }
        [Required]
        public string Adresa { get; set; }
        public string Komentar { get; set; }
        [Required]
        public double Cena { get; set; }
        [Required]
        public string StatusPor { get; set; }
        [Required]
        public string Email { get; set; }
        [Required]
        public Int32 DostavljacID { get; set; }
    }
}