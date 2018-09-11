//
// Util.cs - The Building Coder Revit API utility methods
//
// Copyright (C) 2008-2018 by Jeremy Tammik,
// Autodesk Inc. All rights reserved.
//
// Keywords: The Building Coder Revit API C# .NET add-in.
//

namespace BuildingCoder
{
    using System.Diagnostics;
    using Autodesk.Revit.DB;

    public static class JtElementExtensionMethods
    {
        /// <summary>
        /// Return the curve from a Revit database Element
        /// location curve, if it has one.
        /// </summary>
        public static Curve GetCurve(this Element e)
        {
            Debug.Assert(e.Location != null, "expected an element with a valid Location");

            LocationCurve lc = e.Location as LocationCurve;

            Debug.Assert(lc != null, "expected an element with a valid LocationCurve");

            return lc.Curve;
        }

        /// <summary>
        /// Predicate to determine whether given element
        /// is a physical element, i.e. valid category,
        /// not view specific, etc.
        /// </summary>
        public static bool IsPhysicalElement(
          this Element e)
        {
            if (e.Category == null) {
                return false;
            }
            //// does this produce same result as
            //// WhereElementIsViewIndependent ?
            if (e.ViewSpecific) {
                return false;
            }
            //// exclude specific unwanted categories
            if (((BuiltInCategory)e.Category.Id.IntegerValue)
              == BuiltInCategory.OST_HVAC_Zones) {
                return false;
            }
            return e.Category.CategoryType == CategoryType.Model
              && e.Category.CanAddSubcategory;
        }
    }
}