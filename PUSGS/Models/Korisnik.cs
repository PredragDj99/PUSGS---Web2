﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace PUSGS.Models
{
    public class Korisnik
    {
        //BEZ SLIKE
        public Korisnik(string korisnickoIme, string email, string lozinka, string potvrdaLozinke, string ime, string prezime, DateTime datumRodjenja, string adresa, KorisnikType tipKorisnika)
        {
            KorisnickoIme = korisnickoIme;
            Email = email;
            Lozinka = lozinka;
            PotvrdaLozinke = potvrdaLozinke;
            Ime = ime;
            Prezime = prezime;
            DatumRodjenja = datumRodjenja;
            Adresa = adresa;
            TipKorisnika = tipKorisnika;
        }

        public Korisnik(string korisnickoIme, string email, string lozinka, string potvrdaLozinke, string ime, string prezime, DateTime datumRodjenja, string adresa, KorisnikType tipKorisnika, string slika)
        {
            KorisnickoIme = korisnickoIme;
            Email = email;
            Lozinka = lozinka;
            PotvrdaLozinke = potvrdaLozinke;
            Ime = ime;
            Prezime = prezime;
            DatumRodjenja = datumRodjenja;
            Adresa = adresa;
            TipKorisnika = tipKorisnika;
            Slika = slika;
        }

        [Required]
        public string KorisnickoIme { get; set; }
        [Required]
        public string Email { get; set; }
        [Required]
        public string Lozinka { get; set; }
        [Required]
        public string PotvrdaLozinke { get; set; }
        [Required]
        public string Ime { get; set; }
        [Required]
        public string Prezime { get; set; }
        [Required]
        public DateTime DatumRodjenja { get; set; }
        [Required]
        public string Adresa { get; set; }
        [Required]
        public KorisnikType TipKorisnika { get; set; }
        //nije obavezna
        public string Slika { get; set; }
        //INSERT INTO PUSGS.dbo.Slike values('Dostavljac', (SELECT * FROM OPENROWSET(BULK N'C:\Users\Pedja\Desktop\SlikeWEB2\dostavljac.png', SINGLE_BLOB)as T1))

        public Korisnik()
        {

        }
    }
}