namespace SCaddins.HatchEditor.ViewModels
{
    using System.Collections.ObjectModel;
    using System.Linq;
    using Autodesk.Revit.DB;
    using Caliburn.Micro;

    internal class HatchEditorViewModel : Screen
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Microsoft.Usage", "CA2213: Disposable fields should be disposed", Justification = "Parameter intialized by Revit", MessageId = "doc")]
        private Document doc;
        private ObservableCollection<Hatch> fillPattensInModel;
        private Hatch selectedFillPattern;
        private Hatch userFillPattern;

        public HatchEditorViewModel(Autodesk.Revit.DB.Document doc)
        {
            this.doc = doc;
            FillPatterns = new ObservableCollection<Hatch>(Command.FillPatterns(doc));
            userFillPattern = new Hatch(new FillPattern());
            userFillPattern.Name = @"<Custom>";
            userFillPattern.HatchPattern.Target = FillPatternTarget.Model;
            userFillPattern.Definition =
                "90,0,0,0,100,50,25" + System.Environment.NewLine +
                "90,50,0,0,100,50,25" + System.Environment.NewLine +
                "90,75,0,0,100,50,25" + System.Environment.NewLine +
                "0,0,0,0,25,25,75" + System.Environment.NewLine +
                "0,50,25,0,75,25,75";
            FillPatterns.Add(userFillPattern);
            selectedFillPattern = FillPatterns.LastOrDefault();
        }

        public string UserFillPatternDefinition {
            get
            {
                return UserFillPattern.Definition != null ? UserFillPattern.Definition : string.Empty;
            }

            set
            {
                UserFillPattern.Definition = value;
                NotifyOfPropertyChange(() => UserFillPatternDefinition);
                NotifyOfPropertyChange(() => UserFillPattern);
            }
        }

        public bool DraftingPattern {
            get
            {
                return UserFillPattern.IsDrafting;
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

        public bool ModelPattern {
            get
            {
                return !UserFillPattern.IsDrafting;
            }
        }

        public Hatch UserFillPattern
        {
            get
            {
                return userFillPattern;
            }
            set
            {
                userFillPattern = value;
                NotifyOfPropertyChange(() => UserFillPatternDefinition);
                NotifyOfPropertyChange(() => UserFillPattern);
                NotifyOfPropertyChange(() => ModelPattern);
                NotifyOfPropertyChange(() => DraftingPattern);
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
                UserFillPattern = selectedFillPattern.Clone();
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

        public void LoadPatternFromModel()
        {
            var vm = new ViewModels.SelectHatchViewModel(doc);
            var result = SCaddinsApp.WindowManager.ShowDialog(vm, null, SelectHatchViewModel.DefualtWindowSettings());
            if (result.HasValue && result.Value == true) {
                UserFillPattern = vm.SelectedFillPattern.Clone();
            }
        }

        public void NewDraftingPattern()
        {
            UserFillPattern = new Hatch(new FillPattern("New Drafting Pattern",FillPatternTarget.Drafting,FillPatternHostOrientation.ToView));
            UserFillPatternDefinition = (@"0,0,0,0,5");
        }

        public void NewModelPattern()
        {
            UserFillPattern = new Hatch(new FillPattern("New Drafting Pattern", FillPatternTarget.Model, FillPatternHostOrientation.ToHost));
            UserFillPatternDefinition = (@"0,0,0,0,50");
        }

        public void RotatePattern()
        {
            UserFillPattern.Rotate(45);
            NotifyOfPropertyChange(() => UserFillPattern);
            NotifyOfPropertyChange(() => UserFillPatternDefinition);
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
            dynamic settings = new System.Dynamic.ExpandoObject();
            settings.Height = 160;
            settings.Width = 320;
            settings.Title = "Fill Pattern Name";
            settings.ShowInTaskbar = false;
            settings.SizeToContent = System.Windows.SizeToContent.Manual;
            var vm = new SaveToModelViewModel();
            SCaddinsApp.WindowManager.ShowDialog(vm, null, settings);
            //// Command.SaveToModel(doc, SelectedFillPattern.HatchPattern);
        }

        public void ScalePattern()
        {
            UserFillPattern.Scale(2);
            NotifyOfPropertyChange(() => UserFillPattern);
            NotifyOfPropertyChange(() => UserFillPatternDefinition);
        }
    }
}
