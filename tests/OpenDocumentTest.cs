using Autodesk.Revit.ApplicationServices;
using Autodesk.Revit.DB;
using NUnit.Framework;
using static System.Net.Mime.MediaTypeNames;
using System.Reflection.Metadata;
using System.Transactions;

namespace SCaddinsTestProject
{
    public class OpenDocumentTest : OneTimeOpenDocumentTest
    {
        [Test]
        public void RevitTests_DocumentTitle()
        {
            Console.WriteLine(document.Title);
        }

        [TestCase("RevitTest")]
        [TestCase("Author")]
        public void RevitTests_TransactionAuthor(string author)
        {
            using (Autodesk.Revit.DB.Transaction transaction = new Autodesk.Revit.DB.Transaction(document))
            {
                transaction.Start("Change Author");
                document.ProjectInformation.Author = author;
                transaction.Commit();
            }
            Assert.AreEqual(author, document.ProjectInformation.Author);
        }

        [TestCase("RevitTest", ExpectedResult = "RevitTest")]
        [TestCase("ClientName", ExpectedResult = "ClientName")]
        public string RevitTests_TransactionClientName_Expected(string clientName)
        {
            using (Autodesk.Revit.DB.Transaction transaction = new Autodesk.Revit.DB.Transaction(document))
            {
                transaction.Start("Change ClientName");
                document.ProjectInformation.ClientName = clientName;
                transaction.Commit();
            }
            return document.ProjectInformation.ClientName;
        }
    }
}