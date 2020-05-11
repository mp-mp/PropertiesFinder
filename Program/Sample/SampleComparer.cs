using Models;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace Application.Sample
{
    public class SampleComparer : IEqualityComparer<Entry>
    {
      /*  public bool Equals(Entry x, Entry y)
        {
            if (x.OfferDetails.Url.Equals(y.OfferDetails.Url))
                return true;
            return false;
        }*/
        public bool Equals(Entry x, Entry y)
        {
            if (
                x.OfferDetails.Url.Equals(y.OfferDetails.Url) &
                x.PropertyAddress.City.Equals(y.PropertyAddress.City) &
                x.PropertyDetails.Area.Equals(y.PropertyDetails.Area) &
                x.PropertyDetails.NumberOfRooms.Equals(y.PropertyDetails.NumberOfRooms) &
                x.PropertyFeatures.Balconies.Equals(y.PropertyFeatures.Balconies) &
                x.PropertyFeatures.BasementArea.Equals(y.PropertyFeatures.BasementArea) &
                x.PropertyFeatures.GardenArea.Equals(y.PropertyFeatures) &
                x.PropertyFeatures.IndoorParkingPlaces.Equals(y.PropertyFeatures.IndoorParkingPlaces) &
                x.PropertyFeatures.OutdoorParkingPlaces.Equals(y.PropertyFeatures.OutdoorParkingPlaces)
                )
                return true;
            return false;
        }
        public int GetHashCode([DisallowNull] Entry obj)
        {
            return obj.OfferDetails.Url == null ? 0 : obj.OfferDetails.Url.GetHashCode();
        }
    }
}
