﻿using PUSGS.Models;
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
            ViewBag.korisnik = user;

            return View();
        }

        public ActionResult Verifikacija()
        {
            ViewBag.zaVerifikaciju = Baza.VratiSveDostavljace();

            return View();
        }

        public ActionResult Prihvati(string email)
        {
            DostavljaciZaVerifikaciju.VerifikujPrihvati(email);

            return RedirectToAction("Verifikacija","Admin");
        }
        public ActionResult Odbij(string email)
        {
            DostavljaciZaVerifikaciju.VerifikujOdbij(email);

            return RedirectToAction("Verifikacija", "Admin");
        }


        public ActionResult SvePorudzbine()
        {
            return View();
        }

        public ActionResult DodavanjeProizvoda()
        {
            return View();
        }

        #region Izmeni profil
        [HttpPost]
        public ActionResult IzmeniProfil(Korisnik korisnik, HttpPostedFileBase file)
        {
            Korisnik user = (Korisnik)Session["user"];

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