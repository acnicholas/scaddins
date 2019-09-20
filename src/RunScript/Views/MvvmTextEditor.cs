﻿namespace SCaddins.RunScript.Views
{
    using ICSharpCode.AvalonEdit.CodeCompletion;
    using Microsoft.CodeAnalysis.Completion;
    using Microsoft.CodeAnalysis.CSharp.Syntax;
    using Microsoft.CodeAnalysis.CSharp;
    using Microsoft.CodeAnalysis.Host.Mef;
    using Microsoft.CodeAnalysis.Text;
    using Microsoft.CodeAnalysis;
    using System.ComponentModel;
    using System.Threading.Tasks;
    using System.Windows;
    using System;
    using System.Linq;
    using System.Reflection;
    using System.Collections.Generic;

    public class MvvmTextEditor : ICSharpCode.AvalonEdit.TextEditor, INotifyPropertyChanged
    {
        public static readonly DependencyProperty TextProperty =
            DependencyProperty.Register(
                "Text",
                typeof(string),
                typeof(MvvmTextEditor),
                new FrameworkPropertyMetadata
                {
                    DefaultValue = default(string),
                    BindsTwoWayByDefault = true,
                    PropertyChangedCallback = OnDependencyPropertyChanged
                });

        private CompletionWindow completionWindow;
        private MefHostServices host;
        private AdhocWorkspace workspace;


        public MvvmTextEditor()
        {
            ShowLineNumbers = true;

            this.TextArea.TextEntered += TextArea_TextEntered;
            this.TextArea.TextEntering += TextArea_TextEntering;

            Options = new ICSharpCode.AvalonEdit.TextEditorOptions
            {
                IndentationSize = 4,
                ConvertTabsToSpaces = true,
                ShowTabs = true,
                ShowSpaces = true,
            };
        }

        public event PropertyChangedEventHandler PropertyChanged;
        public new string Text
        {
            get
            {
                return (string)GetValue(TextProperty);
            }

            set
            {
                SetValue(TextProperty, value);
                RaisePropertyChanged("Text");
            }
        }

        #pragma warning disable CA1030 // Use events where appropriate
        public void RaisePropertyChanged(string property)
        #pragma warning restore CA1030 // Use events where appropriate
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(property));
            }
        }

        public async void TestCompletionOutput()
        {
            host = MefHostServices.Create(MefHostServices.DefaultAssemblies);
            workspace = new AdhocWorkspace(host);
            var code = Text;

            var compilationOptions = new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary,
                usings: new[] { "System", "Autodesk.Revit.DB", "Autodesk.Revit.UI" });

            var projectInfo = ProjectInfo.Create(
                ProjectId.CreateNewId(),
                VersionStamp.Create(),
                "MyProject", "MyProject",
                LanguageNames.CSharp)
                    .WithMetadataReferences(new[] 
                        { MetadataReference.CreateFromFile(typeof(object).Assembly.Location),
                          MetadataReference.CreateFromFile(typeof(Autodesk.Revit.DB.Document).Assembly.Location)
                        })
                    .WithCompilationOptions(compilationOptions)
                    .WithAssemblyName("RevitAIP")
                    .WithAssemblyName("RevitAPIUI");
                   
            var project = workspace.AddProject(projectInfo);
            var document = workspace.AddDocument(project.Id, "MyFile.cs", SourceText.From(code));
            int idx = this.CaretOffset;
            await PrintCompletionResults(document, idx, completionWindow, this.TextArea);
        }

        protected static void OnDependencyPropertyChanged(DependencyObject dobj, DependencyPropertyChangedEventArgs args)
        {
            var target = (MvvmTextEditor)dobj;

            if (target.Document != null)
            {
                var caretOffset = target.CaretOffset;
                var newValue = args.NewValue;

                if (newValue == null)
                {
                    newValue = string.Empty;
                }

                target.Document.Text = (string)newValue;
                target.CaretOffset = Math.Min(caretOffset, newValue.ToString().Length);
            }
        }

        protected override void OnTextChanged(EventArgs e)
        {
            if (this.Document != null)
            {
                Text = this.Document.Text;
            }

            base.OnTextChanged(e);
        }

        private static async Task PrintCompletionResults(Document document, int position, CompletionWindow cw, ICSharpCode.AvalonEdit.Editing.TextArea ta)
        {
            var completionService = CompletionService.GetService(document);

            if (completionService == null)
            {
                SCaddinsApp.WindowManager.ShowMessageBox("completionService == null");
                return;
            }

            Microsoft.CodeAnalysis.Completion.CompletionList list = null;
            list = completionService.GetCompletionsAsync(document, position).Result;
            //list = completionService.GetCompletionsAsync(document, position, CompletionTrigger.CreateInsertionTrigger('.')).Result;

            if (list == null)
            {
                SCaddinsApp.WindowManager.ShowMessageBox("NULL list!!!");
                return;
            }

            ////SCaddinsApp.WindowManager.ShowMessageBox(list.Items.Count().ToString());

            cw = new CompletionWindow(ta);
            IList<ICompletionData> data = cw.CompletionList.CompletionData;
            foreach (var i in list.Items)
            {
                var test = i.
                data.Add(new MyCompletionData(i.DisplayText));
            }
            cw.CompletionList.SelectedItem = data[0];
            cw.Show();
            cw.Closed += delegate
            {
                cw = null;
            };
        }

        private async void TextArea_TextEntered(object sender, System.Windows.Input.TextCompositionEventArgs e)
        {
            if (e.Text == "." || completionWindow != null)
            {
                TestCompletionOutput();
            }
            if (e.Text == "(" || completionWindow != null)
            {
                TestCompletionOutput();
            }

        }

        private void TextArea_TextEntering(object sender, System.Windows.Input.TextCompositionEventArgs e)
        {
            if (e.Text.Length > 0 && completionWindow != null)
            {
                if (!char.IsLetterOrDigit(e.Text[0]))
                {
            ////        if (e.Text[0] == '\t')
            ////        {
            ////            // IList<ICompletionData> data = completionWindow.CompletionList.CompletionData;
            ////            //completionWindow.CompletionList.SelectedItem = data[1];
            ////            //} else {
                           completionWindow.CompletionList.RequestInsertion(e);
               }
            }
        }
    }
}