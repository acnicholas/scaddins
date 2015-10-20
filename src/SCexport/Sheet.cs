// (C) Copyright 2012-2015 by Andrew Nicholas
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

namespace SCaddins.SCexport
{
    using System;
    using System.Globalization;
    using Autodesk.Revit.DB;
    using SCaddins.Common;

    /// <summary>
    /// Class to hold view sheet information.
    /// </summary>
    public class ExportSheet
    {
        #region Variables
        private DateTime sheetRevisionDateTime;
        private Document doc;
        private ElementId id;
        private PrintSetting printSetting;
        private SegmentedSheetName segmentedFileName;
        private ViewSheet sheet;
        private bool forceDate;
        private bool verified;
        private double height;
        private double width;
        private string displineCode;
        private string fullExportName;
        private string pageSize;
        private string projectNumber;
        private string scale;
        private string scaleBarScale;
        private string sheetDescription;
        private string sheetNumber;
        private string sheetRevision;
        private string sheetRevisionDate;
        private string sheetRevisionDescription;

        public ExportSheet(
                ViewSheet sheet,
                Document doc,
                SegmentedSheetName fileNameTemplate,
                ExportManager scx)
        {
            this.Init(sheet, doc, fileNameTemplate, scx);
        }

        public string PageSize
        {
            get { return this.pageSize; }
        }

        public string PrintSettingName
        {
            get
            {
                return this.printSetting != null ? this.printSetting.Name : string.Empty;
            }
        }

        public SegmentedSheetName SegmentedFileName {
            get {
                return this.segmentedFileName;
            }     
        }

        public string SheetDescription
        {
            get { 
                return this.sheetDescription;
            }
            
            set { 
                this.sheetDescription = value;
                this.SetExportName();
            }
        }

        public string SheetNumber
        {
            get {
                return this.sheetNumber;
            }
            
            set { 
                this.sheetNumber = value;
                this.SetExportName();
            }
        }

        public string SheetRevision
        {
            get {
                return this.sheetRevision ?? "-";
            }
        }

        public string SheetRevisionDescription
        {
            get {
                return this.sheetRevisionDescription ?? "-";
            }
        }

        public string SheetRevisionDate
        {
            get {
                return this.sheetRevisionDate ?? "-";
            }
        }

        public DateTime SheetRevisionDateTime
        {
            get { return this.sheetRevisionDateTime; }
        }

        public string Scale
        {
            get {
                string result = this.scale.Trim();
                int i = 0;
                if (result.Contains(":")) {
                    i = result.IndexOf(':');
                }
                bool flag = false;
                if (string.IsNullOrEmpty(result.Trim())) {
                    result = "0";
                }
                if (!string.IsNullOrEmpty(this.scaleBarScale)) {
                    flag |= i > 0 && !result.Substring(i + 2).Equals(this.scaleBarScale.Trim());
                }
                if (!string.IsNullOrEmpty(this.scaleBarScale.Trim()) && flag) {
                    result += " [**" + this.scaleBarScale + "]";
                }
                return result;
            }
        }
        
        public bool ValidScaleBar
        {
            get { return this.RevitScaleWithoutFormatting() == this.scaleBarScale.Trim(); }
        }
        
        public string ExportDir
        {
            get; set;
        }

        public ViewSheet Sheet
        {
            get { return this.sheet; }
        }

        public string FullExportName
        {
            get { return this.fullExportName; }
        }

        public ElementId Id
        {
            get { return this.id; }
        }

        public double Width
        {
            get { return this.width * 304.8; }
        }

        public double Height
        {
            get { return this.height * 304.8; }
        }

        public PrintSetting SCPrintSetting
        {
            get { return this.printSetting; }
        }

        public bool Verified
        {
            get { return this.verified; }
        }

        public bool ForceDate
        {
            get {
                return this.forceDate;
            }
            
            set {
                this.forceDate = value;
                this.SetExportName();
            }
        }
        #endregion
        
        public static string GetScaleBarScale(Element titleBlock)
        {
          try {
                    #if (REVIT2015 || REVIT2016)
                    var p = titleBlock.GetParameters(Constants.TitleScale);
                    if (p == null || p.Count < 1) {
                        return string.Empty;
                    }
                    var s = p[0].AsValueString();
                    #else
                    string s;
                    try {
                        s = titleBlock.get_Parameter(Constants.TitleScale).AsValueString();
                    } catch {
                        return string.Empty;
                    }
                    #endif
                    var d = Convert.ToDouble(s, CultureInfo.InvariantCulture);
                    return d.ToString(CultureInfo.InvariantCulture);
            } catch (FormatException) {
                    return string.Empty;
            }    
        }

        public string FullExportPath(string extension)
        {
            return this.ExportDir + "\\" + this.fullExportName + extension;
        }

        /// <summary>
        /// Updates some of the sheet info(scale, pagesize).
        /// This could be done at startup, but in some cases
        /// it can take a while.
        /// </summary>
        public void UpdateSheetInfo()
        {
            var titleBlock = ExportManager.TitleBlockInstanceFromSheetNumber(
                this.sheetNumber, this.doc);
            if (titleBlock != null) {
                this.scale = titleBlock.get_Parameter(
                    BuiltInParameter.SHEET_SCALE).AsString();
                this.scaleBarScale = ExportSheet.GetScaleBarScale(titleBlock);
                this.width = titleBlock.get_Parameter(
                        BuiltInParameter.SHEET_WIDTH).AsDouble();
                this.height = titleBlock.get_Parameter(
                        BuiltInParameter.SHEET_HEIGHT).AsDouble();
            }
            this.pageSize = PrintSettings.GetSheetSizeAsString(this);
            this.printSetting = PrintSettings.AssignPrintSetting(
                    this.doc, this.pageSize);
            this.verified = true;
        }
        
