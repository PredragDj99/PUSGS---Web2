using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PUSGS.Models
{
    public class DostavljaciZaVerifikaciju
    {
        public static void VerifikujPrihvati(string email)
        {
            List<Korisnik> spisakDostavljaca = Baza.VratiSveDostavljace();

            foreach (var item in spisakDostavljaca)
            {
                if (item.Email == email)
                {
                    item.Verifikovan = "Prihvacen";
                    Baza.UpdateProfila(item,item.Email);
                }
            }
        }

        public static void VerifikujOdbij(string email)
        {
            List<Korisnik> spisakDostavljaca = Baza.VratiSveDostavljace();

            foreach (var item in spisakDostavljaca)
            {
                if (item.Email == email)
                {
                    item.Verifikovan = "Odbijen";
                    Baza.UpdateProfila(item, item.Email);
                }
            }
        }
    }
}