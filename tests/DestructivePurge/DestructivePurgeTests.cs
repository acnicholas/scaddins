namespace ScaddinsTestProject.DestructivePurge
{
    using System;
    using System.Collections.Generic;
    using System.Drawing;
    using Autodesk.Revit.DB;
    using NUnit.Framework;
    using SCaddins.DestructivePurge;
    using SCaddinsTestProject;

    [TestFixture]
    public class DestructivePurgeUtilitilesTests : OneTimeOpenDocumentTest
    {
        [Test]
        public void CanCallImages()
        {
            // Arrange
            var doc = document;

            // Act
            var result = DestructivePurgeUtilitiles.Images(doc);

            // Assert
            Assert.Fail("Create or modify test");
        }

        [Test]
        public static void CannotCallImagesWithNullDoc()
        {
            Assert.Throws<ArgumentNullException>(() => DestructivePurgeUtilitiles.Images(default(Document)));
        }

        [Test]
        public void CanCallImports()
        {
            // Arrange
            var doc = document;
            var linked = false;

            // Act
            var result = DestructivePurgeUtilitiles.Imports(doc, linked);

            // Assert
            Assert.Fail("Create or modify test");
        }

        [Test]
        public static void CannotCallImportsWithNullDoc()
        {
            Assert.Throws<ArgumentNullException>(() => DestructivePurgeUtilitiles.Imports(default(Document), true));
        }

        [Test]
        public void CanCallRemoveElements()
        {
            // Arrange
            var doc = document;
            var elements = new List<DeletableItem>();

            // Act
            DestructivePurgeUtilitiles.RemoveElements(doc, elements);

            // Assert
            Assert.Fail("Create or modify test");
        }

        [Test]
        public static void CannotCallRemoveElementsWithNullDoc()
        {
            Assert.Throws<ArgumentNullException>(() => DestructivePurgeUtilitiles.RemoveElements(default(Document), new List<DeletableItem>()));
        }

        [Test]
        public void CannotCallRemoveElementsWithNullElements()
        {
            Assert.Throws<ArgumentNullException>(() => DestructivePurgeUtilitiles.RemoveElements(document, default(List<DeletableItem>)));
        }

        [Test]
        public void CanCallRevisions()
        {
            // Arrange
            var doc = document;

            // Act
            var result = DestructivePurgeUtilitiles.Revisions(doc);

            // Assert
            Assert.Fail("Create or modify test");
        }

        [Test]
        public static void CannotCallRevisionsWithNullDoc()
        {
            Assert.Throws<ArgumentNullException>(() => DestructivePurgeUtilitiles.Revisions(default(Document)));
        }

        //[Test]
        //public static void CanCallToBitmapImage()
        //{
        //    // Arrange
        //    var bitmap = new Bitmap("TestValue425376716");

        //    // Act
        //    var result = bitmap.ToBitmapImage();

        //    // Assert
        //    Assert.Fail("Create or modify test");
        //}

        //[Test]
        //public static void CannotCallToBitmapImageWithNullBitmap()
        //{
        //    Assert.Throws<ArgumentNullException>(() => default(Bitmap).ToBitmapImage());
        //}

        [Test]
        public void CanCallUnboundRooms()
        {
            // Arrange
            var doc = document;

            // Act
            var result = DestructivePurgeUtilitiles.UnboundRooms(doc);

            // Assert
            Assert.Fail("Create or modify test");
        }

        [Test]
        public static void CannotCallUnboundRoomsWithNullDoc()
        {
            Assert.Throws<ArgumentNullException>(() => DestructivePurgeUtilitiles.UnboundRooms(default(Document)));
        }

        [Test]
        public void CanCallUnusedViewFilters()
        {
            // Arrange
            var doc = document;

            // Act
            var result = DestructivePurgeUtilitiles.UnusedViewFilters(doc);

            // Assert
            Assert.Fail("Create or modify test");
        }

        [Test]
        public static void CannotCallUnusedViewFiltersWithNullDoc()
        {
            Assert.Throws<ArgumentNullException>(() => DestructivePurgeUtilitiles.UnusedViewFilters(default(Document)));
        }

        [Test]
        public void CanCallViews()
        {
            // Arrange
            var doc = document;
            var placedOnSheet = false;
            var @type = ViewType.ProjectBrowser;

            // Act
            var result = DestructivePurgeUtilitiles.Views(doc, placedOnSheet, type);

            // Assert
            Assert.Fail("Create or modify test");
        }

        [Test]
        public static void CannotCallViewsWithNullDoc()
        {
            Assert.Throws<ArgumentNullException>(() => DestructivePurgeUtilitiles.Views(default(Document), true, ViewType.Section));
        }
    }
}