// (C) Copyright 2018-2021 by Andrew Nicholas
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

namespace SCaddins.ParameterUtilities.ViewModels
{
    using System.Collections.Generic;
    using Caliburn.Micro;

    public class SCincrementViewModel : PropertyChangedBase, Common.ViewModels.ISettingPanel
    {
        private bool useDestinationSearchPattern;

        public SCincrementViewModel()
        {
            Reset();
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

        public List<IncrementConfig> Configs { get; set; }

        public string CustomParameterName { get; set; }

        public string DestinationReplacementPattern { get; set; }

        public string DestinationSearchPattern { get; set; }

        public int IncrementValue { get; set; }

        public bool KeepLeadingZeros { get; set; }

        public int OffsetValue { get; set; }

        public string SourceReplacementPattern { get; set; }

        public string SourceSearchPattern { get; set; }

        public bool UseCustomParameter { get; set; }

        public bool UseDestinationSearchPattern
        {
            get
            {
                return useDestinationSearchPattern;
            }

            set
            {
                if (value != useDestinationSearchPattern)
                {
                    useDestinationSearchPattern = value;
                    NotifyOfPropertyChange(() => UseDestinationSearchPattern);
                }
            }
        }

        public void ResetToDefault()
        {
            IncrementSettings.Default.Reset();
            Reset();
        }

        public void Apply()
        {
            IncrementSettings.Default.OffsetValue = OffsetValue;
            IncrementSettings.Default.IncrementValue = IncrementValue;
            IncrementSettings.Default.SourceSearchPattern = SourceSearchPattern;
            IncrementSettings.Default.SourceReplacePattern = SourceReplacementPattern;
            IncrementSettings.Default.DestinationReplacePattern = DestinationReplacementPattern;
            IncrementSettings.Default.DestinationSearchPattern = DestinationSearchPattern;
            IncrementSettings.Default.CustomParameterName = CustomParameterName;
            IncrementSettings.Default.UseCustomParameterName = UseCustomParameter;
            IncrementSettings.Default.KeepLeadingZeros = KeepLeadingZeros;
            IncrementSettings.Default.UseDestinationSearchPattern = UseDestinationSearchPattern;
            IncrementSettings.Default.Save();
        }

        public void Reset()
        {
            OffsetValue = IncrementSettings.Default.OffsetValue;
            IncrementValue = IncrementSettings.Default.IncrementValue;
            SourceReplacementPattern = IncrementSettings.Default.SourceReplacePattern;
            SourceSearchPattern = IncrementSettings.Default.SourceSearchPattern;
            DestinationReplacementPattern = IncrementSettings.Default.DestinationReplacePattern;
            DestinationSearchPattern = IncrementSettings.Default.DestinationSearchPattern;
            CustomParameterName = IncrementSettings.Default.CustomParameterName;
            UseCustomParameter = IncrementSettings.Default.UseCustomParameterName;
            KeepLeadingZeros = IncrementSettings.Default.KeepLeadingZeros;
            UseDestinationSearchPattern = IncrementSettings.Default.UseDestinationSearchPattern;
            NotifyOfPropertyChange(() => OffsetValue);
            NotifyOfPropertyChange(() => IncrementValue);
            NotifyOfPropertyChange(() => SourceReplacementPattern);
            NotifyOfPropertyChange(() => SourceSearchPattern);
            NotifyOfPropertyChange(() => DestinationReplacementPattern);
            NotifyOfPropertyChange(() => DestinationSearchPattern);
            NotifyOfPropertyChange(() => CustomParameterName);
            NotifyOfPropertyChange(() => UseCustomParameter);
            NotifyOfPropertyChange(() => KeepLeadingZeros);
            NotifyOfPropertyChange(() => UseDestinationSearchPattern);
        }
    }
}