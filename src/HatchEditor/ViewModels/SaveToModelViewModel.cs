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
    using System.Collections.Generic;
    using System.Linq;
    using Caliburn.Micro;

    public class SaveToModelViewModel : Screen
    {
        private List<Hatch> fillPatternsInModel;
        private string newPatternName;

        public SaveToModelViewModel(List<Hatch> fillPatternsInModel, string name)
        {
            this.fillPatternsInModel = fillPatternsInModel;
            NewPatternName = name;
            Label = "Pattern Name (Must be unique)";
        }

        public SaveToModelViewModel(string name)
        {
            fillPatternsInModel = null;
            NewPatternName = name;
            Label = "Pattern Name";
        }

        public static System.Dynamic.ExpandoObject DefaultWindowSettings
        {
            get
            {
                dynamic settings = new System.Dynamic.ExpandoObject();
                settings.Height = 160;
                settings.Width = 320;
                settings.Title = "Fill Pattern Name";
                settings.ShowInTaskbar = false;
                settings.SizeToContent = System.Windows.SizeToContent.Manual;
                return settings;
            }
        }

        public string Label
        {
            get; private set;
        }

        public string NewPatternName
        {
            get
            {
                return newPatternName;
            }

            set
            {
                newPatternName = value;
                NotifyOfPropertyChange(() => SavingIsEnabled);
            }
        }

        public bool SavingIsEnabled
        {
            get
            {
                if (fillPatternsInModel == null)
                {
                    return true;
                }
                var n = fillPatternsInModel.Select(s => s.Name).ToList().Contains(NewPatternName);
                return !string.IsNullOrEmpty(NewPatternName) && !n;
            }
        }

        public void Cancel()
        {
            TryCloseAsync(false);
        }

        public void Save()
        {
            TryCloseAsync(true);
        }
    }
}
