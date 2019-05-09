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
            SelectedFillPattern.HatchPattern.Target = FillPatternTarget.Model;
            SelectedFillPattern.Definition = 
                "90,0,0,0,100,50,25" + System.Environment.NewLine +
                "90,50,0,0,100,50,25" + System.Environment.NewLine +
                "90,75,0,0,100,50,25" + System.Environment.NewLine +
                "0,0,0,0,25,25,75" + System.Environment.NewLine +
                "0,50,25,0,75,25,75";
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
                var type = SelectedFillPattern.HatchPattern.Target;
                SelectedFillPattern = null;
                customFillPattern.Definition = value;
                customFillPattern.HatchPattern.Target = type;
                SelectedFillPattern = customFillPattern;
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
