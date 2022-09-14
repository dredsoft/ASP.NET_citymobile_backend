using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CityApp.Common.Extensions
{
    public static class DateTimeExtensions
    {
        static readonly DateTime UNIX_EPOCH = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);

        public static long DateTimeToUnixTime(DateTime d)
        {
            var span = d.ToUniversalTime() - UNIX_EPOCH;
            return (long)span.TotalSeconds;
        }

        public static DateTime UnixTimeToDateTime(long d)
        {
            return UNIX_EPOCH.AddSeconds(d);
        }

        public static long ToUnixTime(this DateTime d)
        {
            return DateTimeToUnixTime(d);
        }

        public static DateTime ToDateTime(this long d)
        {
            return UnixTimeToDateTime(d);
        }


        public static DateTime UTCToAccountLocalTime(this DateTime date, string localTimeZone)
        {
            var timeZone = TimeZoneInfo.FindSystemTimeZoneById(localTimeZone);
            var utcFrom = TimeZoneInfo.ConvertTimeFromUtc(date, timeZone);
            return utcFrom;
        }

        public static DateTime? UTCToAccountLocalTime(DateTime? date, string localTimeZone)
        {
            if (date.HasValue)
            {
                return date.Value.UTCToAccountLocalTime(localTimeZone);
            }
            else
            {
                return null;
            }
        }

        public static DateTime LocalToUTC(this DateTime date, string localTimeZone)
        {
            var timeZone = TimeZoneInfo.FindSystemTimeZoneById(localTimeZone);
            return TimeZoneInfo.ConvertTimeToUtc(date, timeZone);
        }

        public static DateTime Floor(this DateTime d)
        {
            return d.Date;
        }

        public static DateTime Ceiling(this DateTime d)
        {
            return d.Date.AddDays(1).AddMinutes(-1);
        }

        static public DateTime DateTimeFloor(this DateTime dt, TimeInterval Interval = TimeInterval.OneSecond)
        {
            return WorkMethod(dt, 0L, Interval);
        }
        static public DateTime DateTimeMidpoint(this DateTime dt, TimeInterval Interval = TimeInterval.OneSecond)
        {
            return WorkMethod(dt, 2L, Interval);
        }
        static public DateTime DateTimeCeiling(this DateTime dt, TimeInterval Interval = TimeInterval.OneSecond)
        {
            return WorkMethod(dt, 1L, Interval);
        }
        static public DateTime DateTimeCeilingUnbounded(this DateTime dt, TimeInterval Interval)
        {
            return WorkMethod(dt, 1L, Interval).AddTicks(-1);
        }
        static public DateTime DateTimeRound(this DateTime dt, TimeInterval Interval)
        {
            if (dt >= WorkMethod(dt, 2L, Interval))
                return WorkMethod(dt, 1L, Interval);
            else
                return WorkMethod(dt, 0L, Interval);
        }
        public enum TimeInterval : long
        {
            YearFromJanuary = 120L,
            YearFromFebruary = 121L,
            YearFromMarch = 122L,
            YearFromApril = 123L,
            YearFromMay = 124L,
            YearFromJune = 125L,
            YearFromJuly = 126L,
            YearFromAugust = 127L,
            YearFromSeptember = 128L,
            YearFromOctober = 129L,
            YearFromNovember = 130L,
            YearFromDecember = 131L,
            HalfYearFromJanuary = 60L,
            HalfYearFromFebruary = 61L,
            HalfYearFromMarch = 62L,
            HalfYearFromApril = 63L,
            HalfYearFromMay = 64L,
            HalfYearFromJune = 65L,
            QuarterYearFromJanuary = 30L,
            QuarterYearFromFebruary = 31L,
            QuarterYearFromMarch = 32L,
            BiMonthlyFromJanuary = 20L,
            BiMonthlyFromFebruary = 21L,
            OneMonth = 10L,
            OneWeekFromSunday = 1L,
            OneWeekFromMonday = 2L,
            OneWeekFromTuesday = 3L,
            OneWeekFromWednesday = 4L,
            OneWeekFromThursday = 5L,
            OneWeekFromFriday = 6L,
            OneWeekFromSaturday = 7L,
            OneDay = TimeSpan.TicksPerDay,
            TwelveHour = TimeSpan.TicksPerHour * 12L,
            SixHour = TimeSpan.TicksPerHour * 6L,
            ThreeHour = TimeSpan.TicksPerHour * 3L,
            OneHour = TimeSpan.TicksPerHour,
            HalfHour = TimeSpan.TicksPerMinute * 30L,
            QuarterHour = TimeSpan.TicksPerMinute * 15L,
            OneMinute = TimeSpan.TicksPerMinute,
            HalfMinute = TimeSpan.TicksPerSecond * 30L,
            QuarterMinute = TimeSpan.TicksPerSecond * 15L,
            OneSecond = TimeSpan.TicksPerSecond,
            TenthOfASecond = TimeSpan.TicksPerSecond / 10L,
            HundrethOfASecond = TimeSpan.TicksPerSecond / 100L,
            ThousandthOfASecond = TimeSpan.TicksPerSecond / 1000L
        }

        static private DateTime WorkMethod(DateTime dt, long ReturnType, TimeInterval Interval)
        {
            long Interval1 = (long)Interval;
            long TicksFromFloor = 0L;
            int IntervalFloor = 0;
            int FloorOffset = 0;
            int IntervalLength = 0;
            DateTime floorDate;
            DateTime ceilingDate;

            if (Interval1 > 132L) //Set variables to calculate date for time interval less than one day.
            {
                floorDate = new DateTime(dt.Ticks - (dt.Ticks % Interval1), dt.Kind);
                if (ReturnType != 0L)
                    TicksFromFloor = Interval1 / ReturnType;
            }
            else if (Interval1 < 8L) //Set variables to calculate date for time interval of one week.
            {
                IntervalFloor = (int)(Interval1) - 1;
                FloorOffset = (int)dt.DayOfWeek * -1;
                floorDate = new DateTime(dt.Year, dt.Month, dt.Day, 0, 0, 0, dt.Kind).AddDays(-(IntervalFloor > FloorOffset ? FloorOffset + 7 - IntervalFloor : FloorOffset - IntervalFloor));
                if (ReturnType != 0L)
                    TicksFromFloor = TimeSpan.TicksPerDay * 7L / ReturnType;
            }
            else //Set variables to calculate date for time interval one month or greater.
            {
                IntervalLength = Interval1 >= 130L ? 12 : (int)(Interval1 / 10L);
                IntervalFloor = (int)(Interval1 % IntervalLength);
                FloorOffset = (dt.Month - 1) % IntervalLength;
                floorDate = new DateTime(dt.Year, dt.Month, 1, 0, 0, 0, dt.Kind).AddMonths(-(IntervalFloor > FloorOffset ? FloorOffset + IntervalLength - IntervalFloor : FloorOffset - IntervalFloor));
                if (ReturnType != 0L)
                {
                    ceilingDate = floorDate.AddMonths(IntervalLength);
                    TicksFromFloor = (long)ceilingDate.Subtract(floorDate).Ticks / ReturnType;
                }
            }
            return floorDate.AddTicks(TicksFromFloor);
        }
    }
}
