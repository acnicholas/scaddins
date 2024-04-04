// (C) Copyright 2019 by Andrew Nicholas
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

namespace SCaddins.HatchEditor.ViewModels
{
    using System.Collections.ObjectModel;
    using System.Dynamic;
    using System.Linq;
    using Autodesk.Revit.DB;
    using Caliburn.Micro;

    internal class SelectHatchViewModel : Screen
    {
        private ObservableCollection<Hatch> fillPattensInModel;
        private Hatch selectedFillPattern;

        public SelectHatchViewModel(Document doc)
        {
            FillPatterns = new ObservableCollection<Hatch>(Command.FillPatterns(doc));
        }

        public SelectHatchViewModel(ObservableCollection<Hatch> fillPatterns)
        {
            FillPatterns = fillPatterns;
        }

        public ObservableCollection<Hatch> FillPatterns
        {
            get => fillPattensInModel;

            set
            {
                fillPattensInModel = value;
                NotifyOfPropertyChange(() => FillPatterns);
            }
        }

        public Hatch SelectedFillPattern
        {
            get => selectedFillPattern;

            set
            {
                selectedFillPattern = value;
                NotifyOfPropertyChange(() => SelectedFillPattern);
            }
        }

        public static ExpandoObject DefualtWindowSettings()
        {
            dynamic settings = new ExpandoObject();
            settings.Height = 480;
            settings.Width = 768;
            settings.Title = "Hatch Editor - By Andrew Nicholas";
            settings.ShowInTaskbar = false;
            settings.SizeToContent = System.Windows.SizeToContent.Manual;
            return settings;
        }

        public void Cancel()
        {
            TryCloseAsync(false);
        }

        public void Select()
        {
            TryCloseAsync(true);
        }
    }
}
