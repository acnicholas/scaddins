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
    //using Autodesk.Revit.DB;
    //using Autodesk.Revit.UI;

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

            ////host = MefHostServices.Create(MefHostServices.DefaultAssemblies.Concat(new[] {
            ////    Assembly.Load("RevitAPI"),
            ////    Assembly.Load("RevitAPIUI"),
            ////}));

            workspace = new AdhocWorkspace(host);

            var scriptCode = Text;

            var compilationOptions = new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary,
                usings: new[] { "System", "Autodesk.Revit.DB", "Autodesk.Revit.UI" });

            var scriptProjectInfo = ProjectInfo.Create(ProjectId.CreateNewId(), VersionStamp.Create(), "Script", "Script", LanguageNames.CSharp,
                    isSubmission: true)
                .WithMetadataReferences(new[] { MetadataReference.CreateFromFile(typeof(object).Assembly.Location) })
                .WithCompilationOptions(compilationOptions);

            var scriptProject = workspace.AddProject(scriptProjectInfo);
            var scriptDocumentInfo = DocumentInfo.Create(
                DocumentId.CreateNewId(scriptProject.Id), "Script",
                sourceCodeKind: SourceCodeKind.Script,
                loader: TextLoader.From(TextAndVersion.Create(SourceText.From(scriptCode), VersionStamp.Create())));
            ////SCaddinsApp.WindowManager.ShowMessageBox(scriptDocumentInfo.SourceCodeKind.ToString());
            var scriptDocument = workspace.AddDocument(scriptDocumentInfo);
            var completionService = CompletionService.GetService(scriptDocument);
            if (completionService == null)
            {
                SCaddinsApp.WindowManager.ShowMessageBox("completionService == null");
                return;
            }

            //var results = await completionService.GetCompletionsAsync(scriptDocument, this.CaretOffset, trigger: CompletionTrigger.Default, roles: null, options: null, cancellationToken: System.Threading.CancellationToken.None);

            //SCaddinsApp.WindowManager.ShowMessageBox(this.Text);
            //int idx = this.Text.IndexOf("System.") + 7;
            //SCaddinsApp.WindowManager.ShowMessageBox(idx.ToString());

            int idx = this.CaretOffset;
            SCaddinsApp.WindowManager.ShowMessageBox(idx.ToString());

            //return;

            await PrintCompletionResults(scriptDocument, idx);

            //completionWindow = new CompletionWindow(this.TextArea);
            //IList<ICompletionData> data = results.Items
            //data.Add(new MyCompletionData("Item1"));
            //data.Add(new MyCompletionData("Item2"));
            //data.Add(new MyCompletionData("Item3"));
            //completionWindow.CompletionList.SelectedItem = data[0];
            //completionWindow.Show();
            //completionWindow.Closed += delegate
            //{
            //    completionWindow = null;
            //};

            //await PrintCompletionResults(scriptDocument, 10);
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


        ////private static async Task<Microsoft.CodeAnalysis.Completion.CompletionList> GetCompletionResults(Document document, int position)
        ////{
        ////    var completionService = CompletionService.GetService(document);
        ////    Microsoft.CodeAnalysis.Completion.CompletionList list = null;
        ////    return await completionService.GetCompletionsAsync(document, position, CompletionTrigger.CreateInsertionTrigger('.'));
        ////}

        private static async Task PrintCompletionResults(Document document, int position)
        {
            var completionService = CompletionService.GetService(document);

            Microsoft.CodeAnalysis.Completion.CompletionList list = null;
            //list = completionService.GetCompletionsAsync(document, position).Result;
            list = completionService.GetCompletionsAsync(document, position, CompletionTrigger.CreateInsertionTrigger('.')).Result;

            if (list == null)
            {
                SCaddinsApp.WindowManager.ShowMessageBox("NULL list!!!");
                return;
            }

            foreach (var i in list.Items)
            {
                System.Diagnostics.Debug.WriteLine(i.DisplayText);

                foreach (var prop in i.Properties)
                {
                    System.Diagnostics.Debug.Write($"{prop.Key}:{prop.Value}  ");
                }

                System.Diagnostics.Debug.WriteLine("");
                foreach (var tag in i.Tags)
                {
                    System.Diagnostics.Debug.Write($"{tag}  ");
                }

                System.Diagnostics.Debug.WriteLine("");
                System.Diagnostics.Debug.WriteLine("");
            }
        }

        private async void TextArea_TextEntered(object sender, System.Windows.Input.TextCompositionEventArgs e)
        {
            if (e.Text == ".")
            {
                TestCompletionOutput();
                //        completionWindow = new CompletionWindow(this.TextArea);
                //        IList<ICompletionData> data = completionWindow.CompletionList.CompletionData;
                //        data.Add(new MyCompletionData("Item1"));
                //        data.Add(new MyCompletionData("Item2"));
                //        data.Add(new MyCompletionData("Item3"));
                //        completionWindow.CompletionList.SelectedItem = data[0];
                //        completionWindow.Show();
                //        completionWindow.Closed += delegate
                //        {
                //            completionWindow = null;
                //        };
            }
        }

        private void TextArea_TextEntering(object sender, System.Windows.Input.TextCompositionEventArgs e)
        {
            ////if (e.Text.Length > 0 && completionWindow != null)
            ////{
            ////    if (!char.IsLetterOrDigit(e.Text[0]))
            ////    {
            ////        if (e.Text[0] == '\t')
            ////        {
            ////            // IList<ICompletionData> data = completionWindow.CompletionList.CompletionData;
            ////            //completionWindow.CompletionList.SelectedItem = data[1];
            ////            //} else {
            ////            //completionWindow.CompletionList.RequestInsertion(e);
            ////        }
            ////    }
            ////}
        }
    }
}
