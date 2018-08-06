using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.Generic;
using Autodesk.Revit.DB;
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

