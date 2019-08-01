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
            this.fillPatternsInModel = null;
            NewPatternName = name;
            Label = "Pattern Name";
        }

        public static System.Dynamic.ExpandoObject DefaultWindowSettings {
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

        public string Label {
            get; private set;
        }

        public string NewPatternName {
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

        public bool SavingIsEnabled {
            get
            {
                if (fillPatternsInModel == null) {
                    return true;
                }
                var n = fillPatternsInModel.Select(s => s.Name).ToList().Contains(NewPatternName);
                return !string.IsNullOrEmpty(NewPatternName) && !n;
            }
        }

        public void Cancel()
        {
            this.TryClose(false);
        }

        public void Save()
        {
            this.TryClose(true);
        }
    }
}
