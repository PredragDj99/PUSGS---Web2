using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;

namespace PUSGS.Models
{
    public class Baza
    {
        static string myCon = ConfigurationManager.ConnectionStrings["Myconnection"].ConnectionString;

        #region Dodaj korisnika -> Registruj
        public static bool DodajKorisnika(Korisnik korisnik)
        {
            using (SqlConnection connection = new SqlConnection(myCon))
            {
                try
                {
                    string komanda = "INSERT INTO PUSGS.dbo.Korisnik(KorisnickoIme,Email,Lozinka,Ime,Prezime,DatumRodjenja,Adresa,TipKorisnika,Slika,Verifikovan) VALUES (@KorisnickoIme,@Email,@Lozinka,@Ime,@Prezime,@DatumRodjenja,@Adresa,@TipKorisnika,@Slika,@Verifikovan)";

                    SqlCommand cmd = new SqlCommand(komanda, connection);

                    cmd.Parameters.AddWithValue("@KorisnickoIme", korisnik.KorisnickoIme);
                    cmd.Parameters.AddWithValue("@Email", korisnik.Email);
                    cmd.Parameters.AddWithValue("@Lozinka", korisnik.Lozinka);
                    cmd.Parameters.AddWithValue("@Ime", korisnik.Ime);
                    cmd.Parameters.AddWithValue("@Prezime", korisnik.Prezime);
                    cmd.Parameters.AddWithValue("@DatumRodjenja", korisnik.DatumRodjenja.ToString("dd/MM/yyyy"));
                    cmd.Parameters.AddWithValue("@Adresa", korisnik.Adresa);
                    cmd.Parameters.AddWithValue("@TipKorisnika", korisnik.TipKorisnika.ToString());
                    cmd.Parameters.AddWithValue("@Slika", korisnik.Slika);
                    if (korisnik.Verifikovan == null) korisnik.Verifikovan = "NULL";
                    cmd.Parameters.AddWithValue("@Verifikovan", korisnik.Verifikovan);

                    connection.Open();
                    cmd.ExecuteNonQuery();
                    connection.Close();
                    return true;
                }
                catch (Exception ex)
                {
                    if (connection.State == ConnectionState.Open)
                    {
                        connection.Close();
                    }
                    return false;
                }
            }
        }
        #endregion

        #region Prijavi se
        public static Korisnik PrijaviSe(string email, string lozinka)
        {
            Korisnik k = new Korisnik();

            using (SqlConnection connection = new SqlConnection(myCon))
            {
                try
                {
                    string komanda = "SELECT * FROM PUSGS.dbo.Korisnik WHERE Email=@Email AND Lozinka=@Lozinka";

                    SqlCommand cmd = new SqlCommand(komanda, connection);

                    cmd.Parameters.AddWithValue("@Email", email);
                    cmd.Parameters.AddWithValue("@Lozinka", lozinka);

                    connection.Open();
                    using (SqlDataReader dr = cmd.ExecuteReader())
                    {
                        while (dr.Read())
                        {
                            string id = dr[0].ToString();
                            k.KorisnickoIme = dr[1].ToString();
                            k.Email = dr[2].ToString();
                            k.Lozinka = dr[3].ToString();
                            k.Ime = dr[4].ToString();
                            k.Prezime = dr[5].ToString();
                            k.DatumRodjenja = DateTime.ParseExact(dr[6].ToString(), "dd/MM/yyyy", null);
                            k.Adresa = dr[7].ToString();
                            k.TipKorisnika = (KorisnikType)Enum.Parse(typeof(KorisnikType), dr[8].ToString());
                            k.Slika = dr[9].ToString();
                            k.Verifikovan = dr[10].ToString();
                        }
                    }
                    //vraca samo ID
                    //string vrednost = Convert.ToString(cmd.ExecuteScalar());
                    connection.Close();

                    return k;
                }
                catch (Exception ex)
                {
                    if (connection.State == ConnectionState.Open)
                    {
                        connection.Close();
                    }
                    return k;
                }
            }
        }
        #endregion

