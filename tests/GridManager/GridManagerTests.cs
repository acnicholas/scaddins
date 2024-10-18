namespace ScaddinsTestProject.GridManager
{
    using System;
    using System.Collections.Generic;
    using Autodesk.Revit.ApplicationServices;
    using Autodesk.Revit.DB;
    using Autodesk.Revit.UI;
    using NSubstitute;
    using NUnit.Framework;
    using SCaddins.GridManager;

    [TestFixture]
    public class GridManagerTests : SCaddinsTestProject.OneTimeOpenDocumentTest
    {
        private GridManager _testClass;

        [SetUp]
        public void SetUp()
        {
            _testClass = new GridManager();
        }

        [Test]
        public void CanCallShowBottomGridBubblesByView()
        {
            // Arrange
            var activeView = uiapp.ActiveUIDocument.ActiveView;
            var enable = false;
            var selection = new List<ElementId>();

            // Act
            var result = GridManager.ShowBottomGridBubblesByView(activeView, enable, selection);

            // Assert
            Assert.Fail("Create or modify test");
        }

        [Test]
        public void CannotCallShowBottomGridBubblesByViewWithNullActiveView()
        {
            Assert.Throws<ArgumentNullException>(() => GridManager.ShowBottomGridBubblesByView(default(View), false, new List<ElementId>()));
        }

        [Test]
        public void CannotCallShowBottomGridBubblesByViewWithNullSelection()
        {
            Assert.Throws<ArgumentNullException>(() => GridManager.ShowBottomGridBubblesByView(uiapp.ActiveUIDocument.ActiveView, false, default(List<ElementId>)));
        }

        [Test]
        public void CanCallGetViewInformation()
        {
            // Arrange
            var activeView = uiapp.ActiveUIDocument.ActiveView;
            var selection = new List<ElementId>();

            // Act
            var result = GridManager.GetViewInformation(activeView, selection);

            // Assert
            Assert.Fail("Create or modify test");
        }

        [Test]
        public void CannotCallGetViewInformationWithNullActiveView()
        {
            Assert.Throws<ArgumentNullException>(() => GridManager.GetViewInformation(default(View), new List<ElementId>()));
        }

        [Test]
        public void CannotCallGetViewInformationWithNullSelection()
        {
            Assert.Throws<ArgumentNullException>(() => GridManager.GetViewInformation(uiapp.ActiveUIDocument.ActiveView, default(List<ElementId>)));
        }

        [Test]
        public void CanCallShowLeftGridBubblesByView()
        {
            // Arrange
            var activeView = uiapp.ActiveUIDocument.ActiveView;
            var enable = false;
            var selection = new List<ElementId>();

            // Act
            var result = GridManager.ShowLeftGridBubblesByView(activeView, enable, selection);

            // Assert
            Assert.Fail("Create or modify test");
        }

        [Test]
        public void CannotCallShowLeftGridBubblesByViewWithNullActiveView()
        {
            Assert.Throws<ArgumentNullException>(() => GridManager.ShowLeftGridBubblesByView(default(View), false, new List<ElementId>()));
        }

        [Test]
        public void CannotCallShowLeftGridBubblesByViewWithNullSelection()
        {
            Assert.Throws<ArgumentNullException>(() => GridManager.ShowLeftGridBubblesByView(uiapp.ActiveUIDocument.ActiveView, false, default(List<ElementId>)));
        }

        [Test]
        public void CanCallShowLeftLevelEndsByView()
        {
            // Arrange
            var activeView = uiapp.ActiveUIDocument.ActiveView;
            var enable = true;
            var selection = new List<ElementId>();

            // Act
            GridManager.ShowLeftLevelEndsByView(activeView, enable, selection);

            // Assert
            Assert.Fail("Create or modify test");
        }

        [Test]
        public void CannotCallShowLeftLevelEndsByViewWithNullActiveView()
        {
            Assert.Throws<ArgumentNullException>(() => GridManager.ShowLeftLevelEndsByView(default(View), false, new List<ElementId>()));
        }

        [Test]
        public void CannotCallShowLeftLevelEndsByViewWithNullSelection()
        {
            Assert.Throws<ArgumentNullException>(() => GridManager.ShowLeftLevelEndsByView(uiapp.ActiveUIDocument.ActiveView, false, default(List<ElementId>)));
        }

        [Test]
        public void CanCallShowRightGridBubblesByView()
        {
            // Arrange
            var activeView = uiapp.ActiveUIDocument.ActiveView;
            var enable = true;
            var selection = new List<ElementId>();

            // Act
            var result = GridManager.ShowRightGridBubblesByView(activeView, enable, selection);

            // Assert
            Assert.Fail("Create or modify test");
        }

        [Test]
        public void CannotCallShowRightGridBubblesByViewWithNullActiveView()
        {
            Assert.Throws<ArgumentNullException>(() => GridManager.ShowRightGridBubblesByView(default(View), false, new List<ElementId>()));
        }

        [Test]
        public void CannotCallShowRightGridBubblesByViewWithNullSelection()
        {
            Assert.Throws<ArgumentNullException>(() => GridManager.ShowRightGridBubblesByView(uiapp.ActiveUIDocument.ActiveView, true, default(List<ElementId>)));
        }

        [Test]
        public void CanCallShowRightLevelEndsByView()
        {
            // Arrange
            var activeView = uiapp.ActiveUIDocument.ActiveView;
            var enable = true;
            var selection = new List<ElementId>();

            // Act
            var result = GridManager.ShowRightLevelEndsByView(activeView, enable, selection);

            // Assert
            Assert.Fail("Create or modify test");
        }

        [Test]
        public void CannotCallShowRightLevelEndsByViewWithNullActiveView()
        {
            Assert.Throws<ArgumentNullException>(() => GridManager.ShowRightLevelEndsByView(default(View), true, new List<ElementId>()));
        }

        [Test]
        public void CannotCallShowRightLevelEndsByViewWithNullSelection()
        {
            Assert.Throws<ArgumentNullException>(() => GridManager.ShowRightLevelEndsByView(uiapp.ActiveUIDocument.ActiveView, false, default(List<ElementId>)));
        }

        [Test]
        public void CanCallShowTopGridBubblesByView()
        {
            // Arrange
            var activeView = uiapp.ActiveUIDocument.ActiveView;
            var enable = true;
            var selection = new List<ElementId>();

            // Act
            var result = GridManager.ShowTopGridBubblesByView(activeView, enable, selection);

            // Assert
            Assert.Fail("Create or modify test");
        }

        [Test]
        public void CannotCallShowTopGridBubblesByViewWithNullActiveView()
        {
            Assert.Throws<ArgumentNullException>(() => GridManager.ShowTopGridBubblesByView(default(View), true, new List<ElementId>()));
        }

        [Test]
        public void CannotCallShowTopGridBubblesByViewWithNullSelection()
        {
            Assert.Throws<ArgumentNullException>(() => GridManager.ShowTopGridBubblesByView(uiapp.ActiveUIDocument.ActiveView, true, default(List<ElementId>)));
        }

        [Test]
        public void CanCallToggle2dGridsByView()
        {
            // Arrange
            var activeView = uiapp.ActiveUIDocument.ActiveView;
            var make2d = false;
            var selection = new List<ElementId>();

            // Act
            GridManager.Toggle2dGridsByView(activeView, make2d, selection);

            // Assert
            Assert.Fail("Create or modify test");
        }

        [Test]
        public void CannotCallToggle2dGridsByViewWithNullActiveView()
        {
            Assert.Throws<ArgumentNullException>(() => GridManager.Toggle2dGridsByView(default(View), false, new List<ElementId>()));
        }

        [Test]
        public void CannotCallToggle2dGridsByViewWithNullSelection()
        {
            Assert.Throws<ArgumentNullException>(() => GridManager.Toggle2dGridsByView(uiapp.ActiveUIDocument.ActiveView, false, default(List<ElementId>)));
        }

        [Test]
        public void CanCallToggle2dLevelsByView()
        {
            // Arrange
            var activeView = uiapp.ActiveUIDocument.ActiveView;
            var make2d = true;
            var selection = new List<ElementId>();

            // Act
            GridManager.Toggle2dLevelsByView(activeView, make2d, selection);

            // Assert
            Assert.Fail("Create or modify test");
        }

        [Test]
        public void CannotCallToggle2dLevelsByViewWithNullActiveView()
        {
            Assert.Throws<ArgumentNullException>(() => GridManager.Toggle2dLevelsByView(default(View), false, new List<ElementId>()));
        }

        [Test]
        public void CannotCallToggle2dLevelsByViewWithNullSelection()
        {
            Assert.Throws<ArgumentNullException>(() => GridManager.Toggle2dLevelsByView(uiapp.ActiveUIDocument.ActiveView, false, default(List<ElementId>)));
        }

        //[Test]
        //public void CanCallExecute()
        //{
        //    // Arrange
        //    var commandData = new ExternalCommandData
        //    {
        //        JournalData = Substitute.For<IDictionary<string, string>>();
        //        View = uiapp.ActiveUIDocument.ActiveView;
        //        Application = uiapp);
        //    };
        //    var message = "TestValue73377606";
        //    var elements = new ElementSet();

        //    // Act
        //    var result = _testClass.Execute(commandData, ref message, elements);

        //    // Assert
        //    Assert.Fail("Create or modify test");
        //}

        [Test]
        public void CannotCallExecuteWithNullCommandData()
        {
            var message = "TestValue1844039510";
            Assert.Throws<ArgumentNullException>(() => _testClass.Execute(default(ExternalCommandData), ref message, new ElementSet()));
        }

        //[Test]
        //public void CannotCallExecuteWithNullElements()
        //{
        //    var message = "TestValue24142679";
        //    Assert.Throws<ArgumentNullException>(() => _testClass.Execute(new ExternalCommandData
        //    {
        //        JournalData = Substitute.For<IDictionary<string, string>>(),
        //        View = uiapp.ActiveUIDocument.ActiveView,
        //        Application = uiapp
        //    }, ref message, default(ElementSet))); ;
        //}

        //[TestCase(null)]
        //[TestCase("")]
        //[TestCase("   ")]
        //public void CannotCallExecuteWithInvalidMessage(string value)
        //{
        //    var message = default(string);
        //    Assert.Throws<ArgumentNullException>(() => _testClass.Execute(new ExternalCommandData
        //    {
        //        JournalData = Substitute.For<IDictionary<string, string>>(),
        //        View = uiapp.ActiveUIDocument.ActiveView,
        //        Application = uiapp
        //    }, ref message, new ElementSet()));
        //}

        [Test]
        public void CanCallGetAllGridsInView()
        {
            // Arrange
            var activeView = uiapp.ActiveUIDocument.ActiveView;
            var selection = new List<ElementId>();

            // Act
            var result = GridManager.GetAllGridsInView(activeView, selection);

            // Assert
            Assert.Fail("Create or modify test");
        }

        [Test]
        public void CannotCallGetAllGridsInViewWithNullActiveView()
        {
            Assert.Throws<ArgumentNullException>(() => GridManager.GetAllGridsInView(default(View), new List<ElementId>()));
        }

        [Test]
        public void CannotCallGetAllGridsInViewWithNullSelection()
        {
            Assert.Throws<ArgumentNullException>(() => GridManager.GetAllGridsInView(uiapp.ActiveUIDocument.ActiveView, default(List<ElementId>)));
        }

        [Test]
        public void GetAllGridsInViewPerformsMapping()
        {
            // Arrange
            var activeView = uiapp.ActiveUIDocument.ActiveView;
            var selection = new List<ElementId>();

            // Act
            var result = GridManager.GetAllGridsInView(activeView, selection);

            // Assert
            Assert.That(result.Capacity, Is.EqualTo(selection.Capacity));
            Assert.That(result.Count, Is.EqualTo(selection.Count));
        }

        [Test]
        public void CanCallGetAllLevelsInView()
        {
            // Arrange
            var activeView = uiapp.ActiveUIDocument.ActiveView;
            var selection = new List<ElementId>();

            // Act
            var result = GridManager.GetAllLevelsInView(activeView, selection);

            // Assert
            Assert.Fail("Create or modify test");
        }

        [Test]
        public void CannotCallGetAllLevelsInViewWithNullActiveView()
        {
            Assert.Throws<ArgumentNullException>(() => GridManager.GetAllLevelsInView(default(View), new List<ElementId>()));
        }

        [Test]
        public void CannotCallGetAllLevelsInViewWithNullSelection()
        {
            Assert.Throws<ArgumentNullException>(() => GridManager.GetAllLevelsInView(uiapp.ActiveUIDocument.ActiveView, default(List<ElementId>)));
        }

        [Test]
        public void GetAllLevelsInViewPerformsMapping()
        {
            // Arrange
            var activeView = uiapp.ActiveUIDocument.ActiveView;
            var selection = new List<ElementId>();

            // Act
            var result = GridManager.GetAllLevelsInView(activeView, selection);

            // Assert
            Assert.That(result.Capacity, Is.EqualTo(selection.Capacity));
            Assert.That(result.Count, Is.EqualTo(selection.Count));
        }
    }
}