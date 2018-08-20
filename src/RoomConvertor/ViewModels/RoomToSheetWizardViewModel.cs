// (C) Copyright 2018 by Andrew Nicholas
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

namespace SCaddins.RoomConvertor.ViewModels
{
    using System.Collections.Generic;
    using Caliburn.Micro;
    using Autodesk.Revit.DB;

    public class RoomToSheetWizardViewModel : Screen
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
            get
            {
                return manager.TitleBlockId;
            }

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
            get
            {
                return manager.Scale;
            }

            set
            {
                if (value != manager.Scale) {
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
