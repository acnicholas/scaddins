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

namespace SCaddins.ModelSetupWizard.ViewModels
{
    using System.Collections.Generic;
    using System.Collections.Specialized;
    using System.Linq;
    using Caliburn.Micro;

    internal class ModelSetupWizardOptionsViewModel : Screen
    {
        private string systemConfigFilePath;

        public ModelSetupWizardOptionsViewModel()
        {
            Worksets = new BindableCollection<WorksetParameter>();
            SelectedWorksets = new List<WorksetParameter>();
            ProjectInformationReplacements = new BindableCollection<ProjectInformationReplacement>();
            NominatedArchitects = new BindableCollection<NominatedArchitect>();

            ColourSchemes = new BindableCollection<ColourScheme>();

            var loadSystemConfig = ModelSetupWizardSettings.Default.LoadSystemConfigOnStartup;
            if (loadSystemConfig)
            {
                systemConfigFilePath = ModelSetupWizardSettings.Default.SystemConfigFilePath;
                if (!string.IsNullOrEmpty(systemConfigFilePath))
                {
                    if (System.IO.File.Exists(systemConfigFilePath))
                    {
                        SettingsIO.Import(systemConfigFilePath);
                        ModelSetupWizardSettings.Default.SystemConfigFilePath = systemConfigFilePath;
                        ModelSetupWizardSettings.Default.LoadSystemConfigOnStartup = true;
                        ModelSetupWizardSettings.Default.Save();
                    }
                    else
                    {
                        SCaddinsApp.WindowManager.ShowWarningMessageBox(
                            "File Not Found",
                            string.Format(System.Globalization.CultureInfo.InvariantCulture, "Config file: {0} not found", systemConfigFilePath));
                    }
                }
            }

            Init();
        }

        public string FileNameParameterName
        {
            get; set;
        }

        public bool LoadSystemConfigOnInit
        {
            get; set;
        }

        public string NominatedArchitectNumberParameterName
        {
            get; set;
        }

        public string NominatedArchitectParameterName
        {
            get; set;
        }

        public BindableCollection<ColourScheme> ColourSchemes
        {
            get; private set;
        }

        public BindableCollection<NominatedArchitect> NominatedArchitects
        {
            get; private set;
        }

        public BindableCollection<ProjectInformationReplacement> ProjectInformationReplacements
        {
            get; private set;
        }

        public BindableCollection<WorksetParameter> Worksets
        {
            get; private set;
        }

        public WorksetParameter SelectedWorkset
        {
            get; set;
        }

        public List<WorksetParameter> SelectedWorksets
        {
            get; private set;
        }

        public NominatedArchitect SelectedNominatedArchitect
        {
            get; set;
        }

        public string SystemConfigFilePath
        {
            get
            {
                return systemConfigFilePath;
            }

            set
            {
                systemConfigFilePath = value;
                NotifyOfPropertyChange(() => SystemConfigFilePath);
            }
        }

        public ProjectInformationReplacement SelectedProjectInformationReplacement
        {
            get; set;
        }

        public void AddArchitect()
        {
            NominatedArchitects.Add(new NominatedArchitect(string.Empty, string.Empty));
        }

        public void AddReplacement()
        {
            ProjectInformationReplacements.Add(new ProjectInformationReplacement());
        }

        public void AddWorkset()
        {
            Worksets.Add(new WorksetParameter());
        }

        public void Apply()
        {
            if (LoadSystemConfigOnInit)
            {
                SCaddinsApp.WindowManager.ShowWarningMessageBox(
                    "System Config Warning",
                    "Load System Configuration at Start-up(Advanced), is set to true, any changes here will be lost unless you set this option to false");
            }

            var sc = new StringCollection();
            foreach (var s in ProjectInformationReplacements)
            {
                sc.Add(s.ToString());
            }
            ModelSetupWizardSettings.Default.DefaultProjectInformation = sc;

            var wsc = new StringCollection();
            foreach (var w in Worksets)
            {
                wsc.Add(w.ToString());
            }

            ModelSetupWizardSettings.Default.DefaultWorksets = wsc;

            var colors = new StringCollection();
            foreach (var color in ColourSchemes)
            {
                colors.Add(color.Config);
            }

            ModelSetupWizardSettings.Default.ColourSchemes = colors;

            var arch = new StringCollection();
            foreach (var a in NominatedArchitects)
            {
                arch.Add(a.ToString());
            }

            ModelSetupWizardSettings.Default.DefaultArchitectInformation = arch;
            ModelSetupWizardSettings.Default.NomArchitectParamName = NominatedArchitectParameterName;
            ModelSetupWizardSettings.Default.NomArchitectNoumberParamName = NominatedArchitectNumberParameterName;
            ModelSetupWizardSettings.Default.FileNameParameterName = FileNameParameterName;
            ModelSetupWizardSettings.Default.SystemConfigFilePath = SystemConfigFilePath;
            ModelSetupWizardSettings.Default.LoadSystemConfigOnStartup = LoadSystemConfigOnInit;
            ModelSetupWizardSettings.Default.Save();
        }

