
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

        [Test]
        [TestModel(@"SCaddinsTestsR2015.rvt")]
        public void ExportDirDefaultNotNullTest()
        {
            var doc = RevitTestExecutive.CommandData.Application.ActiveUIDocument.Document;
            ExportManager scx = new ExportManager(doc);
            Assert.IsNotNullOrEmpty(scx.ExportDir,"Default ExportDir value is null", null);       
        } 
        
        [Test]
        [TestModel(@"SCaddinsTestsR2015.rvt")]
        public void ExportDirDefaultValidTest()
        {
            var doc = RevitTestExecutive.CommandData.Application.ActiveUIDocument.Document;
            ExportManager scx = new ExportManager(doc);
            Assert.IsTrue(Directory.Exists(scx.ExportDir),"Default ExportDir path us invalid", null);
        } 
        
        [Test]
        [TestModel(@"SCaddinsTestsR2015.rvt")]
        public void ExportDirFallbackOnEmptyPathTest()
        {
            var doc = RevitTestExecutive.CommandData.Application.ActiveUIDocument.Document;
            ExportManager scx = new ExportManager(doc);
            scx.ExportDir = string.Empty;
            Assert.IsTrue(Directory.Exists(scx.ExportDir),"ExportDir failed to fallback on empty path", null);
        } 
        
        [Test]
        [TestModel(@"SCaddinsTestsR2015.rvt")]
        public void ExportDirFallbackOnInvalidPathTest()
        {
            var doc = RevitTestExecutive.CommandData.Application.ActiveUIDocument.Document;
            ExportManager scx = new ExportManager(doc);
            //TRY NASTY DIR
            scx.ExportDir = @"C:/!@#$%^&*()/\\//c:.<>";
            Assert.IsTrue(Directory.Exists(scx.ExportDir),"ExportDir failed to fallback on invalid path", null);
        } 
        
    }
}
