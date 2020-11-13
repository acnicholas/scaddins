using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SCaddins.ExportManager
{
    public class SheetSize
    {
        private double height;
        private double width;


        public SheetSize(double width, double height, string name, string[] otherNames)
        {
            Width = width;
        }

        /// <summary>
        /// Sheet Width in mm
        /// </summary>
        public double Width
        {
            get
            {
                return width;
            }
            set
            {
                width = value;
            }
        }

        /// <summary>
        /// Sheet Height in mm
        /// </summary>
        public double Height
        {
            get; private set;
        }

        public string Name
        {
            get; private set;
        }

        public string RevitPrintSettingName
        {
            get
            {
                return "SCX-" + Name;
            }
        }

        public bool IsValid
        {
            get
            {
                return true;
            }
        }

        public List<string> AdditionalNames
        {
            get; private set;
        }

        /// <summary>
        /// Size tollerence in mm.
        /// </summary>
        public double Tollerence
        {
            get; private set;
        }

        public void IsValidSize() { }


        public bool IsSimilarOrEqual(double width, double height, double tollerance)
        {
            //double w = Math.Round(width);
            //double h = Math.Round(height);

            //// use a tolerance of 2mm.
            //return tw + 2 > w && tw - 2 < w && th + 2 > h && th - 2 < h;
            return true;
        }

    }
}
