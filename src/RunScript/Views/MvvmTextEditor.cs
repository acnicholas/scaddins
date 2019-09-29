namespace SCaddins.RunScript.Views
{
    using ICSharpCode.AvalonEdit.CodeCompletion;
    //using Microsoft.CodeAnalysis.Completion;
    //using Microsoft.CodeAnalysis.CSharp;
    //using Microsoft.CodeAnalysis.Host.Mef;
    //using Microsoft.CodeAnalysis.Text;
    //using Microsoft.CodeAnalysis;
    using System.ComponentModel;
    ////using System.Threading.Tasks;
    using System.Windows;
    using System;
    using ICSharpCode.AvalonEdit.Rendering;
    using System.Windows.Media;
    using ICSharpCode.AvalonEdit.Document;

    ////using System.Collections.Generic;

    public class MvvmTextEditor : ICSharpCode.AvalonEdit.TextEditor, INotifyPropertyChanged, ICSharpCode.AvalonEdit.Rendering.IBackgroundRenderer
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

        public static readonly DependencyProperty CaretColumnProperty =
            DependencyProperty.Register(
                "CaretColumn",
                typeof(int),
                typeof(MvvmTextEditor),
                new FrameworkPropertyMetadata
                {
                    DefaultValue = default(int),
                    BindsTwoWayByDefault = true,
                    PropertyChangedCallback = OnDependencyPropertyChanged
        });

        ////private CompletionWindow completionWindow;
        ////private MefHostServices host;
        ////private AdhocWorkspace workspace;
        ////private Project project;
        ////private CompletionService completionService;
        ////private Microsoft.CodeAnalysis.Document document;
        ////private int dotIndex;
        ////private Microsoft.CodeAnalysis.Completion.CompletionList list;
        ////private string filterText;
        ////private bool vis;


        public MvvmTextEditor()
        {
            ShowLineNumbers = true;
            //SetValue(TextProperty, "Loading...");

            //Init();

            //this.TextArea.Caret.Column = 3;
            //this.TextArea.Caret.Line = 3;

            
            this.TextArea.Caret.Show();
            this.TextArea.TextView.BackgroundRenderers.Add(this);

            //this.TextArea.TextEntered += TextArea_TextEntered;
            //this.TextArea.TextEntering += TextArea_TextEntering;

            Options = new ICSharpCode.AvalonEdit.TextEditorOptions {
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

        public int CaretColumn
        {
            get
            {
                return (int)GetValue(CaretColumnProperty);
            }

            set
            {
                SetValue(CaretColumnProperty, value);
                //this.TextArea.Caret.Location = new ICSharpCode.AvalonEdit.Document.TextLocation(0, value);
                this.TextArea.Caret.Position = new ICSharpCode.AvalonEdit.TextViewPosition(1, value);
                RaisePropertyChanged("CaretColumn");
            }
        }

        public KnownLayer Layer {
            get
            { 
                return KnownLayer.Caret;
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

        //public void Init()
        //{
        //    list = null;
        //    vis = false;
        //    filterText = string.Empty;
        //    host = MefHostServices.Create(MefHostServices.DefaultAssemblies);
        //    workspace = new AdhocWorkspace(host);
        //    var code = Text;

        //    var compilationOptions = new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary,
        //        usings: new[] { "System", "Autodesk.Revit.DB", "Autodesk.Revit.UI" });

        //    var projectInfo = ProjectInfo.Create(
        //        ProjectId.CreateNewId(),
        //        VersionStamp.Create(),
        //        "MyProject", "MyProject",
        //        LanguageNames.CSharp)
        //            .WithMetadataReferences(new[] 
        //                { MetadataReference.CreateFromFile(typeof(object).Assembly.Location),
        //                  MetadataReference.CreateFromFile(typeof(Autodesk.Revit.DB.Document).Assembly.Location)
        //                })
        //            .WithCompilationOptions(compilationOptions)
        //            .WithAssemblyName("RevitAIP")
        //            .WithAssemblyName("RevitAPIUI");
                   
        //    project = workspace.AddProject(projectInfo);
        //    document = workspace.AddDocument(project.Id, "MyFile.cs", SourceText.From(code));
        //    completionService = CompletionService.GetService(document);
        //}

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

        public void Draw(TextView textView, DrawingContext drawingContext)
        {
            textView.EnsureVisualLines();
            var line = this.Document.GetLineByOffset(this.CaretOffset);
            var segment = new TextSegment { StartOffset = line.Offset, EndOffset = line.EndOffset };
            foreach (Rect r in BackgroundGeometryBuilder.GetRectsForSegment(textView, segment))
            {
                drawingContext.DrawRoundedRectangle(Brushes.AliceBlue, new Pen(Brushes.AliceBlue,1), new Rect(r.Location, new Size(textView.ActualWidth, r.Height)), 3, 3);
            }
        }

        //private async Task UpdateCompletionResults()
        //{
        //    document = workspace.AddDocument(project.Id, "MyFile.cs", SourceText.From(Text));

        //    if (completionService == null) {
        //        return;
        //    }

        //    list = completionService.GetCompletionsAsync(document, this.CaretOffset, trigger: CompletionTrigger.Invoke).Result;

        //    if (list == null) {
        //        return;
        //    }

        //    IList<ICompletionData> data = completionWindow.CompletionList.CompletionData;

        //    if (data == null) return;

        //    foreach (var i in list.Items) {
        //        if (string.IsNullOrEmpty(filterText) || i.DisplayText.StartsWith(filterText)) {
        //            data.Add(new MyCompletionData(i.DisplayText));
        //        }
        //    }
        //}

        //private void ShowCompletionWindow()
        //{
        //    if (completionWindow != null) {
        //        if (completionWindow.CompletionList.CompletionData.Count > 0)
        //        {
        //            completionWindow.CompletionList.SelectedItem = completionWindow.CompletionList.CompletionData[0];
        //        }
        //        completionWindow.Show();
        //    }
        //}

        //private void CreateCompletionWindow()
        //{
        //    completionWindow = null;
        //    completionWindow = new CompletionWindow(this.TextArea);
        //    completionWindow.CloseWhenCaretAtBeginning = false;
        //    completionWindow.CloseAutomatically = false;
        //    completionWindow.Closed += delegate
        //    {
        //        vis = false;
        //        completionWindow = null;
        //    };
        //}

        //private async void TextArea_TextEntered(object sender, System.Windows.Input.TextCompositionEventArgs e)
        //{


        //    //FIXME don't create every time....
        //    if (e.Text == ".") {
        //        CreateCompletionWindow();
        //        dotIndex = this.CaretOffset;
        //        vis = true;
        //        filterText = string.Empty;
        //        //CreateCompletionWindow();
        //        UpdateCompletionResults();
        //        ShowCompletionWindow();
        //        return;
        //    }

        //    if (vis && char.IsLetterOrDigit(e.Text[0])) {
        //        CreateCompletionWindow();
        //        //SCaddinsApp.WindowManager.ShowMessageBox("Yep");
        //        completionWindow.StartOffset = this.CaretOffset - dotIndex;
        //        completionWindow.EndOffset = this.CaretOffset;
        //        filterText = this.Text.Substring(dotIndex, this.CaretOffset - dotIndex);
        //        SCaddinsApp.WindowManager.ShowMessageBox(filterText);
        //        UpdateCompletionResults();
        //        ShowCompletionWindow();
        //        return;
        //    }
        //}

        //private void TextArea_TextEntering(object sender, System.Windows.Input.TextCompositionEventArgs e)
        //{
        //    if (e.Text.Length > 0) {
        //        if (!char.IsLetterOrDigit(e.Text[0])) {
        //            filterText = string.Empty;
        //            vis = false;
        //            if(completionWindow != null) completionWindow.CompletionList.RequestInsertion(e);
        //            return;
        //       }
        //    }
        //}
    }
}
