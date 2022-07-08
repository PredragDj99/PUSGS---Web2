using PUSGS.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace PUSGS.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            return View();
        }

        //Fali prijava preko googla
        #region Prijava
        public ActionResult Prijava(string email, string lozinka)
        {
            Korisnik prijavljenKorisnik = Baza.PrijaviSe(email, lozinka);

            if (prijavljenKorisnik.Email == email)
            {
                Session["user"] = prijavljenKorisnik;

                if (prijavljenKorisnik.TipKorisnika.ToString() == "ADMINISTRATOR")
                {
                    return RedirectToAction("Index", "Admin");
                }
                else if(prijavljenKorisnik.TipKorisnika.ToString() == "DOSTAVLJAC")
                {
                    return RedirectToAction("Index", "Dostavljac");
                }
                else
                {
                    return RedirectToAction("Index","Potrosac");
                }
            }
            else
            {
                ViewBag.prijava = "Pogresno uneti podaci za prijavu!";
                return View("Registracija");
            }
        }

        public ActionResult PrijavaPrekoGoogle()
        {
            //Korisnik user = (Korisnik)Session["user"];
            //Session["user"] = prijavljenKorisnik;

            //vraca login zapravo
            return View("Registracija");
        }
        #endregion

        #region Registracija
        public ActionResult Registracija()
        {
            return View("Registracija");
        }

        [HttpPost]
        public ActionResult RegistrujSe(Korisnik korisnik, HttpPostedFileBase file)
        {
            #region Validacija
            if (korisnik.Lozinka != korisnik.PotvrdaLozinke)
            {
                ViewBag.uspesno = "Lozinka i potvrda lozinke se ne poklapaju!";
                return View("Registracija");
            }
            if (korisnik.DatumRodjenja > DateTime.Now)
            {
                ViewBag.uspesno = "Ne mozete da se rodite u buducnosti :)";
                return View("Registracija");
            }
            #endregion

            #region Save image
            if (file != null && file.ContentLength > 0)
            {
                string imgname = Path.GetFileName(file.FileName);
                //ovo ime saljem u bazu
                korisnik.Slika = imgname;

                string imgext = Path.GetExtension(imgname);
                if (imgext == ".jpg" || imgext == ".png")
                {
                    string imgpath = Path.Combine(Server.MapPath("~/App_Data/KorisnikSlike"), imgname);

                    if (System.IO.File.Exists(imgpath))
                    {
                        //vec imam sliku, ne radi save
                    }
                    else
                    {
                        file.SaveAs(imgpath);
                    }
                }
            }
            #endregion

            //Da li postoji registrovan korisnik sa ovim imejlom
            Korisnik k = Baza.PrijaviSe(korisnik.Email, korisnik.Lozinka);
            if (k.Email == null)
            {
                bool upis = Baza.DodajKorisnika(korisnik);

                if (upis)
                {
                    ViewBag.uspesno = "Uspesno registrovan";
                }
                else
                {
                    ViewBag.uspesno = "Registracija nije uspela";
                }
            }
            else
            {
                ViewBag.uspesno = "Vec postoji korisnik sa ovim imejlom";
            }

            return View("Registracija");
        }
        #endregion
    }
}