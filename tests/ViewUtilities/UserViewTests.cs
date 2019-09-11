
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autodesk.Revit.DB;
using NUnit.Framework;
using RTF.Applications;
using RTF.Framework;
using SCaddins.ViewUtilities;
using SCaddins.Tests.Common;


namespace SCaddins.ViewUtilities.Tests
{
    [TestFixture()]
    public class UserViewTests
    {
        [SetUp]
        public void Setup()
        {
            SCaddinsApp.WindowManager = new SCaddins.Common.WindowManager(new SCaddins.Common.MockDialogService());
        }

        [Test()]
        [TestModel(@"./scaddins_test_model.rvt")]
        public void CreateTestCurrentView()
        {
            var doc = RevitTestExecutive.CommandData.Application.ActiveUIDocument.Document;
            var view = new FilteredElementCollector(doc)
               .OfClass(typeof(View))
               .ToElements()
               .Cast<View>()
               .Where(v => v.Name == "Level 1")
               .First();
            SCaddins.Tests.Common.TestUtilities.OpenView(view);
            var newUserViewCount = 0;
            using (Transaction t = new Transaction(doc, "CreateTestViewSelection"))
            {
                if (t.Start() == TransactionStatus.Started)
                {
                    newUserViewCount = UserView.Create(view, doc).Count;
           
                }
                else
                {
                    t.RollBack();
                }
            }
            Assert.IsTrue(newUserViewCount == 1);
        }

        [Test()]
        [TestModel(@"./scaddins_test_model.rvt")]
        public void CreateTestViewSelection()
        {
            var uidoc = RevitTestExecutive.CommandData.Application.ActiveUIDocument;
            var doc = uidoc.Document;
            var sectionSheets = new FilteredElementCollector(doc)
                .OfClass(typeof(View))
                .ToElements()
                .Cast<View>()
                .Where(v => v.Name.Contains("Section") && v.ViewType == ViewType.DrawingSheet);
            var manager = new ExportManager.Manager(uidoc);
            var sheet = new ExportManager.ExportSheet(sectionSheets.First() as ViewSheet, doc, manager.FileNameScheme, false, manager);
            var sheetList = new List<ExportManager.ExportSheet>();
            sheetList.Add(sheet);
            ////using (Transaction t = new Transaction(doc, "CreateTestViewSelection"))
            ////{
            ////    if (t.Start() == TransactionStatus.Started)
            ////    {
                    var newUserViewCount = UserView.Create(sheetList, doc).Count;
                    Assert.IsTrue(newUserViewCount == 2);
            ////    } else
            ////    {
            ////        t.RollBack();
            ////    }
            ////}
            ////Assert.Fail();
        }

        ////[Test()]
        ////public void ShowSummaryDialogTest()
        ////{
        ////    Assert.Fail();
        ////}
    }
}