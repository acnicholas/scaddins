namespace SCaddins.HatchEditor.ViewModels
{
    using System.Collections.ObjectModel;
    using System.Linq;
    using Autodesk.Revit.DB;
    using Caliburn.Micro;
    using System.Dynamic;

    internal class SelectHatchViewModel : Screen
    {
        private ObservableCollection<Hatch> fillPattensInModel;
        private Hatch selectedFillPattern;

        public SelectHatchViewModel(Document doc)
        {
            FillPatterns = new ObservableCollection<Hatch>(Command.FillPatterns(doc));
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
                ////SCaddinsApp.WindowManager.ShowMessageBox("test");
                NotifyOfPropertyChange(() => SelectedFillPattern);
                ////NotifyOfPropertyChange(() => CanSelect);
            }
        }

        //public bool CanSelect {
        //    get
        //    {
        //        return SelectedFillPattern != null;
        //    }
        //}

        public void Select()
        {
            ///SCaddinsApp.WindowManager.ShowMessageBox(SelectedFillPattern.Name);
            TryClose(true);
        }

        public void Cancel()
        {
            TryClose(false);
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
    }
}
