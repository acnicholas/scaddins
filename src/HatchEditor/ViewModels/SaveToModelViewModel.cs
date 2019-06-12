namespace SCaddins.HatchEditor.ViewModels
{
    using Caliburn.Micro;

    public class SaveToModelViewModel : Screen
    {
        public SaveToModelViewModel()
        {
        }

        public static bool SavingIsEnabled
        {
            get
            {
                return false;
            }
        }

        public string NewPatternName
        {
            get; set;
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
