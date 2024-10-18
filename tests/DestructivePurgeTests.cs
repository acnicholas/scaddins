using Autodesk.Revit.ApplicationServices;
using Autodesk.Revit.DB;
using NUnit.Framework;
using static System.Net.Mime.MediaTypeNames;
using System.Reflection.Metadata;
using System.Transactions;
using SCaddins.DestructivePurge;

namespace SCaddinsTestProject
{
    public class DestructivePurgeTests : SCaddinsTestProject.OneTimeOpenDocumentTest
    {
        [Test]
        public void DestructivePurgeTests_GetImages()
        {
            var images = DestructivePurgeUtilitiles.Images(document);
            Assert.IsTrue(images.Count == 2);
        }

        [Test]
        public void DestructivePurgeTests_GetCADImports()
        {
            var imports = DestructivePurgeUtilitiles.Imports(document, false);
            Assert.IsTrue(imports.Count == 2);
        }

        [Test]
        public void DestructivePurgeTests_GetCADLinks()
        {
            var links = DestructivePurgeUtilitiles.Imports(document, true);
            Assert.IsTrue(links.Count == 2);
        }

        [Test]
        public void DestructivePurgeTests_PurgeCAImports()
        {
            var links = DestructivePurgeUtilitiles.Imports(document, true);
            DestructivePurgeUtilitiles.RemoveElements(document, links);
            var newlinks = DestructivePurgeUtilitiles.Imports(document, true);
            Assert.IsTrue(newlinks.Count == 0);
        }

        [Test]
        public void DestructivePurgeTests_PurgeCADLinks()
        {
            var links = DestructivePurgeUtilitiles.Imports(document, true);
            DestructivePurgeUtilitiles.RemoveElements(document, links);
            var newlinks = DestructivePurgeUtilitiles.Imports(document, true);
            Assert.IsTrue(newlinks.Count == 0);
        }
    }
}