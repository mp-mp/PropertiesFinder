using System;
using System.ComponentModel.DataAnnotations;

namespace Models
{
    public class EntryDB
    {
        public int ID { get; set; }
        public virtual OfferDetailsDB OfferDetails { get; set; }

        public virtual PropertyPriceDB PropertyPrice { get; set; }
    
        public virtual PropertyDetailsDB PropertyDetails { get; set; }

        public virtual PropertyAddressDB PropertyAddress { get; set; }

        public virtual PropertyFeaturesDB PropertyFeatures { get; set; }

        /// <summary>
        /// Nieprzetworzony tekst z ogłoszenia
        /// </summary>
        public string RawDescription { get; set; }
    }
}