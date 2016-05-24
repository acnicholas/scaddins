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

namespace SCaddins.SCasfar
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Autodesk.Revit.Attributes;
    using Autodesk.Revit.DB;
    using Autodesk.Revit.UI;

    [Transaction(Autodesk.Revit.Attributes.TransactionMode.Manual)]
    [Regeneration(Autodesk.Revit.Attributes.RegenerationOption.Manual)]
    [Journaling(Autodesk.Revit.Attributes.JournalingMode.NoCommandData)]

 public class Command : IExternalCommand
    {
        public Autodesk.Revit.UI.Result Execute(
            ExternalCommandData commandData,
            ref string message,
            Autodesk.Revit.DB.ElementSet elements)
        {
            Document doc = commandData.Application.ActiveUIDocument.Document;
            //View currentView = doc.ActiveView;
            
            SCasfarForm form = new SCasfarForm(doc);
            form.ShowDialog();
            return Autodesk.Revit.UI.Result.Succeeded;
            
            FilteredElementCollector collector = new FilteredElementCollector(doc);
            collector.OfClass(typeof(SpatialElement));
            foreach (Element e in collector) {

                //if (!isUnit(e)) {
                //    continue;
                //}

                SpatialElement room = e as SpatialElement;
                
                Transaction t = new Transaction(doc, "Rooms to Views");
                t.Start();
                
                //create plans
                ViewPlan plan = ViewPlan.Create(doc,GetFloorPlanViewFamilyTypeId(doc), room.Level.Id);
                BoundingBoxXYZ boundingBox = room.get_BoundingBox(plan);
                
                //set a large bounding box to stop annotations from interfering with placement
                plan.CropBox = CreateOffsetBoundingBox(200, boundingBox);;
                plan.CropBoxActive = true;
                plan.Name = GetViewNameFromRoomNumber(room, doc);
                plan.Scale = 20;
                
                //put them on sheets
                ViewSheet sheet = ViewSheet.Create(doc, GetFirstTitleBlock(doc));
                sheet.Name = GetSheetNameFromRoomNumber(room, doc);
                
                Viewport vp = Viewport.Create(doc, sheet.Id, plan.Id, new XYZ(
                    SCaddins.Common.MiscUtilities.MMtoFeet(840/2),
                    SCaddins.Common.MiscUtilities.MMtoFeet(594/2),
                    0));
                
                //shrink the bounding box now that it is placed
                plan.CropBox = CreateOffsetBoundingBox(2, boundingBox);;
                
                //FIXME - To set an empty vie title 0this seems to work with the standard revit template...
                vp.ChangeTypeId(vp.GetValidTypes().Last());
  
                t.Commit();
            }
            

            return Autodesk.Revit.UI.Result.Succeeded;
     }
     
     //FIXME use this in SCopy.
     private ElementId GetFloorPlanViewFamilyTypeId(Document doc)
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
         return new XYZ (x, y, 0);
     }
     
     private bool isUnit(Element room)
     {
         var depo = room.LookupParameter("Department");
         if (depo == null) return false;
         return depo.AsString() == "1. Unit";
     }
          
     private ElementId GetFirstTitleBlock(Document doc)
     {
         //TODO this is a super hack...
         foreach (Element e in new FilteredElementCollector(doc).OfCategory(BuiltInCategory.OST_TitleBlocks)) {
             return e.Id;
         }
         return ElementId.InvalidElementId;
     }
     
     private string GetViewNameFromRoomNumber(SpatialElement room, Document doc)
     {
         string request = room.Number + " - " + room.Name;
         //FIXME move this to somewhere nices than SCaos.Command
         return SCaddins.SCaos.Command.GetNiceViewName(doc, request);
     }
     
     private string GetSheetNameFromRoomNumber(SpatialElement room, Document doc)
     {
         string request = "MK" + room.Number;
         //FIXME move this to somewhere nices than SCaos.Command
         return SCaddins.SCaos.Command.GetNiceViewName(doc, request);
     }
         
     private BoundingBoxXYZ CreateOffsetBoundingBox(double offset, BoundingBoxXYZ origBox)
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
