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
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Linq;
    using System.Reflection;
    using Autodesk.Revit.ApplicationServices;
    using Autodesk.Revit.DB;
    using Autodesk.Revit.UI;
    using Autodesk.Revit.UI.Selection;
    using WinForms = System.Windows.Forms;

    public class Util
    {
        public const double EPS = 1.0e-9;

        private const string Caption = "The Building Coder";

        private const double CubicFeetToCubicMeters
          = FootToMeter * FootToMeter * FootToMeter;

        private const double FootToMeter = FeetToMm * 0.001;

        private const double FeetToMm = 12 * InchesToMm;

        private const double InchesToMm = 25.4;

        /// <summary>
        /// Minimum slope for a vector to be considered
        /// to be pointing upwards. Slope is simply the
        /// relationship between the vertical and
        /// horizontal components.
        /// </summary>
        private const double MinimumSlope = 0.3;

        /// <summary>
        /// Hard coded abbreviations for the first 26
        /// DisplayUnitType enumeration values.
        /// </summary>
        private static string[] displayUnitTypeAbbreviation
          = new string[] {
      "m", // DUT_METERS = 0,
      "cm", // DUT_CENTIMETERS = 1,
      "mm", // DUT_MILLIMETERS = 2,
      "ft", // DUT_DECIMAL_FEET = 3,
      "N/A", // DUT_FEET_FRACTIONAL_INCHES = 4,
      "N/A", // DUT_FRACTIONAL_INCHES = 5,
      "in", // DUT_DECIMAL_INCHES = 6,
      "ac", // DUT_ACRES = 7,
      "ha", // DUT_HECTARES = 8,
      "N/A", // DUT_METERS_CENTIMETERS = 9,
      "y^3", // DUT_CUBIC_YARDS = 10,
      "ft^2", // DUT_SQUARE_FEET = 11,
      "m^2", // DUT_SQUARE_METERS = 12,
      "ft^3", // DUT_CUBIC_FEET = 13,
      "m^3", // DUT_CUBIC_METERS = 14,
      "deg", // DUT_DECIMAL_DEGREES = 15,
      "N/A", // DUT_DEGREES_AND_MINUTES = 16,
      "N/A", // DUT_GENERAL = 17,
      "N/A", // DUT_FIXED = 18,
      "%", // DUT_PERCENTAGE = 19,
      "in^2", // DUT_SQUARE_INCHES = 20,
      "cm^2", // DUT_SQUARE_CENTIMETERS = 21,
      "mm^2", // DUT_SQUARE_MILLIMETERS = 22,
      "in^3", // DUT_CUBIC_INCHES = 23,
      "cm^3", // DUT_CUBIC_CENTIMETERS = 24,
      "mm^3", // DUT_CUBIC_MILLIMETERS = 25,
      "l" // DUT_LITERS = 26,
          };

        /// <summary>
        /// Base units currently used internally by Revit.
        /// </summary>
        private enum BaseUnit
        {
            BU_Length = 0,         // length, feet (ft)
            BU_Angle,              // angle, radian (rad)
            BU_Mass,               // mass, kilogram (kg)
            BU_Time,               // time, second (s)
            BU_Electric_Current,   // electric current, ampere (A)
            BU_Temperature,        // temperature, kelvin (K)
            BU_Luminous_Intensity, // luminous intensity, candela (cd)
            BU_Solid_Angle,        // solid angle, steradian (sr)
            NumBaseUnits
        }

        public static double Eps
        {
            get
            {
                return EPS;
            }
        }

        public static double MinLineLength {
            get
            {
                return EPS;
            }
        }

        public static double TolPointOnPlane {
            get
            {
                return EPS;
            }
        }

        /// <summary>
        /// Return a string representation in degrees
        /// for an angle given in radians.
        /// </summary>
        public static string AngleString(double angle)
        {
            return RealString(angle * 180 / Math.PI)
              + " degrees";
        }

        /// <summary>
        /// Return a string for this bounding box
        /// with its coordinates formatted to two
        /// decimal places.
        /// </summary>
        public static string BoundingBoxString(BoundingBoxUV bb)
        {
            return string.Format("({0},{1})", PointString(bb.Min), PointString(bb.Max));
        }

        /// <summary>
        /// Return a string for this bounding box
        /// with its coordinates formatted to two
        /// decimal places.
        /// </summary>
        public static string BoundingBoxString(BoundingBoxXYZ bb)
        {
            return string.Format("({0},{1})", PointString(bb.Min), PointString(bb.Max));
        }

        /// <summary>
        /// Create transformation matrix to transform points
        /// from the global space (XYZ) to the local space of
        /// a face (UV representation of a bounding box).
        /// Revit itself only supports Face.Transform(UV) that
        /// translates a UV coordinate into XYZ coordinate space.
        /// I reversed that Method to translate XYZ coords to
        /// UV coords. At first i thought i could solve the
        /// reverse transformation by solving a linear equation
        /// with 2 unknown variables. But this wasn't general.
        /// I finally found out that the transformation
        /// consists of a displacement vector and a rotation matrix.
        /// </summary>
        public static double[,]
          CalculateMatrixForGlobalToLocalCoordinateSystem(
            Face face)
        {
            //// face.Evaluate uses a rotation matrix and
            //// a displacement vector to translate points

            XYZ originDisplacementVectorUV = face.Evaluate(UV.Zero);
            XYZ unitVectorUWithDisplacement = face.Evaluate(UV.BasisU);
            XYZ unitVectorVWithDisplacement = face.Evaluate(UV.BasisV);

            XYZ unitVectorU = unitVectorUWithDisplacement
              - originDisplacementVectorUV;

            XYZ unitVectorV = unitVectorVWithDisplacement
              - originDisplacementVectorUV;

            //// The rotation matrix A is composed of
            //// unitVectorU and unitVectorV transposed.
            //// To get the rotation matrix that translates from
            //// global space to local space, take the inverse of A.

            var a11i = unitVectorU.X;
            var a12i = unitVectorU.Y;
            var a21i = unitVectorV.X;
            var a22i = unitVectorV.Y;

            return new double[2, 2] {
        { a11i, a12i },
        { a21i, a22i } };
        }

        public static int Compare(
          double a,
          double b,
          double tolerance = EPS)
        {
            return IsEqual(a, b, tolerance)
              ? 0
              : (a < b ? -1 : 1);
        }

        public static int Compare(XYZ p, XYZ q)
        {
            int d = Compare(p.X, q.X);

            if (d == 0) {
                d = Compare(p.Y, q.Y);

                if (d == 0) {
                    d = Compare(p.Z, q.Z);
                }
            }
            return d;
        }

        /// <summary>
        /// Implement a comparison operator for lines
        /// in the XY plane useful for sorting into
        /// groups of parallel lines.
        /// </summary>
        public static int Compare(Line a, Line b)
        {
            XYZ pa = a.GetEndPoint(0);
            XYZ qa = a.GetEndPoint(1);
            XYZ pb = b.GetEndPoint(0);
            XYZ qb = b.GetEndPoint(1);
            XYZ va = qa - pa;
            XYZ vb = qb - pb;

            //// Compare angle in the XY plane

            double ang_a = Math.Atan2(va.Y, va.X);
            double ang_b = Math.Atan2(vb.Y, vb.X);

            int d = Compare(ang_a, ang_b);

            if (d == 0) {
                //// Compare distance of unbounded line to origin

                double da = ((qa.X * pa.Y) - (qa.Y * pa.Y)) / va.GetLength();

                double db = ((qb.X * pb.Y) - (qb.Y * pb.Y)) / vb.GetLength();

                d = Compare(da, db);

                if (d == 0) {
                    //// Compare distance of start point to origin

                    d = Compare(pa.GetLength(), pb.GetLength());

                    if (d == 0) {
                        //// Compare distance of end point to origin

                        d = Compare(qa.GetLength(), qb.GetLength());
                    }
                }
            }
            return d;
        }

        public static int Compare(Plane a, Plane b)
        {
            int d = Compare(a.Normal, b.Normal);

            if (d == 0) {
                d = Compare(a.SignedDistanceTo(XYZ.Zero), b.SignedDistanceTo(XYZ.Zero));
                if (d == 0) {
                    d = Compare(a.XVec.AngleOnPlaneTo(b.XVec, b.Normal), 0);
                }
            }
            return d;
        }

        /// <summary>
        /// Connect two MEP elements at a given point p.
        /// </summary>
        /// <exception cref="ArgumentException">Thrown if
        /// one of the given elements lacks connectors.
        /// </exception>
        public static void Connect(
          XYZ p,
          Element a,
          Element b)
        {
            ConnectorManager cm = GetConnectorManager(a);

            if (cm == null) {
                throw new ArgumentException(
                  "Element a has no connectors.");
            }

            Connector ca = GetConnectorClosestTo(
              cm.Connectors, p);

            cm = GetConnectorManager(b);

            if (cm == null) {
                throw new ArgumentException(
                  "Element b has no connectors.");
            }

            Connector cb = GetConnectorClosestTo(
              cm.Connectors, p);

            ca.ConnectTo(cb);
            ////cb.ConnectTo( ca );
        }

        /// <summary>
        /// Return the convex hull of a list of points
        /// using the Jarvis march or Gift wrapping:
        /// https://en.wikipedia.org/wiki/Gift_wrapping_algorithm
        /// Written by Maxence.
        /// </summary>
        public static List<XYZ> ConvexHull(List<XYZ> points)
        {
            if (points == null) {
                throw new ArgumentNullException(nameof(points));
            }
            XYZ startPoint = points.MinBy(p => p.X);
            var convexHullPoints = new List<XYZ>();
            XYZ walkingPoint = startPoint;
            XYZ refVector = XYZ.BasisY.Negate();
            do {
                convexHullPoints.Add(walkingPoint);
                XYZ wp = walkingPoint;
                XYZ rv = refVector;
                walkingPoint = points.MinBy(p =>
                {
                    double angle = (p - wp).AngleOnPlaneTo(rv, XYZ.BasisZ);
                    if (angle < 1e-10) {
                        angle = 2 * Math.PI;
                    }
                    return angle;
                });
                refVector = wp - walkingPoint;
            } while (walkingPoint != startPoint);
            convexHullPoints.Reverse();
            return convexHullPoints;
        }

        /// <summary>
        /// Create an arc in the XY plane from a given
        /// start point, end point and radius.
        /// </summary>
        public static Arc CreateArc2dFromRadiusStartAndEndPoint(
          XYZ ps,
          XYZ pe,
          double radius,
          bool largeSagitta = false,
          bool clockwise = false)
        {
            //// https://forums.autodesk.com/t5/revit-api-forum/create-a-curve-when-only-the-start-point-end-point-amp-radius-is/m-p/7830079

            XYZ midPointChord = 0.5 * (ps + pe);
            XYZ v = pe - ps;
            double d = 0.5 * v.GetLength(); // half chord length

            //// Small and large circle sagitta:
            //// http://www.mathopenref.com/sagitta.html
            //// https://en.wikipedia.org/wiki/Sagitta_(geometry)

            double s = largeSagitta
              ? radius + Math.Sqrt((radius * radius) - (d * d)) // sagitta large
              : radius - Math.Sqrt((radius * radius) - (d * d)); // sagitta small

            XYZ midPointOffset = Transform
              .CreateRotation(XYZ.BasisZ, 0.5 * Math.PI)
              .OfVector(v.Normalize().Multiply(s));

            XYZ midPointArc = clockwise
              ? midPointChord + midPointOffset
              : midPointChord - midPointOffset;

            return Arc.Create(ps, pe, midPointArc);
        }

        /// <summary>
        /// Create a cone-shaped solid at the given base
        /// location pointing along the given axis.
        /// </summary>
        public static Solid CreateCone(
          XYZ center,
          XYZ axis_vector,
          double radius,
          double height)
        {
            XYZ az = axis_vector.Normalize();

            XYZ ax, ay;
            GetArbitraryAxes(az, out ax, out ay);

            //// Define a triangle in XZ plane

            XYZ px = center + (radius * ax);
            XYZ pz = center + (height * az);

            List<Curve> profile = new List<Curve>();

            profile.Add(Line.CreateBound(center, px));
            profile.Add(Line.CreateBound(px, pz));
            profile.Add(Line.CreateBound(pz, center));

            CurveLoop curveLoop = CurveLoop.Create(profile);

            Frame frame = new Frame(center, ax, ay, az);

            ////SolidOptions options = new SolidOptions(
            ////  ElementId.InvalidElementId,
            ////  ElementId.InvalidElementId );

            Solid cone = GeometryCreationUtilities.CreateRevolvedGeometry(frame, new CurveLoop[] { curveLoop }, 0, 2 * Math.PI);

            return cone;

            ////using( Transaction t = new Transaction( Command.Doc, "Create cone" ) )
            ////{
            ////  t.Start();
            ////  DirectShape ds = DirectShape.CreateElement( Command.Doc, new ElementId( BuiltInCategory.OST_GenericModel ) );
            ////  ds.SetShape( new GeometryObject[] { cone } );
            ////  t.Commit();
            ////}
        }

        /// <summary>
        /// Create and return a solid representing
        /// the bounding box of the input solid.
        /// Assumption: aligned with Z axis.
        /// Written, described and tested by Owen Merrick for
        /// http://forums.autodesk.com/t5/revit-api-forum/create-solid-from-boundingbox/m-p/6592486
        /// </summary>
        public static Solid CreateSolidFromBoundingBox(
          Solid inputSolid)
        {
            BoundingBoxXYZ bbox = inputSolid.GetBoundingBox();

            //// Corners in BBox coords

            XYZ pt0 = new XYZ(bbox.Min.X, bbox.Min.Y, bbox.Min.Z);
            XYZ pt1 = new XYZ(bbox.Max.X, bbox.Min.Y, bbox.Min.Z);
            XYZ pt2 = new XYZ(bbox.Max.X, bbox.Max.Y, bbox.Min.Z);
            XYZ pt3 = new XYZ(bbox.Min.X, bbox.Max.Y, bbox.Min.Z);

            //// Edges in BBox coords

            Line edge0 = Line.CreateBound(pt0, pt1);
            Line edge1 = Line.CreateBound(pt1, pt2);
            Line edge2 = Line.CreateBound(pt2, pt3);
            Line edge3 = Line.CreateBound(pt3, pt0);

            //// Create loop, still in BBox coords

            List<Curve> edges = new List<Curve>();
            edges.Add(edge0);
            edges.Add(edge1);
            edges.Add(edge2);
            edges.Add(edge3);

            double height = bbox.Max.Z - bbox.Min.Z;

            CurveLoop baseLoop = CurveLoop.Create(edges);

            List<CurveLoop> loopList = new List<CurveLoop>();
            loopList.Add(baseLoop);

            Solid preTransformBox = GeometryCreationUtilities.CreateExtrusionGeometry(loopList, XYZ.BasisZ, height);

            Solid transformBox = SolidUtils.CreateTransformed(preTransformBox, bbox.Transform);

            return transformBox;
        }

        /// <summary>
        /// Create and return a solid sphere
        /// with a given radius and centre point.
        /// </summary>
        public static Solid CreateSphereAt(XYZ centre, double radius)
        {
            //// Use the standard global coordinate system
            //// as a frame, translated to the sphere centre.

            Frame frame = new Frame(centre, XYZ.BasisX, XYZ.BasisY, XYZ.BasisZ);

            //// Create a vertical half-circle loop
            //// that must be in the frame location.

            Arc arc = Arc.Create(
              centre - (radius * XYZ.BasisZ),
              centre + (radius * XYZ.BasisZ),
              centre + (radius * XYZ.BasisX));

            Line line = Line.CreateBound(
              arc.GetEndPoint(1),
              arc.GetEndPoint(0));

            CurveLoop halfCircle = new CurveLoop();
            halfCircle.Append(arc);
            halfCircle.Append(line);

            List<CurveLoop> loops = new List<CurveLoop>(1);
            loops.Add(halfCircle);

            return GeometryCreationUtilities.CreateRevolvedGeometry(frame, loops, 0, 2 * Math.PI);
        }

        /// <summary>
        /// Convert a given volume in feet to cubic meters.
        /// </summary>
        public static double CubicFootToCubicMeter(double volume)
        {
            return volume * CubicFeetToCubicMeters;
        }

        /// <summary>
        /// Return a string representing the data of a
        /// curve. Currently includes detailed data of
        /// line and arc elements only.
        /// </summary>
        public static string CurveString(Curve c)
        {
            string s = c.GetType().Name.ToLower();

            XYZ p = c.GetEndPoint(0);
            XYZ q = c.GetEndPoint(1);

            s += string.Format(" {0} --> {1}", PointString(p), PointString(q));

            Arc arc = c as Arc;

            if (arc != null) {
                s += string.Format(" center {0} radius {1}", PointString(arc.Center), arc.Radius);
            }

            //// Todo: add support for other curve types
            //// besides line and arc.

            return s;
        }

        /// <summary>
        /// Return a string for this curve with its
        /// tessellated point coordinates formatted
        /// to two decimal places.
        /// </summary>
        public static string CurveTessellateString(
          Curve curve)
        {
            return "curve tessellation "
              + PointArrayString(curve.Tessellate());
        }

        /// <summary>
        /// Return a dot (full stop) for zero
        /// or a colon for more than zero.
        /// </summary>
        public static string DotOrColon(int n)
        {
            return n > 0 ? ":" : ".";
        }

        /// <summary>
        /// Return a string for a list of doubles
        /// formatted to two decimal places.
        /// </summary>
        public static string DoubleArrayString(IList<double> a)
        {
            return string.Join(", ", a.Select<double, string>(x => RealString(x)));
        }

        /// <summary>
        /// Return a string describing the given element:
        /// .NET type name,
        /// category name,
        /// family and symbol name for a family instance,
        /// element id and element name.
        /// </summary>
        public static string ElementDescription(
          Element e)
        {
            if (e == null) {
                return "<null>";
            }

            //// For a wall, the element name equals the
            //// wall type name, which is equivalent to the
            //// family name ...

            FamilyInstance fi = e as FamilyInstance;

            string typeName = e.GetType().Name;

            string categoryName = (e.Category == null) ? string.Empty : e.Category.Name + " ";

            string familyName = (fi == null) ? string.Empty : fi.Symbol.Family.Name + " ";

            string symbolName = (fi == null
              || e.Name.Equals(fi.Symbol.Name))
                ? string.Empty
                : fi.Symbol.Name + " ";

            return string.Format("{0} {1}{2}{3}<{4} {5}>", typeName, categoryName, familyName, symbolName, e.Id.IntegerValue, e.Name);
        }

        public static void ErrorMsg(string msg)
        {
            Debug.WriteLine(msg);
            WinForms.MessageBox.Show(
              msg,
              Caption,
              WinForms.MessageBoxButtons.OK,
              WinForms.MessageBoxIcon.Error);
        }

        /// <summary>
        /// Given a specific family and symbol name,
        /// return the appropriate family symbol.
        /// </summary>
        public static FamilySymbol FindFamilySymbol(
          Document doc,
          string familyName,
          string symbolName)
        {
            FilteredElementCollector collector
              = new FilteredElementCollector(doc)
                .OfClass(typeof(Family));

            foreach (Family f in collector) {
                if (f.Name.Equals(familyName)) {
                    ////foreach( FamilySymbol symbol in f.Symbols ) // 2014

                    ISet<ElementId> ids = f.GetFamilySymbolIds(); // 2015

                    foreach (ElementId id in ids) {
                        FamilySymbol symbol = doc.GetElement(id)
                          as FamilySymbol;

                        if (symbol.Name == symbolName) {
                            return symbol;
                        }
                    }
                }
            }
            return null;
        }

        /// <summary>
        /// Convert a given length in feet to metres.
        /// </summary>
        public static double FootToMetre(double length)
        {
            return length * FootToMeter;
        }

        /// <summary>
        /// Convert a given length in feet to millimetres.
        /// </summary>
        public static double FootToMm(double length)
        {
            return length * FeetToMm;
        }

        /// <summary>
        /// Return arbitrary X and Y axes for the given
        /// normal vector according to the AutoCAD
        /// Arbitrary Axis Algorithm
        /// https://www.autodesk.com/techpubs/autocad/acadr14/dxf/arbitrary_axis_algorithm_al_u05_c.htm
        /// </summary>
        public static void GetArbitraryAxes(
          XYZ normal,
          out XYZ ax,
          out XYZ ay)
        {
            double limit = 1.0 / 64;

            XYZ pick_cardinal_axis
              = (IsZero(normal.X, limit)
                && IsZero(normal.Y, limit))
                  ? XYZ.BasisY
                  : XYZ.BasisZ;

            ax = pick_cardinal_axis.CrossProduct(normal).Normalize();
            ay = normal.CrossProduct(ax).Normalize();
        }

        /// <summary>
        /// Return the bottom four XYZ corners of the given
        /// bounding box in the XY plane at the given
        /// Z elevation in the order lower left, lower
        /// right, upper right, upper left:
        /// </summary>
        public static XYZ[] GetBottomCorners(
          BoundingBoxXYZ b,
          double z)
        {
            return new XYZ[] {
        new XYZ(b.Min.X, b.Min.Y, z),
        new XYZ(b.Max.X, b.Min.Y, z),
        new XYZ(b.Max.X, b.Max.Y, z),
        new XYZ(b.Min.X, b.Max.Y, z)
      };
        }

        /// <summary>
        /// Return the bottom four XYZ corners of the given
        /// bounding box in the XY plane at the bb minimum
        /// Z elevation in the order lower left, lower
        /// right, upper right, upper left:
        /// </summary>
        public static XYZ[] GetBottomCorners(
          BoundingBoxXYZ b)
        {
            return GetBottomCorners(b, b.Min.Z);
        }

        /// <summary>
        /// Return the connector on the element
        /// closest to the given point.
        /// </summary>
        public static Connector GetConnectorClosestTo(
          Element e,
          XYZ p)
        {
            ConnectorManager cm = GetConnectorManager(e);

            return cm == null
              ? null
              : GetConnectorClosestTo(cm.Connectors, p);
        }

        /// <summary>
        /// Get distinct connectors from a set of MEP elements.
        /// </summary>
        public static HashSet<Connector> GetDistinctConnectors(
          List<Connector> cons)
        {
            return cons.Distinct(new ConnectorXyzComparer())
              .ToHashSet();
        }

        /// <summary>
        /// Return a location for the given element using
        /// its LocationPoint Point property,
        /// LocationCurve start point, whichever
        /// is available.
        /// </summary>
        /// <param name="p">Return element location point</param>
        /// <param name="e">Revit Element</param>
        /// <returns>True if a location point is available
        /// for the given element, otherwise false.</returns>
        public static bool GetElementLocation(out XYZ p, Element e)
        {
            p = XYZ.Zero;
            bool rc = false;
            Location loc = e.Location;
            if (loc != null) {
                LocationPoint lp = loc as LocationPoint;
                if (lp != null) {
                    p = lp.Point;
                    rc = true;
                } else {
                    LocationCurve lc = loc as LocationCurve;
                    Debug.Assert(lc != null, "expected location to be either point or curve");
                    p = lc.Curve.GetEndPoint(0);
                    rc = true;
                }
            }
            return rc;
        }

        /// <summary>
        /// Return all elements of the requested class i.e. System.Type
        /// matching the given built-in category in the given document.
        /// </summary>
        public static FilteredElementCollector GetElementsOfType(
          Document doc,
          Type type,
          BuiltInCategory bic)
        {
            FilteredElementCollector collector
              = new FilteredElementCollector(doc);

            collector.OfCategory(bic);
            collector.OfClass(type);

            return collector;
        }

        /// <summary>
        /// Return the location point of a family instance or null.
        /// This null coalesces the location so you won't get an
        /// error if the FamilyInstance is an invalid object.
        /// </summary>
        public static XYZ GetFamilyInstanceLocation(
          FamilyInstance fi)
        {
            return ((LocationPoint)fi?.Location)?.Point;
        }

        /// <summary>
        /// Return the first element of the given type and name.
        /// </summary>
        public static Element GetFirstElementOfTypeNamed(
          Document doc,
          Type type,
          string name)
        {
            FilteredElementCollector collector
              = new FilteredElementCollector(doc)
                .OfClass(type);

            Func<Element, bool> nameEquals = e => e.Name.Equals(name);

            return collector.Any<Element>(nameEquals)
              ? collector.First<Element>(nameEquals)
              : null;
        }

        /// <summary>
        /// Return the first 3D view which is not a template,
        /// useful for input to FindReferencesByDirection().
        /// In this case, one cannot use FirstElement() directly,
        /// since the first one found may be a template and
        /// unsuitable for use in this method.
        /// This demonstrates some interesting usage of
        /// a .NET anonymous method.
        /// </summary>
        public static Element GetFirstNonTemplate3dView(Document doc)
        {
            FilteredElementCollector collector
              = new FilteredElementCollector(doc);

            collector.OfClass(typeof(View3D));

            return collector
              .Cast<View3D>()
              .First<View3D>(v3 => !v3.IsTemplate);
        }

        /// <summary>
        /// Revit text colour parameter value stored as an integer
        /// in text note type BuiltInParameter.LINE_COLOR.
        /// </summary>
        public static int GetRevitTextColorFromSystemColor(
          System.Drawing.Color color)
        {
            //// from https://forums.autodesk.com/t5/revit-api-forum/how-to-change-text-color/td-p/2567672

            return ToColorParameterValue(color.R, color.G, color.B);
        }

        /// <summary>
        /// Retrieve all pre-selected elements of the specified type,
        /// if any elements at all have been pre-selected. If not,
        /// retrieve all elements of specified type in the database.
        /// </summary>
        /// <param name="a">Return value container</param>
        /// <param name="uidoc">Active document</param>
        /// <param name="t">Specific type</param>
        /// <returns>True if some elements were retrieved</returns>
        public static bool GetSelectedElementsOrAll(
          List<Element> a,
          UIDocument uidoc,
          Type t)
        {
            Document doc = uidoc.Document;

            ICollection<ElementId> ids
              = uidoc.Selection.GetElementIds();

            if (ids.Count > 0) {
                a.AddRange(ids
                  .Select<ElementId, Element>(
                    id => doc.GetElement(id))
                  .Where<Element>(
                    e => t.IsInstanceOfType(e)));
            } else {
                a.AddRange(new FilteredElementCollector(doc)
                  .OfClass(t));
            }
            return a.Count > 0;
        }

        public static Element GetSingleSelectedElement(
          UIDocument uidoc)
        {
            ICollection<ElementId> ids
              = uidoc.Selection.GetElementIds();

            Element e = null;

            if (ids.Count == 1) {
                foreach (ElementId id in ids) {
                    e = uidoc.Document.GetElement(id);
                }
            }
            return e;
        }

        /// <summary>
        /// Return a hash string for a real number
        /// formatted to nine decimal places.
        /// </summary>
        public static string HashString(double a)
        {
            return a.ToString("0.#########");
        }

        /// <summary>
        /// Return a hash string for an XYZ point
        /// or vector with its coordinates
        /// formatted to nine decimal places.
        /// </summary>
        public static string HashString(XYZ p)
        {
            return string.Format(
              "({0},{1},{2})",
              HashString(p.X),
              HashString(p.Y),
              HashString(p.Z));
        }

        public static void InfoMsg(string msg)
        {
            Debug.WriteLine(msg);
            WinForms.MessageBox.Show(
              msg,
              Caption,
              WinForms.MessageBoxButtons.OK,
              WinForms.MessageBoxIcon.Information);
        }

        public static void InfoMsg2(
          string instruction,
          string content)
        {
            Debug.WriteLine(instruction + "\r\n" + content);
            TaskDialog d = new TaskDialog(Caption);
            d.MainInstruction = instruction;
            d.MainContent = content;
            d.Show();
        }

        /// <summary>
        /// Return the 2D intersection point between two
        /// unbounded lines defined in the XY plane by the
        /// start and end points of the two given curves.
        /// By Magson Leone.
        /// Return null if the two lines are coincident,
        /// in which case the intersection is an infinite
        /// line, or non-coincident and parallel, in which
        /// case it is empty.
        /// https://en.wikipedia.org/wiki/Line%E2%80%93line_intersection
        /// </summary>
        public static XYZ Intersection(Curve c1, Curve c2)
        {
            XYZ p1 = c1.GetEndPoint(0);
            XYZ q1 = c1.GetEndPoint(1);
            XYZ p2 = c2.GetEndPoint(0);
            XYZ q2 = c2.GetEndPoint(1);
            XYZ v1 = q1 - p1;
            XYZ v2 = q2 - p2;
            XYZ w = p2 - p1;

            XYZ p5 = null;

            double c = ((v2.X * w.Y) - (v2.Y * w.X)) / ((v2.X * v1.Y) - (v2.Y * v1.X));

            if (!double.IsInfinity(c)) {
                double x = p1.X + (c * v1.X);
                double y = p1.Y + (c * v1.Y);

                p5 = new XYZ(x, y, 0);
            }
            return p5;
        }

        public static bool IsCollinear(Line a, Line b)
        {
            XYZ v = a.Direction;
            XYZ w = b.Origin - a.Origin;
            return IsParallel(v, b.Direction)
              && IsParallel(v, w);
        }

        public static bool IsEqual(
          double a,
          double b,
          double tolerance = EPS)
        {
            return IsZero(b - a, tolerance);
        }

        public static bool IsEqual(XYZ p, XYZ q)
        {
            return Compare(p, q) == 0;
        }

        public static bool IsHorizontal(XYZ v)
        {
            return IsZero(v.Z);
        }

        public static bool IsHorizontal(Edge e)
        {
            XYZ p = e.Evaluate(0);
            XYZ q = e.Evaluate(1);
            return IsHorizontal(q - p);
        }

        public static bool IsHorizontal(PlanarFace f)
        {
            return IsVertical(f.FaceNormal);
        }

        public static bool IsParallel(XYZ p, XYZ q)
        {
            return p.CrossProduct(q).IsZeroLength();
        }

        public static bool IsVertical(XYZ v)
        {
            return IsZero(v.X) && IsZero(v.Y);
        }

        public static bool IsVertical(XYZ v, double tolerance)
        {
            return IsZero(v.X, tolerance)
              && IsZero(v.Y, tolerance);
        }

        public static bool IsVertical(PlanarFace f)
        {
            return IsHorizontal(f.FaceNormal);
        }

        public static bool IsVertical(CylindricalFace f)
        {
            return IsVertical(f.Axis);
        }

        public static bool IsZero(
                                                                                                                                                                                                                                                                                                                                                                                                                                                  double a,
          double tolerance = EPS)
        {
            return tolerance > Math.Abs(a);
        }

        /// <summary>
        /// Return the maximum value from an array of real numbers.
        /// </summary>
        public static double Max(double[] a)
        {
            Debug.Assert(a.Rank == 1, "expected one-dimensional array");
            Debug.Assert(a.GetLowerBound(0) == 0, "expected zero-based array");
            Debug.Assert(a.GetUpperBound(0) > 0, "expected non-empty array");
            double max = a[0];
            for (int i = 1; i <= a.GetUpperBound(0); ++i) {
                if (max < a[i]) {
                    max = a[i];
                }
            }

            return max;
        }

        /// <summary>
        /// Return the midpoint between two points.
        /// </summary>
        public static XYZ Midpoint(XYZ p, XYZ q)
        {
            return 0.5 * (p + q);
        }

        /// <summary>
        /// Return the midpoint of a Line.
        /// </summary>
        public static XYZ Midpoint(Line line)
        {
            return Midpoint(line.GetEndPoint(0), line.GetEndPoint(1));
        }

        /// <summary>
        /// Return a string for a length in millimetres
        /// formatted as an integer value.
        /// </summary>
        public static string MmString(double length)
        {
            ////return RealString( FootToMm( length ) ) + " mm";
            return Math.Round(FootToMm(length))
              .ToString() + " mm";
        }

        /// <summary>
        /// Convert a given length in millimetres to feet.
        /// </summary>
        public static double MmToFoot(double length)
        {
            return length / FeetToMm;
        }

        /// <summary>
        /// Convert a given point or vector from millimetres to feet.
        /// </summary>
        public static XYZ MmToFoot(XYZ v)
        {
            return v.Divide(FeetToMm);
        }

        /// <summary>
        /// Return the normal of a Line in the XY plane.
        /// </summary>
        public static XYZ Normal(Line line)
        {
            XYZ p = line.GetEndPoint(0);
            XYZ q = line.GetEndPoint(1);
            XYZ v = q - p;
            return v.CrossProduct(XYZ.BasisZ).Normalize();
        }

        /// <summary>
        /// Return a string for this plane
        /// with its coordinates formatted to two
        /// decimal places.
        /// </summary>
        public static string PlaneString(Plane p)
        {
            return string.Format(
              "plane origin {0}, plane normal {1}",
              PointString(p.Origin),
              PointString(p.Normal));
        }

        /// <summary>
        /// Return an English plural suffix for the given
        /// number of items, i.e. 's' for zero or more
        /// than one, and nothing for exactly one.
        /// </summary>
        public static string PluralSuffix(int n)
        {
            return n == 1 ? string.Empty : "s";
        }

        /// <summary>
        /// Return an English plural suffix 'ies' or
        /// 'y' for the given number of items.
        /// </summary>
        public static string PluralSuffixY(int n)
        {
            return n == 1 ? "y" : "ies";
        }

        /// <summary>
        /// Return a string for this point array
        /// with its coordinates formatted to two
        /// decimal places.
        /// </summary>
        public static string PointArrayString(IList<XYZ> pts)
        {
            return string.Join(", ", pts.Select<XYZ, string>(p => PointString(p)));
        }

        /// <summary>
        /// Return a string for a UV point
        /// or vector with its coordinates
        /// formatted to two decimal places.
        /// </summary>
        public static string PointString(UV p)
        {
            return string.Format("({0},{1})", RealString(p.U), RealString(p.V));
        }

        /// <summary>
        /// Return a string for an XYZ point
        /// or vector with its coordinates
        /// formatted to two decimal places.
        /// </summary>
        public static string PointString(XYZ p)
        {
            return string.Format(
              "({0},{1},{2})",
              RealString(p.X),
              RealString(p.Y),
              RealString(p.Z));
        }

        /// <summary>
        /// Return true if the Z coordinate of the
        /// given vector is positive and the slope
        /// is larger than the minimum limit.
        /// </summary>
        public static bool PointsUpwards(XYZ v)
        {
            double horizontalLength = (v.X * v.X) + (v.Y * v.Y);
            double verticalLength = v.Z * v.Z;
            return v.Z > 0 && MinimumSlope < verticalLength / horizontalLength;
        }

        /// <summary>
        /// Return a string for a real number
        /// formatted to two decimal places.
        /// </summary>
        public static string RealString(double a)
        {
            return a.ToString("0.##");
        }

        public static Element SelectSingleElement(
                  UIDocument uidoc,
                  string description)
        {
            if (ViewType.Internal == uidoc.ActiveView.ViewType) {
                TaskDialog.Show("Error", "Cannot pick element in this view: " + uidoc.ActiveView.Name);
                return null;
            }

            try {
                Reference r = uidoc.Selection.PickObject(ObjectType.Element, "Please select " + description);
                return uidoc.Document.GetElement(r); // 2012
            } catch (Autodesk.Revit.Exceptions.OperationCanceledException) {
                return null;
            }
        }

        public static Element SelectSingleElementOfType(
                  UIDocument uidoc,
                  Type t,
                  string description,
                  bool acceptDerivedClass)
        {
            Element e = GetSingleSelectedElement(uidoc);

            if (!HasRequestedType(e, t, acceptDerivedClass)) {
                e = Util.SelectSingleElement(
                  uidoc, description);
            }
            return HasRequestedType(e, t, acceptDerivedClass)
              ? e
              : null;
        }

        /// <summary>
        /// Revit text colour parameter value stored as an integer
        /// in text note type BuiltInParameter.LINE_COLOR.
        /// </summary>
        public static int ToColorParameterValue(
          int red,
          int green,
          int blue)
        {
            //// from https://forums.autodesk.com/t5/revit-api-forum/how-to-change-text-color/td-p/2567672

            int c = red + (green << 8) + (blue << 16);
            return c;
        }

        /// <summary>
        /// Return a string for this transformation
        /// with its coordinates formatted to two
        /// decimal places.
        /// </summary>
        public static string TransformString(Transform t)
        {
            return string.Format("({0},{1},{2},{3})", PointString(t.Origin), PointString(t.BasisX), PointString(t.BasisY), PointString(t.BasisZ));
        }

        /// <summary>
        /// Convert a UnitSymbolType enumeration value
        /// to a brief human readable abbreviation string.
        /// </summary>
        public static string UnitSymbolTypeString(UnitSymbolType u)
        {
            string s = u.ToString();
            Debug.Assert(s.StartsWith("UST_"), "expected UnitSymbolType enumeration value " + "to begin with 'UST_'");
            s = s.Substring(4).Replace("_SUP_", "^").ToLower();
            return s;
        }

        /// <summary>
        /// Return true if the given bounding box bb
        /// contains the given point p in its interior.
        /// </summary>
        public bool BoundingBoxXyzContains(
          BoundingBoxXYZ bb,
          XYZ p)
        {
            return Compare(bb.Min, p) > 0 && Compare(p, bb.Max) > 0;
        }

        /// <summary>
        /// Create and return a cube of
        /// side length d at the origin.
        /// </summary>
        private static Solid CreateCube(double d)
        {
            return CreateRectangularPrism(
              XYZ.Zero, d, d, d);
        }

        /// <summary>
        /// Create and return a rectangular prism of the
        /// given side lengths centered at the given point.
        /// </summary>
        private static Solid CreateRectangularPrism(
          XYZ center,
          double d1,
          double d2,
          double d3)
        {
            List<Curve> profile = new List<Curve>();
            XYZ profile00 = new XYZ(-d1 / 2, -d2 / 2, -d3 / 2);
            XYZ profile01 = new XYZ(-d1 / 2, d2 / 2, -d3 / 2);
            XYZ profile11 = new XYZ(d1 / 2, d2 / 2, -d3 / 2);
            XYZ profile10 = new XYZ(d1 / 2, -d2 / 2, -d3 / 2);

            profile.Add(Line.CreateBound(profile00, profile01));
            profile.Add(Line.CreateBound(profile01, profile11));
            profile.Add(Line.CreateBound(profile11, profile10));
            profile.Add(Line.CreateBound(profile10, profile00));

            CurveLoop curveLoop = CurveLoop.Create(profile);

            SolidOptions options = new SolidOptions(
              ElementId.InvalidElementId,
              ElementId.InvalidElementId);

            return GeometryCreationUtilities.CreateExtrusionGeometry(new CurveLoop[] { curveLoop }, XYZ.BasisZ, d3, options);
        }

        /// <summary>
        /// Return the element's connector at the given
        /// location, and its other connector as well,
        /// in case there are exactly two of them.
        /// </summary>
        /// <param name="e">An element, e.g. duct, pipe or family instance</param>
        /// <param name="location">The location of one of its connectors</param>
        /// <param name="otherConnector">The other connector, in case there are just two of them</param>
        /// <returns>The connector at the given location</returns>
        private static Connector GetConnectorAt(
          Element e,
          XYZ location,
          out Connector otherConnector)
        {
            otherConnector = null;

            Connector targetConnector = null;

            ConnectorManager cm = GetConnectorManager(e);

            bool hasTwoConnectors = cm.Connectors.Size == 2;

            foreach (Connector c in cm.Connectors) {
                if (c.Origin.IsAlmostEqualTo(location)) {
                    targetConnector = c;

                    if (!hasTwoConnectors) {
                        break;
                    }
                } else if (hasTwoConnectors) {
                    otherConnector = c;
                }
            }
            return targetConnector;
        }

        /// <summary>
        /// Return the connector set element
        /// closest to the given point.
        /// </summary>
        private static Connector GetConnectorClosestTo(
          ConnectorSet connectors,
          XYZ p)
        {
            Connector targetConnector = null;
            double minDist = double.MaxValue;

            foreach (Connector c in connectors) {
                double d = c.Origin.DistanceTo(p);

                if (d < minDist) {
                    targetConnector = c;
                    minDist = d;
                }
            }
            return targetConnector;
        }

        /// <summary>
        /// Return the given element's connector manager,
        /// using either the family instance MEPModel or
        /// directly from the MEPCurve connector manager
        /// for ducts and pipes.
        /// </summary>
        private static ConnectorManager GetConnectorManager(
          Element e)
        {
            MEPCurve mc = e as MEPCurve;
            FamilyInstance fi = e as FamilyInstance;

            if (mc == null && fi == null) {
                throw new ArgumentException("Element is neither an MEP curve nor a fitting.");
            }

            return mc == null ? fi.MEPModel.ConnectorManager : mc.ConnectorManager;
        }

        private static bool HasRequestedType(
                  Element e,
                  Type t,
                  bool acceptDerivedClass)
        {
            bool rc = e != null;

            if (rc) {
                Type t2 = e.GetType();

                rc = t2.Equals(t);

                if (!rc && acceptDerivedClass) {
                    rc = t2.IsSubclassOf(t);
                }
            }
            return rc;
        }

        /// <summary>
        /// Return true if the vectors v and w
        /// are non-zero and perpendicular.
        /// </summary>
        private bool IsPerpendicular(XYZ v, XYZ w)
        {
            double a = v.GetLength();
            double b = v.GetLength();
            double c = Math.Abs(v.DotProduct(w));
            return EPS < a
              && EPS < b
              && EPS > c;
            //// c * c < _eps * a * b
        }

        /// <summary>
        /// Compare Connector objects based on their location point.
        /// </summary>
        public class ConnectorXyzComparer : IEqualityComparer<Connector>
        {
            public bool Equals(Connector x, Connector y)
            {
                return x != null && y != null && IsEqual(x.Origin, y.Origin);
            }

            public int GetHashCode(Connector x)
            {
                return HashString(x.Origin).GetHashCode();
            }
        }

        /// <summary>
        /// Wrapper to fix a spelling error prior to Revit 2016.
        /// </summary>
        public class SpellingErrorCorrector
        {
            private static Type externalDefinitionCreationOptionsType;
            private static bool inRevit2015OrEarlier;

            public SpellingErrorCorrector(Application app)
            {
                ////inRevit2015OrEarlier = 0 <= app.VersionNumber.CompareTo("2015");
                inRevit2015OrEarlier = false;
                string s = inRevit2015OrEarlier ? "ExternalDefinitonCreationOptions" : "ExternalDefinitionCreationOptions";
                externalDefinitionCreationOptionsType = System.Reflection.Assembly.GetExecutingAssembly().GetType(s);
            }

            public Definition NewDefinition(
              Definitions definitions,
              string name,
              ParameterType parameterType)
            {
                object opt
                  = NewExternalDefinitionCreationOptions(
                    name,
                    parameterType);

                return typeof(Definitions).InvokeMember("Create", BindingFlags.InvokeMethod, null, definitions, new object[] { opt }) as Definition;
            }

            private object NewExternalDefinitionCreationOptions(string name, ParameterType parameterType)
            {
                object[] args = new object[] { name, parameterType };
                return externalDefinitionCreationOptionsType.GetConstructor(new Type[] { externalDefinitionCreationOptionsType }).Invoke(args);
            }
        }
    }
}