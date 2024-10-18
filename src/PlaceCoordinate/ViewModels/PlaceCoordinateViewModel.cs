// (C) Copyright 2018-2020 by Andrew Nicholas
//
// This file is part of SCaddins.
//
// SCaddins is free software: you can redistribute it and/or modify
// it under the terms of the GNU Lesser General Public License as published by
// the Free Software Foundation, either version 3 of the License, or
// (at your option) any later version.
//
// SCaddins is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU Lesser General Public License for more details.
//
// You should have received a copy of the GNU Lesser General Public License
// along with SCaddins.  If not, see <http://www.gnu.org/licenses/>.

namespace SCaddins.PlaceCoordinate.ViewModels
{
    using System.Collections.Generic;
    using Autodesk.Revit.DB;
    using Caliburn.Micro;

    public class PlaceCoordinateViewModel : Screen
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Microsoft.Usage", "CA2213: Disposable fields should be disposed", Justification = "Parameter initialized by Revit", MessageId = "doc")]
        private Document doc;
        private List<FamilySymbol> familiesInModel;
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Microsoft.Usage", "CA2213: Disposable fields should be disposed", Justification = "Parameter intialized by Revit", MessageId = "selectedFamilySymbol")]
        private FamilySymbol selectedFamilySymbol;
        private bool useSharedCoordinates;

        public PlaceCoordinateViewModel(Document doc)
        {
            this.doc = doc;
            useSharedCoordinates = true;
            XCoordinate = 0;
            YCoordinate = 0;
            ZCoordinate = 0;
            familiesInModel = Command.GetAllFamilySymbols(doc);
            selectedFamilySymbol = Command.TryGetDefaultSpotCoordFamily(familiesInModel);
            if (selectedFamilySymbol == null)
            {
                selectedFamilySymbol = Command.TryLoadDefaultSpotCoordFamily(doc);
                if (selectedFamilySymbol != null)
                {
                    familiesInModel.Add(selectedFamilySymbol);
                }
            }
        }

        public static dynamic DefaultWindowSettings
        {
            get
            {
                dynamic settings = new System.Dynamic.ExpandoObject();
                settings.Height = 400;

                ////settings.Icon = new System.Windows.Media.Imaging.BitmapImage(
                ////  new System.Uri("pack://application:,,,/SCaddins;component/Assets/scasfar.png")
                ////  );
                settings.Title = "Place Coordinate - By Andrew Nicholas";
                settings.ShowInTaskbar = false;
                settings.SizeToContent = System.Windows.SizeToContent.WidthAndHeight;
                settings.ResizeMode = System.Windows.ResizeMode.CanResize;
                settings.WindowStartupLocation = System.Windows.WindowStartupLocation.CenterScreen;
                return settings;
            }
        }

        public List<FamilySymbol> FamilySymbols
        {
            get
            {
                return familiesInModel;
            }

            set
            {
                if (value != familiesInModel)
                {
                    familiesInModel = value;
                    NotifyOfPropertyChange(() => FamilySymbols);
                }
            }
        }

        public bool PlaceFamilyAtCoordinateIsEnabled
        {
            ////get { return false; }
            get { return SelectedFamilySymbol != null; }
        }

        public FamilySymbol SelectedFamilySymbol
        {
            get
            {
                return selectedFamilySymbol;
            }

            set
            {
                if (value != selectedFamilySymbol)
                {
                    selectedFamilySymbol = value;
                    NotifyOfPropertyChange(() => SelectedFamilySymbol);
                    NotifyOfPropertyChange(() => PlaceFamilyAtCoordinateIsEnabled);
                }
            }
        }

        public bool UseSharedCoordinates
        {
            get
            {
                return useSharedCoordinates;
            }

            set
            {
                if (value != useSharedCoordinates)
                {
                    useSharedCoordinates = value;
                    NotifyOfPropertyChange(() => XCoordinateLabel);
                    NotifyOfPropertyChange(() => YCoordinateLabel);
                    NotifyOfPropertyChange(() => ZCoordinateLabel);
                }
            }
        }

        public double XCoordinate
        {
            get; set;
        }

        public string XCoordinateLabel
        {
            get
            {
                return UseSharedCoordinates ? "East / West" : "X Value";
            }
        }

        public double YCoordinate
        {
            get; set;
        }

        public string YCoordinateLabel
        {
            get
            {
                return UseSharedCoordinates ? "North / South" : "Y Value";
            }
        }

        public double ZCoordinate
        {
            get; set;
        }

        public string ZCoordinateLabel
        {
            get
            {
                return UseSharedCoordinates ? "Elevation" : "Z Value";
            }
        }

        public void Cancel()
        {
            TryCloseAsync(false);
        }

        public void PlaceFamilyAtCoordinate()
        {
            Command.PlaceFamilyAtCoordinate(doc, SelectedFamilySymbol, new XYZ(XCoordinate, YCoordinate, ZCoordinate), UseSharedCoordinates);
            TryCloseAsync(true);
        }
    }
}