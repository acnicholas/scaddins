using Autodesk.Revit.ApplicationServices;
using Autodesk.Revit.DB;
using NUnit.Framework;
using static System.Net.Mime.MediaTypeNames;
using System.Reflection.Metadata;
using System.Transactions;
using SCaddins.RoomConverter;

namespace SCaddinsTestProject
{
    public class RoomToolsTests : OneTimeOpenDocumentTest
    {

        [Test]
        public void RoomToolsTests_CreateRoomMasses()
        {
            var manager = new RoomConversionManager(document);
            manager.CreateRoomMasses(manager.Candidates);
            var fec = new FilteredElementCollector(document);
            fec.OfCategory(BuiltInCategory.OST_Mass);
            Assert.IsTrue(fec.Count() == 4);
        }
    }
}