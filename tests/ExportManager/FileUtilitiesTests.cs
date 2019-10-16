using NUnit.Framework;

namespace SCaddins.Tests.ExportManager
{
    [TestFixture()]
    public class FileUtilitiesTests
    {
        ////[Test()]
        ////public void CanOverwriteFileTest()
        ////{
        ////    Assert.Pass();
        ////}

        ////[Test()]
        ////[TestModel(@"./scaddins_test_model.rvt")]
        ////public void ConfigFileExistsTest()
        ////{
        ////    var doc = RevitTestExecutive.CommandData.Application.ActiveUIDocument.Document;
        ////    var config = Manager.GetConfigFileName(doc);
        ////    Assert.IsFalse(File.Exists(config));
        ////}

        ////[Test()]
        ////[TestModel(@"./scaddins_test_model.rvt")]
        ////public void CreateConfigFileTest()
        ////{
        ////    var doc = RevitTestExecutive.CommandData.Application.ActiveUIDocument.Document;
        ////    Manager.CreateSCexportConfig(doc);
        ////}

        ////[Test()]
        ////[TestModel(@"./scaddins_test_model.rvt")]
        ////public void GetCentralFileNameTest()
        ////{
        ////    var doc = RevitTestExecutive.CommandData.Application.ActiveUIDocument.Document;
        ////    Assert.IsEmpty(FileUtilities.GetCentralFileName(doc));
        ////}

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