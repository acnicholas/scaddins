// (C) Copyright 2019-2025 by Andrew Nicholas
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

using System.Threading.Tasks;

namespace SCaddins.RunScript.ViewModels
{
    using System;
    using System.Diagnostics;
    using System.Dynamic;
    using System.IO;
    using System.Text;
    using System.Web;
    using Autodesk.Revit.DB;
    using Autodesk.Revit.UI;
    using Caliburn.Micro;

    internal class RunScriptSimpleViewModel : Screen
    {
        private string output;
        private string script;
        private BindableCollection<string> outputList;
        private string currentFileName;
        private ExternalCommandData commandData;
        private ElementSet elements;
        private OutputWindowViewModel outputWindowViewModel;

        public RunScriptSimpleViewModel(ExternalCommandData commandData, ElementSet elements)
        {
            this.commandData = commandData;
            this.elements = elements;
            currentFileName = string.Empty;
            Output = " test ";
            outputList = new BindableCollection<string>();
            LoadScratch();
            outputWindowViewModel = new OutputWindowViewModel();
        }

        public static dynamic DefaultViewSettings
        {
            get
            {
                dynamic settings = new ExpandoObject();
                settings.Height = 640;
                settings.Width = 1024;
                settings.Title = "Run Lua Script";
                settings.ShowInTaskbar = false;
                settings.ResizeMode = System.Windows.ResizeMode.CanResizeWithGrip;
                settings.Icon = new System.Windows.Media.Imaging.BitmapImage(
                    new Uri("pack://application:,,,/SCaddins;component/Assets/lua.png"));
                settings.SizeToContent = System.Windows.SizeToContent.WidthAndHeight;
                return settings;
            }
        }


        public bool CanSave
        {
            get
            {
                if (!string.IsNullOrEmpty(CurrentFileName))
                {
                    var f = Directory.GetDirectoryRoot(CurrentFileName);
                    var di = new DirectoryInfo(f);
                    return di.Exists && !di.Attributes.HasFlag(FileAttributes.ReadOnly);
                }
                return false;
            }
        }

        public string CurrentFileName
        {
            get => currentFileName;
            set
            {
                currentFileName = value;
                NotifyOfPropertyChange(() => CanSave);
            }
        }

        public string Output
        {
            get => output;

            set
            {
                if (string.IsNullOrEmpty(value))
                {
                    return;
                }
                output = "Output:" + System.Environment.NewLine + value;
                NotifyOfPropertyChange(() => Output);
            }
        }

        public string Script
        {
            get => script;

            set
            {
                if (string.IsNullOrEmpty(value))
                {
                    return;
                }
                script = value;
                NotifyOfPropertyChange(() => Script);
            }
        }

        public static void NavigateTo(System.Uri url)
        {
            var ps = new ProcessStartInfo(url.ToString())
            {
                UseShellExecute = true,
                Verb = "open"
            };
            Process.Start(ps);
        }

        public void ClearOutputWindow()
        {

            Output = " ";
        }

        public void OpenInExternalEditor()
        {
            SaveScratch();
            // SCaddinsApp.WindowManager.ShowMessageBox(GetScratchPath());
            if (File.Exists(GetScratchPath()))
            {
                Process process = new Process();
                //process.StartInfo.FileName = "notepad++.exe";
                process.StartInfo.FileName = RunScriptSettings.Default.ExternalEditor;
                process.StartInfo.Arguments = GetScratchPath();
                process.StartInfo.UseShellExecute = true;
                process.Start();
                process.WaitForExit(); // This makes it "modal" — waits until the app closes
            }
            LoadScratch();
        }

        public string GetScratchPath()
        {
            var s = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
            var p = Path.Combine(s, "SCaddins", "Script.lua");
            return p;
        }

        public void ShowOutputWindow()
        {
            outputWindowViewModel.Output = this.Output;
            SCaddinsApp.WindowManager.ShowWindowAsync(outputWindowViewModel, null, ViewModels.OutputWindowViewModel.DefaultViewSettings);
        }

