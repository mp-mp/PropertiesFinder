using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Utilities;

namespace Application.BezposrednieIntegration
{
    class BezpostednieGetOfferDetails
    {
        public bool Flat { get; set; }
        public string RawDescription { get; set; }
        public Dictionary<string, string> OfferDetails;
        public Dictionary<string, string> SellerContact;
        public Dictionary<string, string> PropertyPrice;
        public Dictionary<string, string> PropertyDetails;
        public Dictionary<string, string> PropertyAddress;
        public Dictionary<string, string> PropertyFeatures;

        public BezpostednieGetOfferDetails(string PageUrl)
        {
            OfferDetails = new Dictionary<string, string>();
            SellerContact = new Dictionary<string, string>();
            PropertyPrice = new Dictionary<string, string>();
            PropertyDetails = new Dictionary<string, string>();
            PropertyAddress = new Dictionary<string, string>();
            PropertyFeatures = new Dictionary<string, string>();
            string OfferPage = new Utilities.PageToString(PageUrl).GetHtml();
            OfferPage = OfferPage.Substring(OfferPage.IndexOf("property-detail-container-left") - 9, OfferPage.IndexOf("property-detail-container-right") - OfferPage.IndexOf("property-detail-container-left"));
            FillRawDescription(OfferPage);
            FillOfferDetails(OfferPage, PageUrl);
            FillSellerContact(OfferPage);
            FillPropertyPrice(OfferPage);
            FillPropertyDetails(OfferPage);
            FillPropertyAddress(OfferPage);
            FillPropertyFeatures(OfferPage);
        }

        private void FillOfferDetails(string OfferPage, string PageUrl)
        {
            OfferDetails.Add("Url", PageUrl);
            string CreationDateTime = OfferPage.Substring(OfferPage.IndexOf("type-added-at"), OfferPage.IndexOf("like-button-set-property") - OfferPage.IndexOf("type-added-at"));
            try
            {
                CreationDateTime = CreationDateTime.Substring(CreationDateTime.IndexOf("<strong>") + 8, CreationDateTime.IndexOf("</strong>") - CreationDateTime.IndexOf("<strong>") - 8);
            }
            catch
            {
                CreationDateTime = CreationDateTime.Substring(CreationDateTime.IndexOf("dodano:"), CreationDateTime.IndexOf("</div") - CreationDateTime.IndexOf("dodano:"));
                CreationDateTime = CreationDateTime.Substring(CreationDateTime.IndexOf(">") + 1);
            }
            if (CreationDateTime.Contains(" ")) CreationDateTime = CreationDateTime.Replace(" ", "");
            OfferDetails.Add("CreationDateTime", CreationDateTime);
            string LastUpdateDateTime;
            if (OfferPage.Contains("Data aktualizacji"))
            {
                LastUpdateDateTime = OfferPage.Substring(OfferPage.IndexOf("Data aktualizacji"));
            }
            else
            {
                LastUpdateDateTime = OfferPage.Substring(OfferPage.IndexOf("Data dodania"));
            }
            LastUpdateDateTime = LastUpdateDateTime.Substring(LastUpdateDateTime.IndexOf("<dd"), LastUpdateDateTime.IndexOf("</dd") - LastUpdateDateTime.IndexOf("<dd"));
            LastUpdateDateTime = LastUpdateDateTime.Substring(LastUpdateDateTime.IndexOf("span") + 5, LastUpdateDateTime.IndexOf("/span") - LastUpdateDateTime.IndexOf("span") - 6);
            OfferDetails.Add("LastUpdateDateTime", LastUpdateDateTime);
            string OfferKind = OfferPage.Substring(OfferPage.IndexOf("type-added-at"), OfferPage.IndexOf("like-button-set-property") - OfferPage.IndexOf("type-added-at"));
            if (OfferKind.Contains("Dom")) Flat = false;
            else if (OfferKind.Contains("Mieszkanie")) Flat = true;
            if (OfferKind.Contains("wynajęcia")) OfferKind = "RENT";
            else if (OfferKind.Contains("sprzeda")) OfferKind = "SELL";
            OfferDetails.Add("OfferKind", OfferKind);
            OfferDetails.Add("IsStillValid", "true");
        }

