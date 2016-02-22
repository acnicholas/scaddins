
using System;
using System.IO;
using NUnit.Framework;
using RTF.Applications;
using RTF.Framework;
using SCaddins.SCexport;


namespace SCaddins.Tests
{
    [TestFixture]
    public class SCexportTests
    {     
        [Test]
        [TestModel(@"SCaddinsTestsR2015.rvt")]
        public void GhostscriptBinDirTests()
        {
            var doc = RevitTestExecutive.CommandData.Application.ActiveUIDocument.Document;
            ExportManager scx = new ExportManager(doc);
            Assert.IsNotNullOrEmpty(scx.GhostscriptBinDir,"Default GhostscriptBinDir val is null", null);
            Assert.IsTrue(Directory.Exists(scx.GhostscriptBinDir),"Default GhostscriptBinDir does not exist.", null);          
        }      
    }
}
