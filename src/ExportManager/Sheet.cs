// (C) Copyright 2012-2016 by Andrew Nicholas
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

namespace SCaddins.ExportManager
{
    using System;
    using System.ComponentModel;
    using System.Globalization;
    using System.Runtime.CompilerServices;
    using Autodesk.Revit.DB;
    using SCaddins.Common;

    public class ExportSheet : INotifyPropertyChanged
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Microsoft.Usage", "CA2213: Disposable fields should be disposed", Justification = "Parameter intialized by Revit", MessageId = "doc")]
        private Document doc;
        private string exportDirectory;
        private bool forceDate;
        private bool forceRasterPrint;
        private string fullExportName;
        private double height;
        private ElementId id;
        private bool? northPointVisible;
        private string pageSize;
        ////[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA2204:Literals should be spelled correctly", MessageId = "PrinterJobControl")]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Microsoft.Usage", "CA2213: Disposable fields should be disposed", Justification = "Parameter intialized by Revit", MessageId = "printSetting")]
        private PrintSetting printSetting;
        private string projectNumber;
        private string scale;
        private string scaleBarScale;
        private SegmentedSheetName segmentedFileName;
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Microsoft.Usage", "CA2213: Disposable fields should be disposed", Justification = "Parameter intialized by Revit", MessageId = "sheet")]
        private ViewSheet sheet;
        private string sheetDescription;
        private string sheetNumber;
        private string sheetRevision;
        private string sheetRevisionDate;
        private DateTime sheetRevisionDateTime;
        private string sheetRevisionDescription;
        private bool useDateForEmptyRevisions;
        private bool verified;
        private double width;