        private void FillSellerContact(string OfferPage)
        {
            SellerContact.Add("Email", null);
            string Telephone = OfferPage.Substring(OfferPage.IndexOf("phone-info"));
            Telephone = Telephone.Substring(Telephone.IndexOf(":\n") + 3, Telephone.IndexOf("</div>") - Telephone.IndexOf(":\n") - 3);
            try
            {
                Telephone = Telephone.Replace(" ", "");
            }
            catch { }
            SellerContact.Add("Telephone", Telephone);
            string Name = OfferPage.Substring(OfferPage.IndexOf("offerer-name"));
            Name = Name.Substring(Name.IndexOf(">") + 1, Name.IndexOf("</") - Name.IndexOf(">") - 1);
            SellerContact.Add("Name", Name);
        }

        private void FillPropertyPrice(string OfferPage)
        {
            string ResidentalRent;
            try
            {
                ResidentalRent = OfferPage.Substring(OfferPage.IndexOf("Czynsz dla adm."));
                ResidentalRent = ResidentalRent.Substring(ResidentalRent.IndexOf("dd"), ResidentalRent.IndexOf("/dd") - ResidentalRent.IndexOf("dd"));
                ResidentalRent = ResidentalRent.Substring(ResidentalRent.IndexOf("span") + 5, ResidentalRent.IndexOf("/span") - ResidentalRent.IndexOf("span") - 6);
            }
            catch
            {
                ResidentalRent = null;
            }
            PropertyPrice.Add("ResidentalRent", ResidentalRent);
            string TotalGrossPrice = OfferPage.Substring(OfferPage.IndexOf("price"));
            TotalGrossPrice = TotalGrossPrice.Substring(TotalGrossPrice.IndexOf("/span"));
            TotalGrossPrice = TotalGrossPrice.Substring(TotalGrossPrice.IndexOf(">") + 1, TotalGrossPrice.IndexOf("<") - TotalGrossPrice.IndexOf(">") - 1);
            if (TotalGrossPrice.Contains("&nbsp;")) TotalGrossPrice = TotalGrossPrice.Replace("&nbsp;", " ");
            PropertyPrice.Add("TotalGrossPrice", TotalGrossPrice);
            string PricePerMeter = OfferPage.Substring(OfferPage.IndexOf("price-m2"));
            PricePerMeter = PricePerMeter.Substring(PricePerMeter.IndexOf("(") + 1, PricePerMeter.IndexOf(")") - PricePerMeter.IndexOf("(") - 1);
            if (PricePerMeter.Contains("&#178;")) PricePerMeter = PricePerMeter.Replace("&#178;", "2");
            PropertyPrice.Add("PricePerMeter", PricePerMeter);  //main-info -> price-m2
        }

