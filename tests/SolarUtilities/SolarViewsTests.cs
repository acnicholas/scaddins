using NUnit.Framework;
using System;
using RTF.Applications;
using Autodesk.Revit.DB;
using RTF.Framework;

namespace SCaddins.SolarAnalysis.Tests
{
    [TestFixture()]
    public class SolarViewsTests
    {
        [Test()]
        [TestModel(@"./rac_basic_sample_project.rvt")]
        public void ViewNameIsAvailableTest()
        {
            var doc = RevitTestExecutive.CommandData.Application.ActiveUIDocument.Document;
            Assert.IsTrue(SolarAnalysisManager.ViewNameIsAvailable(doc, "This_view_name_is_not_used"));
            Assert.IsFalse(SolarAnalysisManager.ViewNameIsAvailable(doc, "Level 1"));
        }

        [Test()]
        [TestModel(@"./rac_basic_sample_project.rvt")]
        public void GetNiceViewNameTest()
        {
            var doc = RevitTestExecutive.CommandData.Application.ActiveUIDocument.Document;
            Assert.IsTrue(SolarAnalysisManager.GetNiceViewName(doc, "This_view_name_is_not_used") == "This_view_name_is_not_used");
            Assert.IsTrue(SolarAnalysisManager.GetNiceViewName(doc, "Level 1") != "Level 1");
        }

        [Test()]
        [TestModel(@"./rac_basic_sample_project.rvt")]
        public void GetHighestLevelTest()
        {
            var doc = RevitTestExecutive.CommandData.Application.ActiveUIDocument.Document;
            ElementId highestLevelId = SolarAnalysisManager.GetHighestLevel(doc);
            Element highestLevel = doc.GetElement(highestLevelId);
            Assert.IsTrue(highestLevel.Name == "Roof Line");
        }

        [Test()]
        [TestModel(@"./rac_basic_sample_project.rvt")]
        public void ViewIsIsoTest()
        {
            throw new NotImplementedException();
        }

        [Test()]
        [TestModel(@"./rac_basic_sample_project.rvt")]
        public void GoTest()
        {
            throw new NotImplementedException();
        }
    }
}
