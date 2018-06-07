using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autodesk.Revit.UI;
using System.Collections.ObjectModel;
using Caliburn.Micro;
using Autodesk.Revit.DB;

namespace SCaddins.RoomConvertor.ViewModels
{

    class RoomToSheetWizardViewModel : Screen
    {
        private RoomConversionManager manager;

        public RoomToSheetWizardViewModel(RoomConversionManager manager)
        {
            this.manager = manager;
        }

        public Dictionary<string, ElementId> TitleBlocks
        {
            get { return manager.TitleBlocks; }
        }

        public ElementId SelectedTitleBlock
        {
            get { return manager.TitleBlockId;  }
            set
            {
                if (value != manager.TitleBlockId)
                {
                    manager.TitleBlockId = value;
                    NotifyOfPropertyChange(() => SelectedTitleBlock);
                }
            }
        }

        public int Scale
        {
            get { return manager.Scale; }
            set
            {
                if (value != manager.Scale)
                {
                    manager.Scale = value;
                    NotifyOfPropertyChange(() => Scale);
                }
            }
        }

        public int CropOffset
        {
            get { return manager.CropRegionEdgeOffset; }
            set
            {
                if (value != manager.CropRegionEdgeOffset)
                {
                    manager.CropRegionEdgeOffset = value;
                    NotifyOfPropertyChange(() => CropOffset);
                }
            }
        }

        public void OK()
        {
            TryClose(true);
        }

        public void Cancel()
        {
            TryClose(false);
        }

    }

}
