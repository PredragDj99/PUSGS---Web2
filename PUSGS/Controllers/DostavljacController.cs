using PUSGS.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Web;
using System.Web.Mvc;

namespace PUSGS.Controllers
{
    public class DostavljacController : Controller
    {
        public static string zauzet = "";

        // GET: Dostavljac
        public ActionResult Index()
        {
            #region Status verifikacije
            Korisnik user = (Korisnik)Session["user"];
            if (user == null || user.TipKorisnika.ToString() != "DOSTAVLJAC")
            {
                return RedirectToAction("Index", "Home");
            }
            ViewBag.korisnik = user;

            if (user.TipKorisnika.ToString() == "DOSTAVLJAC")
            {
                List<Korisnik> dostavljaci= Baza.VratiSveDostavljace();
                foreach (var item in dostavljaci)
                {
                    if (item.Email == user.Email)
                    {
                        if (item.Verifikovan == "Prihvacen")
                        {
                            ViewBag.statusVerifikacije = "Zahtev za verifikaciju je prihvacen";
                        }
                        else if(item.Verifikovan == "Odbijen")
                        {
                            ViewBag.statusVerifikacije = "Zahtev je odbijen";
                        }
                        else
                        {
                            ViewBag.statusVerifikacije = "Zahtev se procesira";
                        }
                    }
                }
            }
            #endregion

            return View();
        }

        #region Log out
        public ActionResult LogOut()
        {
            Session["user"] = null;

            return RedirectToAction("Index", "Home");
        }
        #endregion

        #region Prikaz novih porudzbina(ako je zauzet ne moze da ih prihvati)
        public ActionResult NovePorudzbine()
        {
            #region Status verifikacije
            Korisnik user = (Korisnik)Session["user"];
            if (user == null || user.TipKorisnika.ToString() != "DOSTAVLJAC")
            {
                return RedirectToAction("Index", "Home");
            }
            ViewBag.korisnik = user;

            if (user.TipKorisnika.ToString() == "DOSTAVLJAC")
            {
                List<Korisnik> dostavljaci = Baza.VratiSveDostavljace();
                foreach (var item in dostavljaci)
                {
                    if (item.Email == user.Email)
                    {
                        if (item.Verifikovan == "Odbijen")
                        {
                            ViewBag.statusVerifikacije = "Zahtev je odbijen";
                            return View();
                        }
                        else if (item.Verifikovan == "Nije verifikovan")
                        {
                            ViewBag.statusVerifikacije = "Zahtev se procesira";
                            return View();
                        }
                        else
                        {
                            ViewBag.statusVerifikacije = "Prihvacen";
                        }
                    }
                }
            }
            #endregion

            List<SpojeneTabele> svePor = Baza.PrikazPorudzbina();
            List<SpojeneTabele> prikaz = new List<SpojeneTabele>();
            foreach (var item in svePor)
            {
                if(item.StatusPor == "Poruceno")
                {
                    prikaz.Add(item);
                }
            }

            #region Da li postoji neka aktivna porudzbina
            var dostavljacevePorudzbine = Baza.PrikaziDostaveDostavljaca(user);

            var por = new List<SpojeneTabele>();
            foreach (var item in dostavljacevePorudzbine)
            {
                zauzet = "slobodan";
                if (item.StatusPor == "U toku")
                {
                    zauzet = "zauzet";
                    break;
                }
            }
            #endregion
            if (zauzet == "zauzet")
            {
                ViewBag.zauzet = "zauzet";
            }
            ViewBag.por = "Poruceno";
            ViewBag.prikaz = prikaz;

            return View();
        }
        #endregion

        #region Prihvati dostavu
        public ActionResult PrihvatiDostavu(string email, string status)
        {
            Korisnik user = (Korisnik)Session["user"];
            if (user == null || user.TipKorisnika.ToString() != "DOSTAVLJAC")
            {
                return RedirectToAction("Index", "Home");
            }

            List<SpojeneTabele> svePor = Baza.PrikazPorudzbina();
            List<SpojeneTabele> prikaz = new List<SpojeneTabele>();
            foreach (var item in svePor)
            {
                if(item.Email==email && item.StatusPor==status)
                {
                    item.StatusPor = "U toku";
                    ViewBag.zauzet = "zauzet";
                    Baza.DostavljacPrihvatioPorudzbinu(item, user);
                }

                if (item.StatusPor == "Poruceno")
                {
                    prikaz.Add(item);
                }
            }
            ViewBag.por = "Poruceno";
            ViewBag.prikaz = prikaz;

            return View("NovePorudzbine");
        }
        #endregion

