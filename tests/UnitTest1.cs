using System;
using NUnit.Framework;
using RTF.Framework;
using RTF.Applications;

using System;
using System.Collections.Generic;
using System.Linq;
using Autodesk.Revit.DB;
using NUnit.Framework;
using RTF.Applications;
using RTF.Framework;

namespace SCaddins.Tests
{
    [TestFixture]
    public class UnitTest1
    {
        [SetUp]
        public void Setup()
        {
            //startup logic executed before every test
        }

        [TearDown]
        public void Shutdown()
        {
            //shutdown logic executed after every test
        }

        /// <summary>
        /// This is the Hello World of Revit testing. Here we
        /// simply call the Revit API to create a new ReferencePoint
        /// in the default empty.rfa file.
        /// </summary>
        [Test]
        public void CanCreateAReferencePoint()
        {
            var doc = RevitTestExecutive.CommandData.Application.ActiveUIDocument.Document;

            using (var t = new Transaction(doc)) {
                if (t.Start("Test one.") == TransactionStatus.Started) {
                    //create a reference point
                    var pt = doc.FamilyCreate.NewReferencePoint(new XYZ(5, 5, 5));

                    if (t.Commit() != TransactionStatus.Committed) {
                        t.RollBack();
                    }
                } else {
                    throw new Exception("Transaction could not be started.");
                }
            }

            //verify that the point was created
            var collector = new FilteredElementCollector(doc);
            collector.OfClass(typeof(ReferencePoint));

            Assert.AreEqual(2, collector.ToElements().Count);
        }

        [Test]
        public void OnePlusOne()
        {
            Assert.AreEqual(1, 1);
        }

        /// <summary>
        /// Opens and activates a new model, and closes the old model.
        /// </summary>
        private void SwapCurrentModel(string modelPath)
        {
            var app = RevitTestExecutive.CommandData.Application;
            var doc = RevitTestExecutive.CommandData.Application.ActiveUIDocument.Document;

            app.OpenAndActivateDocument(modelPath);
            doc.Close(false);
        }

    }
}
