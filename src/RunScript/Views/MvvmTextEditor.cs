namespace SCaddins.RunScript.Views
{
    using System;
    using System.ComponentModel;
    using System.Windows;

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

        public MvvmTextEditor()
        {
            ShowLineNumbers = true;

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
