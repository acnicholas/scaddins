// (C) Copyright 2019-2020 by Andrew Nicholas
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

namespace SCaddins.HatchEditor
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Linq;
    using System.Runtime.CompilerServices;
    using System.Text;
    using Autodesk.Revit.DB;

    public class Hatch : INotifyPropertyChanged
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Microsoft.Usage", "CA2213: Disposable fields should be disposed", Justification = "Parameter intialized by Revit", MessageId = "fillPattern")]
        private readonly FillPattern fillPattern;
        private string definition;
        private string name;

        public Hatch() : this(new FillPattern() { Name = string.Empty })
        {
        }

        public Hatch(FillPattern pattern)
        {
            fillPattern = pattern;
            UpdatePatternDefinition();
            Name = pattern.Name;
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public string Definition
        {
            get
            {
                return definition;
            }

            set
            {
                definition = value;
                var lines = definition.Split(new[] { Environment.NewLine }, StringSplitOptions.None);
                TryAssignFillGridsFromStrings(lines, 1, 0);
                NotifyPropertyChanged(nameof(Definition));
            }
        }

        public FillPattern HatchPattern => fillPattern;

        public bool IsDrafting => fillPattern.Target == FillPatternTarget.Drafting;

        public string Name
        {
            get => name;

            set
            {
                name = value;
                HatchPattern.Name = name;
            }
        }

        public string PatFileString
        {
            get
            {
                var s = new StringBuilder();
                s.AppendLine(@";%VERSION=3.0");
                s.AppendLine(@";%UNITS=MM");
                s.AppendLine();
                s.Append(@"*");
                s.AppendLine(Name);
                var type = IsDrafting ? @";%TYPE=DRAFTING" : @";%TYPE=MODEL";
                s.AppendLine(type);
                s.AppendLine(Definition);
                return s.ToString();
            }
        }

        public Hatch Clone()
        {
            var result = new Hatch
            {
                Definition = Definition,
                HatchPattern =
                {
                    Target = HatchPattern.Target
                },
                Name = Name
            };
            return result;
        }

        public void Rotate(double angle)
        {
            var lines = definition.Split(new[] { Environment.NewLine }, StringSplitOptions.None);
            TryAssignFillGridsFromStrings(lines, 1, angle);
            UpdatePatternDefinition();
        }

        public void Scale(double scale)
        {
            var lines = definition.Split(new[] { Environment.NewLine }, StringSplitOptions.None);
            TryAssignFillGridsFromStrings(lines, scale, 0);
            UpdatePatternDefinition();
        }

        public bool TryAssignFillGridsFromStrings(string[] grids, double scale, double angle)
        {
            return AssignFillGridsFromString(grids, scale, angle);
        }

        public void UpdatePatternDefinition()
        {
            if (fillPattern == null)
            {
                SCaddinsApp.WindowManager.ShowMessageBox("Null Hatch");
                return;
            }
            StringBuilder s = new StringBuilder();
            foreach (var p in fillPattern.GetFillGrids())
            {
                double angle = p.Angle.ToDeg();
                s.Append(string.Format(System.Globalization.CultureInfo.InvariantCulture, "{0},\t{1},\t{2},\t{3},\t{4}", angle, p.Origin.U.ToMM(), p.Origin.V.ToMM(), p.Shift.ToMM(), p.Offset.ToMM()));
                foreach (var d in p.GetSegments())
                {
                    s.Append(string.Format(System.Globalization.CultureInfo.InvariantCulture, ",\t{0}", d.ToMM()));
                }
                s.Append(Environment.NewLine);
            }
            definition = s.ToString();
        }

        private bool AssignFillGridsFromString(string[] grids, double scale, double rotationAngle)
        {
            var newFillGrids = new List<FillGrid>();
            foreach (var s in grids)
            {
                if (string.IsNullOrEmpty(s.Trim()))
                {
                    continue;
                }
                string[] segs = s.Split(',');
                if (segs.Count() < 5)
                {
                    return false;
                }
                var f = new FillGrid();
                List<double> lineSegs = new List<double>();
                if (!double.TryParse(segs[0], out var angle))
                {
                    return false;
                }
                if (!double.TryParse(segs[1], out var x))
                {
                    return false;
                }
                if (!double.TryParse(segs[2], out var y))
                {
                    return false;
                }
                if (!double.TryParse(segs[3], out var shift))
                {
                    return false;
                }
                if (!double.TryParse(segs[4], out var offset))
                {
                    return false;
                }
                for (int i = 5; i < segs.Length; i++)
                {
                    if (!double.TryParse(segs[i], out var individualSeg))
                    {
                        return false;
                    }
                    individualSeg *= scale;
                    lineSegs.Add(Math.Abs(individualSeg.ToFeet()));
                }
                x *= scale;
                y *= scale;
                shift *= scale;
                offset *= scale;
                angle += rotationAngle;
                f.Angle = angle.ToRad();
                f.Origin = new UV(x.ToFeet(), y.ToFeet());
                f.Shift = shift.ToFeet();
                f.Offset = offset.ToFeet();
                f.SetSegments(lineSegs);
                newFillGrids.Add(f);
            }
            fillPattern.SetFillGrids(newFillGrids);
            return true;
        }

        private void NotifyPropertyChanged([CallerMemberName] string propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
