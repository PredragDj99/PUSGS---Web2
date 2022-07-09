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
        // GET: Dostavljac
        public ActionResult Index()
        {
            #region Status verifikacije
            Korisnik user = (Korisnik)Session["user"];
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
                            ViewBag.statusVerifikacije = "Zahtev je prihvacen";
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

        public ActionResult NovePorudzbine()
        {
            return View(); //Ne moze da radi ako nije verifikovan!!!!
        }

        public ActionResult MojePorudzbine()
        {
            return View(); //Ne moze da radi ako nije verifikovan!!!!
        }

        public ActionResult TrenutnaPorudzbina()
        {
            return View(); //Ne moze da radi ako nije verifikovan!!!!
        }

        #region Izmeni profil
        [HttpPost]
        public ActionResult IzmeniProfil(Korisnik korisnik, HttpPostedFileBase file)
        {
            Korisnik user = (Korisnik)Session["user"];

            if(korisnik.PotvrdaLozinke != korisnik.Lozinka)
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