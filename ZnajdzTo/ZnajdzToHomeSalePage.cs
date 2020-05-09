using HtmlAgilityPack;
using Models;
using System;
using System.Text.RegularExpressions;

namespace ZnajdzTo
{
    public class ZnajdzToHomeSalePage
    {
        private HtmlDocument homeSalePage;
        private string url;

        public ZnajdzToHomeSalePage(HtmlDocument homeSalePage, string url) {
            this.homeSalePage = homeSalePage;
            this.url = url;
        }

        public string Url
        {
            get
            {
                return url;
            }
        }

        public DateTime CreationDateTime
        {
            get 
            {
                return DateTime.Now;
            }
        }

        public OfferKind OfferKind
        {
            get
            {
                return OfferKind.SALE;
            }
        }

        public bool IsStillValid
        {
            get
            {
                return true;
            }
        }

        public bool IsCity
        {
            get
            {
                string xPath = "//div[@id='left-col']/p";
                HtmlNode offerBottomInfoNode = homeSalePage.DocumentNode.SelectSingleNode(xPath);
                string cityRecognitionInOffer = "lokalizacja:";
                if(!(offerBottomInfoNode is null) && offerBottomInfoNode.InnerText.Contains(cityRecognitionInOffer))
                {
                    string offerBottomInfo = offerBottomInfoNode.InnerText;
                    int cityIndex = offerBottomInfo.LastIndexOf(cityRecognitionInOffer) + cityRecognitionInOffer.Length;
                    string offerInfoFromCityToEnd = offerBottomInfo.Substring(cityIndex, offerBottomInfo.Length - cityIndex);
                    string city = offerInfoFromCityToEnd.Split(",")[0].Trim();
                    string cityUppercase = city.Replace(" ", "_").ToUpper();
                    string cityWithoutPolishLetters = cityUppercase.Replace("Ą", "A").Replace("Ć", "C").Replace("Ę", "E").Replace("Ń", "N")
                        .Replace("Ł", "L").Replace("Ó", "O").Replace("Ś", "S").Replace("Ż", "Z").Replace("Ź", "Z");

                    return Enum.TryParse(cityWithoutPolishLetters, out PolishCity polishCity);
                }
                return false;
            }
        }

        public PolishCity City
        {
            get
            {
                string xPath = "//div[@id='left-col']/p";
                HtmlNode offerBottomInfoNode = homeSalePage.DocumentNode.SelectSingleNode(xPath);
                string cityRecognitionInOffer = "lokalizacja:";
                string offerBottomInfo = offerBottomInfoNode.InnerText;
                int cityIndex = offerBottomInfo.LastIndexOf(cityRecognitionInOffer) + cityRecognitionInOffer.Length;
                string offerInfoFromCityToEnd = offerBottomInfo.Substring(cityIndex, offerBottomInfo.Length - cityIndex);
                string city = offerInfoFromCityToEnd.Split(",")[0].Trim();
                string cityUppercase = city.Replace(" ", "_").ToUpper();
                string cityWithoutPolishLetters = cityUppercase.Replace("Ą", "A").Replace("Ć", "C").Replace("Ę", "E").Replace("Ń", "N")
                    .Replace("Ł", "L").Replace("Ó", "O").Replace("Ś", "S").Replace("Ż", "Z").Replace("Ź", "Z");

                Enum.TryParse(cityWithoutPolishLetters, out PolishCity polishCity);
                return polishCity;
            }
        }

        public string StreetName
        {
            get
            {
                try
                {
                    string xPath = "//div[@id='left-col']/p";
                    HtmlNode offerBottomInfoNode = homeSalePage.DocumentNode.SelectSingleNode(xPath);
                    string streetRecognitionInOffer = "dzielnica/adres:";
                    if (!(offerBottomInfoNode is null) && offerBottomInfoNode.InnerText.Contains(streetRecognitionInOffer))
                    {
                        string offerBottomInfo = offerBottomInfoNode.InnerText;
                        int streetIndex = offerBottomInfo.LastIndexOf(streetRecognitionInOffer) + streetRecognitionInOffer.Length;
                        string offerInfoFromStreetToEnd = offerBottomInfo.Substring(streetIndex, offerBottomInfo.Length - streetIndex);
                        string street = offerInfoFromStreetToEnd.Split(",")[1].Split(".")[0].Trim();

                        return street;
                    }
                    else
                    {
                        return null;
                    }
                }
                catch
                {
                    return null;
                }
            }
        }

