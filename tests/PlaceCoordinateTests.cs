using Autodesk.Revit.ApplicationServices;
using Autodesk.Revit.DB;
using NUnit.Framework;
using static System.Net.Mime.MediaTypeNames;
using System.Reflection.Metadata;
using System.Transactions;
using SCaddins.PlaceCoordinate;
using System;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using Caliburn.Micro;
using System.Windows.Controls;
using System.Runtime.InteropServices;

namespace SCaddinsTestProject
{
    public class PlaceCoordinateTests : OneTimeOpenDocumentTest
    {

        [Test]
        public void DefaultSpotCoordinateFamilyExists()
        {
            Assert.IsTrue(Command.DefaultSpotCoordinateFamilyExists(document));
        }

        [TestCase("2025")]
        public void DefaultSpotCoordinateFamilyName(string version)
        {
            Assert.IsTrue(string.Equals(
                    Command.DefaultSpotCoordinateFamilyName(document),
                    SCaddins.Constants.FamilyDirectory + version + @"\SC-Survey_Point.rfa"));
        }

        [Test]
        public void TryGetDefaultSpotCoordFamily()
        {
            Assert.NotNull(Command.TryGetDefaultSpotCoordFamily(Command.GetAllFamilySymbols(document)));
        }

        [Test]
        public void TryLoadDefaultSpotCoordFamily()
        {
            Assert.NotNull(Command.TryLoadDefaultSpotCoordFamily(document));
        }

        [Test]
        public void GetAllFamilySymbols()
        {
            Assert.IsNotNull(Command.GetAllFamilySymbols(document));
        }

        [TestCase(0,0,0, true)]
        [TestCase(0,0,0, false)]
        public void PlaceFamilyAtCoordinate(double x, double y, double z, bool useSharedCoordinates)
        {
            int startCount;
            XYZ location = new XYZ(x, y, z);
            using (var collector = new FilteredElementCollector(document))
            {
                collector.OfCategory(BuiltInCategory.OST_GenericModel);
                collector.OfClass(typeof(FamilySymbol));
                startCount = collector.Count();
            }

            Command.PlaceFamilyAtCoordinate(
                document,
                Command.TryLoadDefaultSpotCoordFamily(document),
                location,
                useSharedCoordinates);

            int endCount;
            using (var collector = new FilteredElementCollector(document))
            {
                collector.OfCategory(BuiltInCategory.OST_GenericModel);
                collector.OfClass(typeof(FamilySymbol));
                endCount = collector.Count();
            }

            Assert.IsTrue(startCount + 1 == endCount);
        }

    }
}