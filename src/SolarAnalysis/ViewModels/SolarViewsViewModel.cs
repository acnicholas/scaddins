// (C) Copyright 2018-2020 by Andrew Nicholas
//
// This file is part of SCaddins.
//
// SCaddins is free software: you can redistribute it and/or modify
// it under the terms of the GNU Lesser General Public License as published by
// the Free Software Foundation, either version 3 of the License, or
// (at your option) any later version.
//
// SCaddins is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU Lesser General Public License for more details.
//
// You should have received a copy of the GNU Lesser General Public License
// along with SCaddins.  If not, see <http://www.gnu.org/licenses/>.

using System.Threading;
using System.Threading.Tasks;

namespace SCaddins.SolarAnalysis.ViewModels
{
    using System;
    using System.Collections.Generic;
    using System.Dynamic;
    using Autodesk.Revit.DB;
    using Autodesk.Revit.UI;
    using Caliburn.Micro;

    public class SolarViewsViewModel : Screen
    {
        private DateTime creationDate;
        private DateTime endTime;
        private TimeSpan interval;
        private SolarAnalysisManager model;
        private CloseMode selectedCloseMode;
        private DateTime startTime;

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Microsoft.Usage", "CA2213: Disposable fields should be disposed", Justification = "Parameter intialized by Revit", MessageId = "uidoc")]
        private UIDocument uidoc;

        public SolarViewsViewModel(UIDocument uidoc)
        {
            this.uidoc = uidoc;
            selectedCloseMode = CloseMode.Close;
            MassSelection = null;
            FaceSelection = null;
            AnalysisGridSize = 1000;
            SolarRayLength = 100000; // this is in mm (so 100m is assigned here)
            Size = SCaddinsApp.WindowManager.Size;
            Left = SCaddinsApp.WindowManager.Left;
            Top = SCaddinsApp.WindowManager.Top;
            model = new SolarAnalysisManager(uidoc);
            creationDate = new DateTime(2018, 06, 21);
            startTime = new DateTime(2018, 06, 21, 9, 0, 0, DateTimeKind.Local);
            endTime = new DateTime(2018, 06, 21, 15, 0, 0, DateTimeKind.Local);
            interval = new TimeSpan(1, 00, 00);
            RotateCurrentView = CanRotateCurrentView;
            if (!CanRotateCurrentView)
            {
                Create3dViews = true;
            }
        }

        public enum CloseMode
        {
            Close,
            MassSelection,
            FaceSelection,
            Analize,
            DrawSolarRay,
            Clear
        }

        public static dynamic DefaultViewSettings
        {
            get
            {
                dynamic settings = new ExpandoObject();
                settings.Height = 620;
                settings.Width = 300;
                settings.Icon = new System.Windows.Media.Imaging.BitmapImage(
                    new Uri("pack://application:,,,/SCaddins;component/Assets/scaos.png"));
                settings.Title = "Solar Analysis - By Andrew Nicholas";
                settings.ShowInTaskbar = false;
                settings.SizeToContent = System.Windows.SizeToContent.WidthAndHeight;
                settings.WindowStartupLocation = System.Windows.WindowStartupLocation.CenterScreen;
                return settings;
            }
        }

        public static BindableCollection<TimeSpan> Intervals
        {
            get
            {
                var times = new BindableCollection<TimeSpan>();
                times.Add(new TimeSpan(00, 15, 00));
                times.Add(new TimeSpan(00, 30, 00));
                times.Add(new TimeSpan(1, 00, 00));
                return times;
            }
        }

        public double AnalysisGridSize
        {
            get; set;
        }

        public bool CanCreateAnalysisView
        {
            get
            {
                return model.CanCreateAnalysisView;
            }
        }

        public bool CanRotateCurrentView
        {
            get
            {
                return model.CanRotateActiveView;
            }
        }

        public bool Create3dViews
        {
            get
            {
                return model.Create3dViews;
            }

            set
            {
                if (model.Create3dViews != value)
                {
                    model.Create3dViews = value;
                    NotifyOfPropertyChange(() => CurrentModeSummary);
                    NotifyOfPropertyChange(() => CreateAnalysisView);
                    NotifyOfPropertyChange(() => ShowDateSelectionPanel);
                }
            }
        }

        public bool CreateAnalysisView
        {
            get
            {
                return model.CreateAnalysisView;
            }

            set
            {
                if (model.CreateAnalysisView != value)
                {
                    model.CreateAnalysisView = value;
                    NotifyOfPropertyChange(() => CurrentModeSummary);
                    NotifyOfPropertyChange(() => CreateAnalysisView);
                    NotifyOfPropertyChange(() => ShowDateSelectionPanel);
                }
            }
        }

