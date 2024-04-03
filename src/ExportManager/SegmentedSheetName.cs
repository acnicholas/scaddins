// (C) Copyright 2012-2020 by Andrew Nicholas
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
    using System.Collections.ObjectModel;
    using System.ComponentModel;
    using System.Globalization;
    using System.Runtime.CompilerServices;
    using Autodesk.Revit.DB;

    public class SegmentedSheetName : INotifyPropertyChanged
    {
        private string name;
        private string nameFormat;

        public SegmentedSheetName()
        {
            Hooks = new Collection<string>();
            // ReSharper disable once StringLiteralTypo
            Name = "YYYYMMDD-AD-NNN[R]";
#if REVIT2022 || REVIT2023 || REVIT2024 || REVIT2025
            PDFExportOptions = null;
#endif
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public string Name
        {
            get
            {
                return name;
            }

            set
            {
                name = value;
                NotifyPropertyChanged(nameof(Name));
            }
        }

        public string NameFormat
        {
            get
            {
                return nameFormat;
            }

            set
            {
                nameFormat = value;
                NotifyPropertyChanged(nameof(NameFormat));
            }
        }

#if REVIT2022 || REVIT2023 || REVIT2024 || REVIT2025
        public PDFExportOptions PDFExportOptions { get; set; }
#endif

        public Collection<string> Hooks { get; }

        public void AddHook(string hookName)
        {
            Hooks.Add(hookName);
        }

        public override string ToString()
        {
            return string.Format(CultureInfo.InvariantCulture, "[SegmentedSheetName Hooks={0}, Name={1}, NameFormat={2}]", Hooks, Name, NameFormat);
        }

        private void NotifyPropertyChanged([CallerMemberName] string propertyName = "")
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }
    }
}

/* vim: set ts=4 sw=4 nu expandtab: */
