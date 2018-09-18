namespace SCaddins.SolarAnalysis.ViewModels
{
    using System.Collections.Generic;
    using System.Dynamic;
    using Autodesk.Revit.DB;
    using Autodesk.Revit.Exceptions;
    using Autodesk.Revit.UI;
    using Caliburn.Micro;

    public class DirectSunViewModel : Screen
    {
        private CloseMode selectedCloseMode;
        ////private Rectangle windowBounds;

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Microsoft.Usage", "CA2213: Disposable fields should be disposed", Justification = "Parameter intialized by Revit", MessageId = "uidoc")]
        private UIDocument uidoc;

        public DirectSunViewModel(UIDocument uidoc)
        {
            this.uidoc = uidoc;
            selectedCloseMode = CloseMode.Close;
            MassSelection = null;
            FaceSelection = null;
            AnalysisGridSize = 3;
            Size = SCaddinsApp.WindowManager.Size;
            Left = SCaddinsApp.WindowManager.Left;
            Top = SCaddinsApp.WindowManager.Top;
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

        public double AnalysisGridSize
        {
            get; set;  
        }

        public IList<Reference> FaceSelection
        {
            get; set;
        }

        public double Left
        {
            get; set;
        }

        public IList<Reference> MassSelection
        {
            get; set;
        }

        public CloseMode SelectedCloseMode
        {
            get
            {
                return selectedCloseMode;
            }

            set
            {
                selectedCloseMode = value;
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

        public string SelectedMassInformation
        {
            get
            {
                int m = MassSelection != null ? MassSelection.Count : 0;
                return string.Format("Selected Masses: {0}", m);
            }
        }

        public System.Windows.Size Size
        {
            get; set;
        }

        public double Top
        {
            get; set;
        }

        public static void Respawn(DirectSunViewModel viewModel, bool resize)
        {
            var settings = DefaultViewSettings;

            // Reopen window with previous position / size
            if (resize) {
                settings.Width = viewModel.Size.Width;
                settings.Height = viewModel.Size.Height;
                settings.Top = viewModel.Top;
                settings.Left = viewModel.Left;
                settings.WindowStartupLocation = System.Windows.WindowStartupLocation.Manual;
                settings.SizeToContent = System.Windows.SizeToContent.Manual;
            }
            viewModel.SelectedCloseMode = CloseMode.Close;
            SCaddinsApp.WindowManager.ShowDialog(viewModel, null, settings);
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
            // Get old size/location for respawning (if required)
            Size = SCaddinsApp.WindowManager.Size;
            Left = SCaddinsApp.WindowManager.Left;
            Top = SCaddinsApp.WindowManager.Top;
            switch (selectedCloseMode) {
                case CloseMode.Close:
                    base.OnDeactivate(true);
                    break;
                case CloseMode.FaceSelection:
                    try {
                        FaceSelection = uidoc.Selection.PickObjects(Autodesk.Revit.UI.Selection.ObjectType.Face, "Select Faces");
                    } catch (OperationCanceledException ex) {
                        SCaddinsApp.WindowManager.ShowMessageBox(ex.Message);
                        FaceSelection = null;
                    }
                    Respawn(this, true);
                    break;
                case CloseMode.MassSelection:
                    try {
                        MassSelection = uidoc.Selection.PickObjects(Autodesk.Revit.UI.Selection.ObjectType.Element, "Select Masses");
                    } catch (OperationCanceledException ex) {
                        SCaddinsApp.WindowManager.ShowMessageBox(ex.Message);
                        MassSelection = null;
                    }
                    Respawn(this, true);
                    break;
                case CloseMode.Clear:
                case CloseMode.Analize:
                    base.OnDeactivate(close);
                    break;
                default:
                    base.OnDeactivate(true);
                    break;
            }
        }
    }
}