        public void ExportConfig()
        {
            Apply();
            SCaddinsApp.WindowManager.ShowSaveFileDialog(@"config.xml", "*.xml", "XML |*.xml", out var filePath);
            SettingsIO.Export(filePath);
        }

        public void ImportConfig()
        {
            var result = SCaddinsApp.WindowManager.ShowOpenFileDialog(string.Empty, out var filePath);
            if (!result.HasValue || !result.Value || !System.IO.File.Exists(filePath))
            {
                return;
            }
            SettingsIO.Import(filePath);
            Reset();
        }

        public void RemoveArchitect()
        {
            NominatedArchitects.Remove(SelectedNominatedArchitect);
        }

        public void RemoveReplacement()
        {
            ProjectInformationReplacements.Remove(SelectedProjectInformationReplacement);
        }

        public void RemoveWorksets()
        {
            if (SelectedWorksets != null && SelectedWorksets.Count > 0)
            {
                Worksets.RemoveRange(SelectedWorksets);
            }
        }

        public void Reset()
        {
            Worksets.Clear();
            ProjectInformationReplacements.Clear();
            NominatedArchitects.Clear();
            Init();
        }

        public void SelectSytemConfigFile()
        {
            string filePath = string.Empty;
            bool? result = SCaddinsApp.WindowManager.ShowOpenFileDialog(string.Empty, out filePath);
            if (result.HasValue && result.Value && System.IO.File.Exists(filePath))
            {
                SettingsIO.Import(filePath);
                Reset();
            }
            SystemConfigFilePath = filePath;
        }

        public void OptionsWorksetsSelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs args)
        {
            var addedItems = args.AddedItems.OfType<WorksetParameter>();
            SelectedWorksets.AddRange(addedItems);
            var removedItems = args.RemovedItems.OfType<WorksetParameter>();
            removedItems.ToList().ForEach(w => SelectedWorksets.Remove(w));
        }

        private void AddDefaultWorksets()
        {
            var newWorksets = ModelSetupWizardSettings.Default.DefaultWorksets;
            foreach (var newWorksetDef in newWorksets)
            {
                var segs = newWorksetDef.Split(';');
                var b = false;
                bool.TryParse(segs[1].Trim(), out b);
                if (!string.IsNullOrEmpty(segs[0]))
                {
                    if (segs.Length > 2 && !string.IsNullOrEmpty(segs[2]))
                    {
                        WorksetParameter wp = new WorksetParameter(segs[0], b, segs[2]);
                        Worksets.Add(wp);
                    }
                    else
                    {
                        WorksetParameter wp = new WorksetParameter(segs[0], b, -1);
                        Worksets.Add(wp);
                    }
                }
            }
        }

        private void AddNominatedArchitects()
        {
            var architects = ModelSetupWizardSettings.Default.DefaultArchitectInformation;
            foreach (var architect in architects)
            {
                var segs = architect.Split(';');
                if (segs.Length == 2 && !string.IsNullOrEmpty(segs[0]) && !string.IsNullOrEmpty(segs[1]))
                {
                    NominatedArchitects.Add(new NominatedArchitect(segs[0].Trim(), segs[1].Trim()));
                }
            }
        }

        private void AddColourSchemes()
        {
            var colourSchemesSettings = ModelSetupWizardSettings.Default.ColourSchemes;
            foreach (var colourSchemeConfigString in colourSchemesSettings)
            {
                if (string.IsNullOrEmpty(colourSchemeConfigString))
                {
                    continue;
                }
                ColourSchemes.Add(new ColourScheme(colourSchemeConfigString));
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

        private void Init()
        {
            AddDefaultWorksets();
            AddProjectInformationReplacements();
            AddNominatedArchitects();
            AddColourSchemes();
            NominatedArchitectParameterName = ModelSetupWizardSettings.Default.NomArchitectParamName;
            NominatedArchitectNumberParameterName = ModelSetupWizardSettings.Default.NomArchitectNoumberParamName;
            FileNameParameterName = ModelSetupWizardSettings.Default.FileNameParameterName;
            SystemConfigFilePath = ModelSetupWizardSettings.Default.SystemConfigFilePath;
            LoadSystemConfigOnInit = ModelSetupWizardSettings.Default.LoadSystemConfigOnStartup;
        }
    }
}
