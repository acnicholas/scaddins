namespace ScaddinsTestProject.LineOfSight.ViewModels
{
    using System;
    using Autodesk.Revit.DB;
    using NUnit.Framework;
    using SCaddins.LineOfSight.ViewModels;
    using SCaddinsTestProject;

    [TestFixture]
    public class LineOfSightViewModelTests : OneTimeOpenDocumentTest
    {
        private LineOfSightViewModel _testClass;

        [SetUp]
        public void SetUp()
        {
            _testClass = new LineOfSightViewModel(document);
        }

        [Test]
        public void CanConstruct()
        {
            // Act
            var instance = new LineOfSightViewModel(document);

            // Assert
            Assert.That(instance, Is.Not.Null);
        }

        [Test]
        public void CannotConstructWithNullDoc()
        {
            Assert.Throws<ArgumentNullException>(() => new LineOfSightViewModel(default(Document)));
        }

        [Test]
        public void CanCallDraw()
        {
            // Act
            _testClass.Draw();

            // Assert
            Assert.Fail("Create or modify test");
        }

        [Test]
        public void CanGetCanDraw()
        {
            // Assert
            Assert.That(LineOfSightViewModel.CanDraw, Is.InstanceOf<bool>());

            Assert.Fail("Create or modify test");
        }

        [Test]
        public void CanSetAndGetDistanceToFirstRowX()
        {
            _testClass.CheckProperty(x => x.DistanceToFirstRowX, 1391414904.99, 2067578461.08);
        }

        [Test]
        public void CanSetAndGetDistanceToFirstRowY()
        {
            _testClass.CheckProperty(x => x.DistanceToFirstRowY, 1184220875.97, 182831557.59);
        }

        [Test]
        public void CanSetAndGetEyeHeight()
        {
            _testClass.CheckProperty(x => x.EyeHeight, 1017985393.26, 143933422.05);
        }

        [Test]
        public void CanGetInfoString()
        {
            // Assert
            Assert.That(_testClass.InfoString, Is.InstanceOf<string>());

            Assert.Fail("Create or modify test");
        }

        [Test]
        public void CanSetAndGetMinimumCValue()
        {
            _testClass.CheckProperty(x => x.MinimumCValue, 1381645595.8799999, 100724046.39);
        }

        [Test]
        public void CanSetAndGetMinimumRiserHeight()
        {
            _testClass.CheckProperty(x => x.MinimumRiserHeight, 417322157.67, 1509527552.94);
        }

        [Test]
        public void CanSetAndGetNumberOfRows()
        {
            _testClass.CheckProperty(x => x.NumberOfRows);
        }

        [Test]
        public void CanSetAndGetRiserIncrement()
        {
            _testClass.CheckProperty(x => x.RiserIncrement, 1945974845.43, 72106798.5);
        }

        [Test]
        public void CanSetAndGetTreadSize()
        {
            _testClass.CheckProperty(x => x.TreadSize, 718002703.43999994, 576157623.03);
        }
    }
}