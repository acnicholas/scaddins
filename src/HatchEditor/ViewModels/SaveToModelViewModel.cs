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
        }

        public bool SavingIsEnabled
        {
            get
            {
                var n = fillPatternsInModel.Select(s => s.Name).ToList().Contains(NewPatternName);
                return !string.IsNullOrEmpty(NewPatternName) && !n;
            }
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
