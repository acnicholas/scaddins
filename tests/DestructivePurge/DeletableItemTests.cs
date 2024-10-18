namespace ScaddinsTestProject.DestructivePurge
{
    using System;
    using System.Drawing;
    using Autodesk.Revit.DB;
    using NUnit.Framework;
    using SCaddins.DestructivePurge;

    [TestFixture]
    public class DeletableItemTests
    {
        private DeletableItem _testClass;
        private string _name;

        [SetUp]
        public void SetUp()
        {
            _name = "TestValue578419264";
            _testClass = new DeletableItem(_name);
        }

        [Test]
        public void CanConstruct()
        {
            // Act
            var instance = new DeletableItem(_name);

            // Assert
            Assert.That(instance, Is.Not.Null);
        }

        //[TestCase(null)]
        [TestCase("")]
        [TestCase("   ")]
        public void CannotConstructWithInvalidName(string value)
        {
            Assert.Throws<ArgumentNullException>(() => new DeletableItem(value));
        }

        [Test]
        public void CanSetAndGetId()
        {
            // Arrange
            var testValue = new ElementId(BuiltInParameter.DIVISION_RULE_PARAM);

            // Act
            _testClass.Id = testValue;

            // Assert
            Assert.That(_testClass.Id, Is.SameAs(testValue));
        }

        [Test]
        public void CanSetAndGetHasParent()
        {
            // Arrange
            var testValue = true;

            // Act
            _testClass.HasParent = testValue;

            // Assert
            Assert.That(_testClass.HasParent, Is.EqualTo(testValue));
        }

        [Test]
        public void CanSetAndGetParentId()
        {
            // Arrange
            var testValue = new ElementId(BuiltInParameter.FURNITURE_WIDTH);

            // Act
            _testClass.ParentId = testValue;

            // Assert
            Assert.That(_testClass.ParentId, Is.SameAs(testValue));
        }

        [Test]
        public void CanSetAndGetInfo()
        {
            // Arrange
            var testValue = "TestValue656124446";

            // Act
            _testClass.Info = testValue;

            // Assert
            Assert.That(_testClass.Info, Is.EqualTo(testValue));
        }

        [Test]
        public void NameIsInitializedCorrectly()
        {
            Assert.That(_testClass.Name, Is.EqualTo(_name));
        }

        [Test]
        public void CanSetAndGetName()
        {
            // Arrange
            var testValue = "TestValue568085342";

            // Act
            _testClass.Name = testValue;

            // Assert
            Assert.That(_testClass.Name, Is.EqualTo(testValue));
        }

        //[Test]
        //public void CanSetAndGetPreviewImage()
        //{
        //    // Arrange
        //    var testValue = new Bitmap("TestValue905850981");

        //    // Act
        //    _testClass.PreviewImage = testValue;

        //    // Assert
        //    Assert.That(_testClass.PreviewImage, Is.SameAs(testValue));
        //}
    }
}