        public string DetailedAddress
        {
            get
            {
                return null;
            }
        }

        public string District
        {
            get
            {
                try
                {
                    string xPath = "//div[@id='left-col']/p";
                    HtmlNode offerBottomInfoNode = homeSalePage.DocumentNode.SelectSingleNode(xPath);
                    string districtRecognitionInOffer = "dzielnica/adres:";
                    if (!(offerBottomInfoNode is null) && offerBottomInfoNode.InnerText.Contains(districtRecognitionInOffer))
                    {
                        string offerBottomInfo = offerBottomInfoNode.InnerText;
                        int districtIndex = offerBottomInfo.LastIndexOf(districtRecognitionInOffer) + districtRecognitionInOffer.Length;
                        string offerInfoFromDistrictToEnd = offerBottomInfo.Substring(districtIndex, offerBottomInfo.Length - districtIndex);
                        string district = offerInfoFromDistrictToEnd.Split(",")[0].Trim();

                        return district;
                    }
                    else
                    {
                        return null;
                    }
                }
                catch
                {
                    return null;
                }
            }
        }

        public decimal Area
        {
            get
            {
                try
                {
                    decimal area = TotalGrossPrice / PricePerMeter;
                    return area;
                }
                catch
                {
                    return 0;
                }
            }
        }

        public int? FloorNumber
        {
            get
            {
                try
                {
                    string xPath = "//div[@class='offerDetails']/dl/dd";
                    HtmlNodeCollection offerDetailsInfoNodes = homeSalePage.DocumentNode.SelectNodes(xPath);
                    if (!(offerDetailsInfoNodes is null) && offerDetailsInfoNodes.Count > 0)
                    {
                        HtmlNode floorNumbersNode;
                        if (offerDetailsInfoNodes.Count == 9)
                        {
                            floorNumbersNode = offerDetailsInfoNodes[6];
                        }
                        else
                        {
                            floorNumbersNode = offerDetailsInfoNodes[5];
                        }
                        string floorNumberOfHome = floorNumbersNode.InnerText.Split("/")[0];
                        int floorNumber = int.Parse(floorNumberOfHome);
                        return floorNumber;
                    }
                    else
                    {
                        return null;
                    }
                }
                catch
                {
                    return null;
                }
            }
        }

        public int NumberOfRooms
        {
            get
            {
                try
                {
                    string xPath = "//div[@class='offerDetails']/dl/dd";
                    HtmlNodeCollection offerDetailsInfoNodes = homeSalePage.DocumentNode.SelectNodes(xPath);
                    if (!(offerDetailsInfoNodes is null) && offerDetailsInfoNodes.Count > 0)
                    {
                        HtmlNode numberOfRoomsNode = offerDetailsInfoNodes[3];
                        if (!numberOfRoomsNode.InnerText.Contains("-")) {
                            int numberOfRooms = int.Parse(numberOfRoomsNode.InnerText);
                            return numberOfRooms;
                        }
                        else
                        {
                            return 0;
                        }
                        
                    }
                    else
                    {
                        return 0;
                    }
                }
                catch
                {
                    return 0;
                }
            }
        }

        public int? YearOfConstruction
        {
            get
            {
                try
                {
                    string xPath = "//div[@class='offerDetails']/dl/dd";
                    HtmlNodeCollection offerDetailsInfoNodes = homeSalePage.DocumentNode.SelectNodes(xPath);
                    if (!(offerDetailsInfoNodes is null) && offerDetailsInfoNodes.Count > 0)
                    {
                        HtmlNode yearOfConstructionNode;
                        if (offerDetailsInfoNodes.Count == 9) {
                            yearOfConstructionNode = offerDetailsInfoNodes[7];
                        } 
                        else
                        {
                            yearOfConstructionNode = offerDetailsInfoNodes[6];
                        }
                        int yearOfConstruction = int.Parse(yearOfConstructionNode.InnerText);
                        return yearOfConstruction;

                    }
                    else
                    {
                        return null;
                    }
                }
                catch
                {
                    return null;
                }
            }
        }

