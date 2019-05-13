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
        private FillPattern fillPattern;
        private string definition;

        public Hatch() : this(new FillPattern() { Name = string.Empty})
        {
        }

        public Hatch(FillPattern pattern)
        {
            fillPattern = pattern;
            UpdatePatternDefinition();
            Name = pattern.Name;
        }

        public FillPattern HatchPattern
        {
            get
            {
                return fillPattern;
            }
        }

        public string Name {
            get; set;
        }

        public bool IsDrafting
        {
            get
            {
                return fillPattern.Target == FillPatternTarget.Drafting;
            }
        }

        public string Definition
        {
            get
            {
                return definition;
            }
            set
            {
                definition = value;
                string[] lines = definition.Split(new[] { Environment.NewLine },StringSplitOptions.None );
                TryAssignFillGridsFromStrings(lines);
            }
        }

        public string PatFileString {
            get
            {
                StringBuilder s = new StringBuilder();
                s.AppendLine(@";% VERSION = 3.0");
                s.AppendLine(@";% UNITS = MM");
                s.AppendLine();
                s.Append(@"*");
                s.AppendLine(Name);
                var type = IsDrafting ? @";%TYPE=DRAFTING" : @";%TYPE=MODEL";
                s.Append(type);
                s.Append(Definition);
                return s.ToString();
            }
        }

        public void UpdatePatternDefinition()
        {
            if (fillPattern == null) {
                SCaddinsApp.WindowManager.ShowMessageBox("Null Hatch");
                return;
            }
            StringBuilder s = new StringBuilder();
            foreach (var p in fillPattern.GetFillGrids()) {
                double angle = p.Angle.ToDeg();
                s.Append(string.Format("{0},\t{1},\t{2},\t{3},\t{4}", angle, p.Origin.U.ToMM(), p.Origin.V.ToMM(), p.Shift.ToMM(),p.Offset.ToMM()));
                foreach (double d in p.GetSegments()) {
                    s.Append(string.Format(",\t{0}", d.ToMM()));
                }
                s.Append(System.Environment.NewLine);
            }
            definition = s.ToString();
            return;
        }

        public bool TryAssignFillGridsFromStrings(string[] grids)
        {
            if (AssignFillGridsFromString(grids)) {
                return true;
            } else {
                return false;
            }
        }

        private bool AssignFillGridsFromString(string[] grids)
        {
            var newFillGrids = new List<FillGrid>();
            foreach (string s in grids) {
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
                double angle = 0;
                double x = 0;
                double y = 0;
                double shift = 0;
                double offset = 0;
                List<double> lineSegs = new List<Double>();
                if (!double.TryParse(segs[0], out angle)) {
                    //SCaddinsApp.WindowManager.ShowMessageBox("Nope at seg 0");
                    return false;
                }
                if (!double.TryParse(segs[1], out x)) {
                    //SCaddinsApp.WindowManager.ShowMessageBox("Nope at seg 1");
                    return false;
                }
                if (!double.TryParse(segs[2], out y)) {
                    //SCaddinsApp.WindowManager.ShowMessageBox("Nope at seg 2");
                    return false;
                }
                if (!double.TryParse(segs[3], out shift)) {
                    //SCaddinsApp.WindowManager.ShowMessageBox("Nope at seg 3");
                    return false;
                }
                if (!double.TryParse(segs[4], out offset)) {
                    //SCaddinsApp.WindowManager.ShowMessageBox("Nope at seg 4");
                    return false;
                }
                for (int i = 5; i < segs.Length; i++)
                {
                    double individualSeg;
                    if (!double.TryParse(segs[i], out individualSeg))
                    {
                        return false;
                    }
                    lineSegs.Add(individualSeg.ToFeet());
                }
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
    }
}
