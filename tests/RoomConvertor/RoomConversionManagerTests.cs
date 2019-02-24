using NUnit.Framework;
using SCaddins.RoomConvertor;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;
using SCaddins.SheetCopier;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;
using RTF.Framework;
using System.IO;
using RTF.Applications;
using Autodesk.Revit.DB;

namespace SCaddins.RoomConvertor.Tests
{
    [TestFixture()]
    public class RoomConversionManagerTests
    {
        [Test()]
        public void CreateViewsAndSheetsTest()
        {
            Assert.Fail();
        }

        [Test()]
        [TestModel(@"./rac_basic_sample_project.rvt")]
        public void GetAllTitleBlockTypesTest()
        {
            var doc = RevitTestExecutive.CommandData.Application.ActiveUIDocument.Document;
            Assert.IsTrue(RoomConversionManager.GetAllTitleBlockTypes(doc).Count == 4);
        }

        [Test()]
        public void GetTitleBlockByNameTest()
        {
            Assert.Fail();
        }

        [Test()]
        public void CreateRoomMassesTest()
        {
            var doc = RevitTestExecutive.CommandData.Application.ActiveUIDocument.Document;
            var manager = new RoomConversionManager(doc);
            manager.CreateRoomMasses(manager.Candidates);
            Assert.Fail();
        }

        [Test()]
        public void SynchronizeMassesToRoomsTest()
        {
            Assert.Fail();
        }
    }
}