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
    using System.Dynamic;
    using System.IO;
    using System.Text;
    using System.Web;
    using Autodesk.Revit.DB;
    using Autodesk.Revit.UI;
    using Caliburn.Micro;
    using CefSharp;
    using CefSharp.Wpf;
    using Microsoft.IdentityModel.Tokens;

    internal class RunScriptViewModel : Screen
    {
        private string output;
        private BindableCollection<string> outputList;
        private string currentFileName;
        private ExternalCommandData commandData;
        private ElementSet elements;
        private static ChromiumWebBrowser browser;
        private OutputWindowViewModel outputWindowViewModel;

        public RunScriptViewModel(ExternalCommandData commandData, ElementSet elements)
        {
            this.commandData = commandData;
            this.elements = elements;
            currentFileName = string.Empty;
            Output = " ";
            outputList = new BindableCollection<string>();
            LoadScratch();
            FontSize = 15;
            browser = null;
            outputWindowViewModel = new OutputWindowViewModel();
            NotifyOfPropertyChange(() => FontSize);
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
                settings.ResizeMode = System.Windows.ResizeMode.NoResize;
                settings.Icon = new System.Windows.Media.Imaging.BitmapImage(
                    new Uri("pack://application:,,,/SCaddins;component/Assets/lua.png"));
                settings.SizeToContent = System.Windows.SizeToContent.WidthAndHeight;
                return settings;
            }
        }


        public static void SetBrowser(ChromiumWebBrowser cbrowser)
        {
            browser = cbrowser;
        }

        public bool CanSave
        {
            get
            {
                if (!string.IsNullOrEmpty(CurrentFileName)) {
                    var f = Directory.GetDirectoryRoot(CurrentFileName);
                    var di = new DirectoryInfo(f);
                    return di.Exists && !di.Attributes.HasFlag(FileAttributes.ReadOnly);
                }
                return false;
            }
        }

        public double FontSize
        {
            get; set;
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
            if (browser != null)
            {
                var fullText = @"blockComment();";
                _ = browser.EvaluateScriptAsync(fullText);

            }
        }

        public void DarkMode()
        {
            if (browser != null)
            {
                var fullText = @"darkMode();";
                _ = browser.EvaluateScriptAsync(fullText);
                
            }
        }

        public void LightMode()
        {
            if (browser != null)
            {
                var fullText = @"lightMode();";
                _ = browser.EvaluateScriptAsync(fullText);

            }
        }


        public void IncreaseFontSize()
        {
            if (browser != null)
            {
                var fullText = @"increaseFontSize();";
                _ = browser.EvaluateScriptAsync(fullText);

            }
        }

        public void DecreaseFontSize()
        {
            if (browser != null)
            {
                var fullText = @"decreaseFontSize();";
                _ = browser.EvaluateScriptAsync(fullText);
            }
        }

        public async Task<string> GetScript()
        {
            if (browser != null)
            {
                var script = @"editor.getValue();";
                var jsScript = HttpUtility.JavaScriptStringEncode(script);
                var result = await browser.EvaluateScriptAsync(jsScript);
                if (result.Success)
                {
                    return (string)result.Result;
                } else
                {
                    return string.Empty;
                }
            } else
            {
                return string.Empty;
            }
        }

        public async void SetScript(string script)
        {
            if (browser != null)
            {
                var jsText = HttpUtility.JavaScriptStringEncode(script);
                var fullText = @"editor.setValue('" + jsText + @"');";
                _ = await browser.EvaluateScriptAsync(fullText);
            }
        }

        public void ClearOutputWindow()
        {

            Output = " ";
        }

        //public void ShowOutputWindow()
        //{
        //    outputWindowViewModel.Output = this.Output;
        //    SCaddinsApp.WindowManager.ShowWindowAsync(outputWindowViewModel, null, ViewModels.OutputWindowViewModel.DefaultViewSettings);
        //}

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

                    SetScript(File.ReadAllText(CurrentFileName));
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
            SetScript(File.ReadAllText(p));
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
                    SetScript(File.ReadAllText(CurrentFileName));
                    SCaddinsApp.WindowManager.LastWindow.Title = currentFileName;
                    NotifyOfPropertyChange(() => CanSave);
                }
            }
        }

        public void NewFile()
        {
            // TODO ask to save current script
            var script = @"-- 'commandData' - Autodesk.Revit.UI.ExternalCommandData as passed to the host addin" + System.Environment.NewLine +
                @"-- 'fec' - FilteredElementCOllector on active doc" + System.Environment.NewLine +
                @"-- 'fecv' - FilteredElementCOllector on active view" + System.Environment.NewLine;
            if (browser != null)
            {
                var jsText = HttpUtility.JavaScriptStringEncode(script);
                var fullText = @"editor.setValue('" + jsText + @"');";
                _ = browser.EvaluateScriptAsync(fullText);
                CurrentFileName = string.Empty;
            }
        }

        public override async Task TryCloseAsync(bool? dialogResult = false)
        {
            SaveScratch();
            await outputWindowViewModel.TryCloseAsync(dialogResult);
            await base.TryCloseAsync(dialogResult);
        }

        public async void Run()
        {
            var script = await GetScript();
            var r = RunScriptCommand.RunScript(script, commandData, elements, false);

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

        public async void SaveAs()
        {
            var b = SCaddinsApp.WindowManager.ShowSaveFileDialog(defaultFileName: "script.lua", defaultExtension: "*.lua", filter: "lua-script | *.lua", savePath: out var path);
            if (b.HasValue && b.Value)
            {
                var result = await GetScript();
                if (!string.IsNullOrEmpty(result))
                { 
                    File.WriteAllText(path: path, contents: result);
                    CurrentFileName = path;
                    SCaddinsApp.WindowManager.LastWindow.Title = CurrentFileName;
                }
            }
        }

        public async void Save()
        {
            if (CanSave)
            {
                SCaddinsApp.WindowManager.LastWindow.Title = currentFileName;
                var result = await GetScript();
                if (!string.IsNullOrEmpty(result))
                {
                    File.WriteAllText(currentFileName, result);
                }
            }
            else
            {
                SaveAs();
            }
        }

        public async void SaveScratch()
        {
            var s = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
            var p = Path.Combine(s, "SCaddins");
            if (!Directory.Exists(p))
            {
                Directory.CreateDirectory(p);
            }
            var result = await GetScript();
            File.WriteAllText(Path.Combine(p, "Script.lua"), result);
        }
    }
}
