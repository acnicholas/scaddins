namespace SCaddins.RunScript.Views
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Windows;
    using ICSharpCode.AvalonEdit.CodeCompletion;
    using ICSharpCode.AvalonEdit.Highlighting;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CSharp;
    using Microsoft.CodeAnalysis.Host.Mef;
    using Microsoft.CodeAnalysis.Text;
    using Microsoft.CodeAnalysis.Completion;
    using System.Threading.Tasks;

    public class MvvmTextEditor : ICSharpCode.AvalonEdit.TextEditor, INotifyPropertyChanged
    {
        private CompletionWindow completionWindow;
        private MefHostServices host;
        private AdhocWorkspace workspace;

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

        public event PropertyChangedEventHandler PropertyChanged;

        public MvvmTextEditor()
        {
            ShowLineNumbers = true;

            this.TextArea.TextEntered += TextArea_TextEntered;
            this.TextArea.TextEntering += TextArea_TextEntering;

            Init();
          
            Options = new ICSharpCode.AvalonEdit.TextEditorOptions
            {
                IndentationSize = 4,
                ConvertTabsToSpaces = true,
                ShowTabs = true,
                ShowSpaces = true,
            };
        }

        private async Task Init()
        {
             host = MefHostServices.Create(MefHostServices.DefaultAssemblies);
             workspace = new AdhocWorkspace(host);

        }

        private void TextArea_TextEntering(object sender, System.Windows.Input.TextCompositionEventArgs e)
        {
            if (e.Text.Length > 0 && completionWindow != null)
            {
                if (!char.IsLetterOrDigit(e.Text[0]))
                {
                    if (e.Text[0] == '\t')
                    {
                        // IList<ICompletionData> data = completionWindow.CompletionList.CompletionData;
                        //completionWindow.CompletionList.SelectedItem = data[1];
                        //} else {
                        completionWindow.CompletionList.RequestInsertion(e);
                    }
                }
            }
        }

        //private static async Task PrintCompletionResults(Document document, int position)
        //{
        //    var completionService = CompletionService.GetService(document);
        //    var results = await completionService.GetCompletionsAsync(document, position);

        //    foreach (var i in results.Items)
        //    {
        //        Console.WriteLine(i.DisplayText);

        //        foreach (var prop in i.Properties)
        //        {
        //            Console.Write($"{prop.Key}:{prop.Value}  ");
        //        }

        //        Console.WriteLine();
        //        foreach (var tag in i.Tags)
        //        {
        //            Console.Write($"{tag}  ");
        //        }

        //        Console.WriteLine();
        //        Console.WriteLine();
        //    }
        //}

        private void TextArea_TextEntered(object sender, System.Windows.Input.TextCompositionEventArgs e)
    {
        if (e.Text == ".")
        {
                TestCompletionOutput();
                completionWindow = new CompletionWindow(this.TextArea);
                IList<ICompletionData> data = completionWindow.CompletionList.CompletionData;
                data.Add(new MyCompletionData("Item1"));
                data.Add(new MyCompletionData("Item2"));
                data.Add(new MyCompletionData("Item3"));
                completionWindow.CompletionList.SelectedItem = data[0];
                completionWindow.Show();
                completionWindow.Closed += delegate
                {
                    completionWindow = null;
                };
            }
    }

        public async void TestCompletionOutput()
        {
            var scriptCode = Text;

            var compilationOptions = new CSharpCompilationOptions(
               OutputKind.DynamicallyLinkedLibrary, usings: new[] { "System" });

            var scriptProjectInfo = ProjectInfo.Create(ProjectId.CreateNewId(), VersionStamp.Create(), "Script", "Script", LanguageNames.CSharp, isSubmission: true)
               .WithMetadataReferences(new[]
               {
                    MetadataReference.CreateFromFile(typeof(object).Assembly.Location)
               })
               .WithCompilationOptions(compilationOptions);

            var scriptProject = workspace.AddProject(scriptProjectInfo);
            var scriptDocumentInfo = DocumentInfo.Create(
                DocumentId.CreateNewId(scriptProject.Id), "Script",
                sourceCodeKind: SourceCodeKind.Script,
                loader: TextLoader.From(TextAndVersion.Create(SourceText.From(scriptCode), VersionStamp.Create())));
            var scriptDocument = workspace.AddDocument(scriptDocumentInfo);

            //// cursor position is at the end
            var position = scriptCode.Length - 1;

            var completionService = CompletionService.GetService(scriptDocument);


            //var results = await completionService.GetCompletionsAsync(scriptDocument, position);
        }



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

        protected static void OnDependencyPropertyChanged(DependencyObject dobj, DependencyPropertyChangedEventArgs args)
        {
            var target = (MvvmTextEditor)dobj;

            if (target.Document != null) {
                var caretOffset = target.CaretOffset;
                var newValue = args.NewValue;

                if (newValue == null) {
                    newValue = string.Empty;
                }

                target.Document.Text = (string)newValue;
                target.CaretOffset = Math.Min(caretOffset, newValue.ToString().Length);
            }
        }

        protected override void OnTextChanged(EventArgs e)
        {
            if (this.Document != null) {
                Text = this.Document.Text;
            }

            base.OnTextChanged(e);
        }
    }
}