        public bool CreateShadowPlans
        {
            get
            {
                return model.CreateShadowPlans;
            }

            set
            {
                if (model.CreateShadowPlans != value)
                {
                    model.CreateShadowPlans = value;
                    NotifyOfPropertyChange(() => CurrentModeSummary);
                    NotifyOfPropertyChange(() => CreateAnalysisView);
                    NotifyOfPropertyChange(() => ShowDateSelectionPanel);
                }
            }
        }

        public DateTime CreationDate
        {
            get
            {
                return creationDate;
            }

            set
            {
                if (value != creationDate)
                {
                    var oldStartIndex = StartTimes.IndexOf(SelectedStartTime);
                    var oldEndIndex = EndTimes.IndexOf(SelectedEndTime);
                    creationDate = value;
                    NotifyOfPropertyChange(() => StartTimes);
                    NotifyOfPropertyChange(() => EndTimes);
                    SelectedStartTime = StartTimes[oldStartIndex];
                    SelectedEndTime = EndTimes[oldEndIndex];
                }
            }
        }

        public string CurrentModeSummary
        {
            get
            {
                if (RotateCurrentView)
                {
                    return "Rotate Current View";
                }
                if (Create3dViews)
                {
                    return "Create View[s]";
                }
                if (CreateShadowPlans)
                {
                    return "Create Plans";
                }
                if (CreateAnalysisView)
                {
                    return "Create Analysis View";
                }
                if (DrawSolarRay)
                {
                    return "Draw Solar Ray";
                }
                return "OK";
            }
        }

        public bool DrawSolarRay
        {
            get
            {
                selectedCloseMode = CloseMode.DrawSolarRay;
                return model.DrawSolarRay;
            }

            set
            {
                if (model.DrawSolarRay != value)
                {
                    model.DrawSolarRay = value;
                    NotifyOfPropertyChange(() => CurrentModeSummary);
                    NotifyOfPropertyChange(() => CreateAnalysisView);
                    NotifyOfPropertyChange(() => ShowDateSelectionPanel);
                    NotifyOfPropertyChange(() => ShowSolarRayOptionsPanel);
                }
            }
        }

        public bool EnableRotateCurrentView
        {
            get
            {
                return CanRotateCurrentView;
            }
        }

