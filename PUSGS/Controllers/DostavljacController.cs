using PUSGS.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
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
                if (item.StatusPor == "Dostavljena")
                {
                    zauzet = "zauzet";
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

            zauzet = "zauzet";
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
                }
            }
            ViewBag.prikazKaoKodKorisnika = por;

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