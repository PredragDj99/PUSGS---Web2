using PUSGS.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace PUSGS.Controllers
{
    public class AdminController : Controller
    {
        // GET: Admin
        public ActionResult Index()
        {
            Korisnik user = (Korisnik)Session["user"];
            if (user == null || user.TipKorisnika.ToString() != "ADMINISTRATOR")
            {
                return RedirectToAction("Index", "Home");
            }
            ViewBag.korisnik = user;

            return View();
        }

        #region Log out
        public ActionResult LogOut()
        {
            Session["user"] = null;

            return RedirectToAction("Index", "Home");
        }
        #endregion

        #region Verifikacija, prihvati/odbij dostavljaca
        public ActionResult Verifikacija()
        {
            Korisnik user = (Korisnik)Session["user"];
            if (user == null || user.TipKorisnika.ToString() != "ADMINISTRATOR")
            {
                return RedirectToAction("Index", "Home");
            }
            ViewBag.zaVerifikaciju = Baza.VratiSveDostavljace();

            return View();
        }

        public ActionResult Prihvati(string email)
        {
            Korisnik user = (Korisnik)Session["user"];
            if (user == null || user.TipKorisnika.ToString() != "ADMINISTRATOR")
            {
                return RedirectToAction("Index", "Home");
            }

            DostavljaciZaVerifikaciju.VerifikujPrihvati(email);

            return RedirectToAction("Verifikacija","Admin");
        }
        public ActionResult Odbij(string email)
        {
            Korisnik user = (Korisnik)Session["user"];
            if (user == null || user.TipKorisnika.ToString() != "ADMINISTRATOR")
            {
                return RedirectToAction("Index", "Home");
            }

            DostavljaciZaVerifikaciju.VerifikujOdbij(email);

            return RedirectToAction("Verifikacija", "Admin");
        }
        #endregion

        #region Prikaz svih porudzbina
        public ActionResult SvePorudzbine()
        {
            Korisnik user = (Korisnik)Session["user"];
            if (user == null || user.TipKorisnika.ToString() != "ADMINISTRATOR")
            {
                return RedirectToAction("Index", "Home");
            }
            List<SpojeneTabele> svePorudzbine = Baza.PrikazPorudzbina();

            ViewBag.prikazPorudzbina = svePorudzbine;

            return View();
        }
        #endregion

        #region Dodaj proizvod
        public ActionResult DodavanjeProizvoda()
        {
            Korisnik user = (Korisnik)Session["user"];
            if (user == null || user.TipKorisnika.ToString() != "ADMINISTRATOR")
            {
                return RedirectToAction("Index", "Home");
            }
            return View();
        }

        public ActionResult DodajProizvod(Proizvod proizvod)
        {
            Korisnik user = (Korisnik)Session["user"];
            if (user == null || user.TipKorisnika.ToString() != "ADMINISTRATOR")
            {
                return RedirectToAction("Index", "Home");
            }

            if (proizvod.Cena == 0)
            {
                ViewBag.uspesno = "Cena mora biti uneta kao broj! Koristite '.' umesto ',' ";
                return View("DodavanjeProizvoda");
            }

            Proizvod dodavanjeProizvoda = Baza.PostojanjeProizvoda(proizvod.ImeProizvoda);

            if (dodavanjeProizvoda.ImeProizvoda==null)
            {
                Baza.DodajProizvod(proizvod);
                ViewBag.uspesno = "Proizvod uspesno dodat";
            }
            else
            {
                ViewBag.uspesno = "Proizvod sa unetim nazivom vec postoji";
            }

            return View("DodavanjeProizvoda");
        }
        #endregion

        #region Izmeni profil
        [HttpPost]
        public ActionResult IzmeniProfil(Korisnik korisnik, HttpPostedFileBase file)
        {
            Korisnik user = (Korisnik)Session["user"];
            if (user == null || user.TipKorisnika.ToString() != "ADMINISTRATOR")
            {
                return RedirectToAction("Index", "Home");
            }
            #region Validacija
            if (korisnik.KorisnickoIme == "")
            {
                ViewBag.uspesno = "Unesite sve podatke(slika je opciona)";
                return View("Index");
            }
            else if (korisnik.Email == "")
            {
                ViewBag.uspesno = "Unesite sve podatke(slika je opciona)";
                return View("Index");
            }
            else if (korisnik.Lozinka == "")
            {
                ViewBag.uspesno = "Unesite sve podatke(slika je opciona)";
                return View("Index");
            }
            else if (korisnik.PotvrdaLozinke == "")
            {
                ViewBag.uspesno = "Unesite sve podatke(slika je opciona)";
                return View("Index");
            }
            else if (korisnik.Prezime == "")
            {
                ViewBag.uspesno = "Unesite sve podatke(slika je opciona)";
                return View("Index");
            }
            else if (korisnik.Ime == "")
            {
                ViewBag.uspesno = "Unesite sve podatke(slika je opciona)";
                return View("Index");
            }
            else if (korisnik.Adresa == "")
            {
                ViewBag.uspesno = "Unesite sve podatke(slika je opciona)";
                return View("Index");
            }
            else if (korisnik.TipKorisnika.ToString() == "")
            {
                ViewBag.uspesno = "Unesite sve podatke(slika je opciona)";
                return View("Index");
            }
            else if (korisnik.DatumRodjenja.ToString().Contains("0001"))
            {
                ViewBag.uspesno = "Unesite sve podatke(slika je opciona)";
                return View("Index");
            }

            if (korisnik.Lozinka != korisnik.PotvrdaLozinke)
            {
                ViewBag.uspesno = "Lozinka i potvrda lozinke se ne poklapaju!";
                return View("Index");
            }
            if (korisnik.DatumRodjenja > DateTime.Now)
            {
                ViewBag.uspesno = "Ne mozete da se rodite u buducnosti :)";
                return View("Index");
            }
            #endregion

            if (korisnik.PotvrdaLozinke != korisnik.Lozinka)
            {
                ViewBag.uspesno = "Nepoklapanje lozinki";
                ViewBag.korisnik = user;
                return View("Index");
            }

            if (korisnik.Slika == null)
            {
                korisnik.Slika = user.Slika;
            }
            else
            {
                #region Save image
                if (file != null && file.ContentLength > 0)
                {
                    string imgname = Path.GetFileName(file.FileName);
                    //ovo ime saljem u bazu
                    korisnik.Slika = imgname;

                    string imgext = Path.GetExtension(imgname);
                    if (imgext == ".jpg" || imgext == ".png")
                    {
                        string imgpath = Path.Combine(Server.MapPath("~/Content/Images"), imgname);

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
            }

            #region Update
            Korisnik k = Baza.UpdateProfila(korisnik, user.Email);

            if (k.Email != null)
            {
                ViewBag.uspesno = "Uspesno izmenjeni podaci";
                Session["user"] = k; //novi podaci
                ViewBag.korisnik = k;
            }
            else
            {
                ViewBag.uspesno = "Promena podataka neuspesna";
                ViewBag.korisnik = user;
            }
            #endregion

            return View("Index");
        }
        #endregion
    }
}