        private void FillPropertyDetails(string OfferPage)
        {
            string Area = OfferPage.Substring(OfferPage.IndexOf("Powierzchnia mieszkalna"));
            Area = Area.Substring(Area.IndexOf("dd"), Area.IndexOf("/dd") - Area.IndexOf("dd"));
            Area = Area.Substring(Area.IndexOf("span"), Area.IndexOf("/span") - Area.IndexOf("span"));
            Area = Area.Substring(Area.IndexOf(">") + 1, Area.IndexOf("<") - Area.IndexOf(">") - 1);
            PropertyDetails.Add("Area", Area);
            string NumberOfRooms = OfferPage.Substring(OfferPage.IndexOf("Liczba pokoi"));
            NumberOfRooms = NumberOfRooms.Substring(NumberOfRooms.IndexOf("dd"), NumberOfRooms.IndexOf("/dd") - NumberOfRooms.IndexOf("dd"));
            NumberOfRooms = NumberOfRooms.Substring(NumberOfRooms.IndexOf("span"), NumberOfRooms.IndexOf("/span") - NumberOfRooms.IndexOf("span"));
            NumberOfRooms = NumberOfRooms.Substring(NumberOfRooms.IndexOf(">") + 1, NumberOfRooms.IndexOf("<") - NumberOfRooms.IndexOf(">") - 1);
            PropertyDetails.Add("NumberOfRooms", NumberOfRooms);
            string FloorNumber;
            try
            {
                FloorNumber = OfferPage.Substring(OfferPage.IndexOf("Piętro"));
                FloorNumber = FloorNumber.Substring(FloorNumber.IndexOf("dd"), FloorNumber.IndexOf("/dd") - FloorNumber.IndexOf("dd"));
                FloorNumber = FloorNumber.Substring(FloorNumber.IndexOf("span"), FloorNumber.IndexOf("/span") - FloorNumber.IndexOf("span"));
                FloorNumber = FloorNumber.Substring(FloorNumber.IndexOf(">") + 1, FloorNumber.IndexOf("<") - FloorNumber.IndexOf(">") - 1);
            }
            catch
            {
                FloorNumber = null;
            }
            PropertyDetails.Add("FloorNumber", FloorNumber);

            string YearOfConstruction;
            try
            {
                YearOfConstruction = OfferPage.Substring(OfferPage.IndexOf("Rok budowy"));
                YearOfConstruction = YearOfConstruction.Substring(YearOfConstruction.IndexOf("dd"), YearOfConstruction.IndexOf("/dd") - YearOfConstruction.IndexOf("dd"));
                YearOfConstruction = YearOfConstruction.Substring(YearOfConstruction.IndexOf("span"), YearOfConstruction.IndexOf("/span") - YearOfConstruction.IndexOf("span"));
                YearOfConstruction = YearOfConstruction.Substring(YearOfConstruction.IndexOf(">") + 1, YearOfConstruction.IndexOf("<") - YearOfConstruction.IndexOf(">") - 1);
            }
            catch
            {
                YearOfConstruction = null;
            }
            PropertyDetails.Add("YearOfConstruction", YearOfConstruction);
        }

        private void FillPropertyAddress(string OfferPage)
        {
            string AddressData = OfferPage.Substring(OfferPage.IndexOf("main-info"));
            AddressData = AddressData.Substring(AddressData.IndexOf("location"));
            AddressData = AddressData.Substring(AddressData.IndexOf(">") + 1, AddressData.IndexOf("<") - AddressData.IndexOf(">") - 1);
            if (AddressData.Contains("\n")) AddressData = AddressData.Replace("\n", "");
            string[] AddressDataTable = AddressData.Split(",");
            string StreetName = AddressDataTable[0];
            PropertyAddress.Add("StreetName", StreetName);
            string City = "";
            if (Flat)
            {
                City = AddressDataTable[1].Trim().ToUpper();
                City = ReplacePolishChars(City);
            }
            else
            {
                City = AddressDataTable[AddressDataTable.Length - 1].Trim().ToUpper();
                City = ReplacePolishChars(City);
            }
            PropertyAddress.Add("City", City);

            string District = "";
            if (Flat)
            {
                if (AddressDataTable.Length > 2)
                {
                    for (int i = 2; i < AddressDataTable.Length; i++)
                    {
                        District += AddressDataTable[i];
                        District += " ";
                    }
                }
            }
            else
            {
                District = null;
            }
            PropertyAddress.Add("District", District);
            PropertyAddress.Add("DetailedAddress", null);
        }

