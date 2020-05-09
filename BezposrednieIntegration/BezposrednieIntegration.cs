using Interfaces;
using Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using Utilities;

namespace BezposrednieIntegration
{
    class BezposrednieIntegration : IWebSiteIntegration
    {
        public WebPage WebPage { get; }
        public IDumpsRepository DumpsRepository { get; }

        public IEqualityComparer<Entry> EntriesComparer { get; }
        public BezposrednieGetOffersFromMainPage mainPage;

        public BezposrednieIntegration(IDumpsRepository dumpsRepository,
            IEqualityComparer<Entry> equalityComparer)
        {
            DumpsRepository = dumpsRepository;
            EntriesComparer = equalityComparer;
            WebPage = new WebPage
            {
                Url = "http://bezposrednie.com/",
                Name = "Bezposrednie.com",
                WebPageFeatures = new WebPageFeatures
                {
                    HomeSale = true,
                    HomeRental = true,
                    HouseSale = true,
                    HouseRental = true
                }
            };
            mainPage = new BezposrednieGetOffersFromMainPage(WebPage.Url);
        }

        public Dump GenerateDump()
        {
            List<Entry> EntriesFromMainPage = new List<Entry>();

            foreach (var SingleOfferUrl in mainPage.OffersList)
            {
                BezpostednieGetOfferDetails CurrentOffer = new BezpostednieGetOfferDetails(SingleOfferUrl);
                Entry CurrentEntry = new Entry();

                OfferDetails CurrentOfferDetails = new OfferDetails();
                CurrentOfferDetails.Url = CurrentOffer.OfferDetails["Url"];
                if (CurrentOffer.OfferDetails["OfferKind"] == "RENT")
                {
                    CurrentOfferDetails.OfferKind = OfferKind.RENTAL;
                }
                else
                {
                    CurrentOfferDetails.OfferKind = OfferKind.SALE;
                }
                switch (CurrentOffer.OfferDetails["CreationDateTime"])
                {
                    case "dzisiaj":
                        CurrentOfferDetails.CreationDateTime = DateTime.Today;
                        break;
                    case "wczoraj":
                        CurrentOfferDetails.CreationDateTime = DateTime.Now.AddDays(-1);
                        break;
                    default:
                        CurrentOfferDetails.CreationDateTime = DateTime.ParseExact(CurrentOffer.OfferDetails["CreationDateTime"], "dd.MM.yyyy", System.Globalization.CultureInfo.InvariantCulture);
                        break;
                }
                try
                {
                    CurrentOfferDetails.LastUpdateDateTime = DateTime.ParseExact(CurrentOffer.OfferDetails["LastUpdateDateTime"], "yyyy-MM-dd hh:mm:ss", System.Globalization.CultureInfo.InvariantCulture);
                }
                catch 
                {
                    CurrentOfferDetails.LastUpdateDateTime = null;
                }
                SellerContact CurrentSellerContact = new SellerContact();
                CurrentSellerContact.Email = CurrentOffer.SellerContact["Email"];
                CurrentSellerContact.Name = CurrentOffer.SellerContact["Name"];
                CurrentSellerContact.Telephone = CurrentOffer.SellerContact["Telephone"];
                CurrentOfferDetails.SellerContact = CurrentSellerContact;
                CurrentOfferDetails.IsStillValid = Boolean.TryParse(CurrentOffer.OfferDetails["IsStillValid"], out bool IsStillValid);
                CurrentEntry.OfferDetails = CurrentOfferDetails;

                PropertyAddress CurrentPropertyAddress = new PropertyAddress();
                try
                {
                    CurrentPropertyAddress.City = (PolishCity)Enum.Parse(typeof(PolishCity), CurrentOffer.PropertyAddress["City"].Replace(" ", "_"));
                }
                catch
                {
                   CurrentPropertyAddress.City = PolishCity.NIEZNANE;
                }
                CurrentPropertyAddress.DetailedAddress = CurrentOffer.PropertyAddress["DetailedAddress"];
                CurrentPropertyAddress.District = CurrentOffer.PropertyAddress["District"];
                CurrentPropertyAddress.StreetName = CurrentOffer.PropertyAddress["StreetName"].TrimStart();
                CurrentEntry.PropertyAddress = CurrentPropertyAddress;

                PropertyDetails CurrentPropertyDetails = new PropertyDetails();
                CurrentPropertyDetails.Area = Convert.ToDecimal(CurrentOffer.PropertyDetails["Area"].Substring(0, CurrentOffer.PropertyDetails["Area"].IndexOf(" ")));
                CurrentPropertyDetails.NumberOfRooms = int.Parse(CurrentOffer.PropertyDetails["NumberOfRooms"]);
                switch (CurrentOffer.PropertyDetails["FloorNumber"])
                {
                    case null:
                        CurrentPropertyDetails.FloorNumber = null;
                        break;
                    case "parter":
                        CurrentPropertyDetails.FloorNumber = 0;
                        break;
                    case "Parter":
                        CurrentPropertyDetails.FloorNumber = 0;
                        break;
                    default:
                        CurrentPropertyDetails.FloorNumber = int.Parse(CurrentOffer.PropertyDetails["FloorNumber"]);
                        break;
                }
                if (CurrentOffer.PropertyDetails["YearOfConstruction"] == null) CurrentPropertyDetails.YearOfConstruction = null;
                else CurrentPropertyDetails.YearOfConstruction = int.Parse(CurrentOffer.PropertyDetails["YearOfConstruction"]);
                CurrentEntry.PropertyDetails = CurrentPropertyDetails;

                PropertyPrice CurrentPropertyPrice = new PropertyPrice();
                CurrentPropertyPrice.PricePerMeter = Convert.ToDecimal(CurrentOffer.PropertyPrice["PricePerMeter"].Substring(0, CurrentOffer.PropertyPrice["PricePerMeter"].IndexOf(" ")));
                CurrentPropertyPrice.TotalGrossPrice = Convert.ToDecimal(Regex.Replace(CurrentOffer.PropertyPrice["TotalGrossPrice"], "[^0-9]", ""));
                if (CurrentOffer.PropertyPrice["ResidentalRent"] != null) CurrentPropertyPrice.ResidentalRent = Convert.ToDecimal(CurrentOffer.PropertyPrice["ResidentalRent"].Substring(0, CurrentOffer.PropertyPrice["ResidentalRent"].IndexOf(" ")));
                else CurrentPropertyPrice.ResidentalRent = null;
                CurrentEntry.PropertyPrice = CurrentPropertyPrice;

                PropertyFeatures CurrentPropertyFeatures = new PropertyFeatures();
                if (CurrentOffer.PropertyFeatures["GardenArea"] != null)
                {
                    CurrentPropertyFeatures.GardenArea = Convert.ToDecimal(CurrentOffer.PropertyFeatures["GardenArea"].Substring(0, CurrentOffer.PropertyFeatures["GardenArea"].IndexOf(" ")));
                }
                else CurrentPropertyFeatures.GardenArea = null;
                switch (CurrentOffer.PropertyFeatures["Balconies"])
                {
                    case "Tak":
                        CurrentPropertyFeatures.Balconies = 1;
                        break;
                    case "Nie":
                        CurrentPropertyFeatures.Balconies = 0;
                        break;
                    default:
                        CurrentPropertyFeatures.Balconies = null;
                        break;
                }
                try
                {
                    CurrentPropertyFeatures.OutdoorParkingPlaces = int.Parse(CurrentOffer.PropertyFeatures["OutdoorParkingPlaces"]);
                }
                catch
                {
                    CurrentPropertyFeatures.OutdoorParkingPlaces = null;
                }
                try
                {
                    CurrentPropertyFeatures.IndoorParkingPlaces = int.Parse(CurrentOffer.PropertyFeatures["IndoorParkingPlaces"]);
                }
                catch
                {
                    CurrentPropertyFeatures.IndoorParkingPlaces = null;
                }
                if (CurrentOffer.PropertyFeatures["BasementArea"] != null)
                {
                    CurrentPropertyFeatures.BasementArea = Convert.ToDecimal(CurrentOffer.PropertyFeatures["BasementArea"]);
                }
                else
                {
                    CurrentPropertyFeatures.BasementArea = null;
                }
                CurrentEntry.PropertyFeatures = CurrentPropertyFeatures;

                CurrentEntry.RawDescription = CurrentOffer.RawDescription;

                EntriesFromMainPage.Add(CurrentEntry);
            }

            Dump currentDump = new Dump {DateTime=DateTime.Now, WebPage=WebPage, Entries = new List<Entry>(EntriesFromMainPage) };

            return currentDump;
        }
    }
}
