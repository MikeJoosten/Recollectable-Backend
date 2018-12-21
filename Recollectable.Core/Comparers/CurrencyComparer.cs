using Recollectable.Core.Entities.Collectables;
using System.Collections.Generic;

namespace Recollectable.Core.Comparers
{
    public class CurrencyComparer : IEqualityComparer<Currency>
    {
        private readonly CollectorValueComparer _collectorValueComparer;

        public CurrencyComparer()
        {
            _collectorValueComparer = new CollectorValueComparer();
        }

        public bool Equals(Currency x, Currency y)
        {
            if (ReferenceEquals(x, y))
            {
                return true;
            }

            if (ReferenceEquals(x, null) || ReferenceEquals(y, null))
            {
                return false;
            }

            return x.FaceValue == y.FaceValue && x.Type == y.Type &&
                x.ReleaseDate == y.ReleaseDate && x.CountryId == y.CountryId &&
                _collectorValueComparer.Equals(x.CollectorValue, y.CollectorValue);
        }

        public int GetHashCode(Currency currency)
        {
            if (ReferenceEquals(currency, null))
            {
                return 0;
            }

            unchecked
            {
                int hash = (int)2166136261;

                hash = (hash * 16777619) ^ currency.FaceValue.GetHashCode();
                hash = (hash * 16777619) ^ currency.Type.GetHashCode();
                hash = (hash * 16777619) ^ currency.ReleaseDate.GetHashCode();
                hash = (hash * 16777619) ^ currency.CountryId.GetHashCode();
                hash = (hash * 16777619) ^ _collectorValueComparer.GetHashCode();

                return hash;
            }
        }
    }
}