using System;
using System.Globalization;

namespace Sonolib.Extensions
{
    internal static class InternalStringExtensions
    {
        /// <summary>
        /// Converts string representation of TimeSpan to int value or returns default value
        /// </summary>
        /// <param name="tsValue"></param>
        /// <param name="defaultValue">default value</param>
        /// <returns>TimeSpan representation of string value</returns>
        public static TimeSpan ToSeconds(this string tsValue, int defaultValue)
        {
            return TimeSpan.FromSeconds(tsValue.ToDouble(defaultValue));
        }
        
        /// <summary>
        /// Converts string to double or returns default value
        /// </summary>
        /// <param name="stringDouble">string value</param>
        /// <param name="defaultValue">default value</param>
        /// <returns>double representation of string value</returns>
        public static double ToDouble(this string stringDouble, double defaultValue)
        {
            return double.TryParse(stringDouble, out var d) ? d : defaultValue;
        }
        
        /// <summary>
        /// Converts string to int value or returns default value
        /// </summary>
        /// <param name="stringInt">string value</param>
        /// <param name="defaultValue">default value</param>
        /// <returns>int representation of string value</returns>
        public static int ToInt(this string stringInt, int defaultValue)
        {
            return int.TryParse(stringInt, out var d) ? d : defaultValue;
        }
        
        /// <summary>
        /// Capitalize 1st char in word
        /// </summary>
        /// <param name="s"></param>
        /// <returns></returns>
        public static string Capitalize(this string s)
        {
            return CultureInfo.CurrentCulture.TextInfo.ToTitleCase(s);
        }
        
        /// <summary>
        /// Remove s chars from src
        /// </summary>
        /// <param name="src"></param>
        /// <param name="s"></param>
        /// <returns></returns>
        public static string Remove(this string src, string s)
        {
            return src.Replace(s, "");
        }

        public static bool HasValue(this string str)
        {
            return !string.IsNullOrWhiteSpace(str);
        }
    }
}