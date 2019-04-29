namespace SCaddins.HatchEditor.ViewModels
{
    using System.Collections.ObjectModel;
    using Autodesk.Revit.DB;
    using Caliburn.Micro;

    internal class HatchEditorViewModel : Screen
    {
        private Document doc;
        private ObservableCollection<Hatch> fillPattensInModel;
        private Hatch selectedFillPattern;
        private string currentPatternDefinition;

        public HatchEditorViewModel(Autodesk.Revit.DB.Document doc)
        {
            this.doc = doc;
            FillPatterns = new ObservableCollection<Hatch>(Command.FillPatterns(doc));
            CurrentPatternDefinition = string.Empty;
        }

        public ObservableCollection<Hatch> FillPatterns
        {
            get
            {
                return fillPattensInModel;
            }

            set
            {
                fillPattensInModel = value;
                NotifyOfPropertyChange(() => FillPatterns);
            }
        }

        public Hatch SelectedFillPattern
        {
            get
            {
                return selectedFillPattern;
            }

            set
            {
                selectedFillPattern = value;
                CurrentPatternDefinition = SelectedFillPattern.ToString();
                NotifyOfPropertyChange(() => SelectedFillPattern);
                NotifyOfPropertyChange(() => CurrentPatternType);
            }
        }

        public string CurrentPatternDefinition
        {
            get
            {
                return currentPatternDefinition;
            }

            set
            {
                currentPatternDefinition = value;
                NotifyOfPropertyChange(() => CurrentPatternDefinition);
            }
        }

        public FillPatternTarget CurrentPatternType
        {
            get
            {
                return selectedFillPattern.HatchPattern.Target;
            } 
        }
    }
}
