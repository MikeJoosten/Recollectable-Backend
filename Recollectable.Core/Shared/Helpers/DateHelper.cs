using System;

namespace Recollectable.Core.Shared.Helpers
{
    public static class DateHelper
    {
        public static long ToUnixEpochDate(DateTime date) =>
            (long)Math.Round((date.ToUniversalTime() - new DateTimeOffset(1970, 1, 1, 0, 0, 0, TimeSpan.Zero)).TotalSeconds);
    }
}