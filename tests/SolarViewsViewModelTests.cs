using Autodesk.Revit.ApplicationServices;
using Autodesk.Revit.DB;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using NUnit.Framework;
using static System.Net.Mime.MediaTypeNames;
using System.Reflection.Metadata;
using System.Transactions;
using Caliburn.Micro;
using SCaddins;
using SCaddins.SolarAnalysis.ViewModels;
using System.Threading;
using System.Threading.Tasks;
using SCaddins.Common;

namespace SCaddinsTestProject
{
    public class SolarViewsViewModelTests : OneTimeOpenDocumentTest
    {
        SCaddins.SolarAnalysis.ViewModels.SolarViewsViewModel vm;

        [OneTimeSetUp]
        public void Setup()
        {
            vm = new SolarViewsViewModel(uiapp.ActiveUIDocument);
        }

        [Test]
        public void Intervals()
        {
            Assert.That(SolarViewsViewModel.Intervals.Count(), Is.EqualTo(3));
        }

        [Test]
        public void CanCreateAnalysisView()
        {
            Assert.IsTrue(vm.CanCreateAnalysisView == vm.CreateAnalysisView);
        }

        [Test]
        public void CanRotateCurrentView()
        {
            //change view for teis test...
            Assert.IsTrue(vm.CanRotateCurrentView == vm.RotateCurrentView);
        }

        [Test]
        public void CreationDate()
        {
            Assert.NotNull(vm.CreationDate);
        }

        [Test]
        public void CurrentModeSummary()
        {
            Assert.IsTrue(!string.IsNullOrEmpty(vm.CurrentModeSummary));
        }

        [Test]
        public void EnableRotateCurrentView()
        {
            Assert.IsTrue(vm.CanRotateCurrentView == vm.EnableRotateCurrentView);
        }

        [Test]
        public void EndTimes()
        {
            Assert.IsNotNull(vm.EndTimes);
        }

        [Test]
        public void CreateShadowPlans()
        {
            var fec = new FilteredElementCollector(document);
            fec.OfCategory(BuiltInCategory.OST_Views);
            var startViewCount = fec.Count();

            var vm = new SolarViewsViewModel(uiapp.ActiveUIDocument);
            vm.CreateShadowPlans = true;
            vm.OK();

            var fecEnd = new FilteredElementCollector(document);
            fecEnd.OfCategory(BuiltInCategory.OST_Views);
            var endViewCount = fecEnd.Count();

            Assert.IsTrue(endViewCount - startViewCount == 7);
        }

        [Test]
        public void SolarViewsViewModelTests_OK_Create3dViews()
        {
            var fec = new FilteredElementCollector(document);
            fec.OfCategory(BuiltInCategory.OST_Views);
            var startViewCount = fec.Count();

            var vm = new SolarViewsViewModel(uiapp.ActiveUIDocument);
            vm.Create3dViews = true;
            vm.OK();

            var fecEnd = new FilteredElementCollector(document);
            fecEnd.OfCategory(BuiltInCategory.OST_Views);
            var endViewCount = fecEnd.Count();

            Assert.IsTrue(endViewCount - startViewCount == 7);
        }
    }
}