using NUnit.Framework;
using RTF.Framework;
using System.IO;
using RTF.Applications;
using Autodesk.Revit.DB;

namespace SCaddins.ExportManager.Tests
{
    [TestFixture()]
    public class FileUtilitiesTests
    {
        [Test()]
        public void CanOverwriteFileTest()
        {
            Assert.Pass();
        }

        [Test()]
        [TestModel(@"./rac_basic_sample_project.rvt")]
        public void ConfigFileExistsTest()
        {
            var doc = RevitTestExecutive.CommandData.Application.ActiveUIDocument.Document;
            var config = Manager.GetConfigFileName(doc);
            Assert.IsFalse(File.Exists(config));
        }

        [Test()]
        [TestModel(@"./rac_basic_sample_project.rvt")]
        public void CreateConfigFileTest()
        {
            Assert.Fail();
        }

        [Test()]
        [TestModel(@"./rac_basic_sample_project.rvt")]
        public void GetCentralFileNameTest()
        {
            var doc = RevitTestExecutive.CommandData.Application.ActiveUIDocument.Document;
            Assert.IsEmpty(FileUtilities.GetCentralFileName(doc));
        }

        ////[Test()]
        ////public void IsInvalidFileNameTest()
        ////{
        ////    var invalidChars = Path.GetInvalidFileNameChars();
        ////    Assert.False(FileUtilities.IsValidFileName(new string(invalidChars)));
        ////}

        ////[Test()]
        ////public void IsValidFileNameTest()
        ////{
        ////    Assert.True(FileUtilities.IsValidFileName("ValidFileName"));
        ////}
    }
}