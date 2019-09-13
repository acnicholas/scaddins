namespace SCaddins.RunScript.Views
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Windows;
    using ICSharpCode.AvalonEdit.CodeCompletion;
    using ICSharpCode.AvalonEdit.Highlighting;

    public class MvvmTextEditor : ICSharpCode.AvalonEdit.TextEditor, INotifyPropertyChanged
    {
        private CompletionWindow completionWindow;

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

        private void TextArea_TextEntering(object sender, System.Windows.Input.TextCompositionEventArgs e)
        {
            if (e.Text.Length > 0 && completionWindow != null)
            {
                if (!char.IsLetterOrDigit(e.Text[0]))
                {
                    if (e.Text[0] == '\t')
                    {
                        IList<ICompletionData> data = completionWindow.CompletionList.CompletionData;
                        completionWindow.CompletionList.SelectedItem = data[1];
                    } else {
                        completionWindow.CompletionList.RequestInsertion(e);
                    }
                }
            }
            // Do not set e.Handled=true.
            // We still want to insert the character that was typed.
        }

        private void TextArea_TextEntered(object sender, System.Windows.Input.TextCompositionEventArgs e)
        {
            if (e.Text == ".")
            {
                // Open code completion after the user has pressed dot:
                completionWindow = new CompletionWindow(this.TextArea);
                IList<ICompletionData> data = completionWindow.CompletionList.CompletionData;
                data.Add(new MyCompletionData("Item1"));
                data.Add(new MyCompletionData("Item2"));
                data.Add(new MyCompletionData("Item3"));
                completionWindow.CompletionList.SelectedItem = data[0];
                completionWindow.Show();
                completionWindow.Closed += delegate {
                    completionWindow = null;
                };
            }
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
