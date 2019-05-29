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
            optionsVm = new ModelSetupWizardOptionsViewModel();
            foreach (var pinf in optionsVm.ProjectInformationReplacements)
            {
                var match = ProjectInformation.Where(p => p.Name == pinf.ParamaterName);
                if (match.Count() == 1)
                {
                    match.First().Format = pinf.ReplacementFormat;
                    match.First().Value = pinf.ReplacementValue;
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

        public BindableCollection<WorksetParameter> Worksets
        {
            get; set;
        }

        public WorksetParameter SelectedWorkset
        {
            get; set;
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
            Worksets.Add(new WorksetParameter(string.Empty, false, false));
        }

        public void RemoveWorkset()
        {
            Worksets.Remove(SelectedWorkset);
        }
    }
}
