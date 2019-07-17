namespace SCaddins.HatchEditor.ViewModels
{
    using System.Collections.ObjectModel;
    using System.Dynamic;
    using Autodesk.Revit.DB;
    using Caliburn.Micro;

    internal class SelectHatchViewModel : Screen
    {
        private ObservableCollection<Hatch> fillPattensInModel;
        private Hatch selectedFillPattern;

        public SelectHatchViewModel(Document doc)
        {
            FillPatterns = new ObservableCollection<Hatch>(Command.FillPatterns(doc));
        }

        public SelectHatchViewModel(ObservableCollection<Hatch> fillPatterns)
        {
            FillPatterns = fillPatterns;
        }

        public ObservableCollection<Hatch> FillPatterns {
            get
            {
                return fillPattensInModel;
            }

            set
            {
                fillPattensInModel = value;
                NotifyOfPropertyChange(() => FillPatterns);
            }
        }

        public Hatch SelectedFillPattern {
            get
            {
                return selectedFillPattern;
            }

            set
            {
                selectedFillPattern = value;
                NotifyOfPropertyChange(() => SelectedFillPattern);
            }
        }

        public static ExpandoObject DefualtWindowSettings()
        {
            dynamic settings = new ExpandoObject();
            settings.Height = 480;
            settings.Width = 768;
            settings.Title = "Hatch Editor - By Andrew Nicholas";
            settings.ShowInTaskbar = false;
            settings.SizeToContent = System.Windows.SizeToContent.Manual;
            return settings;
        }

        public void Cancel()
        {
            TryClose(false);
        }

        public void Select()
        {
            TryClose(true);
        }
    }
}
