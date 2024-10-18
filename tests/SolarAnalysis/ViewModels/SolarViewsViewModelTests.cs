namespace ScaddinsTestProject.SolarAnalysis.ViewModels
{
    using System;
    using System.Collections.Generic;
    using System.Threading;
    using System.Threading.Tasks;
    using System.Windows;
    using Autodesk.Revit.DB;
    using Autodesk.Revit.UI;
    using Caliburn.Micro;
    using NUnit.Framework;
    using SCaddins.SolarAnalysis.ViewModels;
    using SCaddinsTestProject;

    [TestFixture]
    public class SolarViewsViewModelTests : OneTimeOpenDocumentTest
    {
        private class TestSolarViewsViewModel : SolarViewsViewModel
        {
            public TestSolarViewsViewModel(UIDocument uidoc) : base(uidoc)
            {
            }

            public Task PublicOnDeactivateAsync(bool close, CancellationToken cancellationToken)
            {
                return base.OnDeactivateAsync(close, cancellationToken);
            }
        }

        private TestSolarViewsViewModel _testClass;
        private UIDocument _uidoc;

        [SetUp]
        public void SetUp()
        {
            _uidoc =uiapp.ActiveUIDocument;
            _testClass = new TestSolarViewsViewModel(_uidoc);
        }

        [Test]
        public void CanConstruct()
        {
            // Act
            var instance = new TestSolarViewsViewModel(_uidoc);

            // Assert
            Assert.That(instance, Is.Not.Null);
        }

        [Test]
        public void CannotConstructWithNullUidoc()
        {
            Assert.Throws<ArgumentNullException>(() => new TestSolarViewsViewModel(default(UIDocument)));
        }

        [Test]
        public async Task CanCallRespawn()
        {
            // Arrange
            var viewModel = new SolarViewsViewModel(_uidoc);
            var resize = true;

            // Act
            await TestSolarViewsViewModel.Respawn(viewModel, resize);

            // Assert
            Assert.Fail("Create or modify test");
        }

        [Test]
        public void CannotCallRespawnWithNullViewModel()
        {
            Assert.ThrowsAsync<ArgumentNullException>(() => TestSolarViewsViewModel.Respawn(default(SolarViewsViewModel), false));
        }

        [Test]
        public void CanCallClear()
        {
            // Act
            _testClass.Clear();

            // Assert
            Assert.Fail("Create or modify test");
        }

        [Test]
        public void CanCallOK()
        {
            // Act
            _testClass.OK();

            // Assert
            Assert.Fail("Create or modify test");
        }

        [Test]
        public void CanCallRunAnalysis()
        {
            // Act
            _testClass.RunAnalysis();

            // Assert
            Assert.Fail("Create or modify test");
        }

        [Test]
        public void CanCallSelectAnalysisFaces()
        {
            // Act
            _testClass.SelectAnalysisFaces();

            // Assert
            Assert.Fail("Create or modify test");
        }

        [Test]
        public void CanCallSelectMasses()
        {
            // Act
            _testClass.SelectMasses();

            // Assert
            Assert.Fail("Create or modify test");
        }

        [Test]
        public async Task CanCallOnDeactivateAsync()
        {
            // Arrange
            var close = true;
            var cancellationToken = CancellationToken.None;

            // Act
            await _testClass.PublicOnDeactivateAsync(close, cancellationToken);

            // Assert
            Assert.Fail("Create or modify test");
        }

        [Test]
        public void CanGetDefaultViewSettings()
        {
            // Assert
            Assert.That(TestSolarViewsViewModel.DefaultViewSettings, Is.InstanceOf<dynamic>());

            Assert.Fail("Create or modify test");
        }

        [Test]
        public void CanGetIntervals()
        {
            // Assert
            Assert.That(TestSolarViewsViewModel.Intervals, Is.InstanceOf<BindableCollection<TimeSpan>>());

            Assert.Fail("Create or modify test");
        }

        [Test]
        public void CanSetAndGetAnalysisGridSize()
        {
            _testClass.CheckProperty(x => x.AnalysisGridSize, 1718044152.66, 1032438155.76);
        }

        [Test]
        public void CanGetCanCreateAnalysisView()
        {
            // Assert
            Assert.That(_testClass.CanCreateAnalysisView, Is.InstanceOf<bool>());

            Assert.Fail("Create or modify test");
        }

        [Test]
        public void CanGetCanRotateCurrentView()
        {
            // Assert
            Assert.That(_testClass.CanRotateCurrentView, Is.InstanceOf<bool>());

            Assert.Fail("Create or modify test");
        }

        [Test]
        public void CanSetAndGetCreate3dViews()
        {
            _testClass.CheckProperty(x => x.Create3dViews);
        }

        [Test]
        public void CanSetAndGetCreateAnalysisView()
        {
            _testClass.CheckProperty(x => x.CreateAnalysisView);
        }

        [Test]
        public void CanSetAndGetCreateShadowPlans()
        {
            _testClass.CheckProperty(x => x.CreateShadowPlans);
        }

        [Test]
        public void CanSetAndGetCreationDate()
        {
            _testClass.CheckProperty(x => x.CreationDate);
        }

        [Test]
        public void CanGetCurrentModeSummary()
        {
            // Assert
            Assert.That(_testClass.CurrentModeSummary, Is.InstanceOf<string>());

            Assert.Fail("Create or modify test");
        }

        [Test]
        public void CanSetAndGetDrawSolarRay()
        {
            _testClass.CheckProperty(x => x.DrawSolarRay);
        }

        [Test]
        public void CanGetEnableRotateCurrentView()
        {
            // Assert
            Assert.That(_testClass.EnableRotateCurrentView, Is.InstanceOf<bool>());

            Assert.Fail("Create or modify test");
        }

        [Test]
        public void CanGetEndTimes()
        {
            // Assert
            Assert.That(_testClass.EndTimes, Is.InstanceOf<BindableCollection<DateTime>>());

            Assert.Fail("Create or modify test");
        }

        //[Test]
        //public void CanSetAndGetFaceSelection()
        //{
        //    _testClass.CheckProperty(x => x.FaceSelection, new[] { new Reference(new Element()), new Reference(new Element()), new Reference(new Element()) }, new[] { new Reference(new Element()), new Reference(new Element()), new Reference(new Element()) });
        //}

        [Test]
        public void CanSetAndGetLeft()
        {
            _testClass.CheckProperty(x => x.Left, 1820447657.82, 663709909.5);
        }

        //[Test]
        //public void CanSetAndGetMassSelection()
        //{
        //    _testClass.CheckProperty(x => x.MassSelection, new[] { new Reference(new Element()), new Reference(new Element()), new Reference(new Element()) }, new[] { new Reference(new Element()), new Reference(new Element()), new Reference(new Element()) });
        //}

        [Test]
        public void CanSetAndGetRotateCurrentView()
        {
            _testClass.CheckProperty(x => x.RotateCurrentView);
        }

        [Test]
        public void CanSetAndGetSelectedCloseMode()
        {
            _testClass.CheckProperty(x => x.SelectedCloseMode, SolarViewsViewModel.CloseMode.DrawSolarRay, SolarViewsViewModel.CloseMode.Analize);
        }

        [Test]
        public void CanSetAndGetSelectedEndTime()
        {
            _testClass.CheckProperty(x => x.SelectedEndTime);
        }

        [Test]
        public void CanGetSelectedFaceInformation()
        {
            // Assert
            Assert.That(_testClass.SelectedFaceInformation, Is.InstanceOf<string>());

            Assert.Fail("Create or modify test");
        }

        [Test]
        public void CanGetShowSolarRayOptionsPanel()
        {
            // Assert
            Assert.That(_testClass.ShowSolarRayOptionsPanel, Is.InstanceOf<bool>());

            Assert.Fail("Create or modify test");
        }

        [Test]
        public void CanSetAndGetSolarRayLength()
        {
            _testClass.CheckProperty(x => x.SolarRayLength, 996417301.77, 682345539.81);
        }

        [Test]
        public void CanSetAndGetSelectedInterval()
        {
            _testClass.CheckProperty(x => x.SelectedInterval, TimeSpan.FromSeconds(253), TimeSpan.FromSeconds(407));
        }

        [Test]
        public void CanGetSelectedMassInformation()
        {
            // Assert
            Assert.That(_testClass.SelectedMassInformation, Is.InstanceOf<string>());

            Assert.Fail("Create or modify test");
        }

        [Test]
        public void CanSetAndGetSelectedStartTime()
        {
            _testClass.CheckProperty(x => x.SelectedStartTime);
        }

        [Test]
        public void CanGetShowDateSelectionPanel()
        {
            // Assert
            Assert.That(_testClass.ShowDateSelectionPanel, Is.InstanceOf<bool>());

            Assert.Fail("Create or modify test");
        }

        [Test]
        public void CanSetAndGetSize()
        {
            _testClass.CheckProperty(x => x.Size, new Size(), new Size());
        }

        [Test]
        public void CanGetStartTimes()
        {
            // Assert
            Assert.That(_testClass.StartTimes, Is.InstanceOf<BindableCollection<DateTime>>());

            Assert.Fail("Create or modify test");
        }

        [Test]
        public void CanSetAndGetTop()
        {
            _testClass.CheckProperty(x => x.Top, 423577679.58, 219898016.91);
        }

        [Test]
        public void CanGetViewInformation()
        {
            // Assert
            Assert.That(_testClass.ViewInformation, Is.InstanceOf<string>());

            Assert.Fail("Create or modify test");
        }
    }
}