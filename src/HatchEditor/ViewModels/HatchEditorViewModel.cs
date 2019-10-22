// (C) Copyright 2019 by Andrew Nicholas
//
// This file is part of SCaddins.
//
// SCaddins is free software: you can redistribute it and/or modify
// it under the terms of the GNU Lesser General Public License as published by
// the Free Software Foundation, either version 3 of the License, or
// (at your option) any later version.
//
// SCaddins is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU Lesser General Public License for more details.
//
// You should have received a copy of the GNU Lesser General Public License
// along with SCaddins.  If not, see <http://www.gnu.org/licenses/>.

namespace SCaddins.HatchEditor.ViewModels
{
    using System.Collections.ObjectModel;
    using Autodesk.Revit.DB;
    using Caliburn.Micro;

    internal class HatchEditorViewModel : Screen
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Microsoft.Usage", "CA2213: Disposable fields should be disposed", Justification = "Parameter intialized by Revit", MessageId = "doc")]
        private readonly Document doc;
        private Hatch userFillPattern;

        public HatchEditorViewModel(Document doc)
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
            var result = SCaddinsApp.WindowManager.ShowFileSelectionDialog("C:/Temp", out var filePath);
            if (!result.HasValue || !result.Value) return;
            var vm = new SelectHatchViewModel(new ObservableCollection<Hatch>(Command.ReadAllPatternsFromFile(filePath)));
            var result2 = SCaddinsApp.WindowManager.ShowDialog(vm, null, SelectHatchViewModel.DefualtWindowSettings());
            if (result2.HasValue && result2.Value)
            {
                UserFillPattern = vm.SelectedFillPattern.Clone();
            }
        }

        public void LoadPatternFromModel()
        {
            var vm = new SelectHatchViewModel(doc);
            var result = SCaddinsApp.WindowManager.ShowDialog(vm, null, SelectHatchViewModel.DefualtWindowSettings());
            if (result.HasValue && result.Value) {
                UserFillPattern = vm.SelectedFillPattern.Clone();
            }
        }

        public void NewDraftingPattern()
        {
            UserFillPattern = new Hatch(new FillPattern("New Drafting Pattern", FillPatternTarget.Drafting, FillPatternHostOrientation.ToView));
            UserFillPatternDefinition = @"0,0,0,0,5";
        }

        public void NewModelPattern()
        {
            UserFillPattern = new Hatch(new FillPattern("New Drafting Pattern", FillPatternTarget.Model, FillPatternHostOrientation.ToHost));
            UserFillPatternDefinition = @"0,0,0,0,50";
        }

        public void RotatePattern()
        {
            UserFillPattern.Rotate(45);
            NotifyOfPropertyChange(() => UserFillPattern);
            NotifyOfPropertyChange(() => UserFillPatternDefinition);
        }

        public void SaveToFile()
        {
            var vm = new SaveToModelViewModel(UserFillPattern.Name);
            bool? nameResult = SCaddinsApp.WindowManager.ShowDialog(vm, null, SaveToModelViewModel.DefaultWindowSettings);
            if (nameResult.HasValue && nameResult.Value) {
                UserFillPattern.Name = vm.NewPatternName;
                string savePath = string.Empty;
                var result = SCaddinsApp.WindowManager.ShowSaveFileDialog("CustomHatch.pat", "*.pat", "Pattern Files (*.pat)| *.pat", out savePath);
                if (result.HasValue && result == true) {
                    Command.SaveToFile(savePath, UserFillPattern);
                }
            }
        }

        public void SaveToModel()
        {
            var vm = new SaveToModelViewModel(Command.FillPatterns(doc), UserFillPattern.Name);
            bool? result = SCaddinsApp.WindowManager.ShowDialog(vm, null, SaveToModelViewModel.DefaultWindowSettings);
            if (result.HasValue && result.Value)
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
