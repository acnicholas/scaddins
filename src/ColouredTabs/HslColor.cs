using System;
using System.Windows.Media;

namespace SCaddins.ColouredTabs {
    // Ported from pyRevit runtime EventHandling.cs (HSLColor)
    // https://tinyurl.com/yj8x4azp
    public class HslColor {
        public readonly double h, s, l, a;

        public HslColor(double h, double s, double l, double a) {
            this.h = h;
            this.s = s;
            this.l = l;
            this.a = a;
        }

        public HslColor(Color rgb) {
            RgbToHls(rgb.R, rgb.G, rgb.B, out h, out l, out s);
            a = rgb.A / 255.0;
        }

        public Color ToRgb() {
            int r, g, b;
            HlsToRgb(h, l, s, out r, out g, out b);
            return Color.FromArgb((byte)(a * 255.0), (byte)r, (byte)g, (byte)b);
        }

        public HslColor Lighten(double amount) {
            return new HslColor(h, s, Clamp(l * amount, 0, 1), a);
        }

        public float Luminance {
            get {
                var c = ToRgb();
                return 0.2126f * c.R + 0.7152f * c.G + 0.0722f * c.B;
            }
        }

        static double Clamp(double value, double min, double max) {
            if (value < min)
                return min;
            if (value > max)
                return max;
            return value;
        }

        static void RgbToHls(int r, int g, int b,
            out double h, out double l, out double s) {
            double double_r = r / 255.0;
            double double_g = g / 255.0;
            double double_b = b / 255.0;

            double max = double_r;
            if (max < double_g) max = double_g;
            if (max < double_b) max = double_b;

            double min = double_r;
            if (min > double_g) min = double_g;
            if (min > double_b) min = double_b;

            double diff = max - min;
            l = (max + min) / 2;
            if (Math.Abs(diff) < 0.00001) {
                s = 0;
                h = 0;  // H is really undefined.
            }
            else {
                if (l <= 0.5) s = diff / (max + min);
                else s = diff / (2 - max - min);

                double r_dist = (max - double_r) / diff;
                double g_dist = (max - double_g) / diff;
                double b_dist = (max - double_b) / diff;

                if (double_r == max) h = b_dist - g_dist;
                else if (double_g == max) h = 2 + r_dist - b_dist;
                else h = 4 + g_dist - r_dist;

                h = h * 60;
                if (h < 0) h += 360;
            }
        }

        static void HlsToRgb(double h, double l, double s,
            out int r, out int g, out int b) {
            double p2;
            if (l <= 0.5) p2 = l * (1 + s);
            else p2 = l + s - l * s;

            double p1 = 2 * l - p2;
            double double_r, double_g, double_b;
            if (s == 0) {
                double_r = l;
                double_g = l;
                double_b = l;
            }
            else {
                double_r = QqhToRgb(p1, p2, h + 120);
                double_g = QqhToRgb(p1, p2, h);
                double_b = QqhToRgb(p1, p2, h - 120);
            }

            r = (int)(double_r * 255.0);
            g = (int)(double_g * 255.0);
            b = (int)(double_b * 255.0);
        }

        static double QqhToRgb(double q1, double q2, double hue) {
            if (hue > 360) hue -= 360;
            else if (hue < 0) hue += 360;

            if (hue < 60) return q1 + (q2 - q1) * hue / 60;
            if (hue < 180) return q2;
            if (hue < 240) return q1 + (q2 - q1) * (240 - hue) / 60;
            return q1;
        }
    }
}
