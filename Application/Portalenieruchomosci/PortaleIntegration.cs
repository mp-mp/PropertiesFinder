using Interfaces;
using Models;
using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Net.Http;
using System.Threading;
using System.Linq;
using System.Data.SqlTypes;
using System.Text;
using System.Text.Json;
using System.ComponentModel;
using System.Net;
using Newtonsoft.Json;

namespace Application.Portalenieruchomosci
{
    class PortaleIntegration : IWebSiteIntegration
    {
        public WebPage WebPage { get; }
        public IDumpsRepository DumpsRepository { get; }

        public IEqualityComparer<Entry> EntriesComparer { get; }

        public PortaleIntegration(IDumpsRepository dumpsRepository,
            IEqualityComparer<Entry> equalityComparer)
        {

            DumpsRepository = dumpsRepository;
            EntriesComparer = equalityComparer;
            WebPage = new WebPage
            {
                Url = "http://portalenieruchomosci.pl",
                Name = "Portalenieruchomosci WebSite Integration",
                WebPageFeatures = new WebPageFeatures
                {
                    HomeSale = true,
                    HomeRental = false,
                    HouseSale = false,
                    HouseRental = false
                }
            };

            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
        }

        private PolishCity? cityNameToEnum(string cityName)
        {
            byte[] tempBytes = System.Text.Encoding.GetEncoding("ISO-8859-8").GetBytes(cityName);
            string asciiStr = System.Text.Encoding.UTF8.GetString(tempBytes).ToUpper().Replace(' ', '_');

            object? ret;

            if (Enum.TryParse(typeof(PolishCity), asciiStr, out ret))
                return (PolishCity)ret;

            return null;
        }

        public Dump GenerateDump()
        {
            var entries = new List<Entry>();

            var base_uri = new Uri(this.WebPage.Url);
            var database_uri = new Uri(base_uri, "offer/json/list/Apartment");

            Newtonsoft.Json.Linq.JObject json;
            using (WebClient wc = new WebClient())
            {
                var json_string = wc.DownloadString(database_uri);

                json = (Newtonsoft.Json.Linq.JObject)JsonConvert.DeserializeObject(json_string);
            }

            foreach (var offer in json["list"])
            {
                var entry = new Entry();

                entry.RawDescription = offer["listing_description"].ToString();

                var fields = new Dictionary<string, string>();
                foreach (var field in offer["fields_filled"])
                {
                    fields[field["asari_field_name"].ToString()] = field["asari_field_value"].ToString();
                }

                entry.OfferDetails = new OfferDetails
                {
                    Url = (new Uri(base_uri, $"offer/show/{offer["id"]}/{offer["slug"]}")).ToString(),
                    OfferKind = OfferKind.SALE,
                    CreationDateTime = DateTime.Parse(offer["created_at"].ToString()),
                    LastUpdateDateTime = DateTime.Parse(offer["updated_at"].ToString()),
                    SellerContact = new SellerContact
                    {
                        Name = $"{offer["contact_name"]} {offer["contact_surname"]}",
                        Telephone = offer["contact_phone"].ToString(),
                        Email = offer["contact_email"].ToString(),
                    },
                    IsStillValid = true
                };

                entry.PropertyPrice = new PropertyPrice
                {
                    TotalGrossPrice = Decimal.Parse(fields["price"].Replace('.', ',')),
                    PricePerMeter = Decimal.Parse(fields["price"].Replace('.', ',')) / Decimal.Parse(fields["totalArea"].Replace('.', ','))
                };

                entry.PropertyDetails = new PropertyDetails
                {
                    Area = Decimal.Parse(fields["totalArea"].Replace('.', ',')),
                    NumberOfRooms = int.Parse(fields["noOfRooms"]),
                    FloorNumber = int.Parse(fields["floorNo"]),
                    YearOfConstruction = int.Parse(fields["yearBuilt"])
                };

                PropertyAddress adr = new PropertyAddress();
                entry.PropertyAddress = adr;


                var address_string = offer["city_name"].ToString();
                string[] general_address = address_string.Split('/', StringSplitOptions.RemoveEmptyEntries);

                /*
                 * Format:
                 * 2 elements: /voivodeship/city
                 * 3 elements: /voivodeship/city/district
                 * 3 elements: /voivodeship/county/city
                 * 4 elements: /voivodeship/county/community/village
                 */

                var city_value = cityNameToEnum(general_address[1]);
                if (city_value != null)
                {
                    adr.City = (PolishCity)city_value;

                    if (general_address.Length == 3)
                        adr.District = general_address[2];
                }
                else if (general_address.Length == 3)
                {
                    city_value = cityNameToEnum(general_address[2]);
                    if (city_value != null)
                    {
                        adr.City = (PolishCity)city_value;
                    }
                }

                var street_name = offer["street_name"];
                var building_number = offer["house_number"];

                if (street_name != null)
                    adr.StreetName = street_name.ToString();
                if (building_number != null)
                    adr.DetailedAddress = building_number.ToString();
                entry.PropertyFeatures = new PropertyFeatures();
                // No basement area provided, only boolean
                if (!fields.ContainsKey("cellar"))
                {
                    entry.PropertyFeatures.BasementArea = null;
                }
                else if (fields["cellar"] == "true")
                {
                    entry.PropertyFeatures.BasementArea = 1;
                }
                else
                {
                    entry.PropertyFeatures.BasementArea = 0;
                }

                entries.Add(entry);
            }


            return new Dump
            {
                DateTime = DateTime.Now,
                WebPage = WebPage,
                Entries = entries
            };
        }
    }
}

