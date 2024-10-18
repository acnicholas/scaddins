using Autodesk.Revit.ApplicationServices;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using SCaddins;
using NUnit.Framework;
using static System.Net.Mime.MediaTypeNames;
using System.Reflection.Metadata;
using System.Transactions;
using SCaddins.LineOfSight;

namespace SCaddinsTestProject
{
   
    /// <summary>
    /// OneTimeOpenDocumentTest
    /// </summary>
    public class OneTimeOpenDocumentTest
    {
        public UIApplication uiapp;
        public Autodesk.Revit.ApplicationServices.Application application;
        public UIControlledApplication uiControlledApplication;
        public ControlledApplication controlledApplication;
        public Autodesk.Revit.DB.Document document;
        public static SCaddins.Common.WindowManager windowManager;

        public virtual string FileName => "C:\\Home\\Andrew\\Code\\cs\\scaddins\\tests\\SCAddinsTestModel.rvt";

        [OneTimeSetUp]
        public void Setup(UIApplication uiapp,
            Autodesk.Revit.ApplicationServices.Application application,
            UIControlledApplication uiControlledApplication,
            ControlledApplication controlledApplication)
        {
            SCaddinsApp.WindowManager = new SCaddins.Common.WindowManager(new SCaddins.Common.MockDialogService());

            this.application = application;
            if (string.IsNullOrEmpty(FileName))
            {
                document = application.NewProjectDocument(UnitSystem.Metric);
                return;
            }
            document = uiapp.OpenAndActivateDocument(FileName).Document;
            this.uiapp = uiapp;
        }

        [OneTimeTearDown]
        public void CloseProjectDocument()
        {
            this.document.Close(false);
            this.document.Dispose();
        }

    }
}