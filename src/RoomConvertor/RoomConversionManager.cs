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
        private ElementId activeTitleBlock;

        public SCaddins.Common.SortableBindingListCollection<RoomConversionCandidate> Candidates {
            get;
            set;
        }

        public Document Doc {
            get{ return doc; }
        }

        public Dictionary<string, ElementId> TitleBlocks
        {
            get{ return titleBlocks; }
        }

        public ElementId ActiveTitleBlock
        {
            get {
                return activeTitleBlock != null ? activeTitleBlock : ElementId.InvalidElementId;
            }
            set {
                activeTitleBlock = value != null ? value : ElementId.InvalidElementId;
            }
        }

        public RoomConversionManager(Document doc)
        {
            Candidates = new SCaddins.Common.SortableBindingListCollection<RoomConversionCandidate>();
            this.allCandidates = new SCaddins.Common.SortableBindingListCollection<RoomConversionCandidate>();
            this.doc = doc;
            this.titleBlocks = GetAllTitleBlockTypes(this.doc);
            this.activeTitleBlock = this.titleBlocks.FirstOrDefault().Value;
            SheetCopier.SheetCopierManager.GetAllSheets(existingSheets, this.doc);
            SheetCopier.SheetCopierManager.GetAllViewsInModel(existingViews, this.doc);
            FilteredElementCollector collector = new FilteredElementCollector(this.doc);
            collector.OfClass(typeof(SpatialElement));
            foreach (Element e in collector) {
                if (e.IsValidObject && (e is Room)) {
                    Room room = e as Room;
                    if (room.Area > 0 && room.Location != null) {
                        allCandidates.Add(new RoomConversionCandidate(room, doc, existingSheets, existingViews));
                    }
                }
            }
            this.Reset();
        }

        public void CreateViewsAndSheets(System.ComponentModel.BindingList<RoomConversionCandidate> candidates)
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
                    result.Add(s, e.GetTypeId());
                }
            }

            //Add an empty title in case there's none
            result.Add("none",ElementId.InvalidElementId);

            return result;
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
             
        private void CopyAllMassParametersToRooms(Element host, Room  dest)
        {
            #if REVIT2014
            return;
            #else
            Parameter name = host.LookupParameter("Name");
            if (name != null &&  name.StorageType == StorageType.String){
                dest.Name = name.AsString();
            }
                        
            Parameter number = host.LookupParameter("Number");
            if (number != null &&  number.StorageType == StorageType.String){
                dest.Number = number.AsString();
            }
            
            CopyAllParameters(host, dest);
            #endif
        }
        
        private void CopyAllRoomParametersToMasses(Element host, Element  dest)
        {
            #if REVIT2014
            return;
            #else
            Parameter paramRoomId = dest.LookupParameter("RoomId");
            if (paramRoomId != null &&  paramRoomId.StorageType == StorageType.Integer){
                paramRoomId.Set(host.Id.IntegerValue);
            }
            
            CopyAllParameters(host, dest);
            #endif
        }
        
        private bool ValidElements(Element host, Element  dest)
        {
            if (host == null || dest == null) return false;
            if (!host.IsValidObject || !dest.IsValidObject) return false;  
            return true;
        }

        private void CopyAllParameters(Element host, Element  dest)
        {
            #if REVIT2014
            return;
            #else
            if(!ValidElements(host, dest)) return;
                        
            foreach (Parameter param in host.Parameters) {
                
                if (!param.HasValue || param == null) {
                    continue;
                }
                              
                Parameter paramDest = dest.LookupParameter(param.Definition.Name);
                if (paramDest != null && paramDest.UserModifiable && paramDest.StorageType == param.StorageType) {
                    switch (param.StorageType) {
                        case StorageType.String:
                            if(!paramDest.IsReadOnly && paramDest.UserModifiable){
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
                            if (d != null && !paramDest.IsReadOnly) {
                                paramDest.Set(d);
                            }
                            break;
                    }
                }
            }
            #endif
        }
        
        public void SynchronizeMassesToRooms()
        {
          #if REVIT2014
            Autodesk.Revit.UI.TaskDialog.Show(
              "Synchronize Masses To Rooms",
              "Synchronizing masses and rooms is not available in Revit 2014"
             );
            return;
          #else 
            
          var t = new Transaction(doc, "Synchronize Masses to Rooms");
          t.Start(); 
          
          Autodesk.Revit.UI.TaskDialog.Show("test", "Synchronizing masses to rooms.");
          
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
          
          Autodesk.Revit.UI.TaskDialog.Show("test", i + " masses synchronized");
          t.Commit();
          
          #endif
          
        }

        private bool CreateRoomMass(Room room)
        {
            #if REVIT2014
            return false;
            #else
            try {
                var height = room.LookupParameter("Limit Offset");
                var curves = new List<CurveLoop>();
                var spatialBoundaryOptions = new SpatialElementBoundaryOptions();
                spatialBoundaryOptions.StoreFreeBoundaryFaces = true;
                var loop = new CurveLoop();
                var bdySegs = room.GetBoundarySegments(spatialBoundaryOptions);
                foreach (var seg in bdySegs[0]) {
                    #if !REVIT2017
                    loop.Append(seg.Curve);
                    #else
                    loop.Append(seg.GetCurve());
                    #endif
                }

                curves.Add(loop);
                SolidOptions options = new SolidOptions(ElementId.InvalidElementId, ElementId.InvalidElementId);
                Solid roomSolid = GeometryCreationUtilities.CreateExtrusionGeometry(curves, new XYZ(0, 0, 1), height.AsDouble(), options);
                DirectShape roomShape = DirectShape.CreateElement(doc, new ElementId(BuiltInCategory.OST_Mass), "A", "B");
                roomShape.SetShape(new GeometryObject[] { roomSolid });
                //Autodesk.Revit.UI.TaskDialog.Show("test", roomShape.GetType().ToString());
                CopyAllRoomParametersToMasses(room, roomShape);

            } catch (Exception ex) {
                return false;
            }
            return true;
            #endif
        }

        private void CreateViewAndSheet(RoomConversionCandidate candidate)
        {
            //Create plans
            ViewPlan plan = ViewPlan.Create(doc, GetFloorPlanViewFamilyTypeId(doc), candidate.Room.Level.Id);
            BoundingBoxXYZ boundingBox = candidate.Room.get_BoundingBox(plan);

            //Set a large bounding box to stop annotations from interfering with placement
            plan.CropBox = CreateOffsetBoundingBox(200, boundingBox);
            plan.CropBoxActive = true;
            plan.Name = candidate.DestinationViewName;
            plan.Scale = 20;

            //Put them on sheets
            ViewSheet sheet = ViewSheet.Create(doc, GetFirstTitleBlock(doc));
            sheet.Name = candidate.DestinationSheetName;
            sheet.SheetNumber = candidate.DestinationSheetNumber;

            Viewport vp = Viewport.Create(this.doc, sheet.Id, plan.Id, new XYZ(
                              SCaddins.Common.MiscUtilities.MMtoFeet(840 / 2),
                              SCaddins.Common.MiscUtilities.MMtoFeet(594 / 2),
                              0));

            //Shrink the bounding box now that it is placed
            plan.CropBox = CreateOffsetBoundingBox(2, boundingBox);

            //FIXME - To set an empty view title - so far this seems to work with the standard revit template...
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

        private XYZ CentreOfSheet(ViewSheet sheet)
        {
            BoundingBoxXYZ b = sheet.get_BoundingBox(sheet);
            double x = b.Min.X + ((b.Max.X - b.Min.X) / 2);
            double y = b.Min.Y + ((b.Max.Y - b.Min.Y) / 2);
            return new XYZ(x, y, 0);
        }

        private static ElementId GetFirstTitleBlock(Document doc)
        {
            //TODO this is a super hack...
            //Add a dialog to choose title
            foreach (Element e in new FilteredElementCollector(doc).OfCategory(BuiltInCategory.OST_TitleBlocks)) {
                if (e.IsValidObject) {
                    return e.Id;
                }
            }
            return ElementId.InvalidElementId;
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
