namespace ScaddinsTestProject.DestructivePurge.ViewModels
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Windows.Media.Imaging;
    using Autodesk.Revit.DB;
    using NUnit.Framework;
    using SCaddins.DestructivePurge;
    using SCaddins.DestructivePurge.ViewModels;
    using SCaddinsTestProject;

    [TestFixture]
    public class DestructivePurgeViewModelTests : OneTimeOpenDocumentTest
    {
        private DestructivePurgeViewModel _testClass;
        private Document _doc;

        [SetUp]
        public void SetUp()
        {
            _doc = document;
            _testClass = new DestructivePurgeViewModel(_doc);
        }

        [Test]
        public void CanConstruct()
        {
            // Act
            var instance = new DestructivePurgeViewModel(_doc);

            // Assert
            Assert.That(instance, Is.Not.Null);
        }

        [Test]
        public void CannotConstructWithNullDoc()
        {
            Assert.Throws<ArgumentNullException>(() => new DestructivePurgeViewModel(default(Document)));
        }

        [Test]
        public void CanCallCollapseAll()
        {
            // Arrange
            var sender = new object();

            // Act
            _testClass.CollapseAll(sender);

            // Assert
            Assert.Fail("Create or modify test");
        }

        [Test]
        public void CannotCallCollapseAllWithNullSender()
        {
            Assert.Throws<ArgumentNullException>(() => _testClass.CollapseAll(default(object)));
        }

        [Test]
        public void CanCallDeleteElements()
        {
            // Act
            _testClass.DeleteElements();

            // Assert
            Assert.Fail("Create or modify test");
        }

        [Test]
        public void CanCallGetPurgableItems()
        {
            // Act
            var result = _testClass.GetPurgableItems();

            // Assert
            Assert.Fail("Create or modify test");
        }

        [Test]
        public void CanCallRecurseItems()
        {
            // Arrange
            var list = new List<DeletableItem>();
            var item = new CheckableItem(new DeletableItem("TestValue384951740"), default(CheckableItem));

            // Act
            _testClass.RecurseItems(list, item);

            // Assert
            Assert.Fail("Create or modify test");
        }

        [Test]
        public void CannotCallRecurseItemsWithNullList()
        {
            Assert.Throws<ArgumentNullException>(() => _testClass.RecurseItems(default(List<DeletableItem>), new CheckableItem(new DeletableItem("TestValue1433266054"), default(CheckableItem))));
        }

        [Test]
        public void CannotCallRecurseItemsWithNullItem()
        {
            Assert.Throws<ArgumentNullException>(() => _testClass.RecurseItems(new List<DeletableItem>(), default(CheckableItem)));
        }

        [Test]
        public void CanCallSelectAll()
        {
            // Act
            _testClass.SelectAll();

            // Assert
            Assert.Fail("Create or modify test");
        }

        [Test]
        public void CanCallSelectedItemChanged()
        {
            // Arrange
            var item = new CheckableItem(new DeletableItem("TestValue1931965156"), default(CheckableItem));

            // Act
            _testClass.SelectedItemChanged(item);

            // Assert
            Assert.Fail("Create or modify test");
        }

        [Test]
        public void CannotCallSelectedItemChangedWithNullItem()
        {
            Assert.Throws<ArgumentNullException>(() => _testClass.SelectedItemChanged(default(CheckableItem)));
        }

        [Test]
        public void CanCallSelectNone()
        {
            // Act
            _testClass.SelectNone();

            // Assert
            Assert.Fail("Create or modify test");
        }

        [Test]
        public void CanCallShowElement()
        {
            // Act
            _testClass.ShowElement();

            // Assert
            Assert.Fail("Create or modify test");
        }

        [Test]
        public void CanCallTreeViewSourceUpdated()
        {
            // Act
            _testClass.TreeViewSourceUpdated();

            // Assert
            Assert.Fail("Create or modify test");
        }

        [Test]
        public void CanSetAndGetCheckableItems()
        {
            _testClass.CheckProperty(x => x.CheckableItems, new ObservableCollection<CheckableItem>(), new ObservableCollection<CheckableItem>());
        }

        [Test]
        public void CanGetCheckedCount()
        {
            // Assert
            Assert.That(_testClass.CheckedCount, Is.InstanceOf<int>());

            Assert.Fail("Create or modify test");
        }

        [Test]
        public void CanGetDeleteButtonLabel()
        {
            // Assert
            Assert.That(_testClass.DeleteButtonLabel, Is.InstanceOf<string>());

            Assert.Fail("Create or modify test");
        }

        [Test]
        public void CanGetDetails()
        {
            // Assert
            Assert.That(_testClass.Details, Is.InstanceOf<string>());

            Assert.Fail("Create or modify test");
        }

        [Test]
        public void CanGetEnableShowElemant()
        {
            // Assert
            Assert.That(_testClass.EnableShowElemant, Is.InstanceOf<bool>());

            Assert.Fail("Create or modify test");
        }

        [Test]
        public void CanGetImageHeight()
        {
            // Assert
            Assert.That(_testClass.ImageHeight, Is.InstanceOf<int>());

            Assert.Fail("Create or modify test");
        }

        [Test]
        public void CanGetImageMargin()
        {
            // Assert
            Assert.That(_testClass.ImageMargin, Is.InstanceOf<int>());

            Assert.Fail("Create or modify test");
        }

        [Test]
        public void CanGetImageWidth()
        {
            // Assert
            Assert.That(_testClass.ImageWidth, Is.InstanceOf<int>());

            Assert.Fail("Create or modify test");
        }

        [Test]
        public void CanSetAndGetPreviewImage()
        {
            _testClass.CheckProperty(x => x.PreviewImage, new BitmapImage(), new BitmapImage());
        }

        [Test]
        public void CanGetShowButtonLabel()
        {
            // Assert
            Assert.That(_testClass.ShowButtonLabel, Is.InstanceOf<string>());

            Assert.Fail("Create or modify test");
        }

        [Test]
        public void CanGetShowButtonIsVisible()
        {
            // Assert
            Assert.That(_testClass.ShowButtonIsVisible, Is.InstanceOf<bool>());

            Assert.Fail("Create or modify test");
        }
    }
}