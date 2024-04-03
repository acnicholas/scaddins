// (C) Copyright 2020-2021 by Andrew Nicholas
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

namespace SCaddins.ExportManager
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Globalization;
    using System.Linq;
    using Autodesk.Revit.DB;

    public class RecentExport
    {
        public static bool DeleteAll(Autodesk.Revit.DB.Document doc, ObservableCollection<ViewSetItem> allViewSheetSets)
        {
            var viewSetItems = GetAllUserViewSets(allViewSheetSets);
            using (Transaction transaction = new Transaction(doc))
            {
                if (transaction.Start("Delete alluser views") == TransactionStatus.Started)
                {
                    foreach (var vsi in viewSetItems)
                    {
#if REVIT2024 || REVIT2025
                        doc.Delete(new ElementId((long)vsi.Id));
#else
                        doc.Delete(new ElementId(vsi.Id));
#endif
                    }
                }
                else
                {
                    transaction.RollBack();
                    return false;
                }
                var r = transaction.Commit();
                return r == TransactionStatus.Committed ? true : false;
            }
        }

        public static bool DeleteOldest(Autodesk.Revit.DB.Document doc, ObservableCollection<ViewSetItem> allViewSheetSets)
        {
            var oldest = GetOldest(allViewSheetSets);
            if (oldest != null)
            {
                using (Transaction transaction = new Transaction(doc))
                {
                    if (transaction.Start("Delete oldest user export set") == TransactionStatus.Started)
                    {
#if REVIT2024 || REVIT2025
                        doc.Delete(new Autodesk.Revit.DB.ElementId((long)oldest.Id));
#else
                        doc.Delete(new Autodesk.Revit.DB.ElementId(oldest.Id));
#endif
                    }
                    else
                    {
                        transaction.RollBack();
                        return false;
                    }
                    var r = transaction.Commit();
                    return r == TransactionStatus.Committed ? true : false;
                }
            }
            return false;
        }

        public static List<ViewSetItem> GetAllUserViewSets(ObservableCollection<ViewSetItem> allViewSheetSets)
        {
            var result = allViewSheetSets.Where(v => v.Name.Contains(GetUserName())).ToList();
            foreach (var viewSetItem in result)
            {
                var date = TryParseDateString(viewSetItem);
                if (!string.IsNullOrEmpty(date))
                {
                    viewSetItem.CreationDate = DateTime.ParseExact(date, "yyyyMMddHHmmss", CultureInfo.InvariantCulture);
                    viewSetItem.DescriptiveName = viewSetItem.CreationDate.ToString("F") + @"[" + viewSetItem.NumberOfViews + @"]";
                }
                else
                {
                    SCaddinsApp.WindowManager.ShowMessageBox("Could not find user view (There was a probelm parsing the date)");
                }
            }
            return result;
        }

        public static ViewSetItem GetLatest(ObservableCollection<ViewSetItem> allViewSheetSets)
        {
            var userViewSets = GetAllUserViewSets(allViewSheetSets);
            if (userViewSets.Count() == 0)
            {
                return null;
            }

            var latest = userViewSets.First();

            foreach (var viewSet in userViewSets)
            {
                var latestDateInt = TryGetDateInt(latest);
                var dateInt = TryGetDateInt(viewSet);
                if (dateInt > 0 && dateInt > latestDateInt)
                {
                    latest = viewSet;
                }
            }

            return latest;
        }

        public static string GetName()
        {
            var dateString = Common.MiscUtilities.GetVerboseDateString;
            return GetUserName() + "-" + dateString;
        }

        public static void Save(Manager manager, List<ExportSheet> selection)
        {
            if (GetAllUserViewSets(manager.AllViewSheetSets).Count > 4)
            {
                DeleteOldest(manager.Doc, manager.AllViewSheetSets);
            }
            manager.SaveViewSet(GetName(), selection);
        }

        private static ViewSetItem GetOldest(ObservableCollection<ViewSetItem> allViewSheetSets)
        {
            var userViewSets = GetAllUserViewSets(allViewSheetSets);
            if (userViewSets.Count() < 0)
            {
                return null;
            }

            var oldest = userViewSets.First();

            foreach (var viewSet in userViewSets)
            {
                var oldsetDateInt = TryGetDateInt(oldest);
                var dateInt = TryGetDateInt(viewSet);
                if (dateInt > 0 && dateInt < oldsetDateInt)
                {
                    oldest = viewSet;
                }
            }

            return oldest;
        }

        private static string GetUserName()
        {
            return System.Environment.UserName;
        }

        private static string TryParseDateString(ViewSetItem viewSetItem)
        {
            if (!viewSetItem.Name.Contains("-"))
            {
                return string.Empty;
            }

            var dateString = viewSetItem.Name.Substring(viewSetItem.Name.IndexOf("-") + 1);
            if (dateString.Length != 14)
            {
                return string.Empty;
            }

            return dateString;
        }

        private static long TryGetDateInt(ViewSetItem viewSetItem)
        {
            var dateString = TryParseDateString(viewSetItem);
            if (string.IsNullOrEmpty(dateString))
            {
                return -1;
            }

            long dateInt = -1;
            long.TryParse(dateString, out dateInt);

            return dateInt;
        }
    }
}
