using System.Globalization;
using System;
using System.Linq;

namespace GlueGames.Utilities
{
    public static class Parse
    {
        public static int Int(string str)
        {
            return int.Parse(str, NumberStyles.AllowDecimalPoint);
        }

        public static int Int(string str, int defaultValue)
        {
            return int.TryParse(str, NumberStyles.AllowDecimalPoint, CultureInfo.InvariantCulture, out var result)
                ? result
                : defaultValue;
        }

        public static long Long(string str)
        {
            return long.Parse(str, NumberStyles.AllowDecimalPoint);
        }

        public static bool BooleanInt(string str, bool defaultValue)
        {
            string[] BooleanStringOff = { "0", "off", "no" };
            if (string.IsNullOrEmpty(str))
                return defaultValue;
            if (BooleanStringOff.Contains(str, StringComparer.InvariantCultureIgnoreCase))
                return false;

            if (!bool.TryParse(str, out bool result))
                result = true;

            return result;
        }

        public static bool BooleanInt(string str)
        {
            return BooleanInt(str, false);
        }

        public static long Long(string str, long defaultValue)
        {
            return long.TryParse(str, NumberStyles.AllowDecimalPoint, CultureInfo.InvariantCulture, out var result)
                ? result
                : defaultValue;
        }

        public static float Float(string str)
        {
            return float.Parse(str);
        }

        public static float Float(string str, float defaultValue)
        {
            return float.TryParse(str, out var result)
                ? result
                : defaultValue;
        }

        public static DateTime DateTime(string str)
        {
            System.DateTime.TryParse(str, out System.DateTime result);
            return result;
        }

        public static T Enum<T>(string str) where T : struct
        {
            return (T)System.Enum.Parse(typeof(T), str);
        }

        public static T Enum<T>(int value) where T : struct
        {
            return (T)System.Enum.ToObject(typeof(T), value);
        }

        public static TimeSpan TimeSpan(string timerString)
        {
            string[] timer = timerString.Split(',');
            int hours = Parse.Int(timer[0]);
            int minutes = Parse.Int(timer[1]);
            int seconds = Parse.Int(timer[2]);
            return new TimeSpan(hours, minutes, seconds);
        }

        public static bool TryParse(string[] columns, int index, out int value)
        {
            value = default;
            return null != columns
                && index < columns.Length
                && int.TryParse(columns[index], out value);
        }

        public static bool TryParse(string[] columns, int index, out long value)
        {
            value = default;
            return null != columns
                && index < columns.Length
                && long.TryParse(columns[index], out value);
        }

        public static bool TryParse(string[] columns, int index, out float value)
        {
            value = default;
            return null != columns
                && index < columns.Length
                && float.TryParse(columns[index], out value);
        }
    }
}
