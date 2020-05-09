using Models;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace ZnajdzTo
{
    class ZnajdzToComparer : IEqualityComparer<Entry>
    {
        public bool Equals([AllowNull] Entry x, [AllowNull] Entry y)
        {
            if (GetHashCode(x) == GetHashCode(y))
                return true;
            return false;
        }

        public int GetHashCode([DisallowNull] Entry obj)
        {
            return obj.PropertyAddress.City.GetHashCode() +
                   obj.PropertyAddress.StreetName.GetHashCode() +
                   obj.PropertyAddress.District.GetHashCode() +
                   obj.PropertyDetails.Area.GetHashCode() +
                   obj.PropertyDetails.FloorNumber.GetHashCode() +
                   obj.PropertyDetails.NumberOfRooms.GetHashCode() +
                   obj.PropertyDetails.YearOfConstruction.GetHashCode() +
                   obj.PropertyPrice.TotalGrossPrice.GetHashCode();
        }
    }
}
