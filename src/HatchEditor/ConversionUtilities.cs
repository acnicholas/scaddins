namespace SCaddins.HatchEditor
{
    public static class ConversionUtilities
    {
        public static double ToDeg(this double arg)
        {
            return arg * 180 / System.Math.PI;
        }

        public static double ToRad(this double arg)
        {
            return arg * System.Math.PI / 180;
        }

        public static double ToMM(this double arg)
        {
            return arg * 304.8;
        }

        public static double ToMM(this double arg, double scale)
        {
            return arg * 304.8 * scale;
        }


        public static double ToFeet(this double arg)
        {
            return arg / 304.8;
        }
    }
}
