using Autodesk.Revit.DB;
using NUnit.Framework;
using RTF.Applications;
using RTF.Framework;
using SCaddins.RevisionUtilities;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System;

namespace SCaddins.RevisionUtilities.Tests
{
    [TestFixture()]
    public class RevisionUtilitiesTests
    {
        [Test()]
        [TestModel(@"./scaddins_test_model.rvt")]
        public void GetRevisionCloudsTest()
        {
            var doc = RevitTestExecutive.CommandData.Application.ActiveUIDocument.Document;
            var clouds = Manager.GetRevisionClouds(doc);
            Assert.IsTrue(clouds.Count == 5);
        }

        [Test()]
        [TestModel(@"./scaddins_test_model.rvt")]
        public void GetRevisionsTest()
        {
            var doc = RevitTestExecutive.CommandData.Application.ActiveUIDocument.Document;
            var revisions = Manager.GetRevisions(doc);
            Assert.IsTrue(revisions.Count == 5);
        }

        [Test()]
        [TestModel(@"./scaddins_test_model.rvt")]
        public void AssignRevisionToCloudsTest()
        {
            var doc = RevitTestExecutive.CommandData.Application.ActiveUIDocument.Document;
            var revision = Manager.GetRevisions(doc).Where(r => r.Description == "DA Issue").First();
            Manager.AssignRevisionToClouds(doc, Manager.GetRevisionClouds(doc), revision.Id);
            var count = Manager.GetRevisionClouds(doc).Where(c => c.Revision.Equals(revision)).Count();
            Assert.IsTrue(count == 5);
        }

        [Test()]
        [TestModel(@"./scaddins_test_model.rvt")]
        public void DeleteRevisionCloudsTest()
        {
            var doc = RevitTestExecutive.CommandData.Application.ActiveUIDocument.Document;
            Manager.DeleteRevisionClouds(doc, Manager.GetRevisionClouds(doc));
            Assert.IsTrue(Manager.GetRevisionClouds(doc).Count == 0);
        }

        [SetUp]
        public void Setup()
        {
            SCaddinsApp.WindowManager = new SCaddins.Common.WindowManager(new SCaddins.Common.MockDialogService());
        }
    }
}