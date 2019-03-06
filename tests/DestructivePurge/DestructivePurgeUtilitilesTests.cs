using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autodesk.Revit.DB;
using NUnit.Framework;
using RTF.Applications;
using RTF.Framework;
using SCaddins.DestructivePurge;


namespace SCaddins.DestructivePurge.Tests
{
    [TestFixture()]
    public class DestructivePurgeUtilitilesTests
    {
        [SetUp]
        public void Setup()
        {
            SCaddinsApp.WindowManager = new SCaddins.Common.WindowManager(new SCaddins.Common.MockDialogService());
        }

        [Test()]
        [TestModel(@"./scaddins_test_model.rvt")]
        public void ImagesTest()
        {
            var doc = RevitTestExecutive.CommandData.Application.ActiveUIDocument.Document;
            var imagesCount = DestructivePurgeUtilitiles.Images(doc).Count;
            Assert.IsTrue(imagesCount == 8);
        }

        [Test()]
        [TestModel(@"./scaddins_test_model.rvt")]
        public void ImportsTest()
        {
            Assert.Fail();
        }

        [Test()]
        [TestModel(@"./scaddins_test_model.rvt")]
        public void RemoveElementsTest()
        {
            Assert.Fail();
        }

        [Test()]
        [TestModel(@"./scaddins_test_model.rvt")]
        public void RevisionsTest()
        {
            var doc = RevitTestExecutive.CommandData.Application.ActiveUIDocument.Document;
            var revisionsCount = DestructivePurgeUtilitiles.Revisions(doc).Count;
            Assert.IsTrue(revisionsCount == 4);
        }


        ////[Test()]
        ////[TestModel(@"./scaddins_test_model.rvt")]
        ////public void ToBitmapImageTest()
        ////{
        ////    Assert.Fail();
        ////}

        [Test()]
        [TestModel(@"./scaddins_test_model.rvt")]
        public void UnboundRoomsTest()
        {
            var doc = RevitTestExecutive.CommandData.Application.ActiveUIDocument.Document;
            var unboundRoomsCount = DestructivePurgeUtilitiles.UnboundRooms(doc).Count;
            Assert.IsTrue(unboundRoomsCount == 1);
        }

        [Test()]
        [TestModel(@"./scaddins_test_model.rvt")]
        public void UnusedViewFiltersTest()
        {
            var doc = RevitTestExecutive.CommandData.Application.ActiveUIDocument.Document;
            var unusedViewFiltersCount = DestructivePurgeUtilitiles.UnboundRooms(doc).Count;
            Assert.IsTrue(unusedViewFiltersCount == 1);
        }

        [Test()]
        [TestModel(@"./scaddins_test_model.rvt")]
        public void ViewsTest()
        {
            Assert.Fail();
        }
    }
}