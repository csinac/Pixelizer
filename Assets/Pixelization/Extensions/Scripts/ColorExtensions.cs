using UnityEngine;

namespace AngryKoala.Pixelization
{
    public static class ColorExtensions
    {
        public static float Hue(this Color c)
        {
            float h, s, v;
            Color.RGBToHSV(c, out h, out s, out v);
            return h;
        }

        public static float Saturation(this Color c)
        {
            float h, s, v;
            Color.RGBToHSV(c, out h, out s, out v);
            return s;
        }

        public static float Value(this Color c)
        {
            float h, s, v;
            Color.RGBToHSV(c, out h, out s, out v);
            return v;
        }

        public static Color WithH(this Color c, float h)
        {
            float originalH, originalS, originalV;
            Color.RGBToHSV(c, out originalH, out originalS, out originalV);
            return new Color(h, originalS, originalV);
        }

        public static Color WithS(this Color c, float s)
        {
            float originalH, originalS, originalV;
            Color.RGBToHSV(c, out originalH, out originalS, out originalV);
            return new Color(originalH, s, originalV);
        }

        public static Color WithV(this Color c, float v)
        {
            float originalH, originalS, originalV;
            Color.RGBToHSV(c, out originalH, out originalS, out originalV);
            return new Color(originalH, originalS, v);

        }

        public static Color ClearWhite => new Color(1f, 1f, 1f, 0f);
    }
}