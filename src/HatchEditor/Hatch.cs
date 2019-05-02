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

        public Hatch()
        {
            fillPattern = new FillPattern();
            UpdatePatternDefinition();
            Name = string.Empty;
            Scale = 1;
        }

        public Hatch(FillPattern pattern)
        {
            fillPattern = pattern;
            UpdatePatternDefinition();
            Name = pattern.Name;
            Scale = 1;
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

        private void UpdatePatternDefinition()
        {
            if (fillPattern == null) {
                SCaddinsApp.WindowManager.ShowMessageBox("Null Hatch");
                return;
            }
            StringBuilder s = new StringBuilder();
            foreach (var p in fillPattern.GetFillGrids()) {
                s.Append(string.Format("{0},\t{1},\t{2},\t{3},\t{4}", p.Angle.ToDeg(), p.Origin.U.ToMM(), p.Origin.V.ToMM(), p.Shift.ToMM(),p.Offset.ToMM()));
                int i = 0;
                foreach (var d in p.GetSegments()) {
                    //int m = i % 2 == 0 ? 1 : -1;
                    double m = 1;
                    i++;
                    s.Append(string.Format(",\t{0}", d.ToMM() * m));
                }
                s.Append(System.Environment.NewLine);
            }
            definition = s.ToString();
            return;
        }

        public double Scale
        {
            get; set;
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
                    int dir = i % 2 == 1 ? 1 : -1;
                    double individualSeg;
                    if (!double.TryParse(segs[i], out individualSeg))
                    {
                        return false;
                    }
                    lineSegs.Add(individualSeg.ToFeet() * dir);
                }
                f.Angle = angle.ToRad();
                f.Origin = new UV(x.ToFeet(), y.ToFeet());
                f.Shift = shift.ToFeet();
                f.Offset = offset.ToFeet();
                f.SetSegments(lineSegs);
                newFillGrids.Add(f);              
            }
            //SCaddinsApp.WindowManager.ShowMessageBox(HatchPattern.IsValidObject.ToString());
            fillPattern.SetFillGrids(newFillGrids);
            return true;
        }
    }
}
