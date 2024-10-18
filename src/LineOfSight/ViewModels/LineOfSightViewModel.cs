// (C) Copyright 2018-2020 by Andrew Nicholas
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

namespace SCaddins.LineOfSight.ViewModels
{
    using System;
    using Autodesk.Revit.DB;
    using Caliburn.Micro;

    public class LineOfSightViewModel : PropertyChangedBase
    {
        private StadiumSeatingTier lineOfSight;
        private double tolerance = 0.001;

        public LineOfSightViewModel(Document doc)
        {
            lineOfSight = new StadiumSeatingTier(doc);
        }

        public static bool CanDraw
        {
            get
            {
                return true;
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
                if (Math.Abs(value - lineOfSight.DistanceToFirstRowX) > tolerance)
                {
                    try
                    {
                        lineOfSight.DistanceToFirstRowX = value;
                        //// NotifyOfPropertyChange(() => DistanceToFirstRowX);
                        NotifyOfPropertyChange(() => InfoString);
                    }
                    catch
                    {
                        //// FIXME somehow....
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
                //// NotifyOfPropertyChange(() => DistanceToFirstRowY);
                NotifyOfPropertyChange(() => InfoString);
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
                if (Math.Abs(value - lineOfSight.EyeHeight) > tolerance)
                {
                    lineOfSight.EyeHeight = value;
                    NotifyOfPropertyChange(() => EyeHeight);
                    NotifyOfPropertyChange(() => InfoString);
                }
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
                catch
                {
                    return string.Empty;
                }
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

        public double RiserIncrement
        {
            get
            {
                return lineOfSight.RiserIncrement;
            }

            set
            {
                if (value < 1)
                {
                    return;
                }
                lineOfSight.RiserIncrement = value;
                NotifyOfPropertyChange(() => RiserIncrement);
                NotifyOfPropertyChange(() => InfoString);
            }
        }

        public double TreadSize
        {
            get
            {
                return lineOfSight.TreadSize;
            }

            set
            {
                if (Math.Abs(value - lineOfSight.TreadSize) > tolerance)
                {
                    lineOfSight.TreadSize = value;
                    NotifyOfPropertyChange(() => TreadSize);
                    NotifyOfPropertyChange(() => InfoString);
                }
            }
        }

        public void Draw()
        {
            lineOfSight.Draw();
        }
    }
}