        public ExportSheet(
                ViewSheet sheet,
                Document doc,
                SegmentedSheetName fileNameTemplate,
                bool verifyOnCreation,
                Manager scx)
        {
            this.Init(sheet, doc, fileNameTemplate, scx);
            if (verifyOnCreation)
            {
                this.UpdateSheetInfo();
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public string ExportDirectory
        {
            get
            {
                return exportDirectory;
            }

            set
            {
                if (exportDirectory != value) {
                    exportDirectory = value;
                    NotifyPropertyChanged(nameof(ExportDirectory));
                }
            }
        }

        public bool ForceDate
        {
            get
            {
                return this.forceDate;
            }

            set
            {
                this.forceDate = value;
                this.SetExportName();
                NotifyPropertyChanged(nameof(ForceDate));
                NotifyPropertyChanged(nameof(FullExportName));
                NotifyPropertyChanged(nameof(SheetRevision));
            }
        }

        public bool ForceRasterPrint
        {
            get
            {
                return forceRasterPrint;
            }
        }

        public string FullExportName
        {
            get { return this.fullExportName; }
        }

        public double Height
        {
            get { return this.height * 304.8; }
        }

        public ElementId Id
        {
            get { return this.id; }
        }

        public bool? NorthPointVisible
        {
            get
            {
                return northPointVisible;
            }

            set
            {
                northPointVisible = value;
                NotifyPropertyChanged(nameof(NorthPointVisible));
            }
        }

        public string PageSize
        {
            get
            {
                return this.pageSize;
            }
        }

        public string PrintSettingName
        {
            get
            {
                return this.printSetting != null ? this.printSetting.Name : string.Empty;
            }
        }

        public string ProjectNumber
        {
            get
            {
                return this.projectNumber;
            }
        }

        public string Scale
        {
            get
            {
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
                    flag |= i > 0 && !result.Substring(i + 2).Equals(this.scaleBarScale.Trim(), StringComparison.CurrentCultureIgnoreCase);
                }
                if (!string.IsNullOrEmpty(this.scaleBarScale.Trim()) && flag) {
                    result += " [**" + this.scaleBarScale + "]";
                }
                return result;
            }

            set
            {
                this.scale = value;
                NotifyPropertyChanged(nameof(Scale));
            }
        }

        public PrintSetting SCPrintSetting
        {
            get
            {
                return this.printSetting;
            }
        }

        public SegmentedSheetName SegmentedFileName
        {
            get
            {
                return this.segmentedFileName;
            }     
        }

        public ViewSheet Sheet
        {
            get { return this.sheet; }
        }

        public string SheetDescription
        {
            get
            { 
                return this.sheetDescription;
            }
            
            set
            { 
                this.sheetDescription = value;
                this.SetExportName();
                NotifyPropertyChanged();
            }
        }

        public string SheetNumber
        {
            get
            {
                return this.sheetNumber;
            }
            
            set
            { 
                this.sheetNumber = value;
                this.SetExportName();
                NotifyPropertyChanged();
            }
        }

        public string SheetRevision
        {
            get
            {
                return this.sheetRevision ?? "-";
            }
        }

        public string SheetRevisionDate
        {
            get
            {
                return this.sheetRevisionDate ?? "-";
            }
        }

        public DateTime SheetRevisionDateTime
        {
            get
            {
                return this.sheetRevisionDateTime;
            }
        }

        public string SheetRevisionDescription
        {
            get
            {
                return this.sheetRevisionDescription ?? "-";
            }
        }

        public bool UseDateForEmptyRevisions
        {
            get
            {
                return this.useDateForEmptyRevisions;
            }

            set
            {
                this.useDateForEmptyRevisions = value;
                this.SetExportName();
                NotifyPropertyChanged(nameof(UseDateForEmptyRevisions));
                NotifyPropertyChanged(nameof(FullExportName));
                NotifyPropertyChanged(nameof(SheetRevision));
            }
        }

        public bool ValidScaleBar
        {
            get { return this.RevitScaleWithoutFormatting() == this.scaleBarScale.Trim(); }
        }

        public bool Verified
        {
            get { return this.verified; }
        }

        public double Width
        {
            get { return this.width * 304.8; }
        }

        public static bool? GetNorthPointVisibility(Element titleBlock)
        {
            if (titleBlock == null) {
                return null;
            }
            try {
                    var p = titleBlock.GetParameters(Settings1.Default.NorthPointVisibilityParameter);
                    if (p == null || p.Count < 1) {
                        return null;
                    }
                    int d = p[0].AsInteger();
                    if (d == 0) {
                        return false;
                    }
                    if (d == 1) {
                        return true;
                    }
                    return null;
            } catch (FormatException) {
                    return null;
            }    
        }

        public static string GetScaleBarScale(Element titleBlock)
        {
            if (titleBlock == null)
            {
                return string.Empty;
            }
            try
            {
                var p = titleBlock.GetParameters(Settings1.Default.ScalebarScaleParameter);
                if (p == null || p.Count < 1)
                {
                    return string.Empty;
                }
                double d = p[0].AsDouble();
                return d.ToString(CultureInfo.InvariantCulture);
            }
            catch (FormatException)
            {
                return string.Empty;
            }
        }

        public string FullExportPath(string extension)
        {
            return this.ExportDirectory + "\\" + this.fullExportName + extension;
        }

        public Parameter ParamFromString(string name)
        {
            if (this.Sheet.GetParameters(name).Count > 0) {
                return this.Sheet.GetParameters(name)[0];
            }
            return null;
        }

        public string RevitScaleWithoutFormatting()
        {
            string result = this.scale.Trim();
            int i = 0;
            if (result.Contains(":"))
            {
                i = result.IndexOf(':');
            }
            else
            {
                return "0";
            }
            return string.IsNullOrEmpty(result.Trim()) ? "0" : result.Substring(i + 2).Trim();
        }

        public void SetScaleBarScale(Element titleBlock)
        {
            string titleScale = SCaddins.ExportManager.Settings1.Default.ScalebarScaleParameter;
            if (string.IsNullOrEmpty(titleScale) || titleBlock == null)
            {
                return;
            }

            var tb = titleBlock.GetParameters(titleScale);
            if (tb == null || tb.Count < 1)
            {
                return;
            }

            Parameter p = tb[0];
            p.SetValueString(this.RevitScaleWithoutFormatting());
            this.scaleBarScale = this.RevitScaleWithoutFormatting();
            NotifyPropertyChanged(nameof(Scale));
        }

        public void SetSegmentedSheetName(SegmentedSheetName newSegmentedFileName)
        {
            this.segmentedFileName = newSegmentedFileName;
            this.SetExportName();
            NotifyPropertyChanged(nameof(SheetNumber));
            NotifyPropertyChanged(nameof(FullExportName));
        }

        public void ToggleNorthPoint(bool turnOn)
        {
            var titleBlock = Manager.TitleBlockInstanceFromSheetNumber(
                this.sheetNumber, this.doc);

            string northPointVisibility = SCaddins.ExportManager.Settings1.Default.NorthPointVisibilityParameter;

            var tb = titleBlock.GetParameters(northPointVisibility);
            if (tb == null || tb.Count < 1)
            {
                return;
            }
            Parameter p = tb[0];
            int b = p.AsInteger();
            if (b == 2)
            {
                return;
            }
            b = turnOn == true ? 1 : 0;
            p.Set(b);
            NorthPointVisible = turnOn;
        }

        public override string ToString()
        {
            string s = "Sheet information: " + Environment.NewLine +
                "    SheetRevisionDateTime={0}," + Environment.NewLine +
                "    Doc={1}," + Environment.NewLine +
                "    Id={2}," + Environment.NewLine +
                "    PrintSetting={3}," + Environment.NewLine +
                "    SegmentedFileName={4}," + Environment.NewLine +
                "    Sheet={5}," + Environment.NewLine +
                "    ForceDate={6}," + Environment.NewLine +
                "    Verified={7}," + Environment.NewLine +
                "    Height={8}," + Environment.NewLine +
                "    Width={9}," + Environment.NewLine +
                "    FullExportName={10}," + Environment.NewLine +
                "    PageSize={11}," + Environment.NewLine +
                "    ProjectNumber={12}," + Environment.NewLine +
                "    Scale={13}," + Environment.NewLine +
                "    ScaleBarScale={14}," + Environment.NewLine +
                "    SheetDescription={15}," + Environment.NewLine +
                "    SheetNumber={16}," + Environment.NewLine +
                "    SheetRevision={17}," + Environment.NewLine +
                "    SheetRevisionDate={18}," + Environment.NewLine +
                "    SheetRevisionDescription={19}," + Environment.NewLine +
                "    ExportDir={20}";
            return string.Format(
                System.Globalization.CultureInfo.CurrentCulture,
                s,
                this.sheetRevisionDateTime,
                this.doc.PathName,
                this.id,
                this.printSetting,
                this.segmentedFileName,
                this.sheet,
                this.forceDate,
                this.verified,
                this.height,
                this.width,
                this.fullExportName,
                this.pageSize,
                this.projectNumber,
                Scale,
                this.scaleBarScale,
                this.sheetDescription,
                this.sheetNumber,
                this.sheetRevision,
                this.sheetRevisionDate,
                this.sheetRevisionDescription,
                this.ExportDirectory);
        }

        public void UpdateName()
        {
            this.sheetDescription = this.sheet.get_Parameter(
                    BuiltInParameter.SHEET_NAME).AsString();
            this.SetExportName();
            NotifyPropertyChanged(nameof(SheetDescription));
            NotifyPropertyChanged(nameof(FullExportName));
        }

        public void UpdateNumber()
        {
            this.sheetNumber = this.sheet.get_Parameter(
                    BuiltInParameter.SHEET_NUMBER).AsString();
            this.SetExportName();
            NotifyPropertyChanged(nameof(SheetNumber));
            NotifyPropertyChanged(nameof(FullExportName));
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
            if (refreshExportName)
            {
                this.SetExportName();
            }
            NotifyPropertyChanged(nameof(SheetRevision));
            NotifyPropertyChanged(nameof(SheetRevisionDescription));
            NotifyPropertyChanged(nameof(SheetRevisionDate));
            NotifyPropertyChanged(nameof(FullExportName));
        }

        public void UpdateScaleBarScale()
        {
            var titleBlock = Manager.TitleBlockInstanceFromSheetNumber(
                this.sheetNumber, this.doc);
            if (titleBlock != null)
            {
                this.SetScaleBarScale(titleBlock);
            }
        }

        /// <summary>
        /// Updates some of the sheet info(scale, pagesize).
        /// This could be done at startup, but in some cases
        /// it can take a while.
        /// </summary>
        public void UpdateSheetInfo()
        {
            var titleBlock = Manager.TitleBlockInstanceFromSheetNumber(
                this.sheetNumber, this.doc);
            if (titleBlock != null) {
                this.scale = titleBlock.get_Parameter(
                    BuiltInParameter.SHEET_SCALE).AsString();
                this.scaleBarScale = ExportSheet.GetScaleBarScale(titleBlock);
                NorthPointVisible = ExportSheet.GetNorthPointVisibility(titleBlock);
                this.width = titleBlock.get_Parameter(
                        BuiltInParameter.SHEET_WIDTH).AsDouble();
                this.height = titleBlock.get_Parameter(
                        BuiltInParameter.SHEET_HEIGHT).AsDouble();
            }
            this.pageSize = PrintSettings.GetSheetSizeAsString(this);
            this.printSetting = PrintSettings.GetPrintSettingByName(this.doc, this.pageSize, this.forceRasterPrint);
            this.verified = true;
            NotifyPropertyChanged(nameof(Scale));
            NotifyPropertyChanged(nameof(PrintSettingName));
        }

        private void Init(
                ViewSheet viewSheet,
                Document document,
                SegmentedSheetName sheetName,
                Manager scx)
        {
            this.doc = document;
            this.sheet = viewSheet;
            this.segmentedFileName = sheetName;
            this.verified = false;
            this.ExportDirectory = scx.ExportDirectory;
            this.sheetNumber = viewSheet.get_Parameter(
                    BuiltInParameter.SHEET_NUMBER).AsString();
            this.sheetDescription = viewSheet.get_Parameter(
                    BuiltInParameter.SHEET_NAME).AsString();
            this.projectNumber = document.ProjectInformation.Number;
            this.width = 841;
            this.height = 594;
            this.scale = string.Empty;
            this.scaleBarScale = string.Empty;
            NorthPointVisible = null;
            this.pageSize = string.Empty;
            this.id = viewSheet.Id;
            this.ForceDate = scx.ForceRevisionToDateString;
            this.forceRasterPrint = UseRasterPrinting(Manager.ForceRasterPrintParameterName);
            this.useDateForEmptyRevisions = scx.UseDateForEmptyRevisions;
            this.UpdateRevision(false);
            this.SetExportName();
        }

        private bool UseRasterPrinting(string parameterName)
        {
            bool result = false;
            if (!string.IsNullOrEmpty(parameterName)) {
                var parameters = this.sheet.GetParameters(parameterName);
                if (parameters.Count == 1) {
                    result = parameters[0].HasValue && parameters[0].StorageType == StorageType.Integer && parameters[0].AsInteger() == 1;
                }
            }
            ////if (result) {
                ////Autodesk.Revit.UI.TaskDialog.Show("test", "Raster Printing Enabled for sheet " + this.SheetNumber);
            ////}
            return result;
        }

        private void NotifyPropertyChanged([CallerMemberName] string propertyName = "")
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        private string PopulateSegmentedFileName()
        {
            return PostExportHookCommand.FormatConfigurationString(this, this.segmentedFileName.NameFormat, string.Empty);
        }

        private void SetExportName()
        {
            if (this.forceDate) {
                this.sheetRevision = MiscUtilities.GetDateString;
            } else {
                this.sheetRevision = this.sheet.get_Parameter(
                    BuiltInParameter.SHEET_CURRENT_REVISION).AsString();
            }

            if (this.sheetRevision.Length < 1 && this.useDateForEmptyRevisions) {
                this.sheetRevision = MiscUtilities.GetDateString;
            }

            this.fullExportName = this.PopulateSegmentedFileName();
        }
    }
}

/* vim: set ts=4 sw=4 nu expandtab: */
