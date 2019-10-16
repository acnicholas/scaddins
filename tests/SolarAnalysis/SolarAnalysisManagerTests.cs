using System.Linq;
using Autodesk.Revit.DB;
using NUnit.Framework;
using RTF.Applications;
using RTF.Framework;
using SCaddins.SolarAnalysis;

namespace SCaddins.Tests.SolarAnalysis
{
    [TestFixture()]
    public class SolarAnalysisManagerTests
    {
        [Test()]
        public void CreateTestFacesTest()
        {
            Assert.Fail();
        }

        [Test()]
        public void GetAtmosphericRefractionTest()
        {
            Assert.Fail();
        }

        [Test()]
        [TestModel(@"./scaddins_test_model.rvt")]
        public void GetHighestLevelTest()
        {
            var doc = RevitTestExecutive.CommandData.Application.ActiveUIDocument.Document;
            ElementId highestLevelId = SolarAnalysisManager.GetHighestLevel(doc);
            Element highestLevel = doc.GetElement(highestLevelId);
            Assert.IsTrue(highestLevel.Name == "Ridge Level");
        }

        [Test()]
        [TestModel(@"./scaddins_test_model.rvt")]
        public void GetNiceViewNameTest()
        {
            var doc = RevitTestExecutive.CommandData.Application.ActiveUIDocument.Document;
            Assert.IsTrue(SolarAnalysisManager.GetNiceViewName(doc, "This_view_name_is_not_used") == "This_view_name_is_not_used");
            Assert.IsTrue(SolarAnalysisManager.GetNiceViewName(doc, "Level 1") != "Level 1");
        }

        [Test()]
        public void GetProjectPositionTest()
        {
            Assert.Fail();
        }

        ////[Test()]
        ////public void GetSæmundssonAtmosphericRefractionTest()
        ////{
        ////    Assert.Fail();
        ////}

        ////[Test()]
        ////public void GetSunDirectionalVectorTest()
        ////{
        ////    Assert.Fail();
        ////}

        [Test()]
        [TestModel(@"./scaddins_test_model.rvt")]
        public void GoTest()
        {
            var udoc = RevitTestExecutive.CommandData.Application.ActiveUIDocument;
            var doc = RevitTestExecutive.CommandData.Application.ActiveUIDocument.Document; ;
            var manager = new SolarAnalysisManager(udoc);
            //rotate an iso view.
            manager.RotateCurrentView = true;
            var views = new FilteredElementCollector(doc).OfClass(typeof(View)).ToElements().Cast<View>();
            var isoView = views.Where(v => v.Name == "3D View 01").First();
            SCaddins.Tests.Common.TestUtilities.OpenView(isoView);
            Assert.IsTrue(manager.Go(new ModelSetupWizard.TransactionLog("test")));

            //create solar analysis views (sun eye views)
            manager.RotateCurrentView = false;
            manager.CreateAnalysisView = true;
            Assert.IsTrue(manager.Go(new ModelSetupWizard.TransactionLog("test")));

            //create shadow plan
            manager.CreateShadowPlans = true;
            manager.CreateAnalysisView = false;
            Assert.IsTrue(manager.Go(new ModelSetupWizard.TransactionLog("test")));

        }

        ////[Test()]
        ////public void SolarAnalysisManagerTest()
        ////{
        ////    Assert.Fail();
        ////}

        [Test()]
        [TestModel(@"./scaddins_test_model.rvt")]
        public void ViewIsIsoTest()
        {
            var doc = RevitTestExecutive.CommandData.Application.ActiveUIDocument.Document;
            var views = new FilteredElementCollector(doc).OfClass(typeof(View)).ToElements().Cast<View>();
            var isoView = views.Where(v => v.Name == "3D View 01").First();
            var planView = views.Where(v => v.Name == "Level 1").First();
            Assert.IsTrue(SolarAnalysisManager.ViewIsIso(isoView), "View: " + isoView.Name + " is not an Isometric view.");
            Assert.IsFalse(SolarAnalysisManager.ViewIsIso(planView), "View: " + isoView.Name + " should not be an Isometric view.");
        }

        [Test()]
        [TestModel(@"./scaddins_test_model.rvt")]
        public void ViewIsSingleDayTest()
        {
            var doc = RevitTestExecutive.CommandData.Application.ActiveUIDocument.Document;
            var views = new FilteredElementCollector(doc).OfClass(typeof(View)).ToElements().Cast<View>();
            var singleDay = views.Where(v => v.Name == "3D View - Single Day").First();
            Assert.IsTrue(SolarAnalysisManager.ViewIsSingleDay(singleDay));
            var notSingleDay = views.Where(v => v.Name == "3D View - Not Single Day").First();
            Assert.IsFalse(SolarAnalysisManager.ViewIsSingleDay(notSingleDay));
        }

        [Test()]
        [TestModel(@"./scaddins_test_model.rvt")]
        public void ViewNameIsAvailableTest()
        {
            var doc = RevitTestExecutive.CommandData.Application.ActiveUIDocument.Document;
            Assert.IsTrue(SolarAnalysisManager.ViewNameIsAvailable(doc, "This_view_name_is_not_used"));
            Assert.IsFalse(SolarAnalysisManager.ViewNameIsAvailable(doc, "Level 1"));
            // Assert.IsFalse(SolarAnalysisManager.ViewNameIsAvailable(null, "Level 1"));
            // Assert.IsFalse(SolarAnalysisManager.ViewNameIsAvailable(doc, null));
        }
    }
}