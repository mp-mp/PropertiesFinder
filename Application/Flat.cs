using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application
{
    public class Flat
    {
        public string link { get; set; }
        public int cena { get; set; }
        public int cena_m2 { get; set; }
        public string wojewodztwo { get; set; }
        public string powiat { get; set; }
        public string gmina { get; set; }
        public string kodPocztowy { get; set; }
        public string dzielnicaWies { get; set; }
        public string ulica { get; set; }
        public string rynek { get; set; }//pierwotny / wtórny
        public int pietro { get; set; }
        public int iloscPieter { get; set; }
        public string rodzajBudynku { get; set; }
        public int rokBudowy { get; set; }
        public int liczbaPokoi { get; set; }
        public int powierzchnia { get; set; }
        public string stanMieszkania { get; set; }
        public string okna { get; set; }
        public string instalacje { get; set; }
        public string ogrzewanie { get; set; }
        public string media { get; set; }
        public string pomieszczeniaDodatkowe { get; set; }
        public string okolica { get; set; }
        public int wysokoscCzynszu { get; set; }
        public string opis { get; set; }
        public string telefon { get; set; }
        public string pageData { get; set; }
    }
}
