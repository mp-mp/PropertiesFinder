using System;
using System.Collections.Generic;
using System.Text;
using Models;
using System.Diagnostics.CodeAnalysis;

namespace BezposrednieIntegration
{
    class BezposrednieComparer : IEqualityComparer<Entry>
    {
        public bool Equals(Entry x, Entry y)
        {
            if (GetHashCode(x) == GetHashCode(y))
                return true;
            return false;
        }

        public int GetHashCode([DisallowNull] Entry obj)
        {
            var Hash = PropertyAddressHash(obj) + PropertyFeaturesHash(obj) + PropertyDetailsHash(obj) + PropertyPriceHash(obj) + OfferDetailsHash(obj);
            return Hash;
        }

        private int PropertyFeaturesHash(Entry obj)
        {
            return IfNull(obj.PropertyFeatures.Balconies) + IfNull(obj.PropertyFeatures.IndoorParkingPlaces) + IfNull(obj.PropertyFeatures.OutdoorParkingPlaces) + IfNull(obj.PropertyFeatures.BasementArea) + IfNull(obj.PropertyFeatures.GardenArea);
        }
        private int PropertyAddressHash(Entry obj)
        {
            return IfNull(obj.PropertyAddress.DetailedAddress) + IfNull(obj.PropertyAddress.District) + IfNull(obj.PropertyAddress.StreetName) + IfNull(obj.PropertyAddress.City.ToString());
        }
        private int PropertyDetailsHash(Entry obj)
        {
            return IfNull(obj.PropertyDetails.Area) + IfNull(obj.PropertyDetails.FloorNumber) + IfNull(obj.PropertyDetails.NumberOfRooms) + IfNull(obj.PropertyDetails.YearOfConstruction);
        }
        private int PropertyPriceHash(Entry obj)
        {
            return IfNull(obj.PropertyPrice.PricePerMeter) + IfNull(obj.PropertyPrice.ResidentalRent) + IfNull(obj.PropertyPrice.TotalGrossPrice);
        }
        private int SellerContactHash(Entry obj)
        {
            return IfNull(obj.OfferDetails.SellerContact.Name) + IfNull(obj.OfferDetails.SellerContact.Telephone);
        }
        private int OfferDetailsHash(Entry obj)
        {
            int output = SellerContactHash(obj);
            return output + IfNull(obj.OfferDetails.Url) + obj.OfferDetails.CreationDateTime.GetHashCode() + IfNull(obj.OfferDetails.OfferKind.ToString());
        }

        private int IfNull(int? input)
        {
            int result;
            if (input.HasValue)
            {
                result = input.Value;
            }
            else result = 0;
            return result;
        }
        private int IfNull(string input)
        {
            int result;
            if (input == null)
            {
                result = 0;
            }
            else result = input.GetHashCode();
            return result;
        }
        private int IfNull(decimal? input)
        {
            int result;
            if (input == null)
            {
                result = 0;
            }
            else result = Decimal.ToInt32(Convert.ToDecimal(input));
            return result;
        }
    }
}