        public void LoadSample()
        {
            var f = SCaddinsApp.WindowManager.ShowFileSelectionDialog(
                        @"C:\Program Files\Studio.SC\SCaddins\share\RunScript\HelloWorld.lua",
                        out currentFileName,
                        @"Lua Script (*.lua)|*.lua| All files (*.*)|*.*");
            if (f.HasValue && f.Value)
            {
                if (File.Exists(currentFileName))
                {

                    Script = File.ReadAllText(CurrentFileName);
                    SCaddinsApp.WindowManager.LastWindow.Title = CurrentFileName;
                    NotifyOfPropertyChange(() => CanSave);
                }
            }
        }

        public void LoadScratch()
        {
            if (!File.Exists(GetScratchPath()))
            {
                return;
            }
            Script = File.ReadAllText(GetScratchPath());
        }

        public void LoadScriptFromFile()
        {
            var f = SCaddinsApp.WindowManager.ShowFileSelectionDialog(
                    string.Empty,
                    out currentFileName,
                    @"Lua Script (*.lua)|*.lua| All files (*.*)|*.*");
            if (f.HasValue && f.Value)
            {
                if (File.Exists(currentFileName))
                {
                    Script = File.ReadAllText(CurrentFileName);
                    SCaddinsApp.WindowManager.LastWindow.Title = currentFileName;
                    NotifyOfPropertyChange(() => CanSave);
                }
            }
        }

        public void NewFile()
        {
            Script = @"-- Lua script for RunScript." + System.Environment.NewLine +
                @"-- Runscript is part of SCaddins (https://github.com/acnicholas/scaddins)"  + System.Environment.NewLine +
                System.Environment.NewLine +
                @"-- The following variables are available:" + System.Environment.NewLine +
                @"    -- 'commandData' - Autodesk.Revit.UI.ExternalCommandData as passed to the host addin" + System.Environment.NewLine +
                @"    -- 'fec' - FilteredElementCOllector on active doc" + System.Environment.NewLine +
                @"    -- 'fecv' - FilteredElementCOllector on active view" + System.Environment.NewLine + System.Environment.NewLine +
                "import(\'RevitAPIUI\', \'Autodesk.Revit.UI\')" + System.Environment.NewLine +
                "td = TaskDialog.Show(\"Hello\", \"Hello World\")" + System.Environment.NewLine +
                "return \'Hello World\'" + System.Environment.NewLine;
            CurrentFileName = string.Empty;
        }

        public override async Task TryCloseAsync(bool? dialogResult = false)
        {
            SaveScratch();
            await outputWindowViewModel.TryCloseAsync(dialogResult);
            await base.TryCloseAsync(dialogResult);
        }

        public void Run()
        {
            var r = RunScriptCommand.RunScript(script, commandData, elements, false);

            var sb = new StringBuilder();
            foreach (var v in r)
            {
                sb.Append(v.ToString());
                sb.AppendLine();
            }
            if (sb.Length == 0)
            {
                Output = "No output";
                return;
            }
            Output = sb.ToString();
            SaveScratch();
        }

        public void SaveAs()
        {
            var b = SCaddinsApp.WindowManager.ShowSaveFileDialog(defaultFileName: "script.lua", defaultExtension: "*.lua", filter: "lua-script | *.lua", savePath: out var path);
            if (b.HasValue && b.Value)
            {
                var result = script;
                if (!string.IsNullOrEmpty(result))
                {
                    File.WriteAllText(path: path, contents: result);
                    CurrentFileName = path;
                    SCaddinsApp.WindowManager.LastWindow.Title = CurrentFileName;
                }
            }
        }

        public void Save()
        {
            if (CanSave)
            {
                SCaddinsApp.WindowManager.LastWindow.Title = currentFileName;
                if (!string.IsNullOrEmpty(script))
                {
                    File.WriteAllText(currentFileName, script);
                }
            }
            else
            {
                SaveAs();
            }
        }

        public void SaveScratch()
        {
            if (!Directory.Exists(Path.GetDirectoryName(GetScratchPath())))
            {
                Directory.CreateDirectory(Path.GetDirectoryName(GetScratchPath()));
            }
            File.WriteAllText(GetScratchPath(), script);
        }
    }
}
