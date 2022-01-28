namespace SCaddins.ViewUtilities
{
    using System.Collections.Generic;
    using System.Linq;
    using Autodesk.Revit.DB;
    using SCaddins.ExportManager;

    public class ViewAlignmentUtils
    {
        public static void AlignViews(Document doc, List<ExportSheet> selectedSheets, ExportSheet selectedSheet)
        {
            var viewPorts = selectedSheet.Sheet.GetAllViewports();
            if (viewPorts.Count != 1)
            {
                return;
            }

            /// Save template view crops

            var firstViewportId = viewPorts.First();
            var firstViewport = doc.GetElement(firstViewportId) as Autodesk.Revit.DB.Viewport;
            var templateView = doc.GetElement(firstViewport.ViewId) as Autodesk.Revit.DB.View;
            bool templateViewCropEnabled = templateView.CropBox.Enabled;

            if (!templateViewCropEnabled)
            {
                templateView.CropBox.Enabled = true;
                templateView.CropBox = templateView.get_BoundingBox(templateView);
                doc.Regenerate();
            }

            var templateViewOriginalBoxOutline = firstViewport.GetBoxOutline();
            var templateViewOriginalCenter = firstViewport.GetBoxCenter();
            var templateViewOriginaCrop = templateView.CropBox;

            var templateViewScopeBoxParam = templateView.LookupParameter("Scope Box");
            var templateViewScopeBoxID = templateViewScopeBoxParam.AsElementId();
            bool templateViewHasScopeBox = templateViewScopeBoxID != ElementId.InvalidElementId;

            if (!templateViewCropEnabled)
            {
                templateView.CropBox.Enabled = false;
            }

            using (var transaction = new Transaction(doc))
            {
                if (transaction.Start("Align Views") == TransactionStatus.Started)
                {
                    foreach (var sheet in selectedSheets)
                    {
                        // Don't align template sheet views
                        if (sheet != selectedSheet)
                        {
                            var vps = sheet.Sheet.GetAllViewports();
                            if (vps.Count == 1)
                            {
                                var destViewport = doc.GetElement(vps.First()) as Autodesk.Revit.DB.Viewport;
                                var destView = doc.GetElement(destViewport.ViewId) as Autodesk.Revit.DB.View;
                                var desteViewScopeBoxParam = destView.LookupParameter("Scope Box");
                                var destViewScopeBoxID = desteViewScopeBoxParam.AsElementId();
                                bool destViewHasScopeBox = destViewScopeBoxID != ElementId.InvalidElementId;
                                var ob = destViewport.GetBoxOutline();

                                // try to remove the scope box temporarily
                                if (!desteViewScopeBoxParam.IsReadOnly)
                                {
                                    desteViewScopeBoxParam.Set(ElementId.InvalidElementId);
                                }

                                // set the crop to match the template view
                                bool destViewHasCropShape = false;
                                IList<CurveLoop> oldShape = null;
                                ViewCropRegionShapeManager destViewCropManager = null;
                                if (destView.ViewType != ViewType.Elevation && destView.ViewType != ViewType.Section)
                                {
                                    destViewCropManager = destView.GetCropRegionShapeManager();
                                    destViewHasCropShape = destViewCropManager.ShapeSet;
                                    oldShape = destViewCropManager.GetCropShape();
                                    destViewCropManager.RemoveCropRegionShape();
                                }

                                if (destView.ViewType == ViewType.Elevation || destView.ViewType == ViewType.Section)
                                {
                                    var newCrop = new BoundingBoxXYZ();
                                    newCrop.Min = new XYZ(destView.CropBox.Min.X, destView.CropBox.Min.Y, templateView.CropBox.Min.Z);
                                    newCrop.Max = new XYZ(destView.CropBox.Max.X, destView.CropBox.Max.Y, templateView.CropBox.Max.Z);
                                    destView.CropBox = newCrop;
                                    destView.CropBox.Enabled = true;
                                }
                                else
                                {
                                    destView.CropBox = templateViewOriginaCrop;
                                    destView.CropBox.Enabled = true;
                                }

                                // regenerate just in case
                                doc.Regenerate();

                                var destViewportBoxOutline = destViewport.GetBoxOutline();
                                var destViewportOriginalBoxOutline = destViewport.GetBoxOutline();

                                if (destView.ViewType == ViewType.Elevation || destView.ViewType == ViewType.Section)
                                {
                                    var min = templateViewOriginalBoxOutline.MinimumPoint;
                                    var max = templateViewOriginalBoxOutline.MaximumPoint;
                                    var newMin = new XYZ(
                                        destViewportBoxOutline.MinimumPoint.X,
                                        min.Y,
                                        destViewportBoxOutline.MinimumPoint.Z);
                                    var newMax = new XYZ(
                                        destViewportBoxOutline.MaximumPoint.X,
                                        max.Y,
                                        destViewportBoxOutline.MaximumPoint.Z);
                                    Outline ol = new Outline(newMin, newMax);
                                    destViewportBoxOutline = ol;
                                }
                                else
                                {
                                    destViewportBoxOutline = templateViewOriginalBoxOutline;
                                }

                                if (destView.ViewType == ViewType.Elevation || destView.ViewType == ViewType.Section)
                                {
                                    var newCenter = new XYZ(
                                        destViewport.GetBoxCenter().X,
                                        templateViewOriginalCenter.Y,
                                        destViewport.GetBoxCenter().Z);
                                    destViewport.SetBoxCenter(newCenter);
                                }
                                else
                                {
                                    destViewport.SetBoxCenter(templateViewOriginalCenter);
                                }

                                // reset crop and scope
                                if (!desteViewScopeBoxParam.IsReadOnly)
                                {
                                    desteViewScopeBoxParam.Set(destViewScopeBoxID);
                                }

                                if (destView.ViewType != ViewType.Elevation && destView.ViewType != ViewType.Section)
                                {
                                    destViewportBoxOutline = destViewportOriginalBoxOutline;
                                    if (destViewHasCropShape)
                                    {
                                        destViewCropManager.SetCropShape(oldShape.First());
                                    }
                                }
                            }
                        }
                    }
                    transaction.Commit();
                }
                else
                {
                    transaction.RollBack();
                }
            }
        }
    }
}
