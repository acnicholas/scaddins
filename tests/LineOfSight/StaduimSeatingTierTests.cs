namespace ScaddinsTestProject.LineOfSight
{
    using System;
    using Autodesk.Revit.DB;
    using NUnit.Framework;
    using SCaddins.LineOfSight;
    using SCaddinsTestProject;

    [TestFixture]
    public class StadiumSeatingTierTests : OneTimeOpenDocumentTest
    {
        private StadiumSeatingTier _testClass;
        private Document _doc;
        private double _eyeHeight;
        private double _treadSize;
        private double _riserIncrement;
        private double _minimumCValue;
        private double _minimumRiserHeight;
        private int _numberOfRows;
        private double _distanceToFirstRowX;
        private double _distanceToFirstRowY;

        [SetUp]
        public void SetUp()
        {
            _doc = document;
            _eyeHeight = 1891881868.1399999;
            _treadSize = 772092632.52;
            _riserIncrement = 1663351824.42;
            _minimumCValue = 298191124.44;
            _minimumRiserHeight = 2099394685.08;
            _numberOfRows = 1175190299;
            _distanceToFirstRowX = 1990886282.55;
            _distanceToFirstRowY = 1140949648.08;
            _testClass = new StadiumSeatingTier(_doc, _eyeHeight, _treadSize, _riserIncrement, _minimumCValue, _minimumRiserHeight, _numberOfRows, _distanceToFirstRowX, _distanceToFirstRowY);
        }

        [Test]
        public void CanConstruct()
        {
            // Act
            var instance = new StadiumSeatingTier(_doc, _eyeHeight, _treadSize, _riserIncrement, _minimumCValue, _minimumRiserHeight, _numberOfRows, _distanceToFirstRowX, _distanceToFirstRowY);

            // Assert
            Assert.That(instance, Is.Not.Null);
        }

        [Test]
        public void CannotConstructWithNullDoc()
        {
            Assert.Throws<ArgumentNullException>(() => new StadiumSeatingTier(default(Document), _eyeHeight, _treadSize, _riserIncrement, _minimumCValue, _minimumRiserHeight, _numberOfRows, _distanceToFirstRowX, _distanceToFirstRowY));
        }

        [Test]
        public void CanCallCreateLineOfSightDraftingView()
        {
            // Arrange
            var newViewName = "TestValue23580438";

            // Act
            var result = _testClass.CreateLineOfSightDraftingView(newViewName);

            // Assert
            Assert.IsTrue(result is ViewDrafting);
        }

        //[TestCase(null)]
        [TestCase("")]
        [TestCase("   ")]
        public void CannotCallCreateLineOfSightDraftingViewWithInvalidNewViewName(string value)
        {
            Assert.Throws<ArgumentNullException>(() => _testClass.CreateLineOfSightDraftingView(value));
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
        public void CanCallGetCValue()
        {
            // Arrange
            var i = 742496152;
            var nextn = 158539467.24;

            // Act
            var result = _testClass.GetCValue(i, nextn);

            // Assert
            Assert.Fail("Create or modify test");
        }

        [Test]
        public void DistanceToFirstRowXIsInitializedCorrectly()
        {
            Assert.That(_testClass.DistanceToFirstRowX, Is.EqualTo(_distanceToFirstRowX));
        }

        [Test]
        public void CanSetAndGetDistanceToFirstRowX()
        {
            _testClass.CheckProperty(x => x.DistanceToFirstRowX, 290102196.78, 616378405.5);
        }

        [Test]
        public void DistanceToFirstRowYIsInitializedCorrectly()
        {
            Assert.That(_testClass.DistanceToFirstRowY, Is.EqualTo(_distanceToFirstRowY));
        }

        [Test]
        public void CanSetAndGetDistanceToFirstRowY()
        {
            _testClass.CheckProperty(x => x.DistanceToFirstRowY, 1386624933.54, 1917269863.74);
        }

        [Test]
        public void EyeHeightIsInitializedCorrectly()
        {
            Assert.That(_testClass.EyeHeight, Is.EqualTo(_eyeHeight));
        }

        [Test]
        public void CanSetAndGetEyeHeight()
        {
            _testClass.CheckProperty(x => x.EyeHeight, 356191130.79, 1117715559.84);
        }

        [Test]
        public void CanGetInfoString()
        {
            // Assert
            Assert.That(_testClass.InfoString, Is.InstanceOf<string>());

            Assert.Fail("Create or modify test");
        }

        [Test]
        public void MinimumCValueIsInitializedCorrectly()
        {
            Assert.That(_testClass.MinimumCValue, Is.EqualTo(_minimumCValue));
        }

        [Test]
        public void CanSetAndGetMinimumCValue()
        {
            _testClass.CheckProperty(x => x.MinimumCValue, 1273475480.31, 1373185455.84);
        }

        [Test]
        public void MinimumRiserHeightIsInitializedCorrectly()
        {
            Assert.That(_testClass.MinimumRiserHeight, Is.EqualTo(_minimumRiserHeight));
        }

        [Test]
        public void CanSetAndGetMinimumRiserHeight()
        {
            _testClass.CheckProperty(x => x.MinimumRiserHeight, 164275551.99, 535163929.74);
        }

        [Test]
        public void NumberOfRowsIsInitializedCorrectly()
        {
            Assert.That(_testClass.NumberOfRows, Is.EqualTo(_numberOfRows));
        }

        [Test]
        public void CanSetAndGetNumberOfRows()
        {
            _testClass.CheckProperty(x => x.NumberOfRows);
        }

        [Test]
        public void RiserIncrementIsInitializedCorrectly()
        {
            Assert.That(_testClass.RiserIncrement, Is.EqualTo(_riserIncrement));
        }

        [Test]
        public void CanSetAndGetRiserIncrement()
        {
            _testClass.CheckProperty(x => x.RiserIncrement, 1177584869.34, 1400103185.58);
        }

        [Test]
        public void TreadSizeIsInitializedCorrectly()
        {
            Assert.That(_testClass.TreadSize, Is.EqualTo(_treadSize));
        }

        [Test]
        public void CanSetAndGetTreadSize()
        {
            _testClass.CheckProperty(x => x.TreadSize, 1150712439.03, 1867423633.02);
        }

        [Test]
        public void CanGetView()
        {
            // Assert
            Assert.That(_testClass.View, Is.InstanceOf<View>());

            Assert.Fail("Create or modify test");
        }
    }
}