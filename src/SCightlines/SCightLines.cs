//
// (C) Copyright 2012-2013 by Andrew Nicholas
//
// This file is part of SCightlines.
//
// SCightlines is free software: you can redistribute it and/or modify
// it under the terms of the GNU Lesser General Public License as published by
// the Free Software Foundation, either version 3 of the License, or
// (at your option) any later version.
//
// SCightlines is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU Lesser General Public License for more details.
//
// You should have received a copy of the GNU Lesser General Public License
// along with SCightlines.  If not, see <http://www.gnu.org/licenses/>.
//

using System;
using Autodesk.Revit.DB;
using Autodesk.Revit.ApplicationServices;

namespace SCaddins.SCightLines
{

    /// <summary>
    /// Create sight lines diagrams and/or Tables
    /// </summary>
    /// <author>
    /// Andrew Nicholas
    /// </author>

    class SCightLines
    {

        private Document doc;
        private double treadSize; 
        private double eyeHeight;
        private double xDistanceToFirstRow;
        private double yDistanceToFirstRow;
        //private double focusHeight;
        private int numberOfRows;
        private double minimumRiserHeight;
        private double minimumCValue;
        private double riserIncrement;
        private View view;
        private SCightLinesRow[] rows;

        private String infoString;
        /// <summary>
        /// The string that will get outputed to the Information form
        /// </summary>
        public String InfoString
        {
            get { return infoString; }
        }

        
        /// <summary>
        /// A class to create line of sight drafting views in Revit
        /// </summary>
        /// <param name="doc">The Revit Document.</param>
        /// <param name="eyeHeight">The eye height of the seated person.</param>
        /// <param name="treadSize">The tread size of each seating plat.</param>
        /// <param name="riserIncrement">The max increment in each r</param>
        /// <param name="minimumCValue">The minimum C value.</param>
        /// <param name="minimumRiserHeight">The minimum riser height of each seating plat.</param>
        /// <param name="numberOfRows">The number of row in the stand</param>
        /// <param name="xDistanceToFirstRow"></param>
        /// <param name="yDistanceToFirstRow"></param>
        public SCightLines(Document doc, double eyeHeight, double treadSize,
            double riserIncrement,double minimumCValue, double minimumRiserHeight,
            double numberOfRows, double xDistanceToFirstRow, double yDistanceToFirstRow)
        {
            this.doc = doc;
            //focusHeight = 0;
            numberOfRows = 10;
            rows = new SCightLinesRow[100];
            for(int i =0; i < 100; i++)
            {
                rows[i] = new SCightLinesRow();
            }
            Update(eyeHeight, treadSize, riserIncrement, minimumCValue,
                minimumRiserHeight, numberOfRows, xDistanceToFirstRow, yDistanceToFirstRow);
        }

        
        /// <summary>
        /// Update/Change all parameters.
        /// i.e when changes are made to GUI frontend
        /// </summary>
        public void Update(double eyeHeight, double treadSize,
            double riserIncrement,double minimumCValue,
            double minimumRiserHeight,double numberOfRows,
            double xDistanceToFirstRow, double yDistanceToFirstRow)
        {
            this.eyeHeight = eyeHeight;
            this.treadSize = treadSize;
            this.riserIncrement = riserIncrement;
            this.minimumCValue = minimumCValue;
            this.minimumRiserHeight = minimumRiserHeight;
            this.numberOfRows = Convert.ToInt32(numberOfRows);
            this.xDistanceToFirstRow = xDistanceToFirstRow;
            this.yDistanceToFirstRow = yDistanceToFirstRow;
            UpdateRows();
            this.infoString = UpdateInfoString();
        }

        
        /// <summary>
        /// Create a Drafting View with a semi-usefull name
        /// </summary>
        /// <param name="s">
        /// Name of the the drafting view
        /// </param>
        ViewDrafting CreateLineOfSightDraftingView(String s)
        {
            Autodesk.Revit.ApplicationServices.Application app = doc.Application;
            ViewDrafting view = null;
            view = doc.Create.NewViewDrafting();
            view.ViewName = s;
            return view;
        }

        
        /// <summary>
        /// Update all the rows
        /// </summary>
        private void UpdateRows()
        {
            for (int i = 0; i < numberOfRows; i++){
                rows[i].initialize(
                    xDistanceToFirstRow + (i * treadSize), yDistanceToFirstRow, 
                    (yDistanceToFirstRow + eyeHeight), treadSize, eyeHeight);
                if (i > 0){
                    rows[i].RiserHeight = minimumRiserHeight;
                    rows[i].HeightToFocus = rows[i-1].HeightToFocus + minimumRiserHeight;
                    while ( GetCValue( (i-1), rows[i].RiserHeight)  < minimumCValue ){
                        rows[i].RiserHeight += riserIncrement;
                        rows[i].HeightToFocus += riserIncrement;
                    }
                    rows[i - 1].CValue = GetCValue(i - 1, rows[i].RiserHeight);
                }
            }
        }

        
        /// <summary>
        /// Draw the sightlines
        /// </summary>
        public void Draw()
        {
            String times = System.DateTime.Now.ToString();

            view = CreateLineOfSightDraftingView
                ("LOS-X" + xDistanceToFirstRow + "-Y" + yDistanceToFirstRow + "-T" + 
                treadSize + "-MinN" + minimumRiserHeight + "-Inc" + riserIncrement + 
                "-Eye" + eyeHeight + "-MinC" + minimumCValue + "_" + times);

            Autodesk.Revit.ApplicationServices.Application app = doc.Application;
            view.Scale = 50;
            int i;
            
                    for (i = 0; i < numberOfRows; i++){
                        if (i == 0){
                            DrawLine(0, 0, xDistanceToFirstRow, 0, "Thin Lines");
                            DrawText((xDistanceToFirstRow / 2), 0, 1, 0,
                                     xDistanceToFirstRow.ToString(),
                                     TextAlignFlags.TEF_ALIGN_CENTER | TextAlignFlags.TEF_ALIGN_TOP);
                            DrawText(xDistanceToFirstRow, (yDistanceToFirstRow / 2), 0, 1,
                                     yDistanceToFirstRow.ToString(),
                                     TextAlignFlags.TEF_ALIGN_CENTER | TextAlignFlags.TEF_ALIGN_BOTTOM);
                        }
                        //Draw the sight line
                        DrawLine(0, 0, rows[i].EyeToFocusX, rows[i].HeightToFocus, "Dash - 0.1 pen");

                        //Draw the head
                        DrawCircle(rows[i].EyeToFocusX, rows[i].HeightToFocus, "Medium Lines");

                        //Draw the body
                        DrawLine(rows[i].EyeToFocusX, rows[i].HeightToFocus, rows[i].EyeToFocusX,
                                 (rows[i].HeightToFocus - rows[i].EyeHeight), "Thin Lines");

                        //Draw the riser
                        DrawRiser(i);

                        //Draw the going
                        DrawGoing(i);
                        
                        //Draw the c-value text
                        DrawText((rows[i].EyeToFocusX + 125), rows[i].HeightToFocus, 1, 0,
                                 ("c:" + Math.Round(rows[i].CValue, 2).ToString()), TextAlignFlags.TEF_ALIGN_LEFT);

                        //Draw the going text (treadSize)
                        DrawText((rows[i].EyeToFocusX - rows[i].Going / 2), (rows[i].HeightToFocus - rows[i].EyeHeight), 1, 0,
                                 "R" + (i+1).ToString() + " : " + treadSize.ToString(),
                                 TextAlignFlags.TEF_ALIGN_CENTER | TextAlignFlags.TEF_ALIGN_TOP);

                        //Draw the riser text)
                        if (i > 0){
                            DrawText(rows[i].EyeToFocusX - treadSize, (rows[i].HeightToFocus - rows[i].EyeHeight - (rows[i].RiserHeight / 2)), 0, 1,
                                     rows[i].RiserHeight.ToString(), TextAlignFlags.TEF_ALIGN_CENTER | TextAlignFlags.TEF_ALIGN_BOTTOM);
                        }
                }
        }

