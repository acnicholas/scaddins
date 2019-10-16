using System.Linq;
using NUnit.Framework;
using RTF.Applications;
using RTF.Framework;
using SCaddins.SheetCopier;

namespace SCaddins.Tests.SheetCopier
{
    [TestFixture()]
    public class SheetCopierManagerTests
    {
        [Test()]
        [TestModel(@"./scaddins_test_model.rvt")]
        public void AddCurrentSheetTest()
        {
            Assert.Fail();
        }

        [Test()]
        [TestModel(@"./scaddins_test_model.rvt")]
        public void AddEmptySheetToDocumentTest()
        {
            Assert.Fail();
        }

        [Test()]
        [TestModel(@"./scaddins_test_model.rvt")]
        public void AddSheetTest()
        {
            Assert.Fail();
        }

        [Test()]
        [TestModel(@"./scaddins_test_model.rvt")]
        public void CopyElementsBetweenSheetsTest()
        {
            Assert.Fail();
        }

        [Test()]
        [TestModel(@"./scaddins_test_model.rvt")]
        public void CreateAndPopulateNewSheetTest()
        {
            Assert.Fail();
        }

        [Test()]
        [TestModel(@"./scaddins_test_model.rvt")]
        public void CreateViewportsTest()
        {
            Assert.Fail();
        }

        [Test()]
        [TestModel(@"./scaddins_test_model.rvt")]
        public void DeleteRevisionCloudsTest()
        {
            var doc = RevitTestExecutive.CommandData.Application.ActiveUIDocument.Document;
            // SheetCopierManager.DeleteRevisionClouds()
            Assert.Fail();
        }

        [Test()]
        [TestModel(@"./scaddins_test_model.rvt")]
        public void DuplicateViewOntoSheetTest()
        {
            var uidoc = RevitTestExecutive.CommandData.Application.ActiveUIDocument;
            var manager = new SCaddins.SheetCopier.SheetCopierManager(uidoc);
            Assert.Fail();
        }

        [Test()]
        [TestModel(@"./scaddins_test_model.rvt")]
        public void GetAllHiddenRevisionsTest()
        {
            var doc = RevitTestExecutive.CommandData.Application.ActiveUIDocument.Document;
            Assert.AreEqual(SheetCopierManager.GetAllHiddenRevisions(doc).Count, 0);
        }

        [Test()]
        [TestModel(@"./scaddins_test_model.rvt")]
        public void GetAllViewsInModelTest()
        {
            var uidoc = RevitTestExecutive.CommandData.Application.ActiveUIDocument;
            var manager = new SCaddins.SheetCopier.SheetCopierManager(uidoc);
            Assert.AreEqual(manager.ExistingViews.Count, 32);
        }

        [Test()]
        [TestModel(@"./scaddins_test_model.rvt")]
        public void GetNewSheetNumberTest()
        {
            var uidoc = RevitTestExecutive.CommandData.Application.ActiveUIDocument;
            var manager = new SCaddins.SheetCopier.SheetCopierManager(uidoc);
            var testNumber = manager.GetNewSheetNumber("A210");
            var testNumber2 = manager.GetNewSheetNumber("A210");
            var testNumber3 = manager.GetNewSheetNumber(null);
            Assert.IsTrue(testNumber == "A210-1");
            Assert.IsTrue(testNumber2 == "A210-2");
            Assert.IsNull(testNumber3);
        }

        [Test()]
        [TestModel(@"./scaddins_test_model.rvt")]
        public void PlaceNewViewOnSheetTest()
        {
            Assert.Fail();
        }

        [Test()]
        [TestModel(@"./scaddins_test_model.rvt")]
        public void PlaceViewPortOnSheetTest()
        {
            Assert.Fail();
        }

        [SetUp]
        public void Setup()
        {
            SCaddinsApp.WindowManager = new SCaddins.Common.WindowManager(new SCaddins.Common.MockDialogService());
        }

        [Test()]
        [TestModel(@"./scaddins_test_model.rvt")]
        public void SheetNumberAvailableTest()
        {
            var uidoc = RevitTestExecutive.CommandData.Application.ActiveUIDocument;
            var manager = new SCaddins.SheetCopier.SheetCopierManager(uidoc);
            //test some used numbers
            Assert.IsFalse(manager.SheetNumberAvailable("A001"));
            Assert.IsFalse(manager.SheetNumberAvailable("A210"));
            //test some available numbers
            Assert.IsTrue(manager.SheetNumberAvailable("A100"));
            //test some badly formatted numbers
            // Assert.IsFalse(manager.SheetNumberAvailable(@"{!@#$%^&*("));
            Assert.IsFalse(manager.SheetNumberAvailable(string.Empty));
            Assert.IsFalse(manager.SheetNumberAvailable(null));

        }

        [Test()]
        [TestModel(@"./scaddins_test_model.rvt")]
        public void TryAssignViewTemplateTest()
        {
            var uidoc = RevitTestExecutive.CommandData.Application.ActiveUIDocument;
            var manager = new SCaddins.SheetCopier.SheetCopierManager(uidoc);
            var view = manager.ExistingViews.Where(v => v.Key == "Level 1").First().Value;
            manager.TryAssignViewTemplate(view, "Architectural Plan");
            Assert.Pass();
        }

        [Test()]
        [TestModel(@"./scaddins_test_model.rvt")]
        public void ViewNameAvailableTest()
        {
            var uidoc = RevitTestExecutive.CommandData.Application.ActiveUIDocument;
            var manager = new SCaddins.SheetCopier.SheetCopierManager(uidoc);
            //test some used view names
            Assert.IsFalse(manager.ViewNameAvailable("Level 1"));
            Assert.IsFalse(manager.ViewNameAvailable("East"));
            //test some view names
            Assert.IsTrue(manager.ViewNameAvailable("Hello World"));
            //test some badly view names
            // Assert.IsFalse(manager.ViewNameAvailable(@"{!@#$%^&*("));
            Assert.IsFalse(manager.ViewNameAvailable(string.Empty));
            Assert.IsFalse(manager.ViewNameAvailable(null));
        }
    }
}