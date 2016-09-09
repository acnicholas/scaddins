// (C) Copyright 2016 by Andrew Nicholas
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

namespace SCaddins.ModelUpdater
{
    using System;
    using Autodesk.Revit.DB;
    
    
    public class WallUnjoiner : IUpdater
    {
        static AddInId appId;
        static UpdaterId updaterId;

        public WallUnjoiner(AddInId id) 
        {
            appId = id;
            updaterId = new UpdaterId(appId, new Guid("04D4BBFC-D7C2-4327-99B6-02C6B41B163C"));
        }

        public void Execute(UpdaterData data)
        {
            if (!SCaddins.Scaddins.Default.UnjoinNewWalls) {
                return;
            }

            Document doc = data.GetDocument();

            foreach (ElementId id in data.GetAddedElementIds()) {
                Wall wall = doc.GetElement(id) as Wall;
                if (wall != null) {
                    WallUtils.DisallowWallJoinAtEnd(wall, 0);
                    WallUtils.DisallowWallJoinAtEnd(wall, 1);
                }
            }
        }

        public string GetAdditionalInformation()
        {
            return "Wall Unjoiner: Unjoins all newly created walls.";
        }

        public ChangePriority GetChangePriority()
        {
            return ChangePriority.FloorsRoofsStructuralWalls;
        }

        public UpdaterId GetUpdaterId()
        {
            return updaterId;
        }

        public string GetUpdaterName()
        {
            return "Wall Unjoiner";
        }
    }
}
/* vim: set ts=4 sw=4 nu expandtab: */
