using System;
using Autodesk.Revit.DB;
using NUnit.Framework;
using RTF.Applications;
using RTF.Framework;
using SCaddins.LineOfSight;

namespace SCaddins.Tests.LineOfSight
{
    [TestFixture()]
    public class LineOfSightTests
    {
        [Test()]
        [TestModel(@"./rac_basic_sample_project.rvt")]
        public void DrawTest()
        {
            var doc = RevitTestExecutive.CommandData.Application.ActiveUIDocument.Document;
            FilteredElementCollector fec;
            StadiumSeatingTier los = new StadiumSeatingTier(doc);
            los.Draw();
            fec = new FilteredElementCollector(doc, los.View.Id).OfClass(typeof(CurveElement));
            Assert.IsTrue(fec.ToElements().Count > 0);
        }

        [Test()]
        [TestModel(@"./rac_basic_sample_project.rvt")]
        public void CreateLineOfSightDraftingViewTest()
        {
            ViewDrafting view = null;
            var doc = RevitTestExecutive.CommandData.Application.ActiveUIDocument.Document;
            using (var t = new Transaction(doc)) {
                if (t.Start("CreateLineOfSightDraftingViewTest") == TransactionStatus.Started) {
                    StadiumSeatingTier los = new StadiumSeatingTier(doc);
                    view = los.CreateLineOfSightDraftingView("LOS test view 01");
                    if (t.Commit() != TransactionStatus.Committed) {
                        t.RollBack();
                    }
                } else {
                    throw new Exception("Transaction could not be started.");
                }
            }
            Assert.NotNull(view);
        }

//        [Test()]
//        [TestCaseSource("ManyCValues")]
//        public void GetCValueTest(int a, int b)
//        {
//            var doc = RevitTestExecutive.CommandData.Application.ActiveUIDocument.Document;
//            StadiumSeatingTier los = new StadiumSeatingTier(doc, 1200, 900, 15, 60, 180, 20, 12000, 1000);
//            //Assert.Fail();
//            Assert.Pass();
//        }

        [SetUp]
        public void Setup()
        {
            SCaddinsApp.WindowManager = new SCaddins.Common.WindowManager(new SCaddins.Common.MockDialogService());
        }

//        private static List<object[]> ManyCValues()
//        {
//            var testParams = new List<object[]>
//            {
//                new object[] {1, 60},
//                new object[] {2, 90},
//                new object[] {3, 100}
//            };
//
//            return testParams;
//        }
    }
}