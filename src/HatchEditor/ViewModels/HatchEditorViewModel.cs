namespace SCaddins.HatchEditor.ViewModels
{
    using System.Collections.ObjectModel;
    using System.Linq;
    using Autodesk.Revit.DB;
    using Caliburn.Micro;

    internal class HatchEditorViewModel : Screen
    {
        private Document doc;
        private ObservableCollection<Hatch> fillPattensInModel;
        private Hatch selectedFillPattern;
        private Hatch customFillPattern;

        public HatchEditorViewModel(Autodesk.Revit.DB.Document doc)
        {
            this.doc = doc;
            FillPatterns = new ObservableCollection<Hatch>(Command.FillPatterns(doc));
            customFillPattern = new Hatch(new FillPattern());
            customFillPattern.Name = @"<Custom>";
            FillPatterns.Add(customFillPattern);
            SelectedFillPattern = FillPatterns.LastOrDefault();
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
                NotifyOfPropertyChange(() => CurrentPatternType);
                NotifyOfPropertyChange(() => CurrentPatternDefinition);
                NotifyOfPropertyChange(() => SelectedFillPattern);
            }
        }

        public string CurrentPatternDefinition
        {
            get
            {
                return SelectedFillPattern.Definition != null ? SelectedFillPattern.Definition : string.Empty;
            }

            set
            {
                //if (value != null)
                //{   
                SelectedFillPattern = null;
                customFillPattern.Definition = value;
                SelectedFillPattern = customFillPattern;
                //selectedFillPattern = customFillPattern;
                //NotifyOfPropertyChange(() => CurrentPatternDefinition);
                //NotifyOfPropertyChange(() => SelectedFillPattern);
                
                //}
            }
        }

        public FillPatternTarget CurrentPatternType
        {
            get
            {
                return SelectedFillPattern.HatchPattern.Target;
            } 
        }
    }
}
