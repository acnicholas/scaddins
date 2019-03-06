
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
            var newUserViewCount = UserView.Create(doc.ActiveView, doc).Count;
            Assert.IsTrue(newUserView.Count == 1);
        }

        [Test()]
        [TestModel(@"./scaddins_test_model.rvt")]
        public void CreateTestViewSelection()
        {
            var doc = RevitTestExecutive.CommandData.Application.ActiveUIDocument.Document;
            Assert.Fail();
        }

        ////[Test()]
        ////public void ShowSummaryDialogTest()
        ////{
        ////    Assert.Fail();
        ////}
    }
}