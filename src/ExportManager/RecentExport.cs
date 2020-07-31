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
        public static void DeleteOldest(Autodesk.Revit.DB.Document doc, ObservableCollection<ViewSetItem> allViewSheetSets)
        {
            var oldest = GetOldest(allViewSheetSets);
            if (oldest != null)
            {
                using (Transaction transaction = new Transaction(doc))
                {
                    if (transaction.Start("Delete oldest user export set") == TransactionStatus.Started)
                    {
                        doc.Delete(new Autodesk.Revit.DB.ElementId(oldest.Id));
                    }
                    else
                    {
                        transaction.RollBack();
                    }
                    transaction.Commit();
                }
            }
        }

        public static List<ViewSetItem> GetAllUserViewSets(ObservableCollection<ViewSetItem> allViewSheetSets)
        {
            var result = allViewSheetSets.Where(v => v.Name.Contains(GetUserName())).ToList();
            foreach (var viewSetItem in result)
            {
                var date = TryParseDateString(viewSetItem);
                if (!string.IsNullOrEmpty(date)) {
                    viewSetItem.CreationDate = DateTime.ParseExact(date, "yyyyMMddHHmmss", CultureInfo.InvariantCulture);
                    viewSetItem.DescriptiveName = viewSetItem.CreationDate.ToString("F")  + @"[" + viewSetItem.NumberOfViews + @"]";
                } else
                {
                    SCaddinsApp.WindowManager.ShowMessageBox("noped out");
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
            SCaddinsApp.WindowManager.ShowMessageBox(GetUserName() + "-" + dateString);
            return GetUserName() + "-" + dateString;
        }

        public static void Save(Manager manager, List<ExportSheet> selection)
        {
            if (GetAllUserViewSets(manager.AllViewSheetSets).Count > 2)
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
