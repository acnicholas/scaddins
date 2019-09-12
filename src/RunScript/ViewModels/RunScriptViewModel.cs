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

namespace SCaddins.RunScript.ViewModels
{
    using System;
    using System.Collections.Generic;
    using System.Dynamic;
    using System.IO;
    using System.Xml;
    using Autodesk.Revit.DB;
    using Autodesk.Revit.UI;
    using Caliburn.Micro;
    using ICSharpCode.AvalonEdit.Highlighting;

    internal class RunScriptViewModel : Screen
    {
        private string output;

        public RunScriptViewModel()
        {
            //LoadTheme();
            LightMode();
            //SyntaxHighlightingTheme = "C#";
            output = string.Empty;
            Script =
@"
using Autodesk.Revit.UI;
using Autodesk.Revit.DB;
using SCaddins;

public static void Main(Document doc)
{
    using (var t = new Transaction(doc)) {
        t.Start(""Run Script"");
        var fec = new FilteredElementCollector(doc).OfCategory(BuiltInCategory.OST_Rooms);
            foreach (var r in fec)
            {
                var room = r as Autodesk.Revit.DB.Architecture.Room;
                room.Name = room.Name.ToUpper();
            }
            t.Commit();
        }
}
";
        }

        public static dynamic DefaultViewSettings
        {
            get
            {
                dynamic settings = new ExpandoObject();
                settings.Height = 480;
                settings.Width = 300;
                settings.Title = "Run (cs)Script";
                settings.ShowInTaskbar = false;
                settings.SizeToContent = System.Windows.SizeToContent.WidthAndHeight;
                return settings;
            }
        }

        public string Script
        {
            get; set;
        }

        public string Output
        {
            get
            {
                return output;
            }

            set
            {
                output = value;
                NotifyOfPropertyChange(() => Output);
            }
        }

        public IHighlightingDefinition SyntaxHighlightingTheme
        {
            get; set;
        }

        public System.Windows.Media.Brush Background
        {
            get;set;
        }

        public void DarkMode()
        {
            LoadTheme("CSharp-Mode-Molokai.xshd");
            Background = System.Windows.Media.Brushes.Black;
            NotifyOfPropertyChange(() => SyntaxHighlightingTheme);
            NotifyOfPropertyChange(() => Background);
        }

        public void LightMode()
        {
            LoadTheme("CSharp-Mode.xshd");
            Background = System.Windows.Media.Brushes.White;
            NotifyOfPropertyChange(() => SyntaxHighlightingTheme);
            NotifyOfPropertyChange(() => Background);
        }

        public void LoadTheme(string themeFile)
        {
            string dir = @"C:\Code\cs\scaddins\src\RunScript\Resources\";
            if (File.Exists(dir + themeFile))
            {
                try
                {
                    Stream xshd_stream = File.OpenRead(dir + themeFile);
                    XmlTextReader xshd_reader = new XmlTextReader(xshd_stream);
                    var theme = ICSharpCode.AvalonEdit.Highlighting.Xshd.HighlightingLoader.Load(xshd_reader, ICSharpCode.AvalonEdit.Highlighting.HighlightingManager.Instance);
                    SyntaxHighlightingTheme = theme;
                    xshd_reader.Close();
                    xshd_stream.Close();
                }
                catch (Exception e)
                {
                    SCaddinsApp.WindowManager.ShowMessageBox(e.Message);
                }
            }
        }

        public void Run()
        {
            var compileResults = string.Empty;
            var result = RunScriptCommand.VerifyScript(RunScriptCommand.ClassifyScript(Script), out compileResults);
            Output = compileResults;
            if (result)
            {
                TryClose(true);
            }
        }
    }
}