namespace SCaddins.SolarUtilities.ViewModels
{
    using System.Collections.Generic;
    using System.Dynamic;
    using Autodesk.Revit.DB;
    using Autodesk.Revit.UI;
    using Caliburn.Micro;

    public class DirectSunViewModel : Screen
    {
        private CloseMode selectedCloseMode;

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Microsoft.Usage", "CA2213: Disposable fields should be disposed", Justification = "Parameter intialized by Revit", MessageId = "uidoc")]
        private UIDocument uidoc;

        public DirectSunViewModel(UIDocument uidoc)
        {
            this.uidoc = uidoc;
            selectedCloseMode = CloseMode.Close;
            MassSelection = null;
            FaceSelection = null;
            AnalysisGridSize = 3;
        }

        public enum CloseMode
        {
            Close,
            MassSelection,
            FaceSelection,
            Analize,
            Clear
        }

        public static dynamic DefaultViewSettings
        {
            get
            {
                dynamic settings = new ExpandoObject();
                settings.Height = 480;
                settings.Width = 300;
                settings.Title = "Direct Sun - By Andrew Nicholas";
                settings.ShowInTaskbar = false;
                settings.SizeToContent = System.Windows.SizeToContent.WidthAndHeight;
                return settings;
            }
        }

        public int AnalysisGridSize
        {
            get; set;  
        }

        public IList<Reference> FaceSelection
        {
            get; set;
        }

        public IList<Reference> MassSelection
        {
            get; set;
        }

        public CloseMode SelectedCloseMode
        {
            get { return selectedCloseMode; }
        }

        public string SelectedMassInformation
        {
            get
            {
                int m = MassSelection != null ? MassSelection.Count : 0;
                return string.Format("Selected Masses: {0}", m);
            }
        }

        public string SelectedFaceInformation
        {
            get
            {
                int f = FaceSelection != null ? FaceSelection.Count : 0;
                return string.Format("Selected Faces: {0}", f);
            }
        }

        public void Clear()
        {
            selectedCloseMode = CloseMode.Clear;
            TryClose(true);
        }

        public void RunAnalysis()
        {
            selectedCloseMode = CloseMode.Analize;
            TryClose(true);
        }

        public void SelectAnalysisFaces()
        {
            selectedCloseMode = CloseMode.FaceSelection;
            TryClose(false);
        }

        public void SelectMasses()
        {
            selectedCloseMode = CloseMode.MassSelection;
            TryClose(false);
        }

        protected override void OnDeactivate(bool close)
        {
            switch (selectedCloseMode) {
                case CloseMode.Close:
                base.OnDeactivate(close);
                break;
                case CloseMode.FaceSelection:
                FaceSelection = uidoc.Selection.PickObjects(Autodesk.Revit.UI.Selection.ObjectType.Face, "Select Faces");
                Respawn(this);
                break;
                case CloseMode.MassSelection:
                MassSelection = uidoc.Selection.PickObjects(Autodesk.Revit.UI.Selection.ObjectType.Element, "Select Masses");
                Respawn(this);
                break;
                case CloseMode.Clear:
                base.OnDeactivate(close);
                break;
                default:
                base.OnDeactivate(close);
                break;
            }
        }

        private static void Respawn(DirectSunViewModel viewModel)
        {
            SCaddinsApp.WindowManager.ShowDialog(viewModel, null, DefaultViewSettings);
        }
    }
}