        #region Update profila
        public static Korisnik UpdateProfila(Korisnik novProfil,string stariEmail)
        {
            Korisnik k = new Korisnik();

            using (SqlConnection connection = new SqlConnection(myCon))
            {
                try
                {
                    string komanda = "UPDATE PUSGS.dbo.Korisnik SET KorisnickoIme=@KorisnickoIme , Email=@Email , Lozinka=@Lozinka , Ime=@Ime , Prezime=@Prezime , DatumRodjenja=@DatumRodjenja , Adresa=@Adresa , TipKorisnika=@TipKorisnika , Slika=@Slika , Verifikovan=@Verifikovan WHERE Email=@stariEmail";

                    SqlCommand cmd = new SqlCommand(komanda, connection);

                    cmd.Parameters.AddWithValue("@KorisnickoIme", novProfil.KorisnickoIme);
                    cmd.Parameters.AddWithValue("@Email", novProfil.Email);
                    cmd.Parameters.AddWithValue("@Lozinka", novProfil.Lozinka);
                    cmd.Parameters.AddWithValue("@Ime", novProfil.Ime);
                    cmd.Parameters.AddWithValue("@Prezime", novProfil.Prezime);
                    cmd.Parameters.AddWithValue("@DatumRodjenja", novProfil.DatumRodjenja.ToString("dd/MM/yyyy"));
                    cmd.Parameters.AddWithValue("@Adresa", novProfil.Adresa);
                    cmd.Parameters.AddWithValue("@TipKorisnika", novProfil.TipKorisnika.ToString());
                    cmd.Parameters.AddWithValue("@Slika", novProfil.Slika);
                    if (novProfil.Verifikovan == null) novProfil.Verifikovan = "NULL";
                    cmd.Parameters.AddWithValue("@Verifikovan", novProfil.Verifikovan);

                    //podaci starog profila
                    cmd.Parameters.AddWithValue("@stariEmail", stariEmail);

                    connection.Open();
                    cmd.ExecuteNonQuery();
                    connection.Close();
                    return novProfil;
                }
                catch (Exception ex)
                {
                    if (connection.State == ConnectionState.Open)
                    {
                        connection.Close();
                    }
                    return k;
                }
            }
        }
        #endregion

        #region Spisak dostavljaca
        public static List<Korisnik> VratiSveDostavljace()
        {
            List<Korisnik> dostavljaci = new List<Korisnik>();

            using (SqlConnection connection = new SqlConnection(myCon))
            {
                try
                {
                    string komanda = "SELECT * FROM PUSGS.dbo.Korisnik  WHERE TipKorisnika='DOSTAVLJAC' ";

                    SqlCommand cmd = new SqlCommand(komanda, connection);

                    connection.Open();
                    using (SqlDataReader dr = cmd.ExecuteReader())
                    {
                        while (dr.Read())
                        {
                            Korisnik k = new Korisnik();

                            string id = dr[0].ToString();
                            k.KorisnickoIme = dr[1].ToString();
                            k.Email = dr[2].ToString();
                            k.Lozinka = dr[3].ToString();
                            k.Ime = dr[4].ToString();
                            k.Prezime = dr[5].ToString();
                            k.DatumRodjenja = DateTime.ParseExact(dr[6].ToString(), "dd/MM/yyyy", null);
                            k.Adresa = dr[7].ToString();
                            k.TipKorisnika = (KorisnikType)Enum.Parse(typeof(KorisnikType), dr[8].ToString());
                            k.Slika = dr[9].ToString();
                            k.Verifikovan = dr[10].ToString();

                            dostavljaci.Add(k);
                        }
                    }
                    connection.Close();

                    return dostavljaci;
                }
                catch (Exception ex)
                {
                    if (connection.State == ConnectionState.Open)
                    {
                        connection.Close();
                    }
                    return dostavljaci;
                }
            }
        }
        #endregion

