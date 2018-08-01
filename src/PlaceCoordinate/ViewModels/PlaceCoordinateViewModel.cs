
using System.Collections.Generic;
using System.Linq;
using Autodesk.Revit.DB;
using Caliburn.Micro;

namespace SCaddins.PlaceCoordinate.ViewModels
{
    class PlaceCoordinateViewModel : Screen
    {
        private List<Autodesk.Revit.DB.Level> levelsInModel;
        private List<Autodesk.Revit.DB.FamilySymbol> familiesInModel;
        private Document doc;

        public PlaceCoordinateViewModel(Document doc)
        {
            this.doc = doc;
            familiesInModel = SCaddins.PlaceCoordinate.Command.GetAllFamilySymbols(doc);
        }

        public double XCoordinate
        {
            get; set;
        }

        public double YCoordinate
        {
            get; set;
        }

        public double ZCoordinate
        {
            get; set;
        }

        public FamilySymbol SelectedFamily
        {
            get; set;
        }

        public List<Autodesk.Revit.DB.FamilySymbol> Families
        {
            get { return familiesInModel; }
        }

        public static dynamic DefaultWindowSettings
        {
            get
            {
                dynamic settings = new System.Dynamic.ExpandoObject();
                settings.Height = 400;
                //settings.Icon = new System.Windows.Media.Imaging.BitmapImage(
                //  new System.Uri("pack://application:,,,/SCaddins;component/Assets/scasfar.png")
                //  );
                settings.Title = "Place Coordinate - By Andrew Nicholas";
                settings.ShowInTaskbar = false;
                settings.SizeToContent = System.Windows.SizeToContent.WidthAndHeight;
                settings.ResizeMode = System.Windows.ResizeMode.CanResize;
                return settings;
            }
        }

        public void Cancel()
        {
            TryClose(false);
        }

        public void PlaceFamilyAtCoordinate()
        {
            TryClose(true);
        }
    }
}
