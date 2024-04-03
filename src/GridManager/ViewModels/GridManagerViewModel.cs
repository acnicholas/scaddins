namespace SCaddins.GridManager.ViewModels
{
    using System.Collections.Generic;
    using Autodesk.Revit.DB;
    using Caliburn.Micro;
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
        
        public GridManagerViewModel(View activeView, List<ElementId> selection)
        {
            this.activeView = activeView;
            this.selection = selection;
        }
        
        public bool? ShowBottomGridBubbles
        {
            get => showBottomBubbles;
            set
            {
                if (value.HasValue)
                {
                    showBottomBubbles = value;
                    GridManager.ShowBottomGridBubblesByView(activeView, showBottomBubbles.Value, selection);
                }
                else
                {
                    showBottomBubbles = true;
                    GridManager.ShowBottomGridBubblesByView(activeView, true, selection);
                }
                NotifyOfPropertyChange(() => ShowBottomGridBubblesLabel);
                NotifyOfPropertyChange(() => ShowBottomGridBubbles);
            }
        }
        
        public string ShowBottomGridBubblesLabel => "Bottom Grid Bubbles";

        public bool? ShowLeftGridBubbles
        {
            get => showLeftBubbles;
            set
            {
                if (value.HasValue)
                {
                    showLeftBubbles = value;
                    GridManager.ShowLeftGridBubblesByView(activeView, showLeftBubbles.Value, selection);
                }
                else
                {
                    showLeftBubbles = true;
                    GridManager.ShowLeftGridBubblesByView(activeView, true, selection);
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
                
                // NotifyOfPropertyChange(() => ShowRightGridBubblesLabel);
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
                    GridManager.ShowRightGridBubblesByView(activeView, showRightBubbles.Value, selection);
                }
                else
                {
                    showRightBubbles = true;
                    GridManager.ShowRightGridBubblesByView(activeView, true, selection);
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
                    GridManager.ShowTopGridBubblesByView(activeView, showTopBubbles.Value, selection);
                }
                else
                {
                    showTopBubbles = true;
                    GridManager.ShowTopGridBubblesByView(activeView, true, selection);
                }
                NotifyOfPropertyChange(() => ShowTopGridBubblesLabel);
                NotifyOfPropertyChange(() => ShowTopGridBubbles);
            }
        }
        
        public string ShowTopGridBubblesLabel  => "Top Grid Bubbles";
        
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
            SCaddinsApp.WindowManager.ShowMessageBox("Test", "3d");
            GridManager.Toggle2dGridsByView(activeView, false, selection);
        }
    }
}