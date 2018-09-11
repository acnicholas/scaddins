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
    using System;
    using System.Reflection;
    using Autodesk.Revit.DB;

    public static class JtFamilyParameterExtensionMethods
    {
        public static bool IsShared(
          this FamilyParameter familyParameter)
        {
            MethodInfo mi = familyParameter
              .GetType()
              .GetMethod("getParameter", BindingFlags.Instance | BindingFlags.NonPublic);

            if (mi == null) {
                throw new InvalidOperationException("Could not find getParameter method");
            }

            var parameter = mi.Invoke(familyParameter, new object[] { }) as Parameter;
            return parameter.IsShared;
        }
    }
}