        #region Prikaz svih porudzbina ovog dostavljaca koje su dostavljene
        public ActionResult MojePorudzbine()
        {
            #region Status verifikacije
            Korisnik user = (Korisnik)Session["user"];
            if (user == null || user.TipKorisnika.ToString() != "DOSTAVLJAC")
            {
                return RedirectToAction("Index", "Home");
            }
            ViewBag.korisnik = user;

            if (user.TipKorisnika.ToString() == "DOSTAVLJAC")
            {
                List<Korisnik> dostavljaci = Baza.VratiSveDostavljace();
                foreach (var item in dostavljaci)
                {
                    if (item.Email == user.Email)
                    {
                        if (item.Verifikovan == "Odbijen")
                        {
                            ViewBag.statusVerifikacije = "Zahtev je odbijen";
                            return View();
                        }
                        else if(item.Verifikovan =="Nije verifikovan")
                        {
                            ViewBag.statusVerifikacije = "Zahtev se procesira";
                            return View();
                        }
                        else
                        {
                            ViewBag.statusVerifikacije = "Prihvacen";
                        }
                    }
                }
            }
            #endregion

            var dostavljacevePorudzbine = Baza.PrikaziDostaveDostavljaca(user);

            var por = new List<SpojeneTabele>();
            foreach (var item in dostavljacevePorudzbine)
            {
                if (item.StatusPor == "Dostavljena")
                {
                    por.Add(item);
                }
            }
            ViewBag.prikazKaoKodKorisnika = por;

            return View();
        }
        #endregion

        public static string odbrojavanje ="";
        public static string sifraPorudzbine = "";
        #region Prikaz trenutne porudzbine, isto kao kod potrosaca
        public ActionResult TrenutnaPorudzbina()
        {
            #region Status verifikacije
            Korisnik user = (Korisnik)Session["user"];
            if (user == null || user.TipKorisnika.ToString() != "DOSTAVLJAC")
            {
                return RedirectToAction("Index", "Home");
            }
            ViewBag.korisnik = user;

            if (user.TipKorisnika.ToString() == "DOSTAVLJAC")
            {
                List<Korisnik> dostavljaci = Baza.VratiSveDostavljace();
                foreach (var item in dostavljaci)
                {
                    if (item.Email == user.Email)
                    {
                        if (item.Verifikovan == "Odbijen")
                        {
                            ViewBag.statusVerifikacije = "Zahtev je odbijen";
                            return View();
                        }
                        else if (item.Verifikovan == "Nije verifikovan")
                        {
                            ViewBag.statusVerifikacije = "Zahtev se procesira";
                            return View();
                        }
                        else
                        {
                            ViewBag.statusVerifikacije = "Prihvacen";
                        }
                    }
                }
            }
            #endregion

            var dostavljacevePorudzbine = Baza.PrikaziDostaveDostavljaca(user);

            var por = new SpojeneTabele();
            foreach (var item in dostavljacevePorudzbine)
            {
                if(item.StatusPor == "U toku")
                {
                    por = item;
                    //za stopericu
                    odbrojavanje = "krenulo";
                    sifraPorudzbine = item.StaPorucuje;
                }
            }
            ViewBag.prikazKaoKodKorisnika = por;

            //za stopericu
            if (odbrojavanje != "krenulo")
            {
                ViewBag.odbrojavanje = "ne";
            }
            else
            {
                ViewBag.odbrojavanje = "krenulo";

                //da li ova porudzbina vec ima neko vreme porucivanja u bazi
                string vecZabelezeno = Baza.ProcitajSveStoperice(sifraPorudzbine);
                //ako postoji neka vrednost onda je vec upisano vreme
                if (vecZabelezeno == "")
                {
                    //ako vec postoji procitaj iz baze, a ako ne onda samo pokreni
                    Random rnd = new Random();
                    int vremeMinute = rnd.Next(1, 30);
                    int vremeSekunde = 0;

                    DateTime trenutnoIPomeraj = DateTime.Now.AddMinutes(vremeMinute);
                    Baza.DodajVremeStoperica(trenutnoIPomeraj, sifraPorudzbine);

                    DateTime trenutno = DateTime.Now;
                    int minute = (trenutnoIPomeraj - trenutno).Minutes;
                    int sekunde = (trenutnoIPomeraj - trenutno).Seconds;
                    ViewBag.vremeMinute = vremeMinute;
                    ViewBag.vremeSekunde = vremeSekunde;
                }
                else
                {
                    //prosledi ono staro vreme
                    DateTime trenutn = DateTime.Now;
                    int minute = (DateTime.Parse(vecZabelezeno) - trenutn).Minutes;
                    int sekunde = (DateTime.Parse(vecZabelezeno) - trenutn).Seconds;

                    //ako je vreme isteklo
                    if (DateTime.Compare(trenutn, DateTime.Parse(vecZabelezeno)) > 0)
                    {
                        por.StatusPor = "Dostavljena";
                    }
                    else
                    {
                        ViewBag.vremeMinute = minute;
                        ViewBag.vremeSekunde = sekunde;
                    }
                }
            }

            return View();
        }
        #endregion

        #region Izmeni profil
        [HttpPost]
        public ActionResult IzmeniProfil(Korisnik korisnik, HttpPostedFileBase file)
        {
            Korisnik user = (Korisnik)Session["user"];
            if (user == null || user.TipKorisnika.ToString() != "DOSTAVLJAC")
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

            if (file == null)
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
            korisnik.Verifikovan = user.Verifikovan;
            Korisnik k = Baza.UpdateProfila(korisnik, user.Email);

            if (k.Email !=null)
            {
                ViewBag.uspesno = "Uspesno izmenjeni podaci";
                Session["user"]=k; //novi podaci
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