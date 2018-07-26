using System;

namespace dotNSGDX.Utility
{
    public class Utility
    {
        public struct Color3
        {
            public float r, g, b;
        }

        public static Color3 Hsv2RGB(float h, float s, float v)
        {
            h = h % 360;
            float c = v * s;
            float x = c * (1 - Math.Abs((h / 60) % 2 - 1));
            float m = v - c;
            float rt, gt, bt;
            if (h >= 0 && h < 60)
            {
                rt = c; gt = x; bt = 0;
            }
            else if (h >= 60 && h < 120)
            {
                rt = x; gt = c; bt = 0;
            }
            else if (h >= 120 && h < 180)
            {
                rt = 0; gt = c; bt = x;
            }
            else if (h >= 180 && h < 240)
            {
                rt = 0; gt = x; bt = c;
            }
            else if (h >= 240 && h < 300)
            {
                rt = x; gt = 0; bt = c;
            }
            else if (h >= 300 && h < 360)
            {
                rt = c; gt = 0; bt = x;
            }
            else
            {
                rt = gt = bt = 0;
            }
            
            return new Color3
            {
                r = rt + m,
                g = gt + m,
                b = bt + m
            };
        }
    }
}
