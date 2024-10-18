using System;
using System.Collections.Generic;
using System.Windows.Controls;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using Caliburn.Micro;
using Newtonsoft.Json.Linq;
using NSubstitute;
using NUnit.Framework;
using SCaddins;
using SCaddins.ExportManager;
using SCaddins.ViewUtilities;
using SCaddinsTestProject;
using View = Autodesk.Revit.DB.View;

namespace ScaddinsTestProject.ViewUtilities
{
    [TestFixture]
    public class UserViewTests : SCaddinsTestProject.OneTimeOpenDocumentTest
    {
        [Test]
        public void CanCallCreateWithSourceViewAndUidoc()
        {
            // Arrange
            int startCount;
            using (var fec = new FilteredElementCollector(document))
            {
                fec.OfClass(typeof(View));
                startCount = fec.Count();
            }
            var sourceView = uiapp.ActiveUIDocument.ActiveView;
            var uidoc = uiapp.ActiveUIDocument;

            // Act

            using (var t = new Transaction(document))
            {
                if (t.Start("UserViewTest_CanCallCreateWithSourceViewAndUidoc") == TransactionStatus.Started)
                {
                    var result = UserView.Create(sourceView, uidoc);
                    t.Commit();
                }
            }


            int endCount;
            using (var fec = new FilteredElementCollector(document))
            {
                fec.OfClass(typeof(View));
                endCount = fec.Count();
            }

            // Assert
            Assert.IsTrue(startCount + 1 == endCount);
        }

        [Test]
        public void CannotCallCreateWithSourceViewAndUidocWithNullSourceView()
        {
            Assert.IsNull(UserView.Create(null, uiapp.ActiveUIDocument));
        }

        [Test]
        public void CannotCallCreateWithSourceViewAndUidocWithNullUidoc()
        {
            Assert.IsNull(UserView.Create(uiapp.ActiveUIDocument.ActiveView, default(UIDocument)));
        }

        [Test]
        public void CanCallCreateWithSheetsAndDoc()
        {
            // Arrange
            int startCount;
            using (var fec = new FilteredElementCollector(document))
            {
                fec.OfClass(typeof(View));
                startCount = fec.Count();
            }

            var exportManager = new SCaddins.ExportManager.Manager(uiapp.ActiveUIDocument);
            var sheets = exportManager.AllSheets;
            if (sheets.Count == 0)
            {
                Assert.Fail("No sheets to test in current model");
            }
            var sheet = sheets.First();

            // Act
            List<View> result = new List<View>();
            using (Transaction t = new Transaction(document))
            {
                if (t.Start("SCuv Copies User View") == TransactionStatus.Started)
                {
                    result = UserView.Create(sheet.Sheet, uiapp.ActiveUIDocument);
                    t.Commit();
                } 
            }

            SCaddinsApp.WindowManager.ShowMessageBox(result.Count.ToString());
            Console.WriteLine(result.Count.ToString());

            // Assert
            int endCount;
            using (var fec2 = new FilteredElementCollector(document))
            {
                fec2.OfClass(typeof(View));
                endCount = fec2.Count();
            }
            Console.WriteLine((endCount - startCount).ToString());
            Assert.IsTrue(result.Count == (endCount - startCount));
        }

        [Test]
        public void CannotCallCreateWithSheetsAndDocWithNullSheets()
        {
            Assert.IsNull(UserView.Create(default(ICollection<ExportSheet>), document));
        }

        [Test]
        public void CannotCallCreateWithSheetsAndDocWithNullDoc()
        {
            var exportManager = new SCaddins.ExportManager.Manager(uiapp.ActiveUIDocument);
            var exportSheets = exportManager.AllSheets;
            Assert.IsNull(UserView.Create(exportSheets.ToList(), default(Document)));
        }

        [Test]
        public void CreateWithSheetsAndDocPerformsMapping()
        {
            // Arrange
            int startCount;
            using (var fec = new FilteredElementCollector(document))
            {
                fec.OfClass(typeof(View));
                startCount = fec.Count();
            }
            var exportManager = new SCaddins.ExportManager.Manager(uiapp.ActiveUIDocument);
            var sheet = exportManager.AllSheets.First();

            // Act
            var result = UserView.Create(sheet.Sheet, uiapp.ActiveUIDocument);

            // Assert
            int endCount;
            using (var fec = new FilteredElementCollector(document))
            {
                fec.OfClass(typeof(View));
                endCount = fec.Count();
            }
            Assert.That(result.Count, Is.EqualTo(endCount - startCount));
        }

        [Test]
        public void CanCallParamFromString()
        {
            // Arrange
            var name = "TestValue658280405";
            Element element;
            using (var fec = new FilteredElementCollector(document))
            {
                fec.OfClass(typeof(ViewSheet));
                element = fec.OfType<Element>().Where(v => v.Name = ;
            }

            // Act
            var result = UserView.ParamFromString(name, element);

            // Assert
            Assert.IsNotNull(result);
        }

        [Test]
        public static void CannotCallParamFromStringWithNullElement()
        {
            Assert.IsNull(UserView.ParamFromString("TestValue1288063166", null));
        }

        [TestCase(null)]
        [TestCase("")]
        [TestCase("   ")]
        public void CannotCallParamFromStringWithInvalidName(string value)
        {
            using (var fec = new FilteredElementCollector(document))
            {
                fec.OfClass(typeof(ViewSheet));
                var element = fec.OfType<Element>().FirstOrDefault();
                Assert.IsNull(UserView.ParamFromString(value, element));
            }
        }

        [Test]
        public void ParamFromStringPerformsMapping()
        {
            // Arrange
            var name = "TestValue444950335";
            Element element;
            using (var fec = new FilteredElementCollector(document))
            {
                fec.OfClass(typeof(ViewSheet));
                element = fec.OfType<Element>().FirstOrDefault();
            }

            // Act
            var result = UserView.ParamFromString(name, element);

            // Assert
            Assert.That(result.Element, Is.SameAs(element));
        }

        [Test]
        public static void CanCallShowSummaryDialog()
        {
            // Arrange
            var newUserViews = new List<View>();

            // Act
            UserView.ShowSummaryDialog(newUserViews);

            // Assert
            Assert.Fail("Create or modify test");
        }

        [Test]
        public static void CannotCallShowSummaryDialogWithNullNewUserViews()
        {
            Assert.Throws<ArgumentNullException>(() => UserView.ShowSummaryDialog(default(List<View>)));
        }
    }
}