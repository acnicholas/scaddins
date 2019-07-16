namespace SCaddins.HatchEditor.ViewModels
{
    using System.Collections.ObjectModel;
    using System.Linq;
    using Autodesk.Revit.DB;
    using Caliburn.Micro;

    internal class HatchEditorViewModel : Screen
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Microsoft.Usage", "CA2213: Disposable fields should be disposed", Justification = "Parameter intialized by Revit", MessageId = "doc")]
        private readonly Document doc;
        private Hatch userFillPattern;

        public HatchEditorViewModel(Autodesk.Revit.DB.Document doc)
        {
            this.doc = doc;
            userFillPattern = new Hatch(new FillPattern());
            userFillPattern.HatchPattern.Target = FillPatternTarget.Drafting;
            userFillPattern.Definition =
                "0,0,0,0,6" + System.Environment.NewLine +
                "0,2,2,0,6,4,-2" + System.Environment.NewLine +
                "90,0,0,0,6" + System.Environment.NewLine +
                "90,2,2,0,6,4,-2" + System.Environment.NewLine +
                "45,0,0,0,8.485281374,2.8284271247, -5.65685424" + System.Environment.NewLine +
                "45,0,6,0,8.485281374,2.8284271247, -5.65685424";
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

        public void LoadPatternFromFile()
        {
            string filePath = string.Empty;
            var result = SCaddinsApp.WindowManager.ShowFileSelectionDialog("C:/Temp", out filePath);
            if (result.HasValue && result.Value == true)
            {
                var vm = new ViewModels.SelectHatchViewModel(new ObservableCollection<Hatch>(Command.ReadAllPatternsFromFile(filePath)));
                var result2 = SCaddinsApp.WindowManager.ShowDialog(vm, null, SelectHatchViewModel.DefualtWindowSettings());
                if (result2.HasValue && result2.Value == true)
                {
                    UserFillPattern = vm.SelectedFillPattern.Clone();
                }
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
                Command.SaveToFile(savePath, UserFillPattern);
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
            var vm = new SaveToModelViewModel(Command.FillPatterns(doc), UserFillPattern.Name);
            bool? result = SCaddinsApp.WindowManager.ShowDialog(vm, null, settings);
            if (result.HasValue && result.Value == true)
            {
                UserFillPattern.Name = vm.NewPatternName;
                Command.SaveToModel(doc, UserFillPattern.HatchPattern);
            } else
            {
                SCaddinsApp.WindowManager.ShowWarningMessageBox("Save to Model", "Fill pattern not saved to the current model...");
            }
            
        }

        public void ScalePattern()
        {
            UserFillPattern.Scale(2);
            NotifyOfPropertyChange(() => UserFillPattern);
            NotifyOfPropertyChange(() => UserFillPatternDefinition);
        }
    }
}
