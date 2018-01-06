using System;
using System.Collections.Generic;

namespace dotNSGDX.Entity
{
    public class Shape
    {
        public interface Instruction
        {
            bool IsColor();
        }

        public struct Color4 : Instruction
        {
            public float r, g, b, a;
            public Color4(float r, float g, float b, float a)
            {
                this.r = r; this.g = g; this.b = b; this.a = a;
            }
            public bool IsColor() { return true; }
        }

        public struct Vec3 : Instruction
        {
            public double x, y, z;
            public Vec3(double x, double y, double z)
            {
                this.x = x; this.y = y; this.z = z;
            }
            public bool IsColor() { return false; }
        }

        public LinkedList<Instruction> instBuf;

        public Shape()
        {
            instBuf = new LinkedList<Instruction>();
            instBuf.AddLast(new Color4(1.0F, 1.0F, 1.0F, 1.0F));
        }

        public Shape AddColor(float r, float g, float b, float a = 1.0F)
        {
            instBuf.AddLast(new Color4(r, g, b, a));
            return this;
        }

        public Shape AddVertex(double x, double y, double z = 0)
        {
            instBuf.AddLast(new Vec3(x, y, z));
            return this;
        }
    }
}