        public string Email
        {
            get
            {
                try
                {
                    string xPath = "//div[@class='offerDescription']";
                    HtmlNode offerDescriptionNode = homeSalePage.DocumentNode.SelectSingleNode(xPath);
                    if (!(offerDescriptionNode is null) && offerDescriptionNode.InnerText.Contains("mail"))
                    {
                        string offerDescription = offerDescriptionNode.OuterHtml;
                        Regex emailRegex = new Regex(@"\w+([-+.]\w+)*@\w+([-.]\w+)*\.\w+([-.]\w+)*", RegexOptions.IgnoreCase);
                        MatchCollection emailMatches = emailRegex.Matches(offerDescription);
                        string email = emailMatches.Count > 0 ? emailMatches[0].Value : null;
                        return email;
                    }
                    else
                    {
                        return null;
                    }
                }
                catch
                {
                    return null;
                }
            }
        }

        public string Name
        {
            get
            {
                try
                {
                    string xPathOfferDescription = "//div[@class='offerDescription']";
                    HtmlNode offerDescriptionNode = homeSalePage.DocumentNode.SelectSingleNode(xPathOfferDescription);
                    string nameRecognitionInOffer = "sprzedawca";
                    if (!(offerDescriptionNode is null) && offerDescriptionNode.OuterHtml.ToLower().Contains(nameRecognitionInOffer))
                    {
                        string offerDescription = offerDescriptionNode.OuterHtml.ToLower();
                        int nameIndex = offerDescription.LastIndexOf(nameRecognitionInOffer) + nameRecognitionInOffer.Length + 1;
                        string offerDescriptionFromNameToEnd = offerDescriptionNode.OuterHtml.Substring(nameIndex, offerDescription.Length - nameIndex);
                        string nameDescriptionToTag = offerDescriptionFromNameToEnd.Split("<")[0];
                        string name = nameDescriptionToTag.Trim();
                        return name;
                    }
                    else
                    {
                        return null;
                    }
                }
                catch
                {
                    return null;
                }
            }
        }

        public string Telephone
        {
            get
            {
                try
                {
                    string xPath = "//div[@class='offerSection']/div[@class='phone-box']/div[@class='hidden-phone']/h1";
                    HtmlNode telephoneNode = homeSalePage.DocumentNode.SelectSingleNode(xPath);
                    if (!(telephoneNode is null))
                    {
                        string telephoneTag = telephoneNode.OuterHtml.Split("<br>")[0];
                        string telephone = telephoneTag.Replace("<h1>", "").Replace("<\\h1>", "").Replace("\n", "");
                        return telephone;
                    }
                    else {
                        return null;
                    }
                }
                catch
                {
                    return null;
                }
            }
        }

        public decimal PricePerMeter
        {
            get
            {
                try
                {
                    string xPath = "//div[@class='offerDetails']/dl/dd";
                    HtmlNodeCollection offerDetailsInfoNodes = homeSalePage.DocumentNode.SelectNodes(xPath);
                    if (!(offerDetailsInfoNodes is null) && offerDetailsInfoNodes.Count > 0)
                    {
                        HtmlNode pricePerMeterNode = offerDetailsInfoNodes[1];
                        string pricePerMeterWithEntitiesAndZl = pricePerMeterNode.InnerText;
                        string pricePerMeterWithoutEntitiesAndZl = pricePerMeterWithEntitiesAndZl.Split("&")[0].Replace('.', ',');
                        decimal pricePerMeter = decimal.Parse(pricePerMeterWithoutEntitiesAndZl);
                        return pricePerMeter;
                    }
                    else
                    {
                        return 0;
                    }
                }
                catch
                {
                    return 0;
                }
            }
        }

