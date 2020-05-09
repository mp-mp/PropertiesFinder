using HtmlAgilityPack;
using Interfaces;
using Models;
using System;
using System.Collections.Generic;

namespace ZnajdzTo
{
    public class ZnajdzToIntegration : IWebSiteIntegration
    {
        public WebPage WebPage { get; }

        public IDumpsRepository DumpsRepository { get; }

        public IEqualityComparer<Entry> EntriesComparer { get; }

        public ZnajdzToIntegration(IDumpsRepository dumpsRepository,
            IEqualityComparer<Entry> equalityComparer)
        {
            DumpsRepository = dumpsRepository;
            EntriesComparer = equalityComparer;
            WebPage = new WebPage
            {
                Url = "https://nieruchomosci.znajdzto.pl",
                Name = "ZnajdzTo Integration",
                WebPageFeatures = new WebPageFeatures
                {
                    HomeSale = true,
                    HomeRental = false,
                    HouseSale = false,
                    HouseRental = false
                }
            };
        }

        public Dump GenerateDump()
        {
            List<Entry> entries = TakeHomeSalesEntries();

            return new Dump
            {
                DateTime = DateTime.Now,
                WebPage = WebPage,
                Entries = entries,
            };
        }

        private List<Entry> TakeHomeSalesEntries()
        {
            int page = 1;
            List<Entry> homeSales = new List<Entry>();

            while(true)
            {
                List<Entry> homeSalesEntriesFromPage = TakeHomeSalesEntriesFromHomeSalesListPage(page);

                // ogłoszeń dotyczących sprzedaży mieszkań jest ponad 242 tysiące, czyli ponad 24 tysięcy stron
                // aby program nie wykonywał się za długo jest limit 10 stron, czyli w sumie 100 ogłoszeń
                int pagesLimit = 10;
                if (homeSalesEntriesFromPage.Count > 0 && page <= pagesLimit)
                {
                    homeSales.AddRange(homeSalesEntriesFromPage);
                    page++;
                }
                else {
                    break;
                }
            }

            return homeSales;
        }

        private List<Entry> TakeHomeSalesEntriesFromHomeSalesListPage(int page) {
            try
            {
                string pageUrl = GeneratePageUrl(page);
                HtmlWeb htmlWeb = new HtmlWeb();

                HtmlDocument homeSalesListHtmlDoc = htmlWeb.Load(pageUrl);
                HtmlNodeCollection homeSalePageLinkNodes = TakeHomeSaleInfoLinkNodesFromHtmlDoc(homeSalesListHtmlDoc);

                List<Entry> homeSales = new List<Entry>();

                foreach (HtmlNode homeSalePageLinkNode in homeSalePageLinkNodes) {
                    string homeSalePageUrl = GenerateHyperlinkNodeHref(homeSalePageLinkNode);
                    HtmlDocument homeSaleHtmlDoc = htmlWeb.Load(homeSalePageUrl);
                    ZnajdzToHomeSalePage homeSalePage = new ZnajdzToHomeSalePage(homeSaleHtmlDoc, homeSalePageUrl);

                    if (homeSalePage.IsCity)
                    {
                        Entry homeSale = TakeHomeSaleEntryFromPage(homeSalePage);
                        homeSales.Add(homeSale);
                    }
                }

                return homeSales;
            }
            catch
            {
                return new List<Entry>();
            }
        }

        private string GeneratePageUrl(int page) { 
            return $"{WebPage.Url}/mieszkania/?page={page}";
        }

        private HtmlNodeCollection TakeHomeSaleInfoLinkNodesFromHtmlDoc(HtmlDocument htmlDoc) {
            HtmlNode homeSalesListNode = htmlDoc.DocumentNode.SelectSingleNode("//dl[@class='searchList']");
            return homeSalesListNode.SelectNodes("//div[@class='rightside']/h3/a");
        }

        private string GenerateHyperlinkNodeHref(HtmlNode hyperlinkNodeHref) {
            return $"{WebPage.Url}{hyperlinkNodeHref.Attributes["href"].Value}";
        }

        private Entry TakeHomeSaleEntryFromPage(ZnajdzToHomeSalePage homeSalePage)
        {

            Entry entry = new Entry
            {
                OfferDetails = TakeOfferDetailsFromPage(homeSalePage),
                PropertyAddress = TakePropertyAddressFromPage(homeSalePage),
                PropertyDetails = TakePropertyDetailsFromPage(homeSalePage),
                PropertyPrice = TakePropertyPriceFromPage(homeSalePage),
                PropertyFeatures = TakePropertyFeaturesFromPage(homeSalePage),
                RawDescription = homeSalePage.RawDescription
            };

            return entry;
        }

        private OfferDetails TakeOfferDetailsFromPage(ZnajdzToHomeSalePage homeSalePage) {
            return new OfferDetails
            {
                Url = homeSalePage.Url,
                CreationDateTime = homeSalePage.CreationDateTime,
                OfferKind = homeSalePage.OfferKind,
                SellerContact = new SellerContact
                {
                    Email = homeSalePage.Email,
                    Name = homeSalePage.Name,
                    Telephone = homeSalePage.Telephone
                },
                IsStillValid = homeSalePage.IsStillValid
            };
        }

        private PropertyAddress TakePropertyAddressFromPage(ZnajdzToHomeSalePage homeSalePage) {
            return new PropertyAddress
            {
                City = homeSalePage.City,
                StreetName = homeSalePage.StreetName,
                DetailedAddress = homeSalePage.DetailedAddress,
                District = homeSalePage.District
            };
        }

        private PropertyDetails TakePropertyDetailsFromPage(ZnajdzToHomeSalePage homeSalePage) {
            return new PropertyDetails
            {
                Area = homeSalePage.Area,
                FloorNumber = homeSalePage.FloorNumber,
                NumberOfRooms = homeSalePage.NumberOfRooms,
                YearOfConstruction = homeSalePage.YearOfConstruction
            };
        }

        private PropertyPrice TakePropertyPriceFromPage(ZnajdzToHomeSalePage homeSalePage) {
            return new PropertyPrice
            {
                PricePerMeter = homeSalePage.PricePerMeter,
                TotalGrossPrice = homeSalePage.TotalGrossPrice,
                ResidentalRent = homeSalePage.ResidentalRent
            };
        }

        private PropertyFeatures TakePropertyFeaturesFromPage(ZnajdzToHomeSalePage homeSalePage)
        {
            return new PropertyFeatures
            {
                Balconies = homeSalePage.Balconies,
                BasementArea = homeSalePage.BasementArea,
                GardenArea = homeSalePage.GardenArea,
                IndoorParkingPlaces = homeSalePage.IndoorParkingPlaces,
                OutdoorParkingPlaces = homeSalePage.OutdoorParkingPlaces
            };
        }
    }
}
