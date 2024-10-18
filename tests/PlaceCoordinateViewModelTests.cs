using Autodesk.Revit.ApplicationServices;
using Autodesk.Revit.DB;
using NUnit.Framework;
using static System.Net.Mime.MediaTypeNames;
using System.Reflection.Metadata;
using System.Transactions;
using SCaddins.PlaceCoordinate.ViewModels;

namespace SCaddinsTestProject
{
    public class PlaceCoordinateViewModelTests : OneTimeOpenDocumentTest
    {
        [Test]
        public void FamilySymbols()
        {
            var vm = new PlaceCoordinateViewModel(document);
            Assert.IsNotNull(vm.FamilySymbols);
        }

        //[Test]
        //public void PlaceFamilyAtCoordinateIsEnabled()
        //{
        //    var vm = new PlaceCoordinateViewModel(document);
        //    vm.SelectedFamilySymbol[0];
        //}

    }
}