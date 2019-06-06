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
    using System.Collections.Generic;
    using System.Dynamic;
    using System.Linq;
    using Autodesk.Revit.DB;
    using Caliburn.Micro;

    public class ModelSetupWizardViewModel : PropertyChangedBase
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Microsoft.Usage", "CA2213: Disposable fields should be disposed", Justification = "Parameter initialized by Revit", MessageId = "doc")]
        private Document doc;
        private BindableCollection<ProjectInformationParameter> projectInformation;
        private ModelSetupWizardOptionsViewModel optionsVm;
        private NominatedArchitect selectedNominatedArchitect;

        public ModelSetupWizardViewModel(Document doc)
        {
            this.doc = doc;
            ProjectInformation = new BindableCollection<ProjectInformationParameter>(ElementCollectors.GetProjectInformationParameters(doc));
            Worksets = new BindableCollection<WorksetParameter>(ElementCollectors.GetWorksetParameters(doc));
            SelectedWorksets = new List<WorksetParameter>();
            SelectedProjectInformations = new List<ProjectInformationParameter>();
            optionsVm = new ModelSetupWizardOptionsViewModel();
            NominatedArchitects = new BindableCollection<NominatedArchitect>(optionsVm.NominatedArchitects);
            NominatedArchitects.Insert(0, new NominatedArchitect("Architects Name", "0000"));
            selectedNominatedArchitect = NominatedArchitects[0];
            FileName = doc.PathName;

            var fileNameParam = ProjectInformation.Where(p => p.Name == ModelSetupWizardSettings.Default.FileNameParameterName);
            if (fileNameParam.Count() == 1)
            {
                if (string.IsNullOrEmpty(doc.PathName))
                {
                    SCaddinsApp.WindowManager.ShowMessageBox("Document not saved... filename cannot be assigned.");
                }
                var path = System.IO.Path.GetFileName(doc.PathName);
                fileNameParam.First().Value = path;
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

            private set
            {
                projectInformation = value;
                NotifyOfPropertyChange(() => ProjectInformation);
            }
        }

        public BindableCollection<NominatedArchitect> NominatedArchitects {
            get; private set;
        }

        public NominatedArchitect SelectedNominatedArchitect {
            get
            {
                return selectedNominatedArchitect;
            }

            set
            {
                selectedNominatedArchitect = value;
                var name = ProjectInformation.Where(p => p.Name == ModelSetupWizardSettings.Default.NomArchitectParamName).ToList();
                var id = ProjectInformation.Where(p => p.Name == ModelSetupWizardSettings.Default.NomArchitectNoumberParamName).ToList();
                if (name.Count == 1 && id.Count == 1) {
                    name[0].Value = selectedNominatedArchitect.Name;
                    id[0].Value = selectedNominatedArchitect.Id;
                    NotifyOfPropertyChange(() => ProjectInformation);
                }
            }
        }

        public ProjectInformationParameter SelectedProjectInformation
        {
            get; set;
        }

        public List<ProjectInformationParameter> SelectedProjectInformations {
            get; private set;
        }

        public string FileName
        {
            get; set;
        }

        public BindableCollection<WorksetParameter> Worksets
        {
            get; private set;
        }

        public WorksetParameter SelectedWorkset {
            get; set;
        }

        public List<WorksetParameter> SelectedWorksets
        {
            get; private set;
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
            SCaddinsApp.WindowManager.ShowDialog(optionsVm, null, settings);
        }

        public void AddWorkset()
        {
            Worksets.Add(new WorksetParameter(string.Empty, false, -1));
        }

        public void WorksetsSelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs args)
        {
            var addedItems = args.AddedItems.OfType<WorksetParameter>();
            SelectedWorksets.AddRange(addedItems);
            var removedItems = args.RemovedItems.OfType<WorksetParameter>();
            removedItems.ToList().ForEach(w => SelectedWorksets.Remove(w));
        }

        public void ProjectInfoSelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs args)
        {
            var addedItems = args.AddedItems.OfType<ProjectInformationParameter>();
            SelectedProjectInformations.AddRange(addedItems);
            var removedItems = args.RemovedItems.OfType<ProjectInformationParameter>();
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
            ModelSetupWizardUtilities.ApplyWorksetModifications(doc, Worksets.ToList());
            ModelSetupWizardUtilities.ApplyProjectInfoModifications(doc, ProjectInformation.ToList());
        }
    }
}
