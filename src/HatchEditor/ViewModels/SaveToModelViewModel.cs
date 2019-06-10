namespace SCaddins.HatchEditor.ViewModels
{
    using System.Collections.Generic;
    using Caliburn.Micro;

    class SaveToModelViewModel : Screen
    {
        //private List<string> patternsInModel;

        public SaveToModelViewModel()
        {

        }

        public string NewPatternName
        {
            get; set;
        }

        public bool SavingIsEnabled
        {
            get
            {
                return false;
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
