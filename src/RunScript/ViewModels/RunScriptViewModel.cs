// (C) Copyright 2019-2024 by Andrew Nicholas
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
    using System.Drawing;
    using System.Dynamic;
    using System.IO;
    using System.Reflection;
    using System.Text;
    using Autodesk.Revit.DB;
    using Autodesk.Revit.UI;
    using Caliburn.Micro;
    using ICSharpCode.AvalonEdit.Editing;

    internal class RunScriptViewModel : Screen
    {
        private string output;
        private string script;
        private TextArea selectedText;
        private BindableCollection<string> outputList;
        private string currentFileName;
        private ExternalCommandData commandData;
        private ElementSet elements;

        public RunScriptViewModel(ExternalCommandData commandData, ElementSet elements)
        {
            this.commandData = commandData;
            this.elements = elements;
            currentFileName = string.Empty;
            output = string.Empty;
            outputList = new BindableCollection<string>();
            //selectedText = new ICSharpCode.AvalonEdit.Editing.TextArea();
            LoadScratch();
            FontSize = 13;
            NotifyOfPropertyChange(() => FontSize);
            // NotifyOfPropertyChange(() => FontSizeOutput);
        }

        public static dynamic DefaultViewSettings
        {
            get
            {
                dynamic settings = new ExpandoObject();
                settings.Height = 640;
                settings.Width = 640;
                settings.MaxHeight = 800;
                settings.Title = "Run Lua Script";
                settings.ShowInTaskbar = false;
                settings.Icon = new System.Windows.Media.Imaging.BitmapImage(
                    new Uri("pack://application:,,,/SCaddins;component/Assets/lua.png"));
                settings.SizeToContent = System.Windows.SizeToContent.WidthAndHeight; 
                return settings;
            }
        }

        public bool CanSave => !string.IsNullOrEmpty(currentFileName);

        public double FontSize
        {
            get; set;
        }

        public string Script
        {
            get => script;

            set
            {
                script = value;
                NotifyOfPropertyChange(() => Script);
            }
        }

        public TextArea SelectedText
        {
            get => selectedText;

            set
            {
                selectedText = value;
                NotifyOfPropertyChange(() => SelectedText);
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
                output = value;
                NotifyOfPropertyChange(() => Output);
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

        public void CommentSelection()
        {
            SCaddinsApp.WindowManager.ShowMessageBox(SelectedText.Selection.Segments.GetEnumerator().Current.ToString());
        }

        public void DarkMode()
        {
        }

        public void LightMode()
        {
        }

        public void IncreaseFontSize()
        {
            FontSize++;
            NotifyOfPropertyChange(() => FontSize);
        }

        public void DecreaseFontSize()
        {
            FontSize--;
            NotifyOfPropertyChange(() => FontSize);
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
            var s = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
            var p = Path.Combine(s, "SCaddins", "Script.lua");
            if (!File.Exists(p))
            {
                return;
            }
            Script = File.ReadAllText(p);
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
            // TODO
        }

        // ReSharper disable once OptionalParameterHierarchyMismatch
        public override async Task TryCloseAsync(bool? dialogResult = false)
        {
            SaveScratch();
            await base.TryCloseAsync(dialogResult);
        }

        public void Run()
        {
            var r = RunScriptCommand.RunScript(Script, commandData, elements, false);

            var sb = new StringBuilder();
            foreach (var v in r)
            {
                sb.Append(v.ToString());
                sb.AppendLine();
            }
            if (sb.Length == 0) {
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
                File.WriteAllText(path: path, contents: Script);
                CurrentFileName = path;
                SCaddinsApp.WindowManager.LastWindow.Title = CurrentFileName;
            }
        }

        public void Save()
        {
            if (CanSave)
            {
                SCaddinsApp.WindowManager.LastWindow.Title = currentFileName;
                File.WriteAllText(currentFileName, Script);
            }
            else
            {
                SaveAs();
            }
        }

        public void SaveScratch()
        {
            var s = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
            var p = Path.Combine(s, "SCaddins");
            if (!Directory.Exists(p))
            {
                Directory.CreateDirectory(p);
            }
            File.WriteAllText(Path.Combine(p, "Script.lua"), Script);
        }
    }
}
