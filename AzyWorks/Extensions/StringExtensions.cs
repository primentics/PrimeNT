﻿using System;
using System.Collections.Generic;

using UnitsNet.Units;
using UnitsNet;

namespace AzyWorks.Extensions
{
    public static class StringExtensions
    {
        public static string CutAfterIndex(this string str, int index)
        {
            string s = "";

            for (int i = index; i < str.Length; i++)
            {
                s += str[i];
            }

            return s;
        }

        public static string CutToIndex(this string str, int index)
        {
            string s = "";

            for (int i = 0; i < index; i++)
            {
                s += str[i];
            }

            return s;
        }

        public static string CutByChar(this string str, char c, int lastIndex, out (int startIndex, int endIndex) index)
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

        public static string[] GetByChar(this string str, char c)
        {
            int lastIndex = 0;
            int strLastIndex = str.LastIndexOf(c);

            List<string> strs = new List<string>();

            while (strLastIndex != lastIndex)
            {
                strs.Add(CutByChar(str, c, lastIndex, out var indexes));

                lastIndex = indexes.endIndex;
            }

            return strs.ToArray();
        }

        public static string FloorFrequencyToString(this double cpuFrequencyMHz)
        {
            Frequency freq = new Frequency(cpuFrequencyMHz, FrequencyUnit.Megahertz);

            if (freq.Gigahertz > 0)
                return $"{Math.Floor(freq.Gigahertz)} GHz";
            else
                return $"{Math.Floor(freq.Megahertz)} MHz";
        }
    }
}