        public int TotalGrossPrice
        {
            get
            {
                try
                {
                    string xPath = "//div[@class='offerDetails']/dl/dd";
                    HtmlNodeCollection offerDetailsInfoNodes = homeSalePage.DocumentNode.SelectNodes(xPath);
                    if (!(offerDetailsInfoNodes is null) && offerDetailsInfoNodes.Count > 0)
                    {
                        HtmlNode totalGrossPriceNode = offerDetailsInfoNodes[0];
                        string totalGrossWithZl = totalGrossPriceNode.InnerText;
                        int totalGrossPrice = int.Parse(totalGrossWithZl.Split(" ")[0]);
                        return totalGrossPrice;
                    }
                    else
                    {
                        return 0;
                    }
                }
                catch
                {
                    return 0;
                }
            }
        }

        public int? ResidentalRent
        {
            // nie ma informacji w ofertach
            get
            {
                return null;
            }
        }

        public int? Balconies
        {
            // nie ma informacji o ilości balkonów w ofertach, informacja czy balkon jest czy nie pobierana jest z opisu oferty
            get
            {
                try
                {
                    string xPathOfferDescription = "//div[@class='offerDescription']";
                    HtmlNode offerDescriptionNode = homeSalePage.DocumentNode.SelectSingleNode(xPathOfferDescription);
                    string offerDescriptionLowerCase = offerDescriptionNode.OuterHtml.ToLower();
                    if (!(offerDescriptionNode is null) &&
                        offerDescriptionLowerCase.Contains("balkon") &&
                        !offerDescriptionLowerCase.Contains("brak balkonu"))
                    {
                        return 1;
                    }
                    else if (offerDescriptionLowerCase.Contains("brak balkonu"))
                    {
                        return 0;
                    }
                    else
                    {
                        string xPathOfferSection = "//div[@class='offerSection']";
                        HtmlNode offerSectionNode = homeSalePage.DocumentNode.SelectSingleNode(xPathOfferSection);
                        if (!(offerSectionNode is null) && offerSectionNode.OuterHtml.ToLower().Contains("balkon"))
                        {
                            return 1;
                        }
                        else
                        {
                            return null;
                        }
                    }
                }
                catch
                {
                    return null;
                }
            }
        }

        public int? BasementArea
        {
            get
            {
                try
                {
                    string xPathOfferDescription = "//div[@class='offerDescription']";
                    HtmlNode offerDescriptionNode = homeSalePage.DocumentNode.SelectSingleNode(xPathOfferDescription);
                    string basementRecognitionInOffer = "piwnica";
                    if (!(offerDescriptionNode is null) && offerDescriptionNode.OuterHtml.ToLower().Contains(basementRecognitionInOffer))
                    {
                        string offerDescription = offerDescriptionNode.OuterHtml.ToLower();
                        int basementIndex = offerDescription.LastIndexOf(basementRecognitionInOffer);
                        string offerDescriptionFromBasementToEnd = offerDescription.Substring(basementIndex, offerDescription.Length - basementIndex);
                        string basementDescriptionToTag = offerDescriptionFromBasementToEnd.Split("<")[0];
                        int basementArea;
                        bool isBasementArea = int.TryParse(Regex.Match(basementDescriptionToTag, @"\d+").Value, out basementArea);
                        if (isBasementArea)
                        {
                            return basementArea;
                        }
                        else
                        {
                            return null;
                        }
                    }
                    else
                    {
                        return null;
                    }
                }
                catch
                {
                    return null;
                }
            }
        }

        public int? GardenArea
        {
            // nie ma informacji w ofertach
            get
            {
                return null;
            }
        }

        public int? IndoorParkingPlaces
        {
            // nie ma informacji w ofertach
            get
            {
                return null;
            }
        }

        public int? OutdoorParkingPlaces
        {
            // nie ma informacji w ofertach
            get
            {
                return null;
            }
        }

        public string RawDescription
        {
            get
            {
                try
                {
                    string xPath = "//div[@class='offerDescription']";
                    HtmlNode rawDescriptionNode = homeSalePage.DocumentNode.SelectSingleNode(xPath);
                    if (!(rawDescriptionNode is null))
                    {
                        string rawDescription = rawDescriptionNode.InnerText;
                        return rawDescription;
                    }
                    else
                    {
                        return "";
                    }
                }
                catch
                {
                    return "";
                }
            }
        }
    }
}
