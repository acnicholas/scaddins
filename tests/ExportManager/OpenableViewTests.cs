using System.Linq;
using Autodesk.Revit.DB;
using RTF.Applications;
using RTF.Framework;
using NUnit.Framework;
using SCaddins.ExportManager;

namespace SCaddins.Tests.ExportManager
{
    [TestFixture()]
    public class OpenableViewTests
    {
        [Test()]
        [Category("SCaddins.Tests.ExportManager.OpenableViewTests")]
        [TestModel(@"./scaddins_test_model.rvt")]
        public void OpenTest()
        {
            var doc = RevitTestExecutive.CommandData.Application.ActiveUIDocument.Document;
            var viewCollection = new FilteredElementCollector(doc).OfClass(typeof(View));
            var sectionOne = viewCollection.First(v => v.Name == "Section 1") as View;
            var sectionTwo = viewCollection.First(v => v.Name == "Section 2") as View;
            var openableViewOne = new OpenableView(string.Empty, "0", sectionOne);
            var openableViewTwo = new OpenableView(string.Empty, "0", sectionTwo);
            openableViewOne.Open();
            Assert.IsTrue(doc.ActiveView.Name == "Section 1");
            openableViewTwo.Open();
            Assert.IsTrue(doc.ActiveView.Name == "Section 2");
        }
    }
}