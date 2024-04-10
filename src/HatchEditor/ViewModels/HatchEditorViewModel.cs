// (C) Copyright 2019-2020 by Andrew Nicholas
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
    using System;
    using System.Collections.ObjectModel;
    using System.IO;
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

        public string UserFillPatternDefinition
        {
            get
            {
                return UserFillPattern.Definition ?? string.Empty;
            }

            set
            {
                UserFillPattern.Definition = value;
                NotifyOfPropertyChange(() => UserFillPatternDefinition);
                NotifyOfPropertyChange(() => UserFillPattern);
            }
        }

        public bool DraftingPattern
        {
            get
            {
                return UserFillPattern.IsDrafting;
            }
        }

        public bool ModelPattern
        {
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

        public void Help()
        {
            string helpText = System.Environment.NewLine +
            "Line families" + System.Environment.NewLine +
            System.Environment.NewLine +
            "Each line family is a set of parallel lines that build one 'direction' of a pattern. A complete " +
            "pattern may be defined by several families each defining part of the final, such as the vertical " +
            "points of brick coursing. The lines are described in the two dimensions of a face as follows: " +
            System.Environment.NewLine +
            System.Environment.NewLine +
            " angle, x - origin, y - origin, shift, offset, dash, space, dash, space... " +
            System.Environment.NewLine +
            System.Environment.NewLine +
            " Angle specifies the direction, in degrees, of a set of lines relative to the x - axis. " +
            System.Environment.NewLine +
            System.Environment.NewLine +
            " The first line in the set starts at the specified x,y - origin. " +
            System.Environment.NewLine +
            System.Environment.NewLine +
            "Parallel lines are drawn as specified by " +
            "Offset and Shift to fill the entire face. " +
            System.Environment.NewLine +
            System.Environment.NewLine +
            "Offset defines the distance between parallel lines. " + System.Environment.NewLine +
            "Zero Offset is not allowed. " +
            System.Environment.NewLine +
            System.Environment.NewLine +
            "Shift moves the pattern of dashes and spaces along the length of " +
            "each new parallel line.Shift is ignored if the line is solid.Distances are expressed as " +
            "decimal numbers(using peroid as the decimal separator) in the units previously specified. " +
            "The dash-space sequence defines a repeating pattern of dashes and spaces for a line. If it is " +
            "omitted, the line is solid.Positive numbers define dashes, negative numbers define spaces, and " +
            "zero specifies a dot. If you begin a pattern with a space, do not alternate dashes and spaces, " +
            "or do not end with a space, Revit will introduce tiny dashes or spaces to compensate. Revit " +
            "expands dots and very short dashes into dashes of a minimum size.";

            SCaddinsApp.WindowManager.ShowMessageBox(helpText);
        }

        public void LoadPatternFromTemplate()
        {
            var result = SCaddinsApp.WindowManager.ShowFileSelectionDialog(
                SCaddins.Constants.InstallDirectory + SCaddins.Constants.ShareDirectory + Path.DirectorySeparatorChar + "HatchEditor" + Path.DirectorySeparatorChar + "StretcherBond.txt",
                out var filePath);
            if (!result.HasValue || !result.Value)
            {
                return;
            }
            var vm = new SetTemplateParametersViewModel(HatchTemplate.GetHatchParameters(filePath));
            var result2 = SCaddinsApp.WindowManager.ShowDialogAsync(vm, null, SetTemplateParametersViewModel.DefaultWindowSettings);
            if (result2.Result.HasValue && result2.Result.Value)
            {
                var hatch = new Hatch();
                hatch.Definition = HatchTemplate.GetPatternString(filePath, vm.TemplateParameters);
                hatch.HatchPattern.Target = FillPatternTarget.Model;
                hatch.Name = System.IO.Path.GetFileNameWithoutExtension(filePath);
                UserFillPattern = hatch;
            } else
            {
                SCaddinsApp.WindowManager.ShowMessageBox("No Value");
            }
        }

        public void LoadPatternFromFile()
        {
            var result = SCaddinsApp.WindowManager.ShowFileSelectionDialog(
                Environment.GetFolderPath(Environment.SpecialFolder.UserProfile),
                out var filePath);
            if (!result.HasValue || !result.Value)
            {
                return;
            }
            var vm = new SelectHatchViewModel(new ObservableCollection<Hatch>(Command.ReadAllPatternsFromFile(filePath)));
            var result2 = SCaddinsApp.WindowManager.ShowDialogAsync(vm, null, SelectHatchViewModel.DefualtWindowSettings());
            if (result2.Result.HasValue && result2.Result.Value)
            {
                UserFillPattern = vm.SelectedFillPattern.Clone();
            }
        }

        public void LoadPatternFromModel()
        {
            var vm = new SelectHatchViewModel(doc);
            var result = SCaddinsApp.WindowManager.ShowDialogAsync(vm, null, SelectHatchViewModel.DefualtWindowSettings());
            if (result.Result.HasValue && result.Result.Value)
            {
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
            var nameResult = SCaddinsApp.WindowManager.ShowDialogAsync(vm, null, SaveToModelViewModel.DefaultWindowSettings);
            if (!nameResult.Result.HasValue || !nameResult.Result.Value)
            {
                return;
            }
            UserFillPattern.Name = vm.NewPatternName;
            var result = SCaddinsApp.WindowManager.ShowSaveFileDialog("CustomHatch.pat", "*.pat", "Pattern Files (*.pat)| *.pat", out var savePath);
            if (result.HasValue && result == true)
            {
                Command.SaveToFile(savePath, UserFillPattern);
            }
        }

        public void SaveToModel()
        {
            var vm = new SaveToModelViewModel(Command.FillPatterns(doc), UserFillPattern.Name);
            var result = SCaddinsApp.WindowManager.ShowDialogAsync(vm, null, SaveToModelViewModel.DefaultWindowSettings);
            if (result.Result.HasValue && result.Result.Value)
            {
                UserFillPattern.Name = vm.NewPatternName;
                Command.SaveToModel(doc, UserFillPattern.HatchPattern);
            }
            else
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