        #region Postojanje proizvoda
        public static Proizvod PostojanjeProizvoda(string imeProizvoda)
        {
            Proizvod p = new Proizvod();

            using (SqlConnection connection = new SqlConnection(myCon))
            {
                try
                {
                    string komanda = "SELECT * FROM PUSGS.dbo.Proizvod WHERE ImeProizvoda=@ImeProizvoda";

                    SqlCommand cmd = new SqlCommand(komanda, connection);

                    cmd.Parameters.AddWithValue("@ImeProizvoda", imeProizvoda);

                    connection.Open();
                    using (SqlDataReader dr = cmd.ExecuteReader())
                    {
                        while (dr.Read())
                        {
                            string id = dr[0].ToString();
                            p.ImeProizvoda = dr[1].ToString();
                            p.Cena = Double.Parse(dr[2].ToString());
                            p.Sastojci = dr[3].ToString();
                        }
                    }
                    connection.Close();

                    return p;
                }
                catch (Exception ex)
                {
                    if (connection.State == ConnectionState.Open)
                    {
                        connection.Close();
                    }
                    return p;
                }
            }
        }
        #endregion

        #region Dodaj proizvod
        public static void DodajProizvod(Proizvod proizvod)
        {
            using (SqlConnection connection = new SqlConnection(myCon))
            {
                try
                {
                    string komanda = "INSERT INTO PUSGS.dbo.Proizvod(ImeProizvoda,Cena,Sastojci) VALUES (@ImeProizvoda,@Cena,@Sastojci)";

                    SqlCommand cmd = new SqlCommand(komanda, connection);

                    cmd.Parameters.AddWithValue("@ImeProizvoda", proizvod.ImeProizvoda);
                    cmd.Parameters.AddWithValue("@Cena", proizvod.Cena.ToString());
                    cmd.Parameters.AddWithValue("@Sastojci", proizvod.Sastojci);

                    connection.Open();
                    cmd.ExecuteNonQuery();
                    connection.Close();
                }
                catch (Exception ex)
                {
                    if (connection.State == ConnectionState.Open)
                    {
                        connection.Close();
                    }
                }
            }
        }
        #endregion

        #region Prikaz proizvoda
        public static List<Proizvod> PrikazProizvoda()
        {
            List<Proizvod> spisakProizvoda = new List<Proizvod>();

            using (SqlConnection connection = new SqlConnection(myCon))
            {
                try
                {
                    string komanda = "SELECT * FROM PUSGS.dbo.Proizvod";

                    SqlCommand cmd = new SqlCommand(komanda, connection);

                    connection.Open();
                    using (SqlDataReader dr = cmd.ExecuteReader())
                    {
                        while (dr.Read())
                        {
                            Proizvod proizvod = new Proizvod();

                            string id = dr[0].ToString();
                            proizvod.ImeProizvoda = dr[1].ToString();
                            proizvod.Cena = Double.Parse(dr[2].ToString());
                            proizvod.Sastojci = dr[3].ToString();

                            spisakProizvoda.Add(proizvod);
                        }
                    }
                    connection.Close();

                    return spisakProizvoda;
                }
                catch (Exception ex)
                {
                    if (connection.State == ConnectionState.Open)
                    {
                        connection.Close();
                    }
                    return spisakProizvoda;
                }
            }
        }
        #endregion region