        /// <summary>
        /// Draw the Riser.
        /// </summary>
        /// <param name="i">The riser number.</param>
        private void DrawRiser(int i)
        {
            if (i == 0) {
                DrawLine(rows[i].EyeToFocusX, rows[i].HeightToFocus - rows[i].EyeHeight, rows[i].EyeToFocusX,
                    (rows[i].HeightToFocus - (rows[i].EyeHeight + rows[i].RiserHeight)), "Thin Lines");
            }
            else {
                DrawLine((rows[i].EyeToFocusX - rows[i].Going), rows[i].HeightToFocus - rows[i].EyeHeight,
                    (rows[i].EyeToFocusX - rows[i].Going), (rows[i].HeightToFocus - (rows[i].EyeHeight + rows[i].RiserHeight)), "Wide Lines");
            }
        }

        /// <summary>
        /// Draw the Going.
        /// </summary>
        /// <param name="i">The going(tread) number.</param>
        private void DrawGoing(int i)
        {
            String igo = "Wide Lines";
            if (i == 0) {
                igo = "Thin Lines";
            }
            DrawLine(rows[i].EyeToFocusX, (rows[i].HeightToFocus - rows[i].EyeHeight),
                (rows[i].EyeToFocusX - rows[i].Going), (rows[i].HeightToFocus - rows[i].EyeHeight), igo);
        }
        

