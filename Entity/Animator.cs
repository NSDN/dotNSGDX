using System;
using System.Collections.Generic;

using dotNSGDX.Utility;
using static dotNSGDX.Utility.RenderUtil;

namespace dotNSGDX.Entity
{
    public class Animator : IObject
    {
        public enum Interpolation
        {
            NONE, LINEAR, QUADRATIC, SINE
        }

        protected class State
        {
            public IDrawable drawable;

            public readonly Interpolation inter;
            public int length;

            public Vec2 pos;
            public float rotate;
            public float scale;
            public Color4 color;

            public State(IDrawable drawable) : this(drawable, Interpolation.NONE)
            {
            }

            public State(IDrawable drawable, Interpolation inter)
            {
                this.drawable = drawable;
                this.inter = inter;
                pos = (0, 0);
                rotate = scale = 0;
                color = (1.0F, 1.0F, 1.0F);
            }
        }

        protected State original, now;
        protected LinkedList<State> destinations;

        protected int step;

        public Animator() : this(null)
        {
        }

        public Animator(IDrawable drawable)
        {
            now = new State(drawable);
            original = new State(drawable);
            destinations = new LinkedList<State>();
            step = 0;
        }

        public Animator Start(Vec2 pos)
        {
            return Start(pos, 0.0F, 1.0F);
        }

        public Animator Start(Vec2 pos, float rotate, float scale)
        {
            return Start(pos, rotate, scale, 1.0F);
        }

        public Animator Start(Vec2 pos, float rotate, float scale, float alpha)
        {
            return Start(pos, rotate, scale, (1.0F, 1.0F, 1.0F, alpha));
        }

        public Animator Start(Vec2 pos, float rotate, float scale, Color4 color)
        {
            original.pos = new Vec2(pos); original.rotate = rotate; original.scale = scale;
            original.color = new Color4(color); original.length = 0;

            return this;
        }

        public Animator Start(IDrawable drawable, Vec2 pos)
        {
            return Start(drawable, pos, 0.0F, 1.0F);
        }

        public Animator Start(IDrawable drawable, Vec2 pos, float rotate, float scale)
        {
            return Start(drawable, pos, rotate, scale, 1.0F);
        }

        public Animator Start(IDrawable drawable, Vec2 pos, float rotate, float scale, float alpha)
        {
            return Start(drawable, pos, rotate, scale, (1.0F, 1.0F, 1.0F, alpha));
        }

        public Animator Start(IDrawable drawable, Vec2 pos, float rotate, float scale, Color4 color)
        {
            original.drawable = drawable;
            original.pos = new Vec2(pos); original.rotate = rotate; original.scale = scale;
            original.color = new Color4(color); original.length = 0;

            return this;
        }

        public Animator Next(Interpolation inter, int length, Vec2 pos)
        {
            return Next(inter, length, pos, 0.0F, 1.0F);
        }

        public Animator Next(Interpolation inter, int length, Vec2 pos, float rotate, float scale)
        {
            return Next(inter, length, pos, rotate, scale, 1.0F);
        }

        public Animator Next(Interpolation inter, int length, Vec2 pos, float rotate, float scale, float alpha)
        {
            return Next(inter, length, pos, rotate, scale, (1.0F, 1.0F, 1.0F, alpha));
        }

        public Animator Next(Interpolation inter, int length, Vec2 pos, float rotate, float scale, Color4 color)
        {
            IDrawable next = original.drawable;
            if (destinations.Count != 0) next = destinations.Last.Value.drawable;
            State dst = new State(next, inter);
            dst.pos = new Vec2(pos); dst.rotate = rotate; dst.scale = scale;
            dst.color = new Color4(color); dst.length = length;
            destinations.AddLast(dst);

            return this;
        }

        public Animator Next(IDrawable drawable, Interpolation inter, int length, Vec2 pos)
        {
            return Next(drawable, inter, length, pos, 0.0F, 1.0F);
        }

        public Animator Next(IDrawable drawable, Interpolation inter, int length, Vec2 pos, float rotate, float scale)
        {
            return Next(drawable, inter, length, pos, rotate, scale, 1.0F);
        }

        public Animator Next(IDrawable drawable, Interpolation inter, int length, Vec2 pos, float rotate, float scale, float alpha)
        {
            return Next(drawable, inter, length, pos, rotate, scale, (1.0F, 1.0F, 1.0F, alpha));
        }

        public Animator Next(IDrawable drawable, Interpolation inter, int length, Vec2 pos, float rotate, float scale, Color4 color)
        {
            State dst = new State(drawable, inter);
            dst.pos = new Vec2(pos); dst.rotate = rotate; dst.scale = scale;
            dst.color = new Color4(color); dst.length = length;
            destinations.AddLast(dst);

            return this;
        }

        private State Interp(State original, State destination, int step)
        {
            State result = new State(original.drawable);
            result.pos = Interp(destination.inter, original.pos, destination.pos, step, destination.length);
            result.rotate = Interp(destination.inter, original.rotate, destination.rotate, step, destination.length);
            result.scale = Interp(destination.inter, original.scale, destination.scale, step, destination.length);

            result.color.r = Interp(destination.inter, original.color.r, destination.color.r, step, destination.length);
            result.color.g = Interp(destination.inter, original.color.g, destination.color.g, step, destination.length);
            result.color.b = Interp(destination.inter, original.color.b, destination.color.b, step, destination.length);
            result.color.a = Interp(destination.inter, original.color.a, destination.color.a, step, destination.length);

            return result;
        }

        private Vec2 Interp(Interpolation inter, Vec2 original, Vec2 destination, int step, int length)
        {
            Vec2 result = (0, 0);
            result.X = Interp(inter, original.X, destination.X, step, length);
            result.Y = Interp(inter, original.Y, destination.Y, step, length);

            return result;
        }

        private float Interp(Interpolation inter, float original, float destination, int step, int length)
        {
            return Interp(inter, original, destination, step, length);
        }

        private double Interp(Interpolation inter, double original, double destination, int step, int length)
        {
            double x = step / length;
            double k = destination - original;
            switch (inter)
            {
                case Interpolation.LINEAR: // y = x, x = 0 to 1
                    return original + k * x;
                case Interpolation.QUADRATIC: // y = -x^2 + 2x, x = 0 to 1
                    return original + k * (-x * x + 2 * x);
                case Interpolation.SINE: // y = 0.5sin((x - 0.5)pi) + 0.5
                    return original + k * (float)(0.5 * Math.Sin((x - 0.5) * Math.PI) + 0.5);
                default:
                    return original + k * x;
            }
        }

        public Result OnUpdate(int t)
        {
            if (destinations.Count == 0) return Result.END;

            State next = destinations.First.Value;
            now = Interp(original, next, step);

            step += 1;
            if (step > next.length)
            {
                step = 0;
                original = destinations.First.Value;
                destinations.RemoveFirst();
            }

            return Result.DONE;
        }

        public Result OnRender(IRenderer renderer)
        {
            if (now.drawable == null) return Result.END;
            renderer.Draw(now.drawable, (float)now.pos.X, (float)now.pos.Y, now.rotate, now.scale, now.color);
            return Result.DONE;
        }
    }
}