        #region Nova porudzbina
        public static void NovaPorudzbina(Porudzbina porudzbina, List<string> proizvod, List<int> kolicina,string email)
        {
            using (SqlConnection connection = new SqlConnection(myCon))
            {
                try
                {
                    string komanda = "INSERT INTO PUSGS.dbo.Porudzbina(StaPorucuje,Adresa,Komentar,Cena,StatusPor) VALUES (@StaPorucuje,@Adresa,@Komentar,@Cena,@StatusPor)";
                    
                    SqlCommand cmd = new SqlCommand(komanda, connection);

                    //Sifra porucenog
                    cmd.Parameters.AddWithValue("@StaPorucuje", porudzbina.StaPorucuje);
                    cmd.Parameters.AddWithValue("@Adresa", porudzbina.Adresa);
                    cmd.Parameters.AddWithValue("@Komentar", porudzbina.Komentar);
                    cmd.Parameters.AddWithValue("@Cena", porudzbina.Cena);
                    cmd.Parameters.AddWithValue("@StatusPor", porudzbina.Status);

                    connection.Open();
                    cmd.ExecuteNonQuery();
                    connection.Close();

                    int j = 0;
                    //Sada unosim u drugu tabelu proizvode i kolicinu
                    for (int i = 0; i < kolicina.Count; i++)
                    {
                        if(kolicina[i] != 0)
                        {
                            try
                            {
                                if (j == 0)
                                {
                                    int josUvekNepoznat = -1;

                                    string komanda2 = "INSERT INTO PUSGS.dbo.Poruceno(StaPorucuje,Proizvod,Kolicina,Email,DostavljacID) VALUES (@StaPorucuje,@Proizvod,@Kolicina,@Email,@DostavljacID)";
                                    SqlCommand cmd2 = new SqlCommand(komanda2, connection);
                                    cmd2.Parameters.AddWithValue("@StaPorucuje", porudzbina.StaPorucuje);
                                    cmd2.Parameters.AddWithValue("@Proizvod", proizvod[i]);
                                    cmd2.Parameters.AddWithValue("@Kolicina", kolicina[i]);
                                    cmd2.Parameters.AddWithValue("@Email", email);
                                    cmd2.Parameters.AddWithValue("@DostavljacID", josUvekNepoznat);

                                    connection.Open();
                                    cmd2.ExecuteNonQuery();
                                    connection.Close();
                                    j++;
                                }
                                else
                                {
                                    string komanda2 = "UPDATE PUSGS.dbo.Poruceno SET Proizvod=Proizvod+@Proizvod,Kolicina=Kolicina+@Kolicina WHERE StaPorucuje=@StaPorucuje;";
                                    SqlCommand cmd2 = new SqlCommand(komanda2, connection);
                                    cmd2.Parameters.AddWithValue("@StaPorucuje", porudzbina.StaPorucuje);
                                    cmd2.Parameters.AddWithValue("@Proizvod", ","+proizvod[i]);
                                    cmd2.Parameters.AddWithValue("@Kolicina", ","+kolicina[i]);

                                    connection.Open();
                                    cmd2.ExecuteNonQuery();
                                    connection.Close();
                                }
                            }
                            catch (Exception e)
                            {
                                if(connection.State == ConnectionState.Open)
                                {
                                    connection.Close();
                                }
                                break;
                            }
                        }
                    }

                }
                catch (Exception ex)
                {
                    if (connection.State == ConnectionState.Open)
                    {
                        connection.Close();
                    }
                }
            }
        }
        #endregion

        #region Sve porudzbine
        public static List<SpojeneTabele> PrikazPorudzbina()
        {
            List<SpojeneTabele> porudzbine = new List<SpojeneTabele>();

            using (SqlConnection connection = new SqlConnection(myCon))
            {
                try
                {
                    string komanda = "SELECT Porudzbina.StaPorucuje,Proizvod,Kolicina,Email,Adresa,Komentar,Cena,StatusPor FROM PUSGS.dbo.Porudzbina JOIN PUSGS.dbo.Poruceno ON Poruceno.StaPorucuje=Porudzbina.StaPorucuje";

                    SqlCommand cmd = new SqlCommand(komanda, connection);

                    connection.Open();
                    using (SqlDataReader dr = cmd.ExecuteReader())
                    {
                        while (dr.Read())
                        {
                            SpojeneTabele p = new SpojeneTabele();

                            //string id = dr[0].ToString();
                            p.StaPorucuje = dr[0].ToString();
                            p.Proizvod = dr[1].ToString();
                            p.Kolicina = dr[2].ToString();
                            p.Email = dr[3].ToString();
                            p.Adresa = dr[4].ToString();
                            p.Komentar = dr[5].ToString();
                            p.Cena = Double.Parse(dr[6].ToString());
                            p.StatusPor = dr[7].ToString();

                            porudzbine.Add(p);
                        }
                    }
                    connection.Close();

                    return porudzbine;
                }
                catch (Exception ex)
                {
                    if (connection.State == ConnectionState.Open)
                    {
                        connection.Close();
                    }
                    return porudzbine;
                }
            }
        }
        #endregion

