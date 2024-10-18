namespace ScaddinsTestProject.LineOfSight
{
    using System;
    using NUnit.Framework;
    using SCaddins.LineOfSight;

    [TestFixture]
    public class StadiumSeatingTreadTests
    {
        private StadiumSeatingTread _testClass;

        [SetUp]
        public void SetUp()
        {
            _testClass = new StadiumSeatingTread();
        }

        [Test]
        public void CanCallInitialize()
        {
            // Arrange
            var eyeToFocusX = 1479071388.96;
            var riserHeight = 1336565369.7;
            var heightToFocus = 809010524.52;
            var going = 355982778.36;
            var eyeHeight = 395365184.28;

            // Act
            _testClass.Initialize(eyeToFocusX, riserHeight, heightToFocus, going, eyeHeight);

            // Assert
            Assert.Fail("Create or modify test");
        }

        [Test]
        public void CanSetAndGetCValue()
        {
            // Arrange
            var testValue = 1470700737.99;

            // Act
            _testClass.CValue = testValue;

            // Assert
            Assert.That(_testClass.CValue, Is.EqualTo(testValue));
        }

        [Test]
        public void CanSetAndGetEyeHeight()
        {
            // Arrange
            var testValue = 1296411643.89;

            // Act
            _testClass.EyeHeight = testValue;

            // Assert
            Assert.That(_testClass.EyeHeight, Is.EqualTo(testValue));
        }

        [Test]
        public void CanSetAndGetEyeToFocusX()
        {
            // Arrange
            var testValue = 1635711120.45;

            // Act
            _testClass.EyeToFocusX = testValue;

            // Assert
            Assert.That(_testClass.EyeToFocusX, Is.EqualTo(testValue));
        }

        [Test]
        public void CanSetAndGetGoing()
        {
            // Arrange
            var testValue = 1947219042.78;

            // Act
            _testClass.Going = testValue;

            // Assert
            Assert.That(_testClass.Going, Is.EqualTo(testValue));
        }

        [Test]
        public void CanSetAndGetHeightToFocus()
        {
            // Arrange
            var testValue = 1852935284.97;

            // Act
            _testClass.HeightToFocus = testValue;

            // Assert
            Assert.That(_testClass.HeightToFocus, Is.EqualTo(testValue));
        }

        [Test]
        public void CanSetAndGetRiserHeight()
        {
            // Arrange
            var testValue = 789188633.64;

            // Act
            _testClass.RiserHeight = testValue;

            // Assert
            Assert.That(_testClass.RiserHeight, Is.EqualTo(testValue));
        }
    }
}