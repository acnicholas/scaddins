using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SCaddins.ModelSetupWizard.ViewModels
{
    using System.Collections.Specialized;
    using Autodesk.Revit.DB;
    using Caliburn.Micro;

    class ModelSetupWizardOptionsViewModel
    {
        public ModelSetupWizardOptionsViewModel()
        {
            DefaultWorksets = new BindableCollection<WorksetParameter>();
            AddDefaultWorksets();
            ProjectInformationReplacements = new BindableCollection<ProjectInformationReplacement>();
            AddProjectInformationReplacements();
        }

        private void AddDefaultWorksets()
        {
            var newWorksets = ModelSetupWizardSettings.Default.DefaultWorksets;
            foreach (var newWorksetDef in newWorksets)
            {
                var segs = newWorksetDef.Split(';');
                int b = 0;
                var r = int.TryParse(segs[1].Trim(), out b);
                if (!string.IsNullOrEmpty(segs[0]))
                {
                    if (segs.Length > 2 && !string.IsNullOrEmpty(segs[2]))
                    {
                        WorksetParameter wp = new WorksetParameter(segs[0], b != 0, segs[2]); 
                        DefaultWorksets.Add(wp);
                    } else
                    {
                        WorksetParameter wp = new WorksetParameter(segs[0], b != 0, false);
                        DefaultWorksets.Add(wp);
                    }
                }
            }
        }

        private void AddProjectInformationReplacements()
        {
            var pinf = ModelSetupWizardSettings.Default.DefaultProjectInformation;
            foreach (var p in pinf)
            {
                var segs = p.Split(';');
                if (segs.Length > 1)
                {
                    ProjectInformationReplacement wp = new ProjectInformationReplacement(segs[0], segs[1]);
                    ProjectInformationReplacements.Add(wp);
                    if (segs.Length > 2)
                    {
                        wp.ReplacementFormat = segs[2];
                    }
                }
            }
        }

        public BindableCollection<WorksetParameter> DefaultWorksets
        {
            get; set;
        }

        public WorksetParameter SelectedDefaultWorkset
        {
            get; set;
        }

        public void AddWorksets()
        {
            DefaultWorksets.Add(new WorksetParameter());
        }

        public void RemoveWorksets()
        {
            DefaultWorksets.Remove(SelectedDefaultWorkset);
        }

        public BindableCollection<ProjectInformationReplacement> ProjectInformationReplacements
        {
            get; set;
        }

        public ProjectInformationReplacement SelectedProjectInformationReplacement
        {
            get; set;
        }

        public void AddReplacement()
        {
            ProjectInformationReplacements.Add(new ProjectInformationReplacement());
        }

        public void RemoveReplacement()
        {
            ProjectInformationReplacements.Remove(SelectedProjectInformationReplacement);
        }

    }
}
