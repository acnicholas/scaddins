using Autodesk.Revit.DB;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SCaddins.ExportManager
{
    class RecentExport
    {
        //public static List<ExportSheet> GetLastExport()
        //{
        //    return null;
        //}

        public static string GetLastExportName()
        {
            return System.Environment.UserName + "- date";
        }

        public static string GetName()
        {
            var dateString = Common.MiscUtilities.GetVerboseDateString;
            SCaddinsApp.WindowManager.ShowMessageBox(GetUserName() + "-" + dateString);
            return GetUserName() + "-" + dateString;
        }

        public static string GetUserName()
        {
            return System.Environment.UserName;
        }

        public static long TryGetDateInt(ViewSetItem viewSetItem)
        {
            if (!viewSetItem.Name.Contains("-"))
            {
                return -1;
            }

            var dateString = viewSetItem.Name.Substring(viewSetItem.Name.IndexOf("-"));
            SCaddinsApp.WindowManager.ShowMessageBox(dateString);
            if (dateString.Length != 14) //yyyymmddhhmmss
            {
                return -1;
            }

            long dateInt = -1;
            long.TryParse(dateString, out dateInt);

            return dateInt;
        }

        public static ViewSetItem GetOldest(ObservableCollection<ViewSetItem> allViewSheetSets)
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

        public static void Save(Manager manager, List<ExportSheet> selection)
        {
            if (GetAllUserViewSets(manager.AllViewSheetSets).Count > 2)
            {
                DeleteOldest(manager.Doc, manager.AllViewSheetSets);
            }
            manager.SaveViewSet(GetName(), selection);
        }

        public static void DeleteOldest(Autodesk.Revit.DB.Document doc, ObservableCollection<ViewSetItem> allViewSheetSets)
        {
            var oldest = GetOldest(allViewSheetSets);
            if(oldest != null)
            {
                using (Transaction transaction = new Transaction(doc))
                {
                    if (transaction.Start("Delete oldest user export set") == TransactionStatus.Started)
                    {
                        doc.Delete(new Autodesk.Revit.DB.ElementId(oldest.Id));
                    } else
                    {
                        transaction.RollBack();
                    }
                    transaction.Commit();
                }
            }
        }


        public static List<ViewSetItem> GetAllUserViewSets(ObservableCollection<ViewSetItem> allViewSheetSets)
        {
            return allViewSheetSets.Where(v => v.Name.Contains(GetUserName())).ToList();
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

    }
}
