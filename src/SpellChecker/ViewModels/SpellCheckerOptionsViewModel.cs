namespace SCaddins.SpellChecker.ViewModels
{
    using System;
    using System.Collections.Specialized;
    using System.Dynamic;
    using Caliburn.Micro;

    public class SpellCheckerOptionsViewModel : Screen
    {
        private StringCollection stringCollection;

        public SpellCheckerOptionsViewModel()
        {
            stringCollection = SpellCheckerSettings.Default.ElementIgnoreList;
            foreach (var item in stringCollection)
            {
                ElementsToIgnore += item;
                ElementsToIgnore += Environment.NewLine;
            }     
        }

        public static dynamic DefaultWindowSettings {
            get
            {
                dynamic settings = new ExpandoObject();
                settings.Height = 640;
                settings.Width = 480;
                //// settings.Icon = new System.Windows.Media.Imaging.BitmapImage(
                ////     new System.Uri("pack://application:,,,/SCaddins;component/Assets/rename.png"));
                settings.Title = "Spelling Options";
                settings.ShowInTaskbar = false;
                settings.SizeToContent = System.Windows.SizeToContent.Manual;
                settings.ResizeMode = System.Windows.ResizeMode.CanResizeWithGrip;
                return settings;
            }
        }

        public string ElementsToIgnore { get; set; }

        public void Apply()
        {
            var collecton = new StringCollection();
            string[] lines = ElementsToIgnore.Split(new[] { Environment.NewLine }, StringSplitOptions.None);
            foreach (var line in lines) {
                if (!string.IsNullOrEmpty(line)) {
                    collecton.Add(line);
                }
            }

            SpellCheckerSettings.Default.ElementIgnoreList = collecton;
            SpellCheckerSettings.Default.Save();

            TryClose(true);
        }

        public void Cancel()
        {
            TryClose(false);
        }
    }
}
