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
                    string komanda = "INSERT INTO PUSGS.dbo.Korisnik(KorisnickoIme,Email,Lozinka,Ime,Prezime,DatumRodjenja,Adresa,TipKorisnika,Slika) VALUES (@KorisnickoIme,@Email,@Lozinka,@Ime,@Prezime,@DatumRodjenja,@Adresa,@TipKorisnika,@Slika)";

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
                    string komanda = "UPDATE PUSGS.dbo.Korisnik SET KorisnickoIme=@KorisnickoIme , Email=@Email , Lozinka=@Lozinka , Ime=@Ime , Prezime=@Prezime , DatumRodjenja=@DatumRodjenja , Adresa=@Adresa , TipKorisnika=@TipKorisnika , Slika=@Slika WHERE Email=@stariEmail";

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
    }
}