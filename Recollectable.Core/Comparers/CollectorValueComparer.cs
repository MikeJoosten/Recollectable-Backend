using Recollectable.Core.Entities.Collectables;
using System.Collections.Generic;

namespace Recollectable.Core.Comparers
{
    public class CollectorValueComparer : IEqualityComparer<CollectorValue>
    {
        public bool Equals(CollectorValue x, CollectorValue y)
        {
            if (ReferenceEquals(x, y))
            {
                return true;
            }

            if (ReferenceEquals(x, null) || ReferenceEquals(y, null))
            {
                return false;
            }

            return x.G4 == y.G4 && x.VG8 == y.VG8 && x.F12 == y.F12 &&
                x.VF20 == y.VF20 && x.XF40 == y.XF40 && x.AU50 == y.AU50 &&
                x.MS60 == y.MS60 && x.MS63 == y.MS63 && x.PF60 == y.PF60 &&
                x.PF63 == y.PF63 && x.PF65 == y.PF65;
        }

        public int GetHashCode(CollectorValue collectorValue)
        {
            if (ReferenceEquals(collectorValue, null))
            {
                return 0;
            }

            unchecked
            {
                int hash = (int) 2166136261;

                hash = (hash * 16777619) ^ collectorValue.G4.GetHashCode();
                hash = (hash * 16777619) ^ collectorValue.VG8.GetHashCode();
                hash = (hash * 16777619) ^ collectorValue.F12.GetHashCode();
                hash = (hash * 16777619) ^ collectorValue.VF20.GetHashCode();
                hash = (hash * 16777619) ^ collectorValue.XF40.GetHashCode();
                hash = (hash * 16777619) ^ collectorValue.AU50.GetHashCode();
                hash = (hash * 16777619) ^ collectorValue.MS60.GetHashCode();
                hash = (hash * 16777619) ^ collectorValue.MS63.GetHashCode();
                hash = (hash * 16777619) ^ collectorValue.PF60.GetHashCode();
                hash = (hash * 16777619) ^ collectorValue.PF63.GetHashCode();
                hash = (hash * 16777619) ^ collectorValue.PF65.GetHashCode();

                return hash;
            }
        }
    }
}