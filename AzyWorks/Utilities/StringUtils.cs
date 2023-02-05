using System;
using System.Collections.Generic;

using UnitsNet.Units;
using UnitsNet;

namespace AzyWorks.Utilities
{
    public static class StringUtils
    {
        public static IReadOnlyList<string> SizeSuffixes { get; } = new List<string>() { "byte(s)", "KB", "MB", "GB", "TB", "PB", "EB", "ZB", "YB" };

        public static string CutStringAfterIndex(string str, int index)
        {
            string s = "";

            for (int i = index; i < str.Length; i++)
            {
                s += str[i];
            }

            return s;
        }

        public static string CutStringToIndex(string str, int index)
        {
            string s = "";

            for (int i = 0; i < index; i++)
            {
                s += str[i];
            }

            return s;
        }

        public static string CutStringByChar(string str, char c, int lastIndex, out (int startIndex, int endIndex) index)
        {
            int startIndex = 0;
            int endIndex = 0;

            for (int i = lastIndex; i < str.Length; i++)
            {
                if (str[i] == c)
                {
                    if (startIndex == 0)
                        startIndex = i;
                    else
                        endIndex = i;
                }

                if (startIndex != 0 && endIndex != 0)
                    break;
            }

            index.startIndex = startIndex;
            index.endIndex = endIndex;

            return str.Substring(startIndex, endIndex - startIndex);
        }

        public static string[] GetStringsByChar(string str, char c)
        {
            int lastIndex = 0;
            int strLastIndex = str.LastIndexOf(c);

            List<string> strs = new List<string>();
            
            while (strLastIndex != lastIndex)
            {
                strs.Add(CutStringByChar(str, c, lastIndex, out var indexes));

                lastIndex = indexes.endIndex;
            }

            return strs.ToArray();
        }

        public static string FloorFrequencyToString(double cpuFrequencyMHz)
        {
            Frequency freq = new Frequency(cpuFrequencyMHz, FrequencyUnit.Megahertz);

            if (freq.Gigahertz > 0)
                return $"{Math.Floor(freq.Gigahertz)} GHz";
            else
                return $"{Math.Floor(freq.Megahertz)} MHz";
        }

        public static string AddSizeSuffixToStringEnd(double value, int decimalPlaces = 1)
        {
            if (decimalPlaces < 0)
                throw new ArgumentOutOfRangeException("decimalPlaces");
            if (value < 0)
                return "-" + AddSizeSuffixToStringEnd(-value, decimalPlaces);
            if (value == 0)
                return string.Format("{0:n" + decimalPlaces + "} byte(s)", 0);

            int mag = (int)Math.Log(value, 1024);
            decimal adjustedSize = (decimal)value / (1L << (mag * 10));

            if (Math.Round(adjustedSize, decimalPlaces) >= 1000)
            {
                mag += 1;
                adjustedSize /= 1024;
            }

            return string.Format("{0:n" + decimalPlaces + "} {1}",
                adjustedSize,
                SizeSuffixes[mag]);
        }
    }
}