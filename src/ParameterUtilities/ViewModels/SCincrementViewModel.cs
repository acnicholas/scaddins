// (C) Copyright 2018 by Andrew Nicholas
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

using Caliburn.Micro;

namespace SCaddins.ParameterUtilities.ViewModels 
{
    class SCincrementViewModel : Screen
    {

        public SCincrementViewModel()
        {
            LoadSettings();
        }

        public static dynamic DefaultWindowSettings
        {
            get
            {
                dynamic settings = new System.Dynamic.ExpandoObject();
                settings.Height = 400;
                settings.Title = "SCincrement Settigns";
                settings.ShowInTaskbar = false;
                settings.SizeToContent = System.Windows.SizeToContent.WidthAndHeight;
                settings.ResizeMode = System.Windows.ResizeMode.CanResize;
                return settings;
            }
        }

        public void Quit()
        {
            TryClose(false);
        }

        public void ResetSettingsToDefault()
        {
            SCincrementSettings.Default.Reset();
            this.LoadSettings();
        }

        public void SaveSettings()
        {
            SCincrementSettings.Default.OffsetValue = OffsetValue;
            SCincrementSettings.Default.IncrementValue = IncrementValue;
            SCincrementSettings.Default.SourceSearchPattern = SourceSearchPattern;
            SCincrementSettings.Default.SourceReplacePattern = SourceReplacementPattern;
            SCincrementSettings.Default.DestinationReplacePattern = DestinationReplacementPattern;
            SCincrementSettings.Default.DestinationSearchPattern = DestinationSearchPattern;
            SCincrementSettings.Default.CustomParameterName = CustomParameterName;
            SCincrementSettings.Default.UseCustomParameterName = UseCustomParameter;
            SCincrementSettings.Default.Save();
            TryClose(true);
        }

        public string SourceSearchPattern { get; set; }
        public string SourceReplacementPattern { get; set; }
        public string DestinationSearchPattern { get; set; }
        public string DestinationReplacementPattern { get; set; }
        public int IncrementValue { get; set; }
        public int OffsetValue { get; set; }
        public bool UseCustomParameter { get; set; }
        public string CustomParameterName { get; set; }

        private void LoadSettings()
        {
            OffsetValue = SCincrementSettings.Default.OffsetValue;
            IncrementValue = SCincrementSettings.Default.IncrementValue;
            SourceReplacementPattern = SCincrementSettings.Default.SourceReplacePattern;
            SourceSearchPattern = SCincrementSettings.Default.SourceSearchPattern;
            DestinationReplacementPattern = SCincrementSettings.Default.DestinationReplacePattern;
            DestinationSearchPattern= SCincrementSettings.Default.DestinationSearchPattern;
            CustomParameterName = SCincrementSettings.Default.CustomParameterName;
            UseCustomParameter = SCincrementSettings.Default.UseCustomParameterName;
            NotifyOfPropertyChange(() => OffsetValue);
            NotifyOfPropertyChange(() => IncrementValue);
            NotifyOfPropertyChange(() => SourceReplacementPattern);
            NotifyOfPropertyChange(() => SourceSearchPattern);
            NotifyOfPropertyChange(() => DestinationReplacementPattern);
            NotifyOfPropertyChange(() => DestinationSearchPattern);
            NotifyOfPropertyChange(() => CustomParameterName);
            NotifyOfPropertyChange(() => UseCustomParameter);
        }
    }
}

