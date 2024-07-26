// (C) Copyright 2024 by Andrew Nicholas
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

namespace SCaddins.GridManager.ViewModels
{
    using System.Collections.Generic;
    using Autodesk.Revit.DB;
    using Caliburn.Micro;
    using FilterTreeControlWPF;
    using View = Autodesk.Revit.DB.View;
    
    public class GridManagerViewModel : Screen
    {
        private readonly View activeView;
        private bool? showRightBubbles;
        private bool? showRightLevels;
        private bool? showLeftBubbles;
        private bool? showLeftLevels;
        private bool? showTopBubbles;
        private bool? showBottomBubbles;
        private List<ElementId> selection;
        private string statusBarLabel;

        public GridManagerViewModel(View activeView, List<ElementId> selection)
        {
            this.activeView = activeView;
            this.selection = selection;
            StatusBarLabel = GridManager.GetViewInformation(activeView, selection);
        }
        
        public bool? ShowBottomGridBubbles
        {
            get => showBottomBubbles;
            set
            {
                if (value.HasValue)
                {
                    showBottomBubbles = value;
                    var i = GridManager.ShowBottomGridBubblesByView(activeView, showBottomBubbles.Value, selection);
                    StatusBarLabel = i + " Grid bubbles edited";
                }
                else
                {
                    showBottomBubbles = true;
                    var i = GridManager.ShowBottomGridBubblesByView(activeView, true, selection);
                    StatusBarLabel = i + " Grid bubbles edited";
                }
                NotifyOfPropertyChange(() => ShowBottomGridBubblesLabel);
                NotifyOfPropertyChange(() => ShowBottomGridBubbles);
            }
        }

        public bool LevelsGroupBoxEnabled
        {
            get => false;
        }
        
        public string ShowBottomGridBubblesLabel => "Bottom Grid Bubbles";

        //public bool ShowLeftGridBubblesEnabled
        //{
        //    get => false;
        //}

        public bool? ShowLeftGridBubbles
        {
            get => showLeftBubbles;
            set
            {
                if (value.HasValue)
                {
                    showLeftBubbles = value;
                    var i = GridManager.ShowLeftGridBubblesByView(activeView, showLeftBubbles.Value, selection);
                    StatusBarLabel = i + " Grid bubbles edited";
                }
                else
                {
                    showLeftBubbles = true;
                    var i = GridManager.ShowLeftGridBubblesByView(activeView, true, selection);
                    StatusBarLabel = i + " Grid bubbles edited";
                }
                NotifyOfPropertyChange(() => ShowLeftGridBubblesLabel);
                NotifyOfPropertyChange(() => ShowLeftGridBubbles);
            }
        }
        
        public string ShowLeftGridBubblesLabel => "Left Grid Bubbles";
        
        public bool? ShowLeftLevels
        {
            get => showLeftLevels;
            set
            {
                if (value.HasValue)
                {
                    showLeftLevels = value;
                    GridManager.ShowLeftLevelEndsByView(activeView, showLeftLevels.Value, selection);
                }
                else
                {
                    showLeftBubbles = true;
                    GridManager.ShowLeftLevelEndsByView(activeView, true, selection);
                }
                
                NotifyOfPropertyChange(() => ShowLeftLevels);
            }
        }
        
        public string ShowLeftLevelsLabel => "Left Level Heads";
        
        public bool? ShowRightGridBubbles
        {
            get => showRightBubbles;
            set
            {
                if (value.HasValue)
                {
                    showRightBubbles = value;
                    var count = GridManager.ShowRightGridBubblesByView(activeView, showRightBubbles.Value, selection);
                    StatusBarLabel = count + " Grid bubbles edited";
                }
                else
                {
                    showRightBubbles = true;
                    var count = GridManager.ShowRightGridBubblesByView(activeView, true, selection);
                    StatusBarLabel = count + " Grid bubbles edited";
                }
                NotifyOfPropertyChange(() => ShowRightGridBubblesLabel);
                NotifyOfPropertyChange(() => ShowRightGridBubbles);
            }
        }

        public string ShowRightGridBubblesLabel => "Right Grid Bubbles";
        
        public bool? ShowRightLevels
        {
            get
            {
                return showRightLevels;
            }
            
            set
            {
                if (value.HasValue)
                {
                    showRightLevels = value;
                    GridManager.ShowRightLevelEndsByView(activeView, showRightLevels.Value, selection);
                }
                else
                {
                    showRightBubbles = true;
                    GridManager.ShowRightLevelEndsByView(activeView, true, selection);
                }
                
                // NotifyOfPropertyChange(() => ShowRightGridBubblesLabel);
                NotifyOfPropertyChange(() => ShowRightLevels);
            }
        }
        
        public string ShowRightLevelsLabel => "Right Level Heads";

        public bool? ShowTopGridBubbles
        {
            get => showTopBubbles;
            set
            {
                if (value.HasValue)
                {
                    showTopBubbles = value;
                    var i = GridManager.ShowTopGridBubblesByView(activeView, showTopBubbles.Value, selection);
                    StatusBarLabel = i + " Grid bubbles edited";
                }
                else
                {
                    showTopBubbles = true;
                    var i = GridManager.ShowTopGridBubblesByView(activeView, true, selection);
                    StatusBarLabel = i + " Grid bubbles edited";
                }
                NotifyOfPropertyChange(() => ShowTopGridBubblesLabel);
                NotifyOfPropertyChange(() => ShowTopGridBubbles);
            }
        }
        
        public string ShowTopGridBubblesLabel  => "Top Grid Bubbles";

        public string StatusBarLabel
        {
            get
            {
                return statusBarLabel;
            }
            private set
            {
                statusBarLabel = value;
                NotifyOfPropertyChange(() => StatusBarLabel);
            }
        }


        public void ShowAllGridBubbles()
        {
            ShowBottomGridBubbles = true;
            ShowTopGridBubbles = true;
            ShowLeftGridBubbles = true;
            ShowRightGridBubbles = true;
        }
        
        public void HideAllGridBubbles()
        {
            ShowBottomGridBubbles = false;
            ShowTopGridBubbles = false;
            ShowLeftGridBubbles = false;
            ShowRightGridBubbles = false;
        }
        
        public void ShowAllLevelEnds()
        {
            ShowLeftLevels = true;
            ShowRightLevels = true;
        }
        
        public void HideAllLevelEnds()
        {
            ShowLeftLevels = false;
            ShowRightLevels = false;
        }

        public void SetGridsTo2d()
        {
            GridManager.Toggle2dGridsByView(activeView, true, selection);
        }
        
        public void SetGridsTo3d()
        {
            GridManager.Toggle2dGridsByView(activeView, false, selection);
        }

        public void SetLevelsTo2d()
        {
            SCaddinsApp.WindowManager.ShowMessageBox("2d");
            GridManager.Toggle2dLevelsByView(activeView, true, selection);
        }

        public void SetLevelsTo3d()
        {
            GridManager.Toggle2dLevelsByView(activeView, false, selection);
        }
    }
}