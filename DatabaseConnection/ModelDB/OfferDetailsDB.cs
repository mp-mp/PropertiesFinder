using System;
using System.ComponentModel.DataAnnotations;

namespace Models
{
    public class OfferDetailsDB
    {
        [Key]
        public int ID { set; get; }
        /// <summary>
        /// Adres do konkretnej oferty
        /// </summary>
        public string Url { get; set; }

        /// <summary>
        /// Data stworzenia oferty
        /// </summary>
        public DateTime CreationDateTime { get; set; }

        /// <summary>
        /// Data ostatniej aktualizacji oferty
        /// </summary>
        public DateTime? LastUpdateDateTime { get; set; }

        /// <summary>
        /// Rodzaj oferty - wynajem czy sprzedaż
        /// </summary>
        public OfferKind OfferKind { get; set; }

        /// <summary>
        /// Kontakt do sprzedawcy. Którekolwiek z property wewnątrz obiektu musi zostać wypełnione
        /// </summary>
        public virtual SellerContactDB SellerContact { get; set; }

        /// <summary>
        /// Czy oferta jest aktualna
        /// </summary>
        public bool IsStillValid { get; set; }
    }
}