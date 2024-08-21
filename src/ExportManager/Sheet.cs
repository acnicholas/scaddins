// (C) Copyright 2012-2023 by Andrew Nicholas
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
    using Common;

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
        private bool? northPointIsVisible;
        private bool appearsInSheetList;
        private string pageSize;
        ////[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA2204:Literals should be spelled correctly", MessageId = "PrinterJobControl")]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Microsoft.Usage", "CA2213: Disposable fields should be disposed", Justification = "Parameter intialized by Revit", MessageId = "printSetting")]
        private PrintSetting printSetting = null;
        private string projectNumber;
        private string scale;
        private string scaleBarScale;
        private SegmentedSheetName segmentedFileName;
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Microsoft.Usage", "CA2213: Disposable fields should be disposed", Justification = "Parameter intialized by Revit", MessageId = "sheet")]
        private ViewSheet sheet;
        private string customParameter01;
        private string sheetDescription;
        private string sheetNumber;
        private string sheetRevision;
        private string sheetRevisionDate;
        private DateTime sheetRevisionDateTime;
        private string sheetRevisionDescription;
        private bool useDateForEmptyRevisions;
        private bool verified;
        private bool validPrintSettingIsAssigned;
        private double width;

        public ExportSheet(
                ViewSheet sheet,
                Document doc,
                SegmentedSheetName fileNameTemplate,
                bool verifyOnCreation,
                Manager scx)
        {
            Init(sheet, doc, fileNameTemplate, scx);
            if (verifyOnCreation)
            {
                UpdateSheetInfo();
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public string CustomParameter01
        {
            get
            {
                return customParameter01 != null ? customParameter01 : string.Empty;
            }

            set
            {
                if (customParameter01 != value)
                {
                    customParameter01 = value;
                    NotifyPropertyChanged(nameof(CustomParameter01));
                }
            }
        }

        public string ExportDirectory
        {
            get
            {
                return exportDirectory;
            }

            set
            {
                if (exportDirectory != value)
                {
                    exportDirectory = value;
                    NotifyPropertyChanged(nameof(ExportDirectory));
                }
            }
        }

        public bool ForceDate
        {
            get
            {
                return forceDate;
            }

            set
            {
                forceDate = value;
                SetExportName();
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

            set
            {
                forceRasterPrint = value;
                NotifyPropertyChanged(nameof(ForceRasterPrint));
            }
        }

        public string FullExportName
        {
            get { return fullExportName; }
        }

        public double Height
        {
            get { return height * 304.8; }
        }

        public ElementId Id
        {
            get { return id; }
        }

        public bool AppearsInSheetList
        {
            get
            {
                return appearsInSheetList;
            }

            set
            {
                appearsInSheetList = value;
                int i = appearsInSheetList ? 1 : 0;
                this.Sheet.get_Parameter(BuiltInParameter.SHEET_SCHEDULED).Set(i);
                NotifyPropertyChanged(nameof(AppearsInSheetList));
            }
        }

        public bool? NorthPointVisible
        {
            get
            {
                return northPointIsVisible;
            }

            set
            {
                northPointIsVisible = value;
                NotifyPropertyChanged(nameof(NorthPointVisible));
                NotifyPropertyChanged(nameof(NorthPointVisibilityString));
            }
        }

        public string NorthPointVisibilityString
        {
            get
            {
                return (NorthPointVisible.HasValue && NorthPointVisible != null) ? NorthPointVisible.ToString() : @"n/a";
            }
        }

        public string PageSize
        {
            get
            {
                return pageSize;
            }
        }

        public string PrintSettingName
        {
            get
            {
#if REVIT2022 || REVIT2023 || REVIT2024 || REVIT2025
                return SegmentedFileName.Name;
#else
                return printSetting != null ? printSetting.Name : string.Empty;
#endif
            }
        }

        public string ProjectNumber
        {
            get
            {
                return projectNumber;
            }
        }

        public string Scale
        {
            get
            {
                string result = scale.Trim();
                int i = 0;
                if (result.Contains(":"))
                {
                    i = result.IndexOf(':');
                }
                bool flag = false;
                if (string.IsNullOrEmpty(result.Trim()))
                {
                    result = "0";
                }
                if (!string.IsNullOrEmpty(scaleBarScale))
                {
                    flag |= i > 0 && !result.Substring(i + 2).Equals(scaleBarScale.Trim(), StringComparison.CurrentCultureIgnoreCase);
                }
                if (!string.IsNullOrEmpty(scaleBarScale.Trim()) && flag)
                {
                    result += " [**" + scaleBarScale + "]";
                }
                return result;
            }

            set
            {
                scale = value;
                NotifyPropertyChanged(nameof(Scale));
            }
        }

        public PrintSetting SCPrintSetting
        {
            get
            {
                return printSetting != null ? printSetting : null;
            }
        }

        public SegmentedSheetName SegmentedFileName
        {
            get
            {
                return segmentedFileName;
            }
        }

        public ViewSheet Sheet
        {
            get { return sheet; }
        }

        public string SheetDescription
        {
            get
            {
                return sheetDescription;
            }

            set
            {
                sheetDescription = value;
                SetExportName();
                NotifyPropertyChanged();
            }
        }

        //public override string ToString()
        //{
        //    return SheetNumber.ToString() + "-" + SheetDescription;
        //}

        public string SheetNumber
        {
            get
            {
                return sheetNumber;
            }

            set
            {
                sheetNumber = value;
                SetExportName();
                NotifyPropertyChanged();
            }
        }

        public string SheetRevision
        {
#if REVIT2022 || REVIT2023 || REVIT2024 || REVIT2025
            get
            {
                return sheetRevision != string.Empty ? sheetRevision : "-";
            }
#else
            get
            {
                return sheetRevision ?? "-";
            }
#endif
        }

        public string SheetRevisionDate
        {
            get
            {
                return sheetRevisionDate ?? "-";
            }
        }

        public DateTime SheetRevisionDateTime
        {
            get
            {
                return sheetRevisionDateTime;
            }
        }

        public string SheetRevisionDescription
        {
            get
            {
                return sheetRevisionDescription ?? "-";
            }
        }

        public bool UseDateForEmptyRevisions
        {
            get
            {
                return useDateForEmptyRevisions;
            }

            set
            {
                useDateForEmptyRevisions = value;
                SetExportName();
                NotifyPropertyChanged(nameof(UseDateForEmptyRevisions));
                NotifyPropertyChanged(nameof(FullExportName));
                NotifyPropertyChanged(nameof(SheetRevision));
            }
        }

        public bool ValidPrintSettingIsAssigned
        {
            get
            {
                return validPrintSettingIsAssigned;
            }

            private set
            {
                validPrintSettingIsAssigned = value;
                NotifyPropertyChanged(nameof(ValidPrintSettingIsAssigned));
            }
        }

        public bool ValidExportName
        {
            get
            {
                var validName = SheetDescription.IndexOfAny(Settings1.Default.InvalidSheetNameChars.ToCharArray()) < 0;
                var validNumber = SheetNumber.IndexOfAny(Settings1.Default.InvalidSheetNumberChars.ToCharArray()) < 0;
                return FileUtilities.IsValidFileName(FullExportName) && validName && validNumber;
            }
        }

        public bool ValidScaleBar
        {
            get { return RevitScaleWithoutFormatting() == scaleBarScale.Trim(); }
        }

        public bool ScaleBarError
        {
            get { return Scale.Contains(@"*"); }
        }

        public bool Verified
        {
            get { return verified; }
        }

        public double Width
        {
            get { return width * 304.8; }
        }

        public static string GetCustomParameterValueAsString(ViewSheet sheet)
        {
            Parameter p = sheet.LookupParameter(Settings1.Default.CustomSCExportParameter01);
            if (p != null)
            {
                return p.AsValueString();
            }
            return @"N\A";
        }

        public static bool? GetNorthPointVisibility(Element titleBlock)
        {
            if (titleBlock == null)
            {
                return null;
            }
            try
            {
                var p = titleBlock.GetParameters(Settings1.Default.NorthPointVisibilityParameter);
                if (p == null || p.Count < 1)
                {
                    return null;
                }
                int d = p[0].AsInteger();
                if (d == 0)
                {
                    return false;
                }
                if (d == 1)
                {
                    return true;
                }
                return null;
            }
            catch (FormatException)
            {
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
            return ExportDirectory + "\\" + fullExportName + extension;
        }

        public Parameter ParamFromString(string name)
        {
            if (Sheet.GetParameters(name).Count > 0)
            {
                return Sheet.GetParameters(name)[0];
            }
            return null;
        }

        public string RevitScaleWithoutFormatting()
        {
            var result = scale.Trim();
            var i = 0;
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
            string titleScale = Settings1.Default.ScalebarScaleParameter;
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
            p.SetValueString(RevitScaleWithoutFormatting());
            scaleBarScale = RevitScaleWithoutFormatting();
            NotifyPropertyChanged(nameof(Scale));
            NotifyPropertyChanged(nameof(ScaleBarError));
        }

        public void SetSegmentedSheetName(SegmentedSheetName newSegmentedFileName)
        {
            segmentedFileName = newSegmentedFileName;
            SetExportName();
            NotifyPropertyChanged(nameof(SheetNumber));
            NotifyPropertyChanged(nameof(FullExportName));
        }

        public void ToggleNorthPoint(bool turnOn)
        {
            var titleBlock = Manager.TitleBlockInstanceFromSheetNumber(
                sheetNumber, doc);

            string northPointVisibility = Settings1.Default.NorthPointVisibilityParameter;

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
            b = turnOn ? 1 : 0;
            p.Set(b);
            NorthPointVisible = turnOn;
        }

        public void ToggleParameterByName(bool turnOn, string paramName)
        {
            var sheet = this.Sheet.GetParameters(paramName);
            if (sheet == null || sheet.Count < 1)
            {
                return;
            }
            Parameter p = sheet[0];
            int b = p.AsInteger();
            if (b == 2)
            {
                return;
            }
            b = turnOn ? 1 : 0;
            if (p.IsReadOnly == false)
            {
                p.Set(b);
            }
            UpdateSheetInfo();
        }

        public void ToggleTitleParameterByName(bool turnOn, string paramName)
        {
            var titleBlock = Manager.TitleBlockInstanceFromSheetNumber(
                sheetNumber, doc);

            var tb = titleBlock.GetParameters(paramName);
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
            b = turnOn ? 1 : 0;
            if (p.IsReadOnly == false)
            {
                p.Set(b);
            }
        }

        public override string ToString()
        {
#if REVIT2022 || REVIT2023 || REVIT2024 || REVIT2025
            string printSetting = SegmentedFileName.Name;
#endif

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
                "    ForceRasterPrint={20}," + Environment.NewLine +
                "    ExportDir={21}";
            return string.Format(
                CultureInfo.CurrentCulture,
                s,
                sheetRevisionDateTime,
                doc.PathName,
                id,
                printSetting,
                segmentedFileName,
                sheet,
                forceDate,
                verified,
                height,
                width,
                fullExportName,
                pageSize,
                projectNumber,
                Scale,
                scaleBarScale,
                sheetDescription,
                sheetNumber,
                sheetRevision,
                sheetRevisionDate,
                sheetRevisionDescription,
                ForceRasterPrint,
                ExportDirectory);
        }

        public void Refresh()
        {
            NotifyPropertyChanged(nameof(ValidExportName));
        }

        public void UpdateName()
        {
            sheetDescription = sheet.get_Parameter(
                    BuiltInParameter.SHEET_NAME).AsString();
            SetExportName();
            NotifyPropertyChanged(nameof(SheetDescription));
            NotifyPropertyChanged(nameof(FullExportName));
            NotifyPropertyChanged(nameof(ValidExportName));
        }

        public void UpdateNumber()
        {
            sheetNumber = sheet.get_Parameter(
                    BuiltInParameter.SHEET_NUMBER).AsString();
            SetExportName();
            NotifyPropertyChanged(nameof(SheetNumber));
            NotifyPropertyChanged(nameof(FullExportName));
            NotifyPropertyChanged(nameof(ValidExportName));
        }

        public void UpdateRevision(bool refreshExportName)
        {
            sheetRevision = sheet.get_Parameter(
                    BuiltInParameter.SHEET_CURRENT_REVISION).AsString();
            sheetRevisionDescription = sheet.get_Parameter(
                    BuiltInParameter.SHEET_CURRENT_REVISION_DESCRIPTION).AsString();
            sheetRevisionDate = sheet.get_Parameter(
                    BuiltInParameter.SHEET_CURRENT_REVISION_DATE).AsString();
            sheetRevisionDateTime = MiscUtilities.ToDateTime(sheetRevisionDate);
            if (refreshExportName)
            {
                SetExportName();
            }
            NotifyPropertyChanged(nameof(SheetRevision));
            NotifyPropertyChanged(nameof(SheetRevisionDescription));
            NotifyPropertyChanged(nameof(SheetRevisionDate));
            NotifyPropertyChanged(nameof(FullExportName));
        }

        public void UpdateScaleBarScale()
        {
            var titleBlock = Manager.TitleBlockInstanceFromSheetNumber(
                sheetNumber, doc);
            if (titleBlock != null)
            {
                SetScaleBarScale(titleBlock);
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
                sheetNumber, doc);
            if (titleBlock != null)
            {
                scale = titleBlock.get_Parameter(
                    BuiltInParameter.SHEET_SCALE).AsString();
                scaleBarScale = GetScaleBarScale(titleBlock);
                CustomParameter01 = GetCustomParameterValueAsString(this.Sheet);
                NorthPointVisible = GetNorthPointVisibility(titleBlock);
                width = titleBlock.get_Parameter(
                        BuiltInParameter.SHEET_WIDTH).AsDouble();
                height = titleBlock.get_Parameter(
                        BuiltInParameter.SHEET_HEIGHT).AsDouble();
            }
            appearsInSheetList = this.Sheet.get_Parameter(BuiltInParameter.SHEET_SCHEDULED).AsInteger() == 1;
            pageSize = PrintSettings.GetSheetSizeAsString(this);
#if !REVIT2022 && !REVIT2023 && !REVIT2024 || REVIT2025
            printSetting = PrintSettings.GetPrintSettingByName(doc, pageSize, forceRasterPrint);
            if (printSetting == null)
            {
                printSetting = PrintSettings.GetPrintSettingByName(doc, pageSize, forceRasterPrint);
            }
#endif
            verified = true;
#if !REVIT2022 && !REVIT2023 && !REVIT2024 && !REVIT2025
            ValidPrintSettingIsAssigned = printSetting != null;
#else
            ValidPrintSettingIsAssigned = true;
#endif
            NotifyPropertyChanged(nameof(Scale));
            NotifyPropertyChanged(nameof(PrintSettingName));
            NotifyPropertyChanged(nameof(ValidExportName));
        }

        private void Init(
                ViewSheet viewSheet,
                Document document,
                SegmentedSheetName sheetName,
                Manager scx)
        {
            doc = document;
            sheet = viewSheet;
            segmentedFileName = sheetName;
            verified = false;
            validPrintSettingIsAssigned = false;
            ExportDirectory = scx.ExportDirectory;
            sheetNumber = viewSheet.get_Parameter(
                    BuiltInParameter.SHEET_NUMBER).AsString();
            sheetDescription = viewSheet.get_Parameter(
                    BuiltInParameter.SHEET_NAME).AsString();
            projectNumber = document.ProjectInformation.Number;
            width = 841;
            height = 594;
            scale = string.Empty;
            scaleBarScale = string.Empty;
            NorthPointVisible = null;
            pageSize = string.Empty;
            id = viewSheet.Id;
            ForceDate = scx.ForceRevisionToDateString;
            ForceRasterPrint = UseRasterPrinting(Settings1.Default.UseRasterPrinterParameter);
            useDateForEmptyRevisions = scx.UseDateForEmptyRevisions;
            UpdateRevision(false);
            SetExportName();
        }

        private bool UseRasterPrinting(string parameterName)
        {
            if (!string.IsNullOrEmpty(parameterName))
            {
                Parameter parameter = this.sheet.LookupParameter(parameterName);
                if (parameter != null)
                {
                    return parameter.StorageType == StorageType.Integer && parameter.AsInteger() == 1;
                }
            }
            return false;
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
#if REVIT2022 || REVIT2023 || REVIT2024 || REVIT2025
            var opts = this.SegmentedFileName.PDFExportOptions;
            if (opts != null)
            {
                return NativeNamingRulesUtils.GetExportNameFromNamingRule(opts, this);
            }
#endif
            return PostExportHookCommand.FormatConfigurationString(this, segmentedFileName.NameFormat, string.Empty);
        }

        private void SetExportName()
        {
#if REVIT2022 || REVIT2023 || REVIT2024 || REVIT2025
            sheetRevision = sheet.get_Parameter(
                    BuiltInParameter.SHEET_CURRENT_REVISION).AsString();
#else
            if (forceDate) {
                sheetRevision = MiscUtilities.GetDateString;
            } else {
                sheetRevision = sheet.get_Parameter(
                    BuiltInParameter.SHEET_CURRENT_REVISION).AsString();
            }

            if (sheetRevision.Length < 1 && useDateForEmptyRevisions) {
                sheetRevision = MiscUtilities.GetDateString;
            }
#endif
            fullExportName = PopulateSegmentedFileName();
        }
    }
}

/* vim: set ts=4 sw=4 nu expandtab: */