        public string RevitScaleWithoutFormatting()
        {
                string result = this.scale.Trim();
                int i = 0;
                if (result.Contains(":")) {
                    i = result.IndexOf(':');
                } else {
                    return "0";
                }
                return string.IsNullOrEmpty(result.Trim()) ? "0" : result.Substring(i + 2).Trim();
        }
        
        public void SetSegmentedSheetName(SegmentedSheetName segmentedFileName)
        {
            this.segmentedFileName = segmentedFileName;
            this.SetExportName();
        }

        public void UpdateNumber()
        {
            this.sheetNumber = this.sheet.get_Parameter(
                    BuiltInParameter.SHEET_NUMBER).AsString(); 
            this.SetExportName();  
        }
        
        public void UpdateName()
        {
            this.sheetDescription = this.sheet.get_Parameter(
                    BuiltInParameter.SHEET_NAME).AsString();
            this.SetExportName();  
        }
        
        public void UpdateScaleBarScale()
        {
            var titleBlock = ExportManager.TitleBlockInstanceFromSheetNumber(
                this.sheetNumber, this.doc);
            this.SetScaleBarScale(titleBlock);       
        }

        public void UpdateRevision(bool refreshExportName)
        {
            this.sheetRevision = this.sheet.get_Parameter(
                    BuiltInParameter.SHEET_CURRENT_REVISION).AsString();
            this.sheetRevisionDescription = this.sheet.get_Parameter(
                    BuiltInParameter.SHEET_CURRENT_REVISION_DESCRIPTION).AsString();
            this.sheetRevisionDate = this.sheet.get_Parameter(
                    BuiltInParameter.SHEET_CURRENT_REVISION_DATE).AsString();
            this.sheetRevisionDateTime = SCaddins.Common.MiscUtilities.ToDateTime(this.sheetRevisionDate);
            if (refreshExportName) {
                this.SetExportName();
            }
        }
        
        public void SetScaleBarScale(Element titleBlock)
        {
                #if ( REVIT2015 || REVIT2016)
                var tb = titleBlock.GetParameters(Constants.TitleScale);
                if (tb == null) {
                    return;
                }
                Parameter p = tb[0];
                #else
                Parameter p = titleBlock.get_Parameter(Constants.TitleScale);
                if (p == null) {
                    return;
                }
                #endif
                p.SetValueString(this.RevitScaleWithoutFormatting());
                this.scaleBarScale = this.RevitScaleWithoutFormatting();          
        }
               
        private void Init(
                ViewSheet viewSheet,
                Document document,
                SegmentedSheetName sheetName,
                ExportManager scx)
        {
            this.doc = document;
            this.sheet = viewSheet;
            this.segmentedFileName = sheetName;
            this.verified = false;
            this.displineCode = "AD";
            this.ExportDir = scx.ExportDir;
            this.sheetNumber = viewSheet.get_Parameter(
                    BuiltInParameter.SHEET_NUMBER).AsString();
            this.sheetDescription = viewSheet.get_Parameter(
                    BuiltInParameter.SHEET_NAME).AsString();
            this.projectNumber = document.ProjectInformation.Number;
            this.width = 841;
            this.height = 594;
            this.scale = string.Empty;
            this.scaleBarScale = string.Empty;
            this.pageSize = string.Empty;
            this.id = viewSheet.Id;
            this.UpdateRevision(false);
            this.SetExportName();
        }
        
        private void PopulateSegmentedFileName()
        {
            for (int i = 0; i < this.segmentedFileName.Count; i++) {
                switch (this.segmentedFileName[i].TypeOfSegment) {
                case SegmentType.SheetNumber:
                    this.segmentedFileName[i].Text = this.sheetNumber;
                    break;
                case SegmentType.SheetName:
                    this.segmentedFileName[i].Text = this.sheetDescription;
                    break;
                case SegmentType.ProjectNumber:
                    this.segmentedFileName[i].Text = this.projectNumber;
                    break;
                case SegmentType.Discipline:
                    if (string.IsNullOrEmpty(this.segmentedFileName[i].Text.Trim())) {
                        this.segmentedFileName[i].Text = this.displineCode;
                    }
                    break;
                case SegmentType.Revision:
                    this.segmentedFileName[i].Text = this.sheetRevision;
                    break;
                case SegmentType.RevisionDescription:
                    this.segmentedFileName[i].Text =
                        this.sheetRevisionDescription;
                    break;
                case SegmentType.Hyphen:
                    this.segmentedFileName[i].Text = "-";
                    break;
                case SegmentType.Underscore:
                    this.segmentedFileName[i].Text = "_";
                    break;
                }
            }
        }

        private void SetExportName()
        {
            if (this.forceDate) {
                this.sheetRevision = MiscUtilities.GetDateString;
            } else {
                this.sheetRevision = this.sheet.get_Parameter(
                        BuiltInParameter.SHEET_CURRENT_REVISION).AsString();
            }

            if (this.sheetRevision.Length < 1) {
                this.sheetRevision = MiscUtilities.GetDateString;
            }

            this.PopulateSegmentedFileName();
            this.fullExportName = null;
            foreach (SheetNameSegment s in this.segmentedFileName) {
                this.fullExportName += s.Text;
            }
        }
    }
}

/* vim: set ts=4 sw=4 nu expandtab: */
