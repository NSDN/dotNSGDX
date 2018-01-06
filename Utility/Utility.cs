using System;

namespace dotNSGDX.Utility
{
    public class Utility
    {
        public static float dist2(float x1, float y1, float x2, float y2)
        {
            return (float)Math.Sqrt(Math.Pow((double)x2 - (double)x1, (double)2) + Math.Pow((double)y2 - (double)y1, (double)2));
        }
    }
}
