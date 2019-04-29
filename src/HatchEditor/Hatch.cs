using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autodesk.Revit.DB;

namespace SCaddins.HatchEditor
{
    public class Hatch
    {
        public Hatch()
        {
            HatchPattern = null;
            Name = string.Empty;
        }

        public Hatch(FillPattern pattern)
        {
            HatchPattern = pattern;
            Name = pattern.Name;
        }

        public FillPattern HatchPattern {
            get; set;
        }

        public string Name {
            get; set;
        }

        public override string ToString()
        {
            return GetPatternDefinition();
        }

        public string GetPatternDefinition()
        {
            if (HatchPattern == null) {
                return "Hatch not defined";
            }
            StringBuilder s = new StringBuilder();
            foreach (var p in HatchPattern.GetFillGrids()) {
                s.Append(string.Format("{0},\t{1},\t{2},\t{3},\t{4}", p.Angle.ToDeg(), p.Origin.U.ToMM(), p.Origin.V.ToMM(), p.Offset.ToMM(), p.Shift.ToMM()));
                int i = 0;
                foreach (var d in p.GetSegments()) {
                    int m = i % 2 == 0 ? 1 : -1;
                    i++;
                    s.Append(string.Format(",\t{0}", d.ToMM() * m));
                }
                s.Append(System.Environment.NewLine);
            }
            return s.ToString();
        }

        public bool TryAssignFillGridsFromStrings(string[] grids)
        {
            if (AssignFillGridsFromString(grids)) {
                return true;
            } else {
                HatchPattern = null;
                return false;
            }
        }

        private bool AssignFillGridsFromString(string[] grids)
        {
            HatchPattern = new FillPattern();
            foreach (string s in grids) {
                string[] segs = s.Split(',');
                if (segs.Count() < 5) {
                    return false;
                }
                var f = new FillGrid();
                double angle = 0;
                double x = 0;
                double y = 0;
                double shift = 0;
                double offset = 0;
                List<double> lineSegs = new List<Double>();
                if (!double.TryParse(segs[0], out angle)) {
                    return false;
                }
                if (!double.TryParse(segs[1], out x)) {
                    return false;
                }
                if (!double.TryParse(segs[2], out y)) {
                    return false;
                }
                if (!double.TryParse(segs[3], out shift)) {
                    return false;
                }
                if (!double.TryParse(segs[4], out offset)) {
                    return false;
                }
                for (int i = 5; i < segs.Length; i++) {
                    int dir = i % 2 == 1 ? 1 : -1;
                    double individualSeg;
                    if (!double.TryParse(segs[i], out individualSeg)) {
                        return false;
                    }
                    lineSegs.Add(individualSeg * dir);
                }
                f.Angle = angle;
                f.Origin = new UV(x, y);
                f.Shift = shift;
                f.Offset = offset;
                f.SetSegments(lineSegs);
                HatchPattern.SetFillGrid(0, f); 
            }
            return true;
        }
    }
}
