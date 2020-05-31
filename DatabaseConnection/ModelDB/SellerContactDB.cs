using System.ComponentModel.DataAnnotations;

namespace Models
{
    public class SellerContactDB
    {
        [Key]
        public int ID { set; get; }
        public string Email { get; set; }

        public string Telephone { get; set; }

        /// <summary>
        /// Imię i nazwisko sprzedwcy
        /// </summary>
        public string Name { get; set; }
    }
}