        public BindableCollection<DateTime> EndTimes
        {
            get
            {
                var times = new BindableCollection<DateTime>();
                for (int hour = 9; hour < 18; hour++)
                {
                    times.Add(new DateTime(creationDate.Year, creationDate.Month, creationDate.Day, hour, 0, 0, DateTimeKind.Local));
                }
                return times;
            }
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

        public bool RotateCurrentView
        {
            get
            {
                return model.RotateCurrentView;
            }

            set
            {
                if (model.RotateCurrentView != value)
                {
                    model.RotateCurrentView = value;
                    NotifyOfPropertyChange(() => CurrentModeSummary);
                }
            }
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

        public DateTime SelectedEndTime
        {
            get
            {
                return endTime;
            }

            set
            {
                if (value != endTime)
                {
                    endTime = value;
                    NotifyOfPropertyChange(() => SelectedEndTime);
                }
            }
        }

        public string SelectedFaceInformation
        {
            get
            {
                int f = FaceSelection != null ? FaceSelection.Count : 0;
                return $"Selected Faces: {f}";
            }
        }

        public bool ShowSolarRayOptionsPanel
        {
                get
                {
                        return DrawSolarRay;
                }
        }

        public double SolarRayLength
        {
                get; set;
        }

        public TimeSpan SelectedInterval
        {
            get => interval;
            set => interval = value;
        }

        public string SelectedMassInformation
        {
            get
            {
                int m = MassSelection != null ? MassSelection.Count : 0;
                return $"Selected Masses: {m}";
            }
        }

        public DateTime SelectedStartTime
        {
            get
            {
                return startTime;
            }

            set
            {
                if (value != startTime)
                {
                    startTime = value;
                    NotifyOfPropertyChange(() => SelectedStartTime);
                }
            }
        }

        public bool ShowDateSelectionPanel
        {
            get
            {
                return CreateShadowPlans || Create3dViews;
            }
        }

        public System.Windows.Size Size
        {
            get; set;
        }

        public BindableCollection<DateTime> StartTimes
        {
            get
            {
                var times = new BindableCollection<DateTime>();
                for (int hour = 8; hour < 17; hour++)
                {
                    times.Add(new DateTime(creationDate.Year, creationDate.Month, creationDate.Day, hour, 0, 0, DateTimeKind.Local));
                }
                return times;
            }
        }

        /// <summary>
        /// Top left coord of main dialog
        /// </summary>
        public double Top
        {
            get; set;
        }

        public string ViewInformation
        {
            get
            {
                return model.ActiveIewInformation;
            }
        }

        /// <summary>
        /// Attempt to re-open the main dialog after a user selection has been made.
        /// </summary>
        /// <param name="viewModel"></param>
        /// <param name="resize">Resize and relocate to previous location.</param>
        public static Task Respawn(SolarViewsViewModel viewModel, bool resize)
        {
            var settings = DefaultViewSettings;

            // Reopen window with previous position / size
            if (resize)
            {
                settings.Width = viewModel.Size.Width;
                settings.Height = viewModel.Size.Height;
                settings.Top = viewModel.Top;
                settings.Left = viewModel.Left;
                settings.WindowStartupLocation = System.Windows.WindowStartupLocation.Manual;
                settings.SizeToContent = System.Windows.SizeToContent.Manual;
            }
            viewModel.SelectedCloseMode = CloseMode.Close;
            SCaddinsApp.WindowManager.ShowDialogAsync(viewModel, null, settings);
            return Task.FromResult(true);
        }

        public void Clear()
        {
            selectedCloseMode = CloseMode.Clear;
            TryCloseAsync(true);
        }

        /// <summary>
        /// Run the selected mode.
        /// </summary>
        public void OK()
        {
            if (model.CreateAnalysisView || model.DrawSolarRay)
            {
                TryCloseAsync(true);
            }
            else
            {
                var log = new ModelSetupWizard.TransactionLog(CurrentModeSummary);
                model.StartTime = SelectedStartTime.ToLocalTime();
                model.EndTime = SelectedEndTime.ToLocalTime();
                model.ExportTimeInterval = SelectedInterval;
                model.Go(log);
                SCaddinsApp.WindowManager.ShowMessageBox(log.Summary());
                DockablePaneId docablePaneId = DockablePanes.BuiltInDockablePanes.ProjectBrowser;
                DockablePane dP = new DockablePane(docablePaneId);
                dP.Show();
            }
        }

        public void RunAnalysis()
        {
            selectedCloseMode = CloseMode.Analize;
            TryCloseAsync(true);
        }

        public void SelectAnalysisFaces()
        {
            selectedCloseMode = CloseMode.FaceSelection;
            TryCloseAsync(true);
        }

        public void SelectMasses()
        {
            selectedCloseMode = CloseMode.MassSelection;
            TryCloseAsync(true);
        }

        protected override Task OnDeactivateAsync(bool close, CancellationToken cancellationToken)
        {
            // Get old size/location for respawning (if required)
            Size = SCaddinsApp.WindowManager.Size;
            Left = SCaddinsApp.WindowManager.Left;
            Top = SCaddinsApp.WindowManager.Top;
            

            switch (selectedCloseMode)
            {
                case CloseMode.FaceSelection:
                    try
                    {
                        FaceSelection = uidoc.Selection.PickObjects(Autodesk.Revit.UI.Selection.ObjectType.Face, "Select Faces");
                    }
                    catch (Autodesk.Revit.Exceptions.OperationCanceledException ex)
                    {
                        SCaddinsApp.WindowManager.ShowMessageBox(ex.Message);
                        FaceSelection = null;
                    }
                    catch (Autodesk.Revit.Exceptions.ArgumentNullException anex)
                    {
                        SCaddinsApp.WindowManager.ShowMessageBox(anex.Message);
                        FaceSelection = null;
                    }
                    catch (Autodesk.Revit.Exceptions.ArgumentOutOfRangeException aoorex)
                    {
                        SCaddinsApp.WindowManager.ShowMessageBox(aoorex.Message);
                        FaceSelection = null;
                    }
                    catch (Autodesk.Revit.Exceptions.ForbiddenForDynamicUpdateException ffduex)
                    {
                        SCaddinsApp.WindowManager.ShowMessageBox(ffduex.Message);
                        FaceSelection = null;
                    }
                    return Respawn(this, true);

                case CloseMode.MassSelection:
                    try
                    {
                        MassSelection = uidoc.Selection.PickObjects(Autodesk.Revit.UI.Selection.ObjectType.Element, "Select Masses");
                    }
                    catch (Autodesk.Revit.Exceptions.OperationCanceledException ex)
                    {
                        SCaddinsApp.WindowManager.ShowMessageBox(ex.Message);
                        MassSelection = null;
                    }
                    catch (Autodesk.Revit.Exceptions.ArgumentNullException anex)
                    {
                        SCaddinsApp.WindowManager.ShowMessageBox(anex.Message);
                        MassSelection = null;
                    }
                    catch (Autodesk.Revit.Exceptions.ArgumentOutOfRangeException aoorex)
                    {
                        SCaddinsApp.WindowManager.ShowMessageBox(aoorex.Message);
                        MassSelection = null;
                    }
                    catch (Autodesk.Revit.Exceptions.ForbiddenForDynamicUpdateException ffduex)
                    {
                        SCaddinsApp.WindowManager.ShowMessageBox(ffduex.Message);
                        MassSelection = null;
                    }
                    return Respawn(this, true);
            }
            return Task.FromResult(true);
        }
    }
}
