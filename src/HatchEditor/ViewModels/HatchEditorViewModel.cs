namespace SCaddins.HatchEditor.ViewModels
{
    using System.Collections.ObjectModel;
    using System.Linq;
    using Autodesk.Revit.DB;
    using Caliburn.Micro;

    internal class HatchEditorViewModel : Screen
    {
        private Hatch customFillPattern;
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Microsoft.Usage", "CA2213: Disposable fields should be disposed", Justification = "Parameter intialized by Revit", MessageId = "doc")]
        private Document doc;
        private ObservableCollection<Hatch> fillPattensInModel;
        private ObservableCollection<Hatch> loadedFillPatterns;
        private Hatch selectedFillPattern;
        private Hatch selectedLoadedFillPattern;

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

        public string CurrentPatternDefinition {
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

        public FillPatternTarget CurrentPatternType {
            get
            {
                return SelectedFillPattern.HatchPattern.Target;
            }
        }

        public ObservableCollection<Hatch> FillPatterns {
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

        public ObservableCollection<Hatch> LoadedFillPatterns
        {
            get
            {
                return loadedFillPatterns;
            }

            set
            {
                loadedFillPatterns = value;
                NotifyOfPropertyChange(() => LoadedFillPatterns);
            }
        }

        public Hatch SelectedFillPattern {
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

        public Hatch SelectedLoadedFillPattern {
            get
            {
                return selectedLoadedFillPattern;
            }

            set
            {
                selectedLoadedFillPattern = value;
                NotifyOfPropertyChange(() => CurrentPatternType);
                NotifyOfPropertyChange(() => CurrentPatternDefinition);
                NotifyOfPropertyChange(() => SelectedLoadedFillPattern);
            }
        }

        public void LoadPatternsFromFile()
        {
            string filePath = string.Empty;
            var result = SCaddinsApp.WindowManager.ShowFileSelectionDialog("C:/Temp", out filePath);
            if (result.HasValue && result.Value == true)
            {
                FillPatterns = new ObservableCollection<Hatch>(Command.ReadAllPatternsFromFile(filePath));
            }
        }

        public void SaveToFile()
        {
            string savePath = string.Empty;
            var result = SCaddinsApp.WindowManager.ShowSaveFileDialog("CustomHatch.pat", "*.pat", "Pattern Files (*.pat)| *.pat", out savePath);
            if (result.HasValue && result == true) {
                Command.SaveToFile(savePath, SelectedFillPattern);
            }
        }

        public void SaveToModel()
        {
            Command.SaveToModel(doc, SelectedFillPattern.HatchPattern);
        }
    }
}
