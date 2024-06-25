// (C) Copyright 2016-2021 by Andrew Nicholas
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

namespace SCaddins.RoomConverter
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Autodesk.Revit.DB;
    using Autodesk.Revit.DB.Architecture;

    public class RoomConversionManager
    {
        #region PROPERTIES

        private List<RoomConversionCandidate> allCandidates;

        private Dictionary<string, string> departmentsInModel;

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Microsoft.Usage", "CA2213: Disposable fields should be disposed", Justification = "Parameter initialized by Revit", MessageId = "doc")]
        private Document doc;

        private Dictionary<string, View> existingSheets =
                                    new Dictionary<string, View>();

        private Dictionary<string, View> existingViews =
            new Dictionary<string, View>();

        private Dictionary<string, ElementId> titleBlocks =
            new Dictionary<string, ElementId>();

        private Dictionary<string, ElementId> viewTemplates =
            new Dictionary<string, ElementId>();

        private Dictionary<string, ElementId> areaPlanTypes =
            new Dictionary<string, ElementId>();

        public RoomConversionManager(Document doc)
        {
            departmentsInModel = new Dictionary<string, string>();
            allCandidates = new List<RoomConversionCandidate>();
            this.doc = doc;
            titleBlocks = GetAllTitleBlockTypes(this.doc);
            TitleBlockId = ElementId.InvalidElementId;
            viewTemplates = GetViewTemplates(this.doc);
            ViewTemplateId = ElementId.InvalidElementId;
            areaPlanTypes = GetAreaPlanTypes(this.doc);
            AreaPlanTypeId = areaPlanTypes.First().Value;
            Scale = 50;
            CropRegionEdgeOffset = 300;
            SheetCopier.SheetCopierManager.GetAllSheets(existingSheets, this.doc);
            SheetCopier.SheetCopierManager.GetAllViewsInModel(existingViews, this.doc);
            CreatePlan = true;
            CreateRCP = false;
            using (var collector = new FilteredElementCollector(this.doc))
            {
                collector.OfClass(typeof(SpatialElement));
                foreach (Element e in collector)
                {
                    if (e.IsValidObject && (e is Room))
                    {
                        Room room = e as Room;
                        if (room.Area > 0 && room.Location != null)
                        {
                            allCandidates.Add(new RoomConversionCandidate(room, existingSheets, existingViews));
                            Parameter p = room.LookupParameter("Department");
                            if (p != null && p.HasValue)
                            {
                                string depo = p.AsString().Trim();
                                if (departmentsInModel.Count > 0)
                                {
                                    if (!string.IsNullOrEmpty(depo) && !departmentsInModel.ContainsKey(depo))
                                    {
                                        departmentsInModel.Add(depo, depo);
                                    }
                                }
                                else
                                {
                                    if (!string.IsNullOrEmpty(depo))
                                    {
                                        departmentsInModel.Add(depo, depo);
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }

        public List<RoomConversionCandidate> Candidates
        {
            get { return allCandidates; }
        }

        public bool CreatePlan
        {
            get; set;
        }

        public bool CreateAreaPlan
        {
            get; set;
        }

        public bool CreateRCP
        {
            get; set;
        }

        public int CropRegionEdgeOffset
        {
            get; set;
        }

        public Document Doc
        {
            get { return doc; }
        }

        public int Scale
        {
            get; set;
        }

        public ElementId TitleBlockId
        {
            get; set;
        }

        public ElementId ViewTemplateId
        {
            get; set;
        }

        public ElementId AreaPlanTypeId
        {
            get; set;
        }

        public Dictionary<string, ElementId> TitleBlocks => titleBlocks;

        public Dictionary<string, ElementId> ViewTemplates => viewTemplates;

        public Dictionary<string, ElementId> AreaPlanTypes => areaPlanTypes;

        #endregion

        public static Dictionary<string, ElementId> GetAllTitleBlockTypes(Document doc)
        {
            var result = new Dictionary<string, ElementId>();

            using (var collector = new FilteredElementCollector(doc))
            {
                collector.OfCategory(BuiltInCategory.OST_TitleBlocks).OfClass(typeof(FamilySymbol));
                foreach (var element in collector)
                {
                    var e = (FamilySymbol)element;
                    var s = e.Family.Name + "-" + e.Name;
                    if (!result.ContainsKey(s))
                    {
                        result.Add(s, e.Id);
                    }
                }
            }

            // Add an empty title in case there's none
            result.Add("none", ElementId.InvalidElementId);

            return result;
        }

        public void CreateRoomMasses(List<RoomConversionCandidate> rooms)
        {
            var errCount = 0;
            var basicMasses = 0;
            var roomCount = 0;
            if (rooms != null)
            {
                using (var t = new Transaction(doc, "Rooms to Masses"))
                {
                    t.Start();
                    foreach (RoomConversionCandidate c in rooms)
                    {
                        roomCount++;
                        if (CreateRoomMass(c.Room))
                        {
                            continue;
                        }
                        if (CreateSimpleRoomMassByExtrusion(c.Room))
                        {
                            continue;
                        }
                        basicMasses++;
                        if (!CreateRoomMassByBoundingBox(c.Room))
                        {
                            errCount++;
                        }
                    }
                    t.Commit();
                }
            }
            var msg = @"Summary:" + Environment.NewLine +
            @"-   " + (roomCount - errCount) + " Room masses created" + Environment.NewLine +
            @"-   " + basicMasses + " Modeled with basic bounding geometry" + Environment.NewLine +
            @"-   " + errCount + " Errors" + Environment.NewLine +
            Environment.NewLine +
            "Check Revit room geometry if basic masses are bing created";

            SCaddinsApp.WindowManager.ShowMessageBox("Rooms to Masses - Summary", msg);
        }

        public void CreateViewsAndSheets(List<RoomConversionCandidate> rooms)
        {
            if (rooms == null)
            {
                SCaddinsApp.WindowManager.ShowMessageBox("WARNING", "no rooms selected to convert");
                return;
            }
            using (var t = new Transaction(doc, "Rooms to Views"))
            {
                if (t.Start() != TransactionStatus.Started)
                {
                    return;
                }
                foreach (var c in rooms)
                {
                    CreateViewAndSheet(c);
                }
                t.Commit();
            }
        }

        public List<Parameter> GetRoomParameters()
        {
            var s = new List<Parameter>();
            foreach (Parameter p in Candidates[0].Room.Parameters)
            {
                // don't add ElementID values yet (too much effort)
                if (p.StorageType != StorageType.ElementId && p.StorageType != StorageType.None)
                {
                    s.Add(p);
                }
            }
            return s;
        }

        public ElementId GetTitleBlockByName(string titleBlockName)
        {
            if (titleBlockName == null)
            {
                return ElementId.InvalidElementId;
            }
            var id = ElementId.InvalidElementId;
            var titleFound = titleBlocks.TryGetValue(titleBlockName, out id);
            return titleFound ? id : ElementId.InvalidElementId;
        }

        public void SynchronizeMassesToRooms()
        {
            using (var collector = new FilteredElementCollector(doc))
            using (var t = new Transaction(doc, "Synchronize Masses to Rooms"))
            {
                collector.OfCategory(BuiltInCategory.OST_Mass);
                collector.OfClass(typeof(DirectShape));
                t.Start();
                var i = 0;
                foreach (var e in collector)
                {
                    var p = e.LookupParameter("RoomId");
                    if (p == null)
                    {
                        continue;
                    }
                    i++;
                    var intId = p.AsInteger();
                    if (intId <= 0)
                    {
                        continue;
                    }
#if REVIT2024 || REVIT2025
                    var id = new ElementId((long)intId);
#else
                    var id = new ElementId(intId);
#endif
                    var room = doc.GetElement(id);
                    if (room != null)
                    {
                        CopyAllMassParametersToRooms(e, (Room)room);
                    }
                }

                SCaddinsApp.WindowManager.ShowMessageBox("Synchronize Masses to Rooms", i + " masses synchronized");
                t.Commit();
            }
        }

        internal static List<string> GetAllDesignOptionNames(Document doc)
        {
            var result = new List<string>();
            var optIds = new List<ElementId>();
            foreach (var element in new FilteredElementCollector(doc).OfCategory(BuiltInCategory.OST_DesignOptions))
            {
                var dopt = (DesignOption)element;
                ElementId optId = dopt.Id;
                if (!optIds.Contains(optId))
                {
                    optIds.Add(optId);
                }
            }

            result.Add("Main Model");

            foreach (ElementId id in optIds)
            {
                var e = doc.GetElement(id);
                var s = doc.GetElement(e.get_Parameter(BuiltInParameter.OPTION_SET_ID).AsElementId()).Name;
                result.Add(s + @" : " + e.Name.Replace(@"(primary)", string.Empty).Trim());
            }

            return result.OrderBy(q => q).ToList();
        }

        internal List<string> GetAllDepartments()
        {
            var result = new List<string>();
            foreach (var s in departmentsInModel.Values)
            {
                result.Add(s);
            }
            return result;
        }

        private static Dictionary<string, ElementId> GetViewTemplates(Document doc)
        {
            var result = new Dictionary<string, ElementId>();
            using (var c = new FilteredElementCollector(doc))
            {
                c.OfCategory(BuiltInCategory.OST_Views);
                foreach (var element in c)
                {
                    var view = (View)element;
                    if (view.IsTemplate)
                    {
                        result.Add(view.Name, view.Id);
                    }
                }
            }
            return result;
        }

        private static Dictionary<string, ElementId> GetAreaPlanTypes(Document doc)
        {
            var result = new Dictionary<string, ElementId>();
            using (var collector = new FilteredElementCollector(doc))
            {
                foreach (var element in collector.OfClass(typeof(AreaScheme)))
                {
                    var vft = (AreaScheme)element;
                    if (!result.ContainsKey(vft.Name))
                    {
                        result.Add(vft.Name, vft.Id);
                    }
                }
            }
            return result;
        }

        private static XYZ CentreOfSheet(ViewSheet sheet, Document doc)
        {
            FilteredElementCollector c = new FilteredElementCollector(doc, sheet.Id);
            c.OfCategory(BuiltInCategory.OST_TitleBlocks);
            foreach (Element e in c)
            {
                BoundingBoxXYZ b = e.get_BoundingBox(sheet);
                double x = b.Min.X + ((b.Max.X - b.Min.X) / 2);
                double y = b.Min.Y + ((b.Max.Y - b.Min.Y) / 2);
                return new XYZ(x, y, 0);
            }
            return new XYZ(0, 0, 0);
        }

        private static void CopyAllMassParametersToRooms(Element host, Room dest)
        {
            Parameter name = host.LookupParameter("Name");
            if (name != null && name.StorageType == StorageType.String)
            {
                dest.Name = name.AsString();
            }

            Parameter number = host.LookupParameter("Number");
            if (number != null && number.StorageType == StorageType.String)
            {
                dest.Number = number.AsString();
            }

            CopyAllParameters(host, dest);
        }

        private static void CopyAllParameters(Element host, Element dest)
        {
            if (!ValidElements(host, dest))
            {
                return;
            }

            foreach (Parameter param in host.Parameters)
            {
                if (!param.HasValue || param == null)
                {
                    continue;
                }

                Parameter paramDest = dest.LookupParameter(param.Definition.Name);
                if (paramDest != null && paramDest.UserModifiable && paramDest.StorageType == param.StorageType)
                {
                    switch (param.StorageType)
                    {
                        case StorageType.String:
                            if (!paramDest.IsReadOnly && paramDest.UserModifiable)
                            {
                                string v = param.AsString();
                                paramDest.Set(v);
                            }
                            break;

                        case StorageType.Integer:
                            int b = param.AsInteger();
                            if (b != -1 && !paramDest.IsReadOnly)
                            {
                                paramDest.Set(b);
                            }
                            break;

                        case StorageType.Double:
                            double d = param.AsDouble();
                            if (!paramDest.IsReadOnly)
                            {
                                paramDest.Set(d);
                            }
                            break;
                    }
                }
            }
        }

        private static void CopyAllRoomParametersToMasses(Element host, Element dest)
        {
            Parameter paramRoomId = dest.LookupParameter("RoomId");
            if (paramRoomId != null && paramRoomId.StorageType == StorageType.Integer)
            {
#if REVIT2024 || REVIT2025
                paramRoomId.Set(host.Id.Value);
#else
                paramRoomId.Set(host.Id.IntegerValue);
#endif
            }

            CopyAllParameters(host, dest);
        }

        private static BoundingBoxXYZ CreateOffsetBoundingBox(double offset, BoundingBoxXYZ origBox)
        {
            var offsetInFeet = Common.MiscUtilities.MillimetersToFeet(offset);
            var min = new XYZ(origBox.Min.X - offsetInFeet, origBox.Min.Y - offsetInFeet, origBox.Min.Z);
            var max = new XYZ(origBox.Max.X + offsetInFeet, origBox.Max.Y + offsetInFeet, origBox.Max.Z);
            var result = new BoundingBoxXYZ { Min = min, Max = max };
            return result;
        }

        private static ElementId GetFloorPlanViewFamilyTypeId(Document doc)
        {
            using (var collector = new FilteredElementCollector(doc))
            {
                foreach (var element in collector.OfClass(typeof(ViewFamilyType)))
                {
                    var vft = (ViewFamilyType)element;
                    if (vft.ViewFamily == ViewFamily.FloorPlan)
                    {
                        return vft.Id;
                    }
                }
                return null;
            }
        }

        private static ElementId GetRCPViewFamilyTypeId(Document doc)
        {
            using (var collector = new FilteredElementCollector(doc))
            {
                foreach (var element in collector.OfClass(typeof(ViewFamilyType)))
                {
                    var vft = (ViewFamilyType)element;
                    if (vft.ViewFamily == ViewFamily.CeilingPlan)
                    {
                        return vft.Id;
                    }
                }
                return null;
            }
        }

        private static bool ValidElements(Element host, Element dest)
        {
            if (host == null || dest == null)
            {
                return false;
            }

            if (!host.IsValidObject || !dest.IsValidObject)
            {
                return false;
            }
            return true;
        }

        private bool CreateSimpleRoomMassByExtrusion(Room room)
        {
            try
            {
                var height = room.LookupParameter("Limit Offset");
                var curves = new List<CurveLoop>();
                var spatialBoundaryOptions = new SpatialElementBoundaryOptions { StoreFreeBoundaryFaces = true };
                var loop = new CurveLoop();
                var boundarySegments = room.GetBoundarySegments(spatialBoundaryOptions);
                var biggestList = boundarySegments.OrderByDescending(item => item.Count).First();

                foreach (var seg in biggestList)
                {
                    loop.Append(seg.GetCurve());
                }

                curves.Add(loop);

                var options = new SolidOptions(ElementId.InvalidElementId, ElementId.InvalidElementId);
                var roomSolid = GeometryCreationUtilities.CreateExtrusionGeometry(curves, new XYZ(0, 0, 1), height.AsDouble(), options);

                if (roomSolid == null)
                {
                    return false;
                }

                var eid = new ElementId(BuiltInCategory.OST_Mass);
                DirectShape roomShape = DirectShape.CreateElement(doc, eid);
                roomShape.SetShape(new GeometryObject[] { roomSolid });
                CopyAllRoomParametersToMasses(room, roomShape);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(ex.Message);
                return false;
            }
            return true;
        }

        private bool CreateRoomMassByBoundingBox(Room room)
        {
            try
            {
                var bb = room.get_BoundingBox(null);
                var eid = new ElementId(BuiltInCategory.OST_Mass);

                if (bb == null)
                {
                    return false;
                }

                DirectShape roomShape = DirectShape.CreateElement(doc, eid);

                var curves = new List<Curve>();

                var bl = new XYZ(bb.Min.X, bb.Min.Y, bb.Min.Z);
                var br = new XYZ(bb.Max.X, bb.Min.Y, bb.Min.Z);
                var tr = new XYZ(bb.Max.X, bb.Max.Y, bb.Min.Z);
                var tl = new XYZ(bb.Min.X, bb.Max.Y, bb.Min.Z);

                var height = bb.Max.Z - bb.Min.Z;

                curves.Add(Line.CreateBound(bl, br));
                curves.Add(Line.CreateBound(br, tr));
                curves.Add(Line.CreateBound(tr, tl));
                curves.Add(Line.CreateBound(tl, bl));
                var loop = CurveLoop.Create(curves);
                var options = new SolidOptions(ElementId.InvalidElementId, ElementId.InvalidElementId);
                var roomSolid = GeometryCreationUtilities.CreateExtrusionGeometry(new[] { loop }, new XYZ(0, 0, 1), height, options);

                if (roomSolid != null)
                {
                    var geomObj = new GeometryObject[] { roomSolid };
                    if (geomObj.Length > 0)
                    {
                        roomShape.SetShape(geomObj);
                        CopyAllRoomParametersToMasses(room, roomShape);
                        return true;
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(ex.Message);
            }
            return false;
        }

        private bool CreateRoomMass(SpatialElement room)
        {
            if (!SpatialElementGeometryCalculator.CanCalculateGeometry(room))
            {
                return false;
            }
            try
            {
                SpatialElementGeometryResults results;
                using (var calculator = new SpatialElementGeometryCalculator(doc))
                {
                    results = calculator.CalculateSpatialElementGeometry(room);
                }
                using (Solid roomSolid = results.GetGeometry())
                {
                    var eid = new ElementId(BuiltInCategory.OST_Mass);
                    DirectShape roomShape = DirectShape.CreateElement(doc, eid);
                    if (roomShape != null && roomSolid.Volume > 0 && roomSolid.Faces.Size > 0)
                    {
                        var geomObj = new GeometryObject[] { roomSolid };
                        if (geomObj.Length > 0)
                        {
                            roomShape.SetShape(geomObj);
                            CopyAllRoomParametersToMasses(room, roomShape);
                            return true;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(ex.Message);
            }
            return false;
        }

        ////[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Reliability", "CA2000:Dispose objects before losing scope")]
        private void CreateViewAndSheet(RoomConversionCandidate candidate)
        {
            // Create Sheet
            var sheet = ViewSheet.Create(doc, TitleBlockId);
            sheet.Name = candidate.DestinationSheetName;
            sheet.SheetNumber = candidate.DestinationSheetNumber;

            // Get Centre before placing any views
            var sheetCentre = CentreOfSheet(sheet, doc);

            if (CreatePlan)
            {
                // Create plan of room
                var plan = ViewPlan.Create(doc, GetFloorPlanViewFamilyTypeId(doc), candidate.Room.Level.Id);
                plan.CropBoxActive = true;
                plan.ViewTemplateId = ElementId.InvalidElementId;
                plan.Scale = Scale;
                var originalBoundingBox = candidate.Room.get_BoundingBox(plan);

                // Put them on sheets
                plan.CropBox = CreateOffsetBoundingBox(50000, originalBoundingBox);
                plan.Name = candidate.DestinationViewName;

                // Shrink the bounding box now that it is placed
                var vp = Viewport.Create(doc, sheet.Id, plan.Id, sheetCentre);

                // Shrink the bounding box now that it is placed
                plan.CropBox = CreateOffsetBoundingBox(CropRegionEdgeOffset, originalBoundingBox);

                // FIXME - To set an empty view title - so far this seems to work with the standard revit template...
                vp.ChangeTypeId(vp.GetValidTypes().Last());

                // FIXME Apply a view template
                // NOTE This could cause trouble with view scales
                plan.ViewTemplateId = ViewTemplateId;
            }

            if (CreateRCP)
            {
                // Create rcp of room
                var rcp = ViewPlan.Create(doc, GetRCPViewFamilyTypeId(doc), candidate.Room.Level.Id);
                rcp.CropBoxActive = true;
                rcp.ViewTemplateId = ElementId.InvalidElementId;
                rcp.Scale = Scale;
                var originalBoundingBox = candidate.Room.get_BoundingBox(rcp);

                // Put them on sheets
                rcp.CropBox = CreateOffsetBoundingBox(50000, originalBoundingBox);
                rcp.Name = candidate.DestinationViewName;

                // Shrink the bounding box now that it is placed
                var vp = Viewport.Create(doc, sheet.Id, rcp.Id, sheetCentre);

                // Shrink the bounding box now that it is placed
                rcp.CropBox = CreateOffsetBoundingBox(CropRegionEdgeOffset, originalBoundingBox);

                // FIXME - To set an empty view title - so far this seems to work with the standard revit template...
                vp.ChangeTypeId(vp.GetValidTypes().Last());

                // FIXME Apply a view template
                // NOTE This could cause trouble with view scales
                rcp.ViewTemplateId = ViewTemplateId;
            }

            if (CreateAreaPlan)
            {
                // Create plan of room
                var plan = ViewPlan.CreateAreaPlan(doc, AreaPlanTypeId, candidate.Room.Level.Id);
                //// var plan = ViewPlan.Create(doc, GetAreaPlanViewFamilyTypeId(doc), candidate.Room.Level.Id);
                plan.CropBoxActive = true;
                plan.ViewTemplateId = ElementId.InvalidElementId;
                plan.Scale = Scale;
                var originalBoundingBox = candidate.Room.get_BoundingBox(plan);

                // Put them on sheets
                plan.CropBox = CreateOffsetBoundingBox(50000, originalBoundingBox);
                plan.Name = candidate.DestinationViewName;

                // Shrink the bounding box now that it is placed
                var vp = Viewport.Create(doc, sheet.Id, plan.Id, sheetCentre);

                // Shrink the bounding box now that it is placed
                plan.CropBox = CreateOffsetBoundingBox(CropRegionEdgeOffset, originalBoundingBox);

                // FIXME - To set an empty view title - so far this seems to work with the standard revit template...
                vp.ChangeTypeId(vp.GetValidTypes().Last());

                // FIXME Apply a view template
                // NOTE This could cause trouble with view scales
                plan.ViewTemplateId = ViewTemplateId;
                SCaddinsApp.WindowManager.ShowMessageBox("test");
            }
        }
    }
}

/* vim: set ts=4 sw=4 nu expandtab: */
