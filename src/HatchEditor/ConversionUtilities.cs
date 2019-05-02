namespace SCaddins.HatchEditor
{
    public static class ConversionUtilities
    {
        public static double ToDeg(this double arg)
        {
            return System.Math.Round(arg * 180 / System.Math.PI, 15);
        }

        public static double ToRad(this double arg)
        {
            return System.Math.Round(arg * System.Math.PI / 180, 15);
        }

        public static double ToMM(this double arg)
        {
            return System.Math.Round(arg * 304.8, 15);
        }


        public static double ToFeet(this double arg)
        {
            return System.Math.Round(arg / 304.8, 15);
        }
    }
}
