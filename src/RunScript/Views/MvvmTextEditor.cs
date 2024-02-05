namespace SCaddins.RunScript.Views
{
    using System;
    using System.Drawing;
    using System.Windows;
    using System.Windows.Forms.Integration;
    using FastColoredTextBoxNS;

    public class MvvmTextEditor : WindowsFormsHost
    {
        public static readonly DependencyProperty TextProperty = DependencyProperty.Register(
            "Text",
            typeof(string),
            typeof(MvvmTextEditor),
            new PropertyMetadata(string.Empty, TextPropertyChangedCallback(), null));

        public static readonly DependencyProperty SyntaxColoursProperty = DependencyProperty.Register(
            "SyntaxColours",
            typeof(SyntaxHighlighter),
            typeof(MvvmTextEditor),
            new PropertyMetadata(new FastColoredTextBox().SyntaxHighlighter, SyntaxColoursPropertyChangedCallback(), null));

        public static readonly DependencyProperty TextSizeProperty = DependencyProperty.Register(
            "TextSize",
             typeof(double),
             typeof(MvvmTextEditor),
             new PropertyMetadata((double)12, TextSizePropertyChangedCallback(), null));

        public static readonly DependencyProperty BackgroundColourProperty = DependencyProperty.Register(
            "BackgroundColour",
            typeof(System.Drawing.Color),
            typeof(MvvmTextEditor),
            new PropertyMetadata(System.Drawing.Color.Black, BackgroundColourPropertyChangedCallback(), null));

        public static readonly DependencyProperty ForegroundColourProperty = DependencyProperty.Register(
            "ForegroundColour",
            typeof(System.Drawing.Color),
            typeof(MvvmTextEditor),
            new PropertyMetadata(System.Drawing.Color.White, ForegroundColourPropertyChangedCallback(), null));

        private FastColoredTextBox fastColoredTextBox;

        public MvvmTextEditor()
        {
            string fontName = "Consolas";
            fastColoredTextBox = new FastColoredTextBox
            {
                Font = new System.Drawing.Font(fontName, Convert.ToSingle(TextSize), System.Drawing.FontStyle.Bold),
                ForeColor = System.Drawing.Color.White,
                BackColor = System.Drawing.Color.Black,
                Language = FastColoredTextBoxNS.Language.Lua
            };
            Child = fastColoredTextBox;
            fastColoredTextBox.TextChanged += FastColoredTextBox_TextChanged;
        }

        ~MvvmTextEditor()
        {
            fastColoredTextBox.Dispose();   
        }

        public System.Drawing.Color BackgroundColour
        {
            get
            {
                return (System.Drawing.Color)GetValue(BackgroundColourProperty);
            }

            set
            {
                SetValue(BackgroundColourProperty, value);
            }
        }

        public System.Drawing.Color ForegroundColour
        {
            get
            {
                return (System.Drawing.Color)GetValue(ForegroundColourProperty);
            }

            set
            {
                SetValue(ForegroundColourProperty, value);
                if (value == Color.Black)
                {
                    fastColoredTextBox.SyntaxHighlighter.StringStyle = 
                        new FastColoredTextBoxNS.TextStyle(Brushes.Red, null, System.Drawing.FontStyle.Bold);
                }
                else
                {
                    fastColoredTextBox.SyntaxHighlighter.StringStyle =
                     new FastColoredTextBoxNS.TextStyle(Brushes.Green, null, System.Drawing.FontStyle.Bold);
                }
            }
        }

        public SyntaxHighlighter SyntaxColours
        {
            get
            {
                return (SyntaxHighlighter)GetValue(SyntaxColoursProperty);
            }

            set
            {
                SetValue(SyntaxColoursProperty, value);
            }
        }

        public double TextSize
        {
            get
            {
                return (double)GetValue(TextSizeProperty);
            }

            set
            {
                SetValue(TextSizeProperty, value);
            }
        }

        public string Text
        {
            get
            {
                return (string)GetValue(TextProperty);
            }

            set
            {
                SetValue(TextProperty, value);
            }
        }

        private static PropertyChangedCallback BackgroundColourPropertyChangedCallback()
        {
            return new PropertyChangedCallback(
                        (d, e) =>
                        {
                            var textBoxHost = d as MvvmTextEditor;
                            if (textBoxHost != null && textBoxHost.fastColoredTextBox != null)
                            {
                                textBoxHost.fastColoredTextBox.BackColor = (System.Drawing.Color)textBoxHost.GetValue(e.Property);
                            }
                        });
        }

        private static PropertyChangedCallback ForegroundColourPropertyChangedCallback()
        {
            return new PropertyChangedCallback(
                        (d, e) =>
                        {
                            var textBoxHost = d as MvvmTextEditor;
                            if (textBoxHost != null && textBoxHost.fastColoredTextBox != null)
                            {
                                textBoxHost.fastColoredTextBox.ForeColor = (System.Drawing.Color)textBoxHost.GetValue(e.Property);
                                textBoxHost.fastColoredTextBox.SyntaxHighlighter.StringStyle = textBoxHost.fastColoredTextBox.SyntaxHighlighter.CommentStyle;
                            }
                        });
        }

        private static PropertyChangedCallback TextSizePropertyChangedCallback()
        {
            return new PropertyChangedCallback(
                        (d, e) =>
                        {
                            MvvmTextEditor textBoxHost = d as MvvmTextEditor;
                            if (textBoxHost != null && textBoxHost.fastColoredTextBox != null)
                            {
                                var oldName = textBoxHost.fastColoredTextBox.Font.Name;
                                var size = Convert.ToSingle(textBoxHost.GetValue(e.Property));
                                textBoxHost.fastColoredTextBox.Font = new System.Drawing.Font(oldName, size, System.Drawing.FontStyle.Bold);
                            }
                        });
        }

        private static PropertyChangedCallback TextPropertyChangedCallback()
        {
            return new PropertyChangedCallback(
                    (d, e) =>
                    {
                        MvvmTextEditor textBoxHost = d as MvvmTextEditor;
                        if (textBoxHost != null && textBoxHost.fastColoredTextBox != null)
                        {
                            textBoxHost.fastColoredTextBox.Text = textBoxHost.GetValue(e.Property) as string;
                        }
                    });
        }

        private static PropertyChangedCallback SyntaxColoursPropertyChangedCallback()
        {
            return new PropertyChangedCallback(
            (d, e) =>
            {
                MvvmTextEditor textBoxHost = d as MvvmTextEditor;
                if (textBoxHost != null && textBoxHost.fastColoredTextBox != null)
                {
                    var sh = textBoxHost.GetValue(e.Property) as SyntaxHighlighter;
                    textBoxHost.fastColoredTextBox.SyntaxHighlighter = sh;

                    // SCaddinsApp.WindowManager.ShowMessageBox("yep");
                }
            });
        }

        private void FastColoredTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            SetValue(TextProperty, fastColoredTextBox.Text);
        }
    }
}