using NUnit.Framework;
using SCaddins.LineOfSight;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;
using SCaddins.SolarUtilities;
using System;
using RTF.Applications;
using Autodesk.Revit.DB;
using RTF.Framework;

namespace SCaddins.LineOfSight.Tests
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
            LineOfSight los = new LineOfSight(doc);
            los.Draw();
            fec = new FilteredElementCollector(doc, los.View.Id).OfClass(typeof(DetailLine));
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
                    LineOfSight los = new LineOfSight(doc);
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

        [Test()]
        [TestCaseSource("ManyCValues")]
        public void GetCValueTest()
        {
            //var doc = RevitTestExecutive.CommandData.Application.ActiveUIDocument.Document;
            //LineOfSight los = new LineOfSight(doc, 1200, 900, 15, 60, 180, 20, 12000, 1000);
            Assert.Fail();
        }

        private static List<object[]> ManyCValues()
        {
            var testParams = new List<object[]>
            {
                new object[] {1, 1},
                new object[] {2, 2},
                new object[] {3, 3}
            };

            return testParams;
        }
    }
}