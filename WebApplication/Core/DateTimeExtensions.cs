using System;

namespace WebApplication {

    // Java way of dating
    public static class DateTimeExtensions
    {
        private static DateTime Jan1st1970 = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
        public static long CurrentTimeMillis()
        {
            return (long) ((DateTime.UtcNow - Jan1st1970).TotalMilliseconds);
        }
        
        public static DateTime DateTimeFromMilliseconds(long milliseconds) {
            return Jan1st1970.AddMilliseconds(milliseconds);
        }
    }

}