        private void FillPropertyFeatures(string OfferPage)
        {
            string GardenArea;
            try
            {
                GardenArea = OfferPage.Substring(OfferPage.IndexOf("Powierzchnia działki"));
                GardenArea = GardenArea.Substring(GardenArea.IndexOf("dd"), GardenArea.IndexOf("/dd") - GardenArea.IndexOf("dd"));
                GardenArea = GardenArea.Substring(GardenArea.IndexOf("span"), GardenArea.IndexOf("/span") - GardenArea.IndexOf("span"));
                GardenArea = GardenArea.Substring(GardenArea.IndexOf(">") + 1, GardenArea.IndexOf("<") - GardenArea.IndexOf(">") - 1);
            }
            catch
            {
                GardenArea = null;
            }
            PropertyFeatures.Add("GardenArea", GardenArea);
            string Balconies;
            try
            {
                Balconies = OfferPage.Substring(OfferPage.IndexOf("Balkon"));
                Balconies = Balconies.Substring(Balconies.IndexOf("dd"), Balconies.IndexOf("/dd") - Balconies.IndexOf("dd"));
                Balconies = Balconies.Substring(Balconies.IndexOf("span"), Balconies.IndexOf("/span") - Balconies.IndexOf("span"));
                Balconies = Balconies.Substring(Balconies.IndexOf(">") + 1, Balconies.IndexOf("<") - Balconies.IndexOf(">") - 1);
            }
            catch
            {
                Balconies = null;
            }
            PropertyFeatures.Add("Balconies", Balconies);
            PropertyFeatures.Add("BasementArea", null);
            PropertyFeatures.Add("OutdoorParkingPlaces", null);
            PropertyFeatures.Add("IndoorParkingPlaces", null);
        }

        private void FillRawDescription(string OfferPage)
        {
            RawDescription = OfferPage.Substring(OfferPage.IndexOf("description"));
            RawDescription = RawDescription.Substring(RawDescription.IndexOf("<p>"), RawDescription.IndexOf("<div") - RawDescription.IndexOf("<p>"));
            try
            {
                RawDescription = RawDescription.Replace("<p>", "");
                RawDescription = RawDescription.Replace("</p>", "");
            }
            catch { }
            try
            {
                RawDescription = RawDescription.Replace("<span>", "");
                RawDescription = RawDescription.Replace("</span>", "");
            }
            catch { }
            bool ContainsHtml = true;
            while (ContainsHtml)
            {
                try
                {
                    string RawDescriptionPart1 = RawDescription.Substring(0, RawDescription.IndexOf("<"));
                    string RawDescriptionPart2 = RawDescription.Substring(RawDescription.IndexOf(">") + 1);
                    RawDescription = RawDescriptionPart1 + RawDescriptionPart2;
                }
                catch
                {
                    ContainsHtml = false;
                }
            }
        }

        private string ReplacePolishChars(string Input)
        {
            StringBuilder Output = new StringBuilder();
            foreach (var SingleChar in Input)
            {
                switch (SingleChar)
                {
                    case 'ą': Output.Append('a'); break;
                    case 'ć': Output.Append('c'); break;
                    case 'ę': Output.Append('e'); break;
                    case 'ł': Output.Append('l'); break;
                    case 'ń': Output.Append('n'); break;
                    case 'ó': Output.Append('o'); break;
                    case 'ś': Output.Append('s'); break;
                    case 'ź': Output.Append('z'); break;
                    case 'ż': Output.Append('z'); break;
                    case 'Ą': Output.Append('A'); break;
                    case 'Ć': Output.Append('C'); break;
                    case 'Ę': Output.Append('E'); break;
                    case 'Ł': Output.Append('L'); break;
                    case 'Ń': Output.Append('N'); break;
                    case 'Ó': Output.Append('O'); break;
                    case 'Ś': Output.Append('S'); break;
                    case 'Ź': Output.Append('Z'); break;
                    case 'Ż': Output.Append('Z'); break;
                    default: Output.Append(SingleChar); break;
                }
            }
            return Output.ToString();
        }
    }
}