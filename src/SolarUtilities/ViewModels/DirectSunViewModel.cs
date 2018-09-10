namespace SCaddins.SolarUtilities.ViewModels
{
    using System;
    using System.Collections.Generic;
    using System.Dynamic;
    using System.Linq;
    using Autodesk.Revit.DB;
    using Autodesk.Revit.UI;
    using Caliburn.Micro;

    public class DirectSunViewModel : Screen
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Microsoft.Usage", "CA2213: Disposable fields should be disposed", Justification = "Parameter intialized by Revit", MessageId = "uidoc")]
        private UIDocument uidoc;
        private CloseMode selectedCloseMode;

        public enum CloseMode
        {
            Close,
            MassSelection,
            FaceSelection,
            Analize
        };

        public CloseMode SelectedCloseMode
        {
            get { return selectedCloseMode; }
        }

        public DirectSunViewModel(UIDocument uidoc)
        {
            this.uidoc = uidoc;
            selectedCloseMode = CloseMode.Close;
            MassSelection = null;
            FaceSelection = null;
        }

        public IList<Reference> FaceSelection
        {
            get; set;
        }

        public IList<Reference> MassSelection
        {
            get; set;
        }

        public string SelectionInformation
        {
            get
            {
                int m = MassSelection != null ? MassSelection.Count : 0;
                int f = FaceSelection != null ? FaceSelection.Count : 0;
                return string.Format("Masses: {0}, Faces {1}", m, f);
            }
        }

        public void RunAnalysis()
        {
            selectedCloseMode = CloseMode.Analize;
            TryClose(true);
        }

        private static void Respawn(DirectSunViewModel viewModel)
        {
            dynamic settings = new ExpandoObject();
            settings.Height = 480;
            settings.Width = 300;
            settings.Title = "Direct Sun - By Andrew Nicholas";
            settings.ShowInTaskbar = false;
            settings.SizeToContent = System.Windows.SizeToContent.WidthAndHeight;
            SCaddinsApp.WindowManager.ShowDialog(viewModel, null, settings);
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
                default:
                    base.OnDeactivate(close);
                    break;
            }
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
    }
}
