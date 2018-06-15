using NUnit.Framework;
using SCaddins.SolarUtilities;
using System;
using RTF.Applications;
using Autodesk.Revit.DB;
using RTF.Framework;
using System;
using NUnit.Framework;
using RTF.Framework;
using RTF.Applications;

using System;
using System.Collections.Generic;
using System.Linq;
using Autodesk.Revit.DB;
using NUnit.Framework;
using RTF.Applications;
using RTF.Framework;

namespace SCaddins.SolarUtilities.Tests
{
    [TestFixture()]
    public class SolarViewsTests
    {
        [Test()]
        [TestModel(@"./rac_basic_sample_project.rvt")]
        public void ViewNameIsAvailableTest()
        {
            var doc = RevitTestExecutive.CommandData.Application.ActiveUIDocument.Document;
            Assert.IsTrue(SolarViews.ViewNameIsAvailable(doc, "This_view_name_is_not_used"));
            Assert.IsFalse(SolarViews.ViewNameIsAvailable(doc, "Level 1"));
        }

        [Test()]
        [TestModel(@"./rac_basic_sample_project.rvt")]
        public void GetNiceViewNameTest()
        {
            var doc = RevitTestExecutive.CommandData.Application.ActiveUIDocument.Document;
            Assert.IsTrue(SolarViews.GetNiceViewName(doc, "This_view_name_is_not_used") == "This_view_name_is_not_used");
            Assert.IsTrue(SolarViews.GetNiceViewName(doc, "Level 1") != "Level 1");
        }

        [Test()]
        [TestModel(@"./rac_basic_sample_project.rvt")]
        public void GetHighestLevelTest()
        {
            var doc = RevitTestExecutive.CommandData.Application.ActiveUIDocument.Document;
            ElementId highestLevelId = SolarViews.GetHighestLevel(doc);
            Element highestLevel = doc.GetElement(highestLevelId);
            Assert.IsTrue(highestLevel.Name == "Roof Line");
        }

        [Test()]
        public void ViewIsIsoTest()
        {
            throw new NotImplementedException();
        }

        [Test()]
        public void GoTest()
        {
            throw new NotImplementedException();
        }
    }
}
