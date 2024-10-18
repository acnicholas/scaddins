namespace ScaddinsTestProject.DestructivePurge.ViewModels
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using NUnit.Framework;
    using SCaddins.DestructivePurge;
    using SCaddins.DestructivePurge.ViewModels;

    [TestFixture]
    public class CheckableItemTests
    {
        private CheckableItem _testClass;
        private DeletableItem _deletable;
        private CheckableItem _parent;

        [SetUp]
        public void SetUp()
        {
            _deletable = new DeletableItem("TestValue553231308");
            _parent = new CheckableItem(new DeletableItem("TestValue1398148276"), default(CheckableItem));
            _testClass = new CheckableItem(_deletable, _parent);
        }

        [Test]
        public void CanConstruct()
        {
            // Act
            var instance = new CheckableItem(_deletable, _parent);

            // Assert
            Assert.That(instance, Is.Not.Null);
        }

        [Test]
        public void CannotConstructWithNullDeletable()
        {
            Assert.Throws<ArgumentNullException>(() => new CheckableItem(default(DeletableItem), _parent));
        }

        [Test]
        public void CannotConstructWithNullParent()
        {
            Assert.Throws<ArgumentNullException>(() => new CheckableItem(_deletable, default(CheckableItem)));
        }

        [Test]
        public void CanCallAddChild()
        {
            // Arrange
            var deletable = new CheckableItem(new DeletableItem("TestValue490617192"), default(CheckableItem));

            // Act
            _testClass.AddChild(deletable);

            // Assert
            Assert.Fail("Create or modify test");
        }

        [Test]
        public void CannotCallAddChildWithNullDeletable()
        {
            Assert.Throws<ArgumentNullException>(() => _testClass.AddChild(default(CheckableItem)));
        }

        [Test]
        public void CanCallAddChildren()
        {
            // Arrange
            var deletables = new List<DeletableItem>();

            // Act
            _testClass.AddChildren(deletables);

            // Assert
            Assert.Fail("Create or modify test");
        }

        [Test]
        public void CannotCallAddChildrenWithNullDeletables()
        {
            Assert.Throws<ArgumentNullException>(() => _testClass.AddChildren(default(List<DeletableItem>)));
        }

        [Test]
        public void CanGetChildren()
        {
            // Assert
            Assert.That(_testClass.Children, Is.InstanceOf<ObservableCollection<CheckableItem>>());

            Assert.Fail("Create or modify test");
        }

        [Test]
        public void DeletableIsInitializedCorrectly()
        {
            Assert.That(_testClass.Deletable, Is.SameAs(_deletable));
        }

        [Test]
        public void CanSetAndGetDeletable()
        {
            _testClass.CheckProperty(x => x.Deletable, new DeletableItem("TestValue1094966004"), new DeletableItem("TestValue100793485"));
        }

        [Test]
        public void CanGetIsYesOrMaybe()
        {
            // Assert
            Assert.That(_testClass.IsYesOrMaybe, Is.InstanceOf<bool>());

            Assert.Fail("Create or modify test");
        }

        [Test]
        public void CanGetIsYes()
        {
            // Assert
            Assert.That(_testClass.IsYes, Is.InstanceOf<bool>());

            Assert.Fail("Create or modify test");
        }

        [Test]
        public void CanGetIsNo()
        {
            // Assert
            Assert.That(_testClass.IsNo, Is.InstanceOf<bool>());

            Assert.Fail("Create or modify test");
        }

        [Test]
        public void CanGetIsMaybe()
        {
            // Assert
            Assert.That(_testClass.IsMaybe, Is.InstanceOf<bool>());

            Assert.Fail("Create or modify test");
        }

        [Test]
        public void CanGetCheckedCount()
        {
            // Assert
            Assert.That(_testClass.CheckedCount, Is.InstanceOf<int>());

            Assert.Fail("Create or modify test");
        }

        [Test]
        public void CanSetAndGetIsChecked()
        {
            _testClass.CheckProperty(x => x.IsChecked, true, false);
        }

        [Test]
        public void CanGetName()
        {
            // Assert
            Assert.That(_testClass.Name, Is.InstanceOf<string>());

            Assert.Fail("Create or modify test");
        }
    }
}