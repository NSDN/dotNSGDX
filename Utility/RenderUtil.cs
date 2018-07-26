using System;

namespace dotNSGDX.Utility
{
    public static class RenderUtil
    {
        public struct Color4
        {
            public float r, g, b, a;

            public Color4(Color4 color)
            {
                r = color.r;
                g = color.g;
                b = color.b;
                a = color.a;
            }

            public static implicit operator Color4(Utility.Color3 color)
            {
                return new Color4()
                {
                    r = color.r,
                    g = color.g,
                    b = color.b,
                    a = 1.0F
                };
            }

            public static implicit operator Color4(ValueTuple<float, float, float> values)
            {
                return new Color4()
                {
                    r = values.Item1,
                    g = values.Item2,
                    b = values.Item3,
                    a = 1.0F
                };
            }

            public static implicit operator Color4(ValueTuple<float, float, float, float> values)
            {
                return new Color4()
                {
                    r = values.Item1,
                    g = values.Item2,
                    b = values.Item3,
                    a = values.Item4
                };
            }
        }

        public interface IDrawable
        {
        }

        public interface IRenderer
        {
            void Begin();
            void End();
            void DrawStr(float x, float y, float scale, Color4 color, String str);
            void Draw(IDrawable drawable, float x, float y, float rotate, float scale, Color4 color);
        }
    }
}
