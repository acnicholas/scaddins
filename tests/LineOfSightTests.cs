using Autodesk.Revit.ApplicationServices;
using Autodesk.Revit.DB;
using NUnit.Framework;
using static System.Net.Mime.MediaTypeNames;
using System.Reflection.Metadata;
using System.Transactions;
using SCaddins.LineOfSight;

namespace SCaddinsTestProject
{
    public class LineOfSiteTests : OneTimeOpenDocumentTest
    {

        //[TestCase("DraftingViewTest", ExpectedResult = "DraftingViewTest")]
        //[TestCase("DraftingViewTest2", ExpectedResult = "DraftingViewTest2")]
        //public string CreateDraftingView(string newViewName)
        //{
        //    var stadiumSeatingTier = new StadiumSeatingTier(document);
        //    var draftingView = stadiumSeatingTier.CreateLineOfSightDraftingView(newViewName);
        //    return draftingView.Name;
        //}

        [Test]
        public void CreateDraftingViewWithNoValues()
        {
            int startCount;
            using (var fec = new FilteredElementCollector(document))
            {
                fec.OfClass(typeof(ViewDrafting));
                startCount = fec.Count();
            }

            var stadiumSeatingTier = new StadiumSeatingTier(document);
            stadiumSeatingTier.Draw();

            using (var fec = new FilteredElementCollector(document))
            {
                fec.OfClass(typeof(ViewDrafting));
                Assert.IsTrue(fec.Count() - startCount == 1);
            }
        }
    }
}