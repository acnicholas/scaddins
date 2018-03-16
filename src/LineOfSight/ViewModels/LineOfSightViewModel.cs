using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using Autodesk.Revit.DB;
using Caliburn.Micro;

namespace SCaddins.LineOfSight.ViewModels
{
    class LineOfSightViewModel : PropertyChangedBase
    {
        public double TreadSize
        {
            get
            {
                return lineOfSight.TreadSize;
            }
            set
            {
                if (value != lineOfSight.TreadSize) {
                    lineOfSight.TreadSize = value;
                    NotifyOfPropertyChange(() => TreadSize);
                }
            }
        }

        public double EyeHeight
        {
            get
            {
                return lineOfSight.EyeHeight;
            }
            set
            {
                if (value != lineOfSight.EyeHeight) {
                    lineOfSight.EyeHeight = value;
                    NotifyOfPropertyChange(() => EyeHeight);
                }
            }
        }

        public double DistanceToFirstRowX
        {
            get
            {
                return lineOfSight.DistanceToFirstRowX;
            }
            set
            {
                    if (value != lineOfSight.DistanceToFirstRowX) {
                        lineOfSight.DistanceToFirstRowX = value;
                        NotifyOfPropertyChange(() => DistanceToFirstRowX);
                    }
            }
        }

        public double DistanceToFirstRowY
        {
            get
            {
                return lineOfSight.DistanceToFirstRowY;
            }
            set
            {
                lineOfSight.DistanceToFirstRowY = value;
                NotifyOfPropertyChange(() => DistanceToFirstRowY);
            }
        }


        public int NumberOfRows
        {
            get
            {
                return lineOfSight.NumberOfRows;
            }
            set
            {
                lineOfSight.NumberOfRows = value;
                NotifyOfPropertyChange(() => NumberOfRows);
            }
        }

        public double MinimumRiserHeight
        {
            get
            {
                return lineOfSight.MinimumRiserHeight;
            }
            set
            {
                lineOfSight.MinimumRiserHeight = value;
                NotifyOfPropertyChange(() => MinimumRiserHeight);
            }
        }

        public double MinimumCValue
        {
            get
            {
                return lineOfSight.MinimumCValue;
            }
            set
            {
                lineOfSight.MinimumCValue = value;
                NotifyOfPropertyChange(() => MinimumCValue);
            }
        }

        public double RiserIncrement
        {
            get
            {
                return lineOfSight.RiserIncrement;
            }
            set
            {
                lineOfSight.RiserIncrement = value;
            }
        }

        public string InfoString
        {
            get
            {
                try {
                    NotifyOfPropertyChange(() => InfoString);
                    return lineOfSight.InfoString;
                } catch { return string.Empty; }
            }
        }

        public void Draw()
        {
            lineOfSight.Draw();
        }

        public bool CanDraw
        {
            get
            {
                return false;
            }
        }

        private LineOfSight lineOfSight;

        public LineOfSightViewModel(Document doc)
        {
            lineOfSight = new LineOfSight(doc);
        }
    }
}
