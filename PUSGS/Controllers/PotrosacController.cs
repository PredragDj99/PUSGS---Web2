using PUSGS.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace PUSGS.Controllers
{
    public class PotrosacController : Controller
    {
        public static List<Proizvod> porucuje = new List<Proizvod>();
        private double dostava = 200;
        public static SpojeneTabele aktivna = new SpojeneTabele();

        // GET: Potrosac
        public ActionResult Index()
        {
            Korisnik user = (Korisnik)Session["user"];
            if (user == null || user.TipKorisnika.ToString() != "POTROSAC")
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

        #region Kreiranje nove porudzbine
        public ActionResult NovaTrenutnaPorudzbina()
        {
            Korisnik user = (Korisnik)Session["user"];
            if (user == null || user.TipKorisnika.ToString() != "POTROSAC")
            {
                return RedirectToAction("Index", "Home");
            }
            #region Trenutno poruceno
            var trenutno = Baza.PrikazPorudzbina();
            foreach (var item in trenutno)
            {
                if ((item.StatusPor == "Poruceno" || item.StatusPor == "U toku") && item.Email == user.Email)
                {
                    aktivna = item;
                }
            }
            ViewBag.por = "Poruceno";
            ViewBag.TrenutnoPoruceno = aktivna;
            #endregion
            //Ovde treba da prikaze porudzbinu kod koje odbrojava, a ne moze da se ostavi TrenutnoPoruceno = aktivna jer je prazno
            //ViewBag.por = "Poruceno";
            //ViewBag.TrenutnoPoruceno = aktivna;

            List<Proizvod> listaProizvoda = Baza.PrikazProizvoda();
            ViewBag.prikazProizvoda = listaProizvoda;

            porucuje.Clear();
            ViewBag.porucuje = porucuje;

            return View();
        }

        public ActionResult NapraviPorudzbinu(string imeProizvoda, string cena,string sastojci)
        {
            Korisnik user = (Korisnik)Session["user"];
            if (user == null || user.TipKorisnika.ToString() != "POTROSAC")
            {
                return RedirectToAction("Index", "Home");
            }

            #region Validacija
            if (imeProizvoda == "")
            {
                return View("NovaTrenutnaPorudzbina");
            }
            else if (cena == "")
            {
                return View("NovaTrenutnaPorudzbina");
            }
            else if (sastojci == "")
            {
                return View("NovaTrenutnaPorudzbina");
            }
            #endregion

            #region Trenutno poruceno
            var trenutno = Baza.PrikazPorudzbina();
            foreach (var item in trenutno)
            {
                if ((item.StatusPor == "Poruceno" || item.StatusPor == "U toku") && item.Email == user.Email)
                {
                    aktivna = item;
                }
            }
            ViewBag.por = "Poruceno";
            ViewBag.TrenutnoPoruceno = aktivna;
            #endregion

            List<Proizvod> listaProizvoda = Baza.PrikazProizvoda();
            ViewBag.prikazProizvoda = listaProizvoda;

            Proizvod p = new Proizvod(imeProizvoda, Double.Parse(cena), sastojci);

            //ne mozes u korpu dodati vise proizvoda istog imena
            bool poklapanje = false;
            for (int i = 0; i < porucuje.Count; i++)
            {
                if(porucuje[i].ImeProizvoda == imeProizvoda)
                {
                    poklapanje = true;
                }
            }
            if (!poklapanje)
            {
                porucuje.Add(p);
            }

            ViewBag.porucuje = porucuje;

            return View("NovaTrenutnaPorudzbina");
        }
        #endregion

        #region Poruci porudzbinu
        public ActionResult Poruci(string adresa, string komentar, FormCollection formCollection)
        {
            ViewBag.TrenutnoPoruceno = aktivna;
            Korisnik user = (Korisnik)Session["user"];
            if (user == null || user.TipKorisnika.ToString() != "POTROSAC")
            {
                return RedirectToAction("Index", "Home");
            }
            #region Validacija
            if (adresa == "")
            {
                ViewBag.por = "Unesite adresu!";
                return View("NovaTrenutnaPorudzbina");
            }
            #endregion


            List<Proizvod> listaProizvoda = Baza.PrikazProizvoda();
            ViewBag.prikazProizvoda = listaProizvoda;

            //Liste proizvoda koji se porucuju i kolicine
            List<string> proizod = new List<string>();
            List<int> kolicina = new List<int>();

            //cena dostave da bude 200din
            double ukupnaCena = dostava;
            int brojac = 0;
            foreach (var item in porucuje)
            {
                brojac++;
                string naziv = "Kolicina" + brojac.ToString();

                ukupnaCena += item.Cena * Double.Parse(formCollection[naziv]);

                //Liste proizvoda koji se porucuju i kolicine
                proizod.Add(item.ImeProizvoda);
                kolicina.Add(Int32.Parse(formCollection[naziv]));
            }

            ViewBag.por = "Poruceno";
            //Nista nije naruceno
            if (ukupnaCena == 200)
            {
                ViewBag.por = "Morate imati bar 1 proizvod da biste porucili dostavu!";
            }
            else if(ViewBag.TrenutnoPoruceno.StaPorucuje != null)
            {
                ViewBag.por = "Ne mozete imati vise porudzbina istovremeno!";
            }
            else
            {
                #region Random naziv porudzbine
                var chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";
                var stringChars = new char[8];
                var random = new Random();

                for (int i = 0; i < stringChars.Length; i++)
                {
                    stringChars[i] = chars[random.Next(chars.Length)];
                }
                string finalString = new String(stringChars);

                string staPorucuje = "Por" + finalString;
                #endregion

                Porudzbina porudzbina = new Porudzbina(staPorucuje, "", adresa, komentar, ukupnaCena, "Poruceno");
                //porudzbina, listu proizvoda, listu za kolicinu
                Baza.NovaPorudzbina(porudzbina, proizod, kolicina, user.Email);
            }

            #region Trenutno poruceno
            var trenutno = Baza.PrikazPorudzbina();
            foreach (var item in trenutno)
            {
                if (item.StatusPor == "Poruceno" && item.Adresa==adresa)
                {
                    aktivna=item;
                }
            }
            ViewBag.TrenutnoPoruceno = aktivna;
            #endregion

            //Prikazi sta je trenutno poruceno cim kliknem stranicu i vreme koje odbrojava
            //stoperica krece kada dostavljac prihvati dostavu

            porucuje.Clear();
            ViewBag.porucuje = porucuje;
            return View("NovaTrenutnaPorudzbina");
        }
        #endregion

        #region Prethodne porudzbine koje su izvrsene
        public ActionResult PrethodnePorudzbine()
        {
            Korisnik user = (Korisnik)Session["user"];
            if (user == null || user.TipKorisnika.ToString() != "POTROSAC")
            {
                return RedirectToAction("Index", "Home");
            }
            List<SpojeneTabele> svePorudzbine = Baza.PrikazPorudzbina();
            List<SpojeneTabele> mojePorudzine = new List<SpojeneTabele>();

            foreach (var item in svePorudzbine)
            {
                if(item.Adresa==user.Adresa && item.StatusPor == "Dostavljena")
                {
                    mojePorudzine.Add(item);
                }
            }
            ViewBag.mojePorudzbine = mojePorudzine;

            return View();
        }
        #endregion

        #region Izmeni profil
        [HttpPost]
        public ActionResult IzmeniProfil(Korisnik korisnik, HttpPostedFileBase file)
        {
            Korisnik user = (Korisnik)Session["user"];
            if (user == null || user.TipKorisnika.ToString() != "POTROSAC")
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

            return RedirectToAction("Index", "Potrosac");
        }
        #endregion
    }
}