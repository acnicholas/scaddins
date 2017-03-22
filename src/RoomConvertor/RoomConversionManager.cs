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

namespace SCaddins.RoomConvertor
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Autodesk.Revit.DB;
    using Autodesk.Revit.DB.Architecture;

    public class RoomConversionManager
    {
        private Dictionary<string, View> existingSheets =
            new Dictionary<string, View>();

        private Dictionary<string, View> existingViews =
            new Dictionary<string, View>();

        private Dictionary<string, ElementId> titleBlocks = 
            new Dictionary<string, ElementId>();

        private SCaddins.Common.SortableBindingListCollection<RoomConversionCandidate> allCandidates;
        private Document doc;
        private SCaddins.Common.SortableBindingListCollection<RoomConversionCandidate> candidates;

        public SCaddins.Common.SortableBindingListCollection<RoomConversionCandidate> Candidates {
            get{ return candidates; }
        }

        public Document Doc {
            get{ return doc; }
        }

        public Dictionary<string, ElementId> TitleBlocks
        {
            get{ return titleBlocks; }
        }

        public ElementId TitleBlockId
        {
            get; set;
        }

        public int Scale{
            get; set;
        }

        public RoomConversionManager(Document doc)
        {
            candidates = new SCaddins.Common.SortableBindingListCollection<RoomConversionCandidate>();
            this.allCandidates = new SCaddins.Common.SortableBindingListCollection<RoomConversionCandidate>();
            this.doc = doc;
            this.titleBlocks = GetAllTitleBlockTypes(this.doc);
            this.TitleBlockId = ElementId.InvalidElementId;
            this.Scale = 50;
            SheetCopier.SheetCopierManager.GetAllSheets(existingSheets, this.doc);
            SheetCopier.SheetCopierManager.GetAllViewsInModel(existingViews, this.doc);
            FilteredElementCollector collector = new FilteredElementCollector(this.doc);
            collector.OfClass(typeof(SpatialElement));
            foreach (Element e in collector) {
                if (e.IsValidObject && (e is Room)) {
                    Room room = e as Room;
                    if (room.Area > 0 && room.Location != null) {
                        allCandidates.Add(new RoomConversionCandidate(room, existingSheets, existingViews));
                    }
                }
            }
            //Initially add all canditates.
            this.Reset();
        }

        public void CreateViewsAndSheets(
            System.ComponentModel.BindingList<RoomConversionCandidate> candidates)
        {
            Transaction t = new Transaction(doc, "Rooms to Views");
            t.Start(); 
            foreach (RoomConversionCandidate c in candidates) {
                this.CreateViewAndSheet(c);
            }
            t.Commit();
        }

        public static Dictionary<string, ElementId> GetAllTitleBlockTypes(Document doc)
        {
            var result = new Dictionary<string, ElementId>();

            FilteredElementCollector a = new FilteredElementCollector(doc)
                .OfCategory(BuiltInCategory.OST_TitleBlocks).OfClass(typeof(FamilySymbol));

            foreach (FamilySymbol e in a) {
                var s = e.Family.Name + "-" + e.Name;
                if (!result.ContainsKey(s)) {
                    result.Add(s, e.Id);
                }
            }

            //Add an empty title in case there's none
            result.Add("none",ElementId.InvalidElementId);

            return result;
        }

        public ElementId GetTitleBlockByName(string titleBlockName)
        {
            ElementId id = ElementId.InvalidElementId;
            bool titleFound = this.titleBlocks.TryGetValue(titleBlockName, out id);
            return titleFound ? id : ElementId.InvalidElementId;
        }

        public void CreateRoomMasses(System.ComponentModel.BindingList<RoomConversionCandidate> candidates)
        {
            int errCount = 0;
            int roomCount = 0;
            var t = new Transaction(doc, "Rooms to Masses");
            t.Start(); 
            foreach (RoomConversionCandidate c in candidates) {
                roomCount++;
                if (!this.CreateRoomMass(c.Room)) {
                    errCount++;
                }
            }
            t.Commit();
            Autodesk.Revit.UI.TaskDialog.Show(
                        "Rooms To Masses",
                        (roomCount - errCount) + " Room masses created with " + errCount + " errors."
                        );
        }

        public void Reset()
        {
            Candidates.Clear();
            foreach (RoomConversionCandidate c in allCandidates) {
                Candidates.Add(c);
            }
        }

        private static void CopyAllMassParametersToRooms(Element host, Room  dest)
        {
            Parameter name = host.LookupParameter("Name");
            if (name != null &&  name.StorageType == StorageType.String){
                dest.Name = name.AsString();
            }

            Parameter number = host.LookupParameter("Number");
            if (number != null &&  number.StorageType == StorageType.String){
                dest.Number = number.AsString();
            }

            CopyAllParameters(host, dest);
        }

        private static void CopyAllRoomParametersToMasses(Element host, Element  dest)
        {
            Parameter paramRoomId = dest.LookupParameter("RoomId");
            if (paramRoomId != null &&  paramRoomId.StorageType == StorageType.Integer){
                paramRoomId.Set(host.Id.IntegerValue);
            }

            CopyAllParameters(host, dest);
        }

        private static bool ValidElements(Element host, Element  dest)
        {
            if (host == null || dest == null) return false;
            if (!host.IsValidObject || !dest.IsValidObject) return false;  
            return true;
        }

        private static void CopyAllParameters(Element host, Element  dest)
        {
            if (!ValidElements(host, dest)) return;
                        
            foreach (Parameter param in host.Parameters) {
                
                if (!param.HasValue || param == null) {
                    continue;
                }
                              
                Parameter paramDest = dest.LookupParameter(param.Definition.Name);
                if (paramDest != null && paramDest.UserModifiable && paramDest.StorageType == param.StorageType) {
                    switch (param.StorageType) {
                        case StorageType.String:
                            if (!paramDest.IsReadOnly && paramDest.UserModifiable) {
                                string v = param.AsString();
                                paramDest.Set(v);
                            }
                            break;
                        case StorageType.Integer:
                            int b = param.AsInteger();
                            if (b != -1 && !paramDest.IsReadOnly) {
                                paramDest.Set(b);
                            }
                            break;
                        case StorageType.Double:
                            double d = param.AsDouble();
                            if (!paramDest.IsReadOnly) {
                                paramDest.Set(d);
                            }
                            break;
                    }
                }
            }
        }

        public void SynchronizeMassesToRooms()
        {
          var t = new Transaction(doc, "Synchronize Masses to Rooms");
          t.Start(); 

          FilteredElementCollector collector = 
              new FilteredElementCollector(doc).OfCategory(BuiltInCategory.OST_Mass).OfClass(typeof(DirectShape));

          int i = 0;
            foreach (Element e in collector) {
                Parameter p = e.LookupParameter("RoomId");
                i++;
                int intId = p.AsInteger();
                if (intId > 0) {
                    ElementId id = new ElementId(intId);
                    Element room = doc.GetElement(id);
                    if (room != null) {
                        CopyAllMassParametersToRooms(e, (Room)room);
                    }
                }
            }
          
          Autodesk.Revit.UI.TaskDialog.Show("Synchronize Masses to Rooms", i + " masses synchronized");
          t.Commit();         
        }

        private bool CreateRoomMass(Room room)
        {
            try {
                var height = room.LookupParameter("Limit Offset");
                var curves = new List<CurveLoop>();
                var spatialBoundaryOptions = new SpatialElementBoundaryOptions();
                spatialBoundaryOptions.StoreFreeBoundaryFaces = true;
                var loop = new CurveLoop();
                var bdySegs = room.GetBoundarySegments(spatialBoundaryOptions);
                foreach (var seg in bdySegs[0]) {
                    #if REVIT2017 || REVIT2016
                    loop.Append(seg.GetCurve());
                    #else
                    loop.Append(seg.Curve);
                    #endif
                }

                curves.Add(loop);
                SolidOptions options = new SolidOptions(ElementId.InvalidElementId, ElementId.InvalidElementId);
                Solid roomSolid = GeometryCreationUtilities.CreateExtrusionGeometry(curves, new XYZ(0, 0, 1), height.AsDouble(), options);
                DirectShape roomShape = DirectShape.CreateElement(doc, new ElementId(BuiltInCategory.OST_Mass), "A", "B");
                roomShape.SetShape(new GeometryObject[] { roomSolid });
                CopyAllRoomParametersToMasses(room, roomShape);

            } catch (Exception ex) {
                System.Diagnostics.Debug.WriteLine(ex.Message);
                return false;
            }
            return true;
        }

        private void CreateViewAndSheet(RoomConversionCandidate candidate)
        {
            // Create plans
            ViewSheet sheet = ViewSheet.Create(doc, this.TitleBlockId);
            sheet.Name = candidate.DestinationSheetName;
            sheet.SheetNumber = candidate.DestinationSheetNumber;

            // Get Centre before placing any views
            XYZ sheetCentre = CentreOfSheet(sheet, this.doc);

            // Create plan of room
            ViewPlan plan = ViewPlan.Create(doc, GetFloorPlanViewFamilyTypeId(doc), candidate.Room.Level.Id);
            plan.CropBoxActive = true;
            plan.ViewTemplateId = ElementId.InvalidElementId;
            plan.Scale = this.Scale;
            BoundingBoxXYZ originalBoundingBox = candidate.Room.get_BoundingBox(plan);

            // Put them on sheets
            plan.CropBox = CreateOffsetBoundingBox(200, originalBoundingBox);
            plan.Name = candidate.DestinationViewName;

            // Shrink the bounding box now that it is placed
            Viewport vp = Viewport.Create(this.doc, sheet.Id, plan.Id, sheetCentre);

            // Shrink the bounding box now that it is placed
            plan.CropBox = originalBoundingBox;

            // FIXME - To set an empty view title - so far this seems to work with the standard revit template...
            vp.ChangeTypeId(vp.GetValidTypes().Last());
        }

        private static ElementId GetFloorPlanViewFamilyTypeId(Document doc)
        {
            foreach (ViewFamilyType vft in new FilteredElementCollector(doc).OfClass(typeof(ViewFamilyType))) {
                if (vft.ViewFamily == ViewFamily.FloorPlan) {
                    return vft.Id;
                }
            }
            return null;
        }

        private static XYZ CentreOfSheet(ViewSheet sheet, Document doc)
        {
            FilteredElementCollector c = new FilteredElementCollector(doc, sheet.Id);
            c.OfCategory(BuiltInCategory.OST_TitleBlocks);
            foreach (Element e in c) {
                BoundingBoxXYZ b = e.get_BoundingBox(sheet);
                double x = b.Min.X + ((b.Max.X - b.Min.X) / 2);
                double y = b.Min.Y + ((b.Max.Y - b.Min.Y) / 2);
                return new XYZ(x, y, 0);
            }
			return new XYZ(0, 0, 0);
        }

        private static BoundingBoxXYZ CreateOffsetBoundingBox(double offset, BoundingBoxXYZ origBox)
        {
            XYZ min = new XYZ(origBox.Min.X - offset, origBox.Min.Y - offset, origBox.Min.Z);
            XYZ max = new XYZ(origBox.Max.X + offset, origBox.Max.Y + offset, origBox.Max.Z);
            BoundingBoxXYZ result = new BoundingBoxXYZ();
            result.Min = min;
            result.Max = max;
            return result;
        }
    }
}
/* vim: set ts=4 sw=4 nu expandtab: */
