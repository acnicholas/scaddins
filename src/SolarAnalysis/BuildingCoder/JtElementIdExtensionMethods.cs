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
    using Autodesk.Revit.DB;

    public static class JtElementIdExtensionMethods
    {
        /// <summary>
        /// Predicate returning true for invalid element ids.
        /// </summary>
        public static bool IsInvalid(this ElementId id)
        {
            return ElementId.InvalidElementId == id;
        }

        /// <summary>
        /// Predicate returning true for valid element ids.
        /// </summary>
        public static bool IsValid(this ElementId id)
        {
            return !IsInvalid(id);
        }
    }
}