        #region Dostavljac prihvatio porudzbinu
        public static void DostavljacPrihvatioPorudzbinu(SpojeneTabele porudzbina, Korisnik korisnik)
        {
            using (SqlConnection connection = new SqlConnection(myCon))
            {

                try
                {
                    string komandaTrazi = "SELECT ID FROM PUSGS.dbo.Korisnik WHERE Email=@Email";

                    SqlCommand cmdTr = new SqlCommand(komandaTrazi, connection);

                    cmdTr.Parameters.AddWithValue("@Email", korisnik.Email);

                    connection.Open();
                    int dostavljacID=0;
                    using (SqlDataReader dr = cmdTr.ExecuteReader())
                    {
                        while (dr.Read())
                        {
                            dostavljacID = Int32.Parse(dr[0].ToString());
                        }
                    }
                    connection.Close();

                    try
                    {
                        string komanda = "UPDATE PUSGS.dbo.Porudzbina SET StatusPor=@StatusPor WHERE StaPorucuje=@StaPorucuje";

                        SqlCommand cmd = new SqlCommand(komanda, connection);

                        //Sifra porucenog
                        cmd.Parameters.AddWithValue("@StaPorucuje", porudzbina.StaPorucuje);
                        cmd.Parameters.AddWithValue("@StatusPor", porudzbina.StatusPor);

                        connection.Open();
                        cmd.ExecuteNonQuery();
                        connection.Close();

                        // -----------------------------------------

                        string komanda2 = "UPDATE PUSGS.dbo.Poruceno SET DostavljacID = @DostavljacID WHERE StaPorucuje=@StaPorucuje";
                        SqlCommand cmd2 = new SqlCommand(komanda2, connection);

                        //Sifra porucenog
                        cmd2.Parameters.AddWithValue("@StaPorucuje", porudzbina.StaPorucuje);
                        cmd2.Parameters.AddWithValue("@DostavljacID", dostavljacID);

                        connection.Open();
                        cmd2.ExecuteNonQuery();
                        connection.Close();
                    }
                    catch (Exception ex)
                    {
                        if (connection.State == ConnectionState.Open)
                        {
                            connection.Close();
                        }
                    }
                }
                catch (Exception ex)
                {
                    if (connection.State == ConnectionState.Open)
                    {
                        connection.Close();
                    }
                }
            }
        }
        #endregion

