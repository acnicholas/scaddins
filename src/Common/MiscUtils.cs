// (C) Copyright 2012-2015 by Andrew Nicholas
//
// This file is part of SCaddins.
//
// SCaddins is free software: you can redistribute it and/or modify
// it under the terms of the GNU Lesser General Public License as published by
// the Free Software Foundation, either version 3 of the License, or
// (at your option) any later version.
//
// SCaddins is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU Lesser General Public License for more details.
//
// You should have received a copy of the GNU Lesser General Public License
// along with SCaddins.  If not, see <http://www.gnu.org/licenses/>.

namespace SCaddins.Common
{
    using System;
    using System.Diagnostics;
    using System.Globalization;

    public static class MiscUtilities
    {
        public static string GetDateString {
            get {
                DateTime moment = DateTime.Now;
                string syear = moment.Year.ToString(CultureInfo.CurrentCulture);
                string smonth = PadLeftZeros(moment.Month.ToString(CultureInfo.CurrentCulture), 2);
                string sday = PadLeftZeros(moment.Day.ToString(CultureInfo.CurrentCulture), 2);
                return syear + smonth + sday;
            }
        }

        public static string PadLeftZeros(string s, int desiredLength)
        {
            if (desiredLength > 1 && s.Length == desiredLength - 1) {
                return "0" + s;
            }
            return s;
        }

        public static string GetSafeFileName(string fileName)
        {
            if (fileName == null) {
                return null;
            }
            foreach (char c in System.IO.Path.GetInvalidFileNameChars()) {
                fileName = fileName.Replace(c, '_');
            }
            return fileName;
        }

        public static double MillimetersToFeet(double lengthInMM)
        {
            return lengthInMM / 304.8;
        }

        public static DateTime ToDateTime(string dateValue)
        {
            var date = dateValue.Trim();
            const string DateDelimiters = @"-.\/_ ";
            char[] c = DateDelimiters.ToCharArray();
            int d2 = date.LastIndexOfAny(c);
            int d1 = date.IndexOfAny(c);

            try {
                string year = string.Empty;
                string month = string.Empty;
                string day = string.Empty;
                if (date.Length > d2 + 1) {
                    year = date.Substring(d2 + 1);
                }
                if (date.Length > (d1 + 1) && (d2 - d1 - 1) < date.Length - (d1 + 1)) {
                    month = date.Substring(d1 + 1, d2 - d1 - 1);
                }
                if (date.Length > 0 && d1 <= date.Length) {
                    day = date.Substring(0, d1);
                }
                return new DateTime(
                    Convert.ToInt32(year, CultureInfo.InvariantCulture),
                    Convert.ToInt32(month, CultureInfo.InvariantCulture),
                    Convert.ToInt32(day, CultureInfo.InvariantCulture));
            } catch (ArgumentOutOfRangeException e) { 
                Debug.WriteLine("Error in ToDateTime:" + e.Message);
                return new DateTime();
            } catch (FormatException e) {
                Debug.WriteLine("Error in ToDateTime:" + e.Message);
                return new DateTime(); 
            } catch (OverflowException e) {
                Debug.WriteLine("Error in ToDateTime:" + e.Message);
                return new DateTime(); 
            }
        }
    }
}
