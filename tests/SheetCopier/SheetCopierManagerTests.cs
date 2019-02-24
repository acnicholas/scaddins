using NUnit.Framework;
using SCaddins.SheetCopier;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;
using RTF.Framework;
using System.IO;
using RTF.Applications;
using Autodesk.Revit.DB;

namespace SCaddins.SheetCopier.Tests
{
    [TestFixture()]
    public class SheetCopierManagerTests
    {
        [SetUp]
        public void Setup()
        {
            SCaddinsApp.WindowManager = new SCaddins.Common.WindowManager(new SCaddins.Common.MockDialogService());
        }

        [Test()]
        [TestModel(@"./rac_basic_sample_project.rvt")]
        public void SheetNumberAvailableTest()
        {
            var uidoc = RevitTestExecutive.CommandData.Application.ActiveUIDocument;
            var manager = new SheetCopier.SheetCopierManager(uidoc);
            //test some used numbers
            Assert.IsFalse(manager.SheetNumberAvailable("A01"));
            Assert.IsFalse(manager.SheetNumberAvailable("A99"));
            //test some available numbers
            Assert.IsTrue(manager.SheetNumberAvailable("A100"));
            //test some badly formatted numbers
            Assert.IsFalse(manager.SheetNumberAvailable(@"{!@#$%^&*("));
            Assert.IsFalse(manager.SheetNumberAvailable(string.Empty));
            Assert.IsFalse(manager.SheetNumberAvailable(null));

        }

        [Test()]
        [TestModel(@"./rac_basic_sample_project.rvt")]
        public void ViewNameAvailableTest()
        {
            var uidoc = RevitTestExecutive.CommandData.Application.ActiveUIDocument;
            var manager = new SheetCopier.SheetCopierManager(uidoc);
            //test some used view names
            Assert.IsFalse(manager.ViewNameAvailable("A01"));
            Assert.IsFalse(manager.ViewNameAvailable("A99"));
            //test some view names
            Assert.IsTrue(manager.ViewNameAvailable("A100"));
            //test some badly view names
            Assert.IsFalse(manager.ViewNameAvailable(@"{!@#$%^&*("));
            Assert.IsFalse(manager.ViewNameAvailable(string.Empty));
            Assert.IsFalse(manager.ViewNameAvailable(null));
        }

        [Test()]
        public void AddCurrentSheetTest()
        {
            Assert.Fail();
        }

        [Test()]
        public void AddSheetTest()
        {
            Assert.Fail();
        }

        [Test()]
        public void GetAllViewsInModelTest()
        {
            Assert.Fail();
        }

        [Test()]
        public void CreateAndPopulateNewSheetTest()
        {
            Assert.Fail();
        }

        [Test()]
        public void AddEmptySheetToDocumentTest()
        {
            Assert.Fail();
        }

        [Test()]
        public void PlaceViewPortOnSheetTest()
        {
            Assert.Fail();
        }

        [Test()]
        public void GetNewSheetNumberTest()
        {
            Assert.Fail();
        }

        [Test()]
        public void TryAssignViewTemplateTest()
        {
            Assert.Fail();
        }

        [Test()]
        public void PlaceNewViewOnSheetTest()
        {
            Assert.Fail();
        }

        [Test()]
        public void GetAllHiddenRevisionsTest()
        {
            Assert.Fail();
        }

        [Test()]
        public void DeleteRevisionCloudsTest()
        {
            Assert.Fail();
        }

        [Test()]
        public void DuplicateViewOntoSheetTest()
        {
            Assert.Fail();
        }

        [Test()]
        public void CopyElementsBetweenSheetsTest()
        {
            Assert.Fail();
        }

        [Test()]
        public void CreateViewportsTest()
        {
            Assert.Fail();
        }
    }
}