        /// <summary>
        /// Draw some text.
        /// </summary>
        private void DrawText(double x, double y, double vx, double vy, String s, TextAlignFlags f)
        {
            Application app = doc.Application;
            XYZ origin = app.Create.NewXYZ(FeetToMM(x), FeetToMM(y), 0);
            XYZ normal_base = app.Create.NewXYZ(vx, vy, 0);
            XYZ normal_up = app.Create.NewXYZ(0, 1, 0);
            TextElement testy = doc.Create.NewTextNote
            	(view, origin,normal_base, normal_up,FeetToMM(10), f, s);
        }

        
        /// <summary>
        /// Update the string that will get outputed
        /// </summary>
        public String UpdateInfoString()
        {
            String s;
            int i;

            s = "";
            s += "Number of Rows in Section            =\t" + numberOfRows + "\r\n";
            s += "Distance to first spectator          =\t" +  xDistanceToFirstRow + "\r\n";
            s += "Minimum sight line clearance         =\t" +  minimumCValue + "\r\n";
            s += "Eye level above tread                =\t" +  eyeHeight + "\r\n";
            s += "Elevation of first tread above datum =\t" +  yDistanceToFirstRow + "\r\n";
            s += "Tread size                           =\t" +  treadSize + "\r\n";
            s += "Minimum riser height                 =\t" +  minimumRiserHeight + "\r\n";
            s += "Minimum riser increment              =\t" +  riserIncrement + "\r\n\r\n";
            s += ("row:\triser:\tdist:\telev:\tc-value:\r\n");

            for (i = 0; i < numberOfRows; i++){
                String c = i > 0 ? Math.Round(rows[i - 1].CValue, 2).ToString() : "NA";
                String r = i > 0 ? Math.Round(rows[i].RiserHeight, 2).ToString() : "NA";
                s += (i+1 + "\t" + r + "\t"
                    + Math.Round(rows[i].EyeToFocusX, 2) + "\t"
                    + Math.Round(rows[i].HeightToFocus - eyeHeight, 2) + "\t"
                    + c + "\r\n");
            }

            return s;
        }

        
        /// <summary>
        /// Draw a line
        /// </summary>
        private void DrawLine(double x1, double y1, double x2, double y2, String s)
        {
            Autodesk.Revit.ApplicationServices.Application app = doc.Application;
            double z = 0.0;
            XYZ point1 = app.Create.NewXYZ(FeetToMM(x1), FeetToMM(y1), FeetToMM(z));
            XYZ point2 = app.Create.NewXYZ(FeetToMM(x2), FeetToMM(y2), FeetToMM(z));
            try {
//                #if REVIT2014
                Line line = Line.CreateBound(point1, point2);
//                #else
//                Line line = app.Create.NewLineBound(point1, point2);
//                #endif
                DetailLine detailCurve = doc.Create.NewDetailCurve(view, line) as DetailLine;
                SetLineType(detailCurve, s);
            }catch(Exception e){
                Console.WriteLine(e.Message);
            }
        }

        
        /// <summary>
        /// Draw a circle
        /// </summary>
        private void DrawCircle(double x1, double y1, String s)
        {
            Autodesk.Revit.ApplicationServices.Application app = doc.Application;
            double z = 0.0;
            XYZ point1 = app.Create.NewXYZ(FeetToMM(x1), FeetToMM(y1), FeetToMM(z));
//            #if REVIT2014
            Arc arc = Arc.Create(point1, FeetToMM(125), 0, 360, 
                app.Create.NewXYZ(1, 0, 0), app.Create.NewXYZ(0, 1, 0));
//            #else
//            Arc arc = app.Create.NewArc(point1, FeetToMM(125), 0, 360, 
//            app.Create.NewXYZ(1, 0, 0), app.Create.NewXYZ(0, 1, 0));
//            #endif
            DetailArc detailCurve = doc.Create.NewDetailCurve(view,arc) as DetailArc;
            SetLineType(detailCurve, s);
        }

        
        /// <summary>
        /// Set the Revit linetype of a DetailCurve
        /// </summary>
        /// <param name="l">
        /// DetailCurve to set
        /// </param>
        /// <param name="s">
        /// Name of the linetype
        /// </param>
        private void SetLineType(DetailCurve l, String s)
        {
//            #if REVIT2014
            foreach (ElementId styleId in l.GetLineStyleIds()){
                Element style = doc.GetElement(styleId);
                if (style.Name.Equals(s))
                    l.LineStyle = style;
            }
//            #else
//            foreach (Element style in l.LineStyles){
//                if (style.Name.Equals(s))
//                    l.LineStyle = style;
//            }
//            #endif

        }

        
        /// <summary>Converts feet to mm
        /// NOTE: revit system units are feet so this needs to be done
        /// </summary>
        /// <param name="EyeToFocusX">
        /// Length in feet
        /// </param>
        /// <returns>
        /// Returns d in mm
        /// </returns>
        private double FeetToMM(double d)
        {
            return d / 304.8;
        }

        
        /// <summary>Get the C-Value</summary>
        /// <param name="i">
        /// The row to calculate
        /// </param>
        /// <param name="nextn">
        /// The riser height(RiserHeight) of the next row(i+1)
        /// </param>
        /// <returns>
        /// The C-Value as a double
        /// </returns>
        private double GetCValue(int i, double nextn)
        {
            return ((rows[i].EyeToFocusX * (rows[i].HeightToFocus + nextn)) / (rows[i].EyeToFocusX + treadSize)) - rows[i].HeightToFocus;
        }

    }
}
