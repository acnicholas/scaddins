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
                    NotifyOfPropertyChange(() => InfoString);
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
                    NotifyOfPropertyChange(() => InfoString);
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
                if (value != lineOfSight.DistanceToFirstRowX)
                {
                    try
                    {
                        lineOfSight.DistanceToFirstRowX = value;
                        NotifyOfPropertyChange(() => DistanceToFirstRowX);
                        NotifyOfPropertyChange(() => InfoString);
                    }
                    catch
                    {
                        ///FIXME somehow....
                    }
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
                NotifyOfPropertyChange(() => InfoString);
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
                NotifyOfPropertyChange(() => InfoString);
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
                NotifyOfPropertyChange(() => InfoString);
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
                NotifyOfPropertyChange(() => InfoString);
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
                NotifyOfPropertyChange(() => RiserIncrement);
                NotifyOfPropertyChange(() => InfoString);
            }
        }

        public string InfoString
        {
            get
            {
                try
                {
                    return lineOfSight.InfoString;
                }
                catch { return string.Empty; }
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
                return true;
            }
        }

        private LineOfSight lineOfSight;

        public LineOfSightViewModel(Document doc)
        {
            lineOfSight = new LineOfSight(doc);
        }
    }
}
