
using System;

namespace TaskManager.Helpers
{        public static class DateHelper
        {
            public static DateTime Today => DateTime.UtcNow.Date;
            public static DateTime Tomorrow => Today.AddDays(1);
        }
    

}
