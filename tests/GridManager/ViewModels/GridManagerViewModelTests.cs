namespace ScaddinsTestProject.GridManager.ViewModels
{
    using System;
    using System.Collections.Generic;
    using Autodesk.Revit.DB;
    using NUnit.Framework;
    using SCaddins.GridManager.ViewModels;
    using SCaddinsTestProject;

    [TestFixture]
    public class GridManagerViewModelTests : OneTimeOpenDocumentTest
    {
        private GridManagerViewModel _testClass;
        private View _activeView;
        private List<ElementId> _selection;

        [SetUp]
        public void SetUp()
        {
            _activeView = uiapp.ActiveUIDocument.ActiveView;
            _selection = new List<ElementId>();
            _testClass = new GridManagerViewModel(_activeView, _selection);
        }

        [Test]
        public void CanConstruct()
        {
            // Act
            var instance = new GridManagerViewModel(_activeView, _selection);

            // Assert
            Assert.That(instance, Is.Not.Null);
        }

        [Test]
        public void CannotConstructWithNullActiveView()
        {
            Assert.Throws<ArgumentNullException>(() => new GridManagerViewModel(default(View), _selection));
        }

        [Test]
        public void CannotConstructWithNullSelection()
        {
            Assert.Throws<ArgumentNullException>(() => new GridManagerViewModel(_activeView, default(List<ElementId>)));
        }

        [Test]
        public void CanCallShowAllGridBubbles()
        {
            // Act
            _testClass.ShowAllGridBubbles();

            // Assert
            Assert.Fail("Create or modify test");
        }

        [Test]
        public void CanCallHideAllGridBubbles()
        {
            // Act
            _testClass.HideAllGridBubbles();

            // Assert
            Assert.Fail("Create or modify test");
        }

        [Test]
        public void CanCallShowAllLevelEnds()
        {
            // Act
            _testClass.ShowAllLevelEnds();

            // Assert
            Assert.Fail("Create or modify test");
        }

        [Test]
        public void CanCallHideAllLevelEnds()
        {
            // Act
            _testClass.HideAllLevelEnds();

            // Assert
            Assert.Fail("Create or modify test");
        }

        [Test]
        public void CanCallSetGridsTo2d()
        {
            // Act
            _testClass.SetGridsTo2d();

            // Assert
            Assert.Fail("Create or modify test");
        }

        [Test]
        public void CanCallSetGridsTo3d()
        {
            // Act
            _testClass.SetGridsTo3d();

            // Assert
            Assert.Fail("Create or modify test");
        }

        [Test]
        public void CanCallSetLevelsTo2d()
        {
            // Act
            _testClass.SetLevelsTo2d();

            // Assert
            Assert.Fail("Create or modify test");
        }

        [Test]
        public void CanCallSetLevelsTo3d()
        {
            // Act
            _testClass.SetLevelsTo3d();

            // Assert
            Assert.Fail("Create or modify test");
        }

        [Test]
        public void CanSetAndGetShowBottomGridBubbles()
        {
            _testClass.CheckProperty(x => x.ShowBottomGridBubbles, true, true);
        }

        [Test]
        public void CanGetCanShowBottomGridBubbles()
        {
            // Assert
            Assert.That(_testClass.CanShowBottomGridBubbles, Is.InstanceOf<bool>());

            Assert.Fail("Create or modify test");
        }

        [Test]
        public void CanGetLevelsGroupBoxEnabled()
        {
            // Assert
            Assert.That(_testClass.LevelsGroupBoxEnabled, Is.InstanceOf<bool>());

            Assert.Fail("Create or modify test");
        }

        [Test]
        public void CanGetShowBottomGridBubblesLabel()
        {
            // Assert
            Assert.That(_testClass.ShowBottomGridBubblesLabel, Is.InstanceOf<string>());

            Assert.Fail("Create or modify test");
        }

        [Test]
        public void CanSetAndGetShowLeftGridBubbles()
        {
            _testClass.CheckProperty(x => x.ShowLeftGridBubbles, true, true);
        }

        [Test]
        public void CanGetCanShowLeftGridBubbles()
        {
            // Assert
            Assert.That(_testClass.CanShowLeftGridBubbles, Is.InstanceOf<bool>());

            Assert.Fail("Create or modify test");
        }

        [Test]
        public void CanGetShowLeftGridBubblesLabel()
        {
            // Assert
            Assert.That(_testClass.ShowLeftGridBubblesLabel, Is.InstanceOf<string>());

            Assert.Fail("Create or modify test");
        }

        [Test]
        public void CanSetAndGetShowLeftLevels()
        {
            _testClass.CheckProperty(x => x.ShowLeftLevels, true, true);
        }

        [Test]
        public void CanGetShowLeftLevelsIsEnabled()
        {
            // Assert
            Assert.That(_testClass.ShowLeftLevelsIsEnabled, Is.InstanceOf<bool>());

            Assert.Fail("Create or modify test");
        }

        [Test]
        public void CanGetShowLeftLevelsLabel()
        {
            // Assert
            Assert.That(_testClass.ShowLeftLevelsLabel, Is.InstanceOf<string>());

            Assert.Fail("Create or modify test");
        }

        [Test]
        public void CanSetAndGetShowRightGridBubbles()
        {
            _testClass.CheckProperty(x => x.ShowRightGridBubbles, false, false);
        }

        [Test]
        public void CanGetCanShowRightGridBubbles()
        {
            // Assert
            Assert.That(_testClass.CanShowRightGridBubbles, Is.InstanceOf<bool>());

            Assert.Fail("Create or modify test");
        }

        [Test]
        public void CanGetShowRightGridBubblesLabel()
        {
            // Assert
            Assert.That(_testClass.ShowRightGridBubblesLabel, Is.InstanceOf<string>());

            Assert.Fail("Create or modify test");
        }

        [Test]
        public void CanSetAndGetShowRightLevels()
        {
            _testClass.CheckProperty(x => x.ShowRightLevels, false, true);
        }

        [Test]
        public void CanGetShowRightLevelsIsEnabled()
        {
            // Assert
            Assert.That(_testClass.ShowRightLevelsIsEnabled, Is.InstanceOf<bool>());

            Assert.Fail("Create or modify test");
        }

        [Test]
        public void CanGetShowRightLevelsLabel()
        {
            // Assert
            Assert.That(_testClass.ShowRightLevelsLabel, Is.InstanceOf<string>());

            Assert.Fail("Create or modify test");
        }

        [Test]
        public void CanSetAndGetShowTopGridBubbles()
        {
            _testClass.CheckProperty(x => x.ShowTopGridBubbles, false, true);
        }

        [Test]
        public void CanGetCanShowTopGridBubbles()
        {
            // Assert
            Assert.That(_testClass.CanShowTopGridBubbles, Is.InstanceOf<bool>());

            Assert.Fail("Create or modify test");
        }

        [Test]
        public void CanGetShowTopGridBubblesLabel()
        {
            // Assert
            Assert.That(_testClass.ShowTopGridBubblesLabel, Is.InstanceOf<string>());

            Assert.Fail("Create or modify test");
        }

        [Test]
        public void CanGetStatusBarLabel()
        {
            // Assert
            Assert.That(_testClass.StatusBarLabel, Is.InstanceOf<string>());

            Assert.Fail("Create or modify test");
        }

        [Test]
        public void CanGetCanShowAllGridBubbles()
        {
            // Assert
            Assert.That(_testClass.CanShowAllGridBubbles, Is.InstanceOf<bool>());

            Assert.Fail("Create or modify test");
        }

        [Test]
        public void CanGetCanHideAllGridBubbles()
        {
            // Assert
            Assert.That(_testClass.CanHideAllGridBubbles, Is.InstanceOf<bool>());

            Assert.Fail("Create or modify test");
        }

        [Test]
        public void CanGetCanShowAllLevelEnds()
        {
            // Assert
            Assert.That(_testClass.CanShowAllLevelEnds, Is.InstanceOf<bool>());

            Assert.Fail("Create or modify test");
        }

        [Test]
        public void CanGetCanHideAllLevelEnds()
        {
            // Assert
            Assert.That(_testClass.CanHideAllLevelEnds, Is.InstanceOf<bool>());

            Assert.Fail("Create or modify test");
        }

        [Test]
        public void CanGetCanSetGridsTo2d()
        {
            // Assert
            Assert.That(_testClass.CanSetGridsTo2d, Is.InstanceOf<bool>());

            Assert.Fail("Create or modify test");
        }

        [Test]
        public void CanGetCanSetGridsTo3d()
        {
            // Assert
            Assert.That(_testClass.CanSetGridsTo3d, Is.InstanceOf<bool>());

            Assert.Fail("Create or modify test");
        }

        [Test]
        public void CanGetCanSetLevelsTo2d()
        {
            // Assert
            Assert.That(_testClass.CanSetLevelsTo2d, Is.InstanceOf<bool>());

            Assert.Fail("Create or modify test");
        }

        [Test]
        public void CanGetCanSetLevelsTo3d()
        {
            // Assert
            Assert.That(_testClass.CanSetLevelsTo3d, Is.InstanceOf<bool>());

            Assert.Fail("Create or modify test");
        }
    }
}