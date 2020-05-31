using Interfaces;
using Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Utilities;
using System.Data.Entity;
using DatabaseConnection;

namespace SampleApp
{
    class Program
    {
        static void Main()
        {
            var entriesComparersTypes = GetTypesThatImplementsInterface(typeof(IEqualityComparer<Entry>));
            var firstComparer = Activator.CreateInstance(entriesComparersTypes.First());
            
            //Aktualnie dumpy będą zapisywane w plikach
            IDumpsRepository dumpsRepository = new DumpFileRepository();

            var webSiteIntegrationsTypes = GetTypesThatImplementsInterface(typeof(IWebSiteIntegration));

            foreach (var webSiteIntegrationType in webSiteIntegrationsTypes)
            {
                //Poniższa linijka kodu z pomocą refleksji tworzy instancje konkretnej integracji
                var webSiteIngegration = (IWebSiteIntegration)Activator.CreateInstance(
                    webSiteIntegrationType,
                    dumpsRepository,
                    firstComparer);

                //Pobierz wszystkie dane dumpów, jednak bez konkretnych ofert by nie zaśmiecić pamięci
                var oldDumpsDetails = webSiteIngegration.DumpsRepository.GetAllDumpDetails(webSiteIngegration.WebPage);

                var oldDumpsDetailsdas = webSiteIngegration.DumpsRepository.GetAllDumpDetails(webSiteIngegration.WebPage);
                //Tu następuje wykonywanie zrzutu ze strony internetowej
                //var newDump = webSiteIngegration.GenerateDump();

               // var newDump = webSiteIngegration.GenerateDump();
                foreach (var oldDumpDetails in oldDumpsDetails)
                {
                    //Załaduj całego dumpa z pamięci wraz z ofertami
                    var oldDump = webSiteIngegration.DumpsRepository.GetDump(oldDumpDetails);
                    List<EntryDB> EntryListDB = new List<EntryDB>();

                    foreach (Entry e in oldDump.Entries)
                    {
                        EntryDB entryDB = new EntryDB();
                        entryDB.OfferDetails = new OfferDetailsDB();
                        entryDB.OfferDetails.CreationDateTime = DateTime.Now;
                        entryDB.OfferDetails.IsStillValid = e.OfferDetails.IsStillValid;
                        entryDB.OfferDetails.LastUpdateDateTime = e.OfferDetails.LastUpdateDateTime;
                        entryDB.OfferDetails.OfferKind = e.OfferDetails.OfferKind;
                        entryDB.OfferDetails.SellerContact = new SellerContactDB();
                        entryDB.OfferDetails.SellerContact.Email = e.OfferDetails.SellerContact.Email;
                        entryDB.OfferDetails.SellerContact.Name = e.OfferDetails.SellerContact.Name;
                        entryDB.OfferDetails.SellerContact.Telephone = e.OfferDetails.SellerContact.Telephone;
                        entryDB.OfferDetails.Url = e.OfferDetails.Url;
                        entryDB.PropertyAddress = new PropertyAddressDB();
                        entryDB.PropertyAddress.City = e.PropertyAddress.City;
                        entryDB.PropertyAddress.DetailedAddress = e.PropertyAddress.DetailedAddress;
                        entryDB.PropertyAddress.District = e.PropertyAddress.District;
                        entryDB.PropertyAddress.StreetName = e.PropertyAddress.StreetName;
                        entryDB.PropertyDetails = new PropertyDetailsDB();
                        entryDB.PropertyDetails.Area = e.PropertyDetails.Area;
                        entryDB.PropertyDetails.FloorNumber = e.PropertyDetails.FloorNumber;
                        entryDB.PropertyDetails.NumberOfRooms = e.PropertyDetails.NumberOfRooms;
                        entryDB.PropertyDetails.YearOfConstruction = e.PropertyDetails.YearOfConstruction;
                        entryDB.PropertyFeatures = new PropertyFeaturesDB();
                        entryDB.PropertyFeatures.Balconies = e.PropertyFeatures.Balconies;
                        entryDB.PropertyFeatures.BasementArea = e.PropertyFeatures.BasementArea;
                        entryDB.PropertyFeatures.GardenArea = e.PropertyFeatures.GardenArea;
                        entryDB.PropertyFeatures.IndoorParkingPlaces = e.PropertyFeatures.IndoorParkingPlaces;
                        entryDB.PropertyFeatures.OutdoorParkingPlaces = e.PropertyFeatures.OutdoorParkingPlaces;
                        entryDB.PropertyPrice = new PropertyPriceDB();
                        entryDB.PropertyPrice.PricePerMeter = e.PropertyPrice.PricePerMeter;
                        entryDB.PropertyPrice.ResidentalRent = e.PropertyPrice.ResidentalRent;
                        entryDB.PropertyPrice.TotalGrossPrice = e.PropertyPrice.TotalGrossPrice;
                        entryDB.RawDescription = e.RawDescription;
                        EntryListDB.Add(entryDB);
                    }

                    using (var context = new EntryContexst())
                    {
                        context.Entries.AddRange(EntryListDB);

                        context.SaveChanges();
                    }
                        //Znajdź wszystkie oferty w starych dumpach których nie ma w nowym dumpie...

                        //...i oznacz je jako niekatualne
                        //         closedEntry.OfferDetails.IsStillValid = false;

                        //Zapisz zmiany w dumpach do repozytorium
                        webSiteIngegration.DumpsRepository.UpdateDump(oldDump);
                }

                //Zapisz nowy dump do repozytorium
         //       webSiteIngegration.DumpsRepository.InsertDump(newDump);
            }
        }

       
        /// <summary>
        /// Poniższa instrukcja znajduje wszystkie klasy które implementują interfejs IWebSiteIntegration
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        private static IEnumerable<Type> GetTypesThatImplementsInterface(Type interfaceType)
        {
            if (!interfaceType.GetTypeInfo().IsInterface)
                throw new ArgumentException();

            return AppDomain.CurrentDomain.GetAssemblies()
                  .SelectMany(element => element.GetTypes())
                  .Where(type => interfaceType.IsAssignableFrom(type)
                  && type.GetTypeInfo().IsClass);
        }
    }
}
