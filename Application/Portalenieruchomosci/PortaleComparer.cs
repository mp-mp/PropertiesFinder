using Models;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace Application.Portalenieruchomosci
{
    class PortaleComparer : IEqualityComparer<Entry>
    {
        public bool Equals(Entry x, Entry y)
        {
            if (x.OfferDetails.Url.Equals(y.OfferDetails.Url))
                return true;

            if (x.PropertyAddress.Equals(y.PropertyAddress)
                && x.PropertyDetails.Equals(y.PropertyDetails))
                return true;

            if (x.PropertyAddress.Equals(y.PropertyAddress)
                && (x.PropertyFeatures.BasementArea != null &&
                    ((y.PropertyFeatures.BasementArea != 0 && x.PropertyFeatures.BasementArea == 1)
                     || (y.PropertyFeatures.BasementArea.Equals(x.PropertyFeatures.BasementArea)))))
                return true;

            return false;
        }

        public int GetHashCode([DisallowNull] Entry obj)
        {
            int liczba = 0;

            if (obj.OfferDetails.Url != null)
                liczba |= obj.OfferDetails.Url.GetHashCode();

            if (obj.PropertyAddress != null)
                liczba |= obj.PropertyAddress.GetHashCode();

            if (obj.PropertyDetails != null)
                liczba |= obj.PropertyDetails.GetHashCode();

            if (obj.PropertyFeatures != null)
                liczba |= obj.PropertyFeatures.GetHashCode();

            return liczba;
        }
    }
}
