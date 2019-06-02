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


namespace SCaddins.ModelSetupWizard.ViewModels
{
    using System.Dynamic;
    using System.Collections.Generic;
    using Autodesk.Revit.DB;
    using Caliburn.Micro;
    using System.Linq;

    class ModelSetupWizardViewModel : PropertyChangedBase
    {
        private Document doc;
        private BindableCollection<ProjectInformationParameter> projectInformation;
        private ModelSetupWizardOptionsViewModel optionsVm;

        public ModelSetupWizardViewModel(Document doc)
        {
            this.doc = doc;
            ProjectInformation = new BindableCollection<ProjectInformationParameter>(ElementCollectors.GetProjectInformationParameters(doc));
            Worksets = new BindableCollection<WorksetParameter>(ElementCollectors.GetWorksetParameters(doc));
            SelectedWorksets = new List<WorksetParameter>();
            SelectedProjectInformations = new List<ProjectInformationParameter>();
            optionsVm = new ModelSetupWizardOptionsViewModel();
            FileName = doc.PathName;

            var fileNameParam = ProjectInformation.Where(p => p.Name == ModelSetupWizardSettings.Default.FileNameParameterName);
            if (fileNameParam.Count() == 1)
            {
                fileNameParam.First().Value = doc.PathName;
            }

            foreach (var pinf in optionsVm.ProjectInformationReplacements)
            {
                var match = ProjectInformation.Where(p => p.Name == pinf.ParamaterName);
                if (match.Count() == 1)
                {
                    match.First().Format = pinf.ReplacementFormat;
                    match.First().Value = pinf.ReplacementValue;
                }
            }
            foreach (var winf in optionsVm.DefaultWorksets)
            {
                if (Worksets.Select(w => w.Name.Trim()).Contains(winf.Name.Trim())) {
                    continue;
                }
                if (!Worksets.Select(w => w.Name).Contains(winf.ExistingName.Trim())) {
                    Worksets.Add(winf);
                }

                //Worksets.Add(winf);
                var match = Worksets.Where(w => w.Name.Trim() == winf.ExistingName.Trim());
                if (match.Count() > 0) {
                    match.First().Name = winf.Name;
                }
            }
        }

        public BindableCollection<ProjectInformationParameter> ProjectInformation {
            get
            {
                return projectInformation;
            }
            set
            {
                projectInformation = value;
                NotifyOfPropertyChange(() => ProjectInformation);
            }
        }

        public ProjectInformationParameter SelectedProjectInformation
        {
            get; set;
        }

        public List<ProjectInformationParameter> SelectedProjectInformations {
            get; set;
        }

        public string FileName
        {
            get; set;
        }

        public BindableCollection<WorksetParameter> Worksets
        {
            get; set;
        }

        public WorksetParameter SelectedWorkset {
            get; set;
        }

        public List<WorksetParameter> SelectedWorksets
        {
            get; set;
        }

        public void ResetSelectedProjectInfo()
        {
            foreach (var pinf in SelectedProjectInformations) {
                pinf.Value = pinf.OriginalValue;
            }       
        }

        public void Options()
        {
            dynamic settings = new ExpandoObject();
            settings.Height = 480;
            settings.Width = 360;
            settings.Title = "Model Setup Wizard Options";
            settings.ShowInTaskbar = false;
            settings.SizeToContent = System.Windows.SizeToContent.Width;
            //var vm = new ViewModels.ModelSetupWizardOptionsViewModel();
            SCaddinsApp.WindowManager.ShowDialog(optionsVm, null, settings);
        }

        public void AddWorkset()
        {
            Worksets.Add(new WorksetParameter(string.Empty, false, -1));
        }

        public void WorksetsSelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs obj)
        {
            var addedItems = obj.AddedItems.OfType<WorksetParameter>();
            SelectedWorksets.AddRange(addedItems);
            var removedItems = obj.RemovedItems.OfType<WorksetParameter>();
            removedItems.ToList().ForEach(w => SelectedWorksets.Remove(w));
        }

        public void ProjectInfoSelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs obj)
        {
            var addedItems = obj.AddedItems.OfType<ProjectInformationParameter>();
            SelectedProjectInformations.AddRange(addedItems);
            var removedItems = obj.RemovedItems.OfType<ProjectInformationParameter>();
            removedItems.ToList().ForEach(p => SelectedProjectInformations.Remove(p));
        }

        public void RemoveWorksets()
        {
            if (SelectedWorksets != null && SelectedWorksets.Count > 0) {
                Worksets.RemoveRange(SelectedWorksets);
            }
        }

        public void Apply()
        {
            ModelSetupWizard.ApplyWorksetModifications(doc, Worksets.ToList());
            ModelSetupWizard.ApplyProjectInfoModifications(doc, ProjectInformation.ToList());
        }
    }
}
