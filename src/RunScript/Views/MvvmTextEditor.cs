namespace SCaddins.RunScript.Views
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
        private Project project;
        private CompletionService completionService;
        private Microsoft.CodeAnalysis.Document document;
        private int dotIndex;
        private Microsoft.CodeAnalysis.Completion.CompletionList list;
        private string filterText;
        private bool vis;


        public MvvmTextEditor()
        {
            ShowLineNumbers = true;

            Init();

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

        public void Init()
        {
            list = null;
            vis = false;
            filterText = string.Empty;
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
                   
            project = workspace.AddProject(projectInfo);
            document = workspace.AddDocument(project.Id, "MyFile.cs", SourceText.From(code));
            completionService = CompletionService.GetService(document);
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

        private async Task UpdateCompletionResults(ICSharpCode.AvalonEdit.Editing.TextArea ta)
        {
            document = workspace.AddDocument(project.Id, "MyFile.cs", SourceText.From(Text));

            if (completionService == null) {
                return;
            }

            //if (!vis)
            //{
            list = completionService.GetCompletionsAsync(document, this.CaretOffset, trigger: CompletionTrigger.Invoke).Result;
            //}

            if (list == null) {
                return;
            }

            //if (completionWindow == null) {
                completionWindow = new CompletionWindow(ta);
                completionWindow.Closed += delegate
                {
                    vis = false;
                    completionWindow = null;
                };
            //}

            IList<ICompletionData> data = completionWindow.CompletionList.CompletionData;

            if (data == null) return;

            foreach (var i in list.Items) {
                if (string.IsNullOrEmpty(filterText) || i.DisplayText.StartsWith(filterText)) {
                    data.Add(new MyCompletionData(i.DisplayText));
                }
            }
            completionWindow.CompletionList.SelectedItem = data[0];
            completionWindow.CloseWhenCaretAtBeginning = false;
            completionWindow.CloseAutomatically = false;
            vis = true;
            completionWindow.Show();
        }

        private void TextArea_TextEntered(object sender, System.Windows.Input.TextCompositionEventArgs e)
        {
            completionWindow = null;
            if (e.Text != " ")
            {
                dotIndex = this.CaretOffset;
                filterText = string.Empty;
                this.UpdateCompletionResults(this.TextArea);
                return;
            }
            //if (char.IsLetterOrDigit(e.Text[0]) && vis)
            //{
            //    this.UpdateCompletionResults(this.TextArea);
            //    filterText = this.Text.Substring(dotIndex, this.CaretOffset - dotIndex);
            //    completionWindow.StartOffset = dotIndex;
            //    completionWindow.EndOffset = this.CaretOffset;
            //    return;
            //}
        }

        private void TextArea_TextEntering(object sender, System.Windows.Input.TextCompositionEventArgs e)
        {
            if (e.Text.Length > 0 && completionWindow != null)
            {
                if (!char.IsLetterOrDigit(e.Text[0]))
                {
                    filterText = string.Empty;
                    vis = false;
                    //completionWindow.CompletionList.RequestInsertion(e);
                    return;
               }

            }
        }
    }
}
