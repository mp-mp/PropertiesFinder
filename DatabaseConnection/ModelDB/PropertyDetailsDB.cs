using System.ComponentModel.DataAnnotations;

namespace Models
{
    public class PropertyDetailsDB
    {
        [Key]
        public int ID { set; get; }
        /// <summary>
        /// Ilość metrów kwadratowych powierzchni mieszkalnej
        /// </summary>
        public decimal Area { get; set; }

        /// <summary>
        /// Ilość pokoi.
        /// </summary>
        public int NumberOfRooms { get; set; }

        /// <summary>
        /// Numer piętra. Nie wymagany w przypadku domów
        /// </summary>
        public int? FloorNumber { get; set; }

        /// <summary>
        /// Rok zbudowania budynku.
        /// </summary>
        public int? YearOfConstruction { get; set; }


    }
}