        #region Dostave dostavljaca
        public static List<SpojeneTabele> PrikaziDostaveDostavljaca(Korisnik korisnik)
        {
            List<SpojeneTabele> porudzbine = new List<SpojeneTabele>();

            using (SqlConnection connection = new SqlConnection(myCon))
            {
                try
                {
                    string komandaTrazi = "SELECT ID FROM PUSGS.dbo.Korisnik WHERE Email=@Email";

                    SqlCommand cmdTr = new SqlCommand(komandaTrazi, connection);

                    cmdTr.Parameters.AddWithValue("@Email", korisnik.Email);

                    connection.Open();
                    int dostavljacID = 0;
                    using (SqlDataReader dr = cmdTr.ExecuteReader())
                    {
                        while (dr.Read())
                        {
                            dostavljacID = Int32.Parse(dr[0].ToString());
                        }
                    }
                    connection.Close();

                    try
                    {
                        //Onda spojim po nazivuPorudzbine i pokupim sve potrebne podatke
                        string komanda = "SELECT Poruceno.StaPorucuje,Poruceno.Proizvod,Poruceno.Kolicina,Porudzbina.Adresa,Porudzbina.Komentar,Porudzbina.Cena,Porudzbina.StatusPor,Poruceno.Email,Poruceno.DostavljacID FROM PUSGS.dbo.Poruceno JOIN PUSGS.dbo.Porudzbina ON Poruceno.StaPorucuje=Porudzbina.StaPorucuje WHERE Poruceno.DostavljacID=@DostavljacID";

                        SqlCommand cmd = new SqlCommand(komanda, connection);

                        //Sifra porucenog
                        cmd.Parameters.AddWithValue("@DostavljacID", dostavljacID);

                        connection.Open();
                        using (SqlDataReader dr = cmd.ExecuteReader())
                        {
                            while (dr.Read())
                            {
                                SpojeneTabele proizvod = new SpojeneTabele();

                                proizvod.StaPorucuje = dr[0].ToString();
                                proizvod.Proizvod = dr[1].ToString();
                                proizvod.Kolicina = dr[2].ToString();
                                proizvod.Adresa = dr[3].ToString();
                                proizvod.Komentar = dr[4].ToString();
                                proizvod.Cena = Double.Parse(dr[5].ToString());
                                proizvod.StatusPor = dr[6].ToString();
                                proizvod.Email = dr[7].ToString();
                                proizvod.DostavljacID = Int32.Parse(dr[8].ToString());

                                porudzbine.Add(proizvod);
                            }
                        }
                        connection.Close();

                        return porudzbine;
                    }
                    catch (Exception ex)
                    {
                        if (connection.State == ConnectionState.Open)
                        {
                            connection.Close();
                        }
                        return porudzbine;
                    }
                }
                catch (Exception ex)
                {
                    if (connection.State == ConnectionState.Open)
                    {
                        connection.Close();
                    }
                    return porudzbine;
                }
            }
        }
        #endregion

        #region Dodaj vreme tajmera
        public static void DodajVremeStoperica(DateTime vremePokretanjaStoperice,string sifraPorudzbine)
        {
            using (SqlConnection connection = new SqlConnection(myCon))
            {
                try
                {
                    string komanda = "INSERT INTO PUSGS.dbo.Stoperica(VremePorucivanja,SifraPorudzbine) VALUES (@VremePorucivanja,@SifraPorudzbine)";

                    SqlCommand cmd = new SqlCommand(komanda, connection);

                    cmd.Parameters.AddWithValue("@VremePorucivanja", vremePokretanjaStoperice.ToString());
                    cmd.Parameters.AddWithValue("@SifraPorudzbine", sifraPorudzbine);

                    connection.Open();
                    cmd.ExecuteNonQuery();
                    connection.Close();
                }
                catch (Exception ex)
                {
                    if (connection.State == ConnectionState.Open)
                    {
                        connection.Close();
                    }
                }
            }
        }
        #endregion

        #region Provera postoji li vec uneto vreme trajanja dostave
        public static string ProcitajSveStoperice(string sifraPorudzbine)
        {
            string vratiVreme="";

            using (SqlConnection connection = new SqlConnection(myCon))
            {
                try
                {
                    string komanda = "SELECT VremePorucivanja FROM PUSGS.dbo.Stoperica WHERE SifraPorudzbine=@SifraPorudzbine";

                    SqlCommand cmd = new SqlCommand(komanda, connection);

                    cmd.Parameters.AddWithValue("@SifraPorudzbine", sifraPorudzbine);

                    connection.Open();
                    using (SqlDataReader dr = cmd.ExecuteReader())
                    {
                        while (dr.Read())
                        {
                            vratiVreme = dr[0].ToString();
                        }
                    }
                    connection.Close();
                    return vratiVreme;
                }
                catch (Exception ex)
                {
                    if (connection.State == ConnectionState.Open)
                    {
                        connection.Close();
                    }
                    return vratiVreme;
                }
            }
        }
        #endregion
    }
}