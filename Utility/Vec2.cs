using System;

namespace dotNSGDX.Utility
{
    public class Vec2
    {
        /// <summary>
        /// DEG to RAD
        /// </summary>
        /// <param name="d">Deg value</param>
        /// <returns>Rad value</returns>
        public static double D2R(double d) => d / 180 * Math.PI;

        /// <summary>
        /// RAD to DEG
        /// </summary>
        /// <param name="a">Rad value</param>
        /// <returns>Deg value</returns>
        public static double R2D(double a) => a / Math.PI * 180;

        public double X { get; set; }
        public double Y { get; set; }
        public double A
        {
            get
            {
                if (X == 0)
                    return Y > 0 ? Math.PI / 2 : (Y < 0 ? -Math.PI / 2 : 0);
                return Math.Atan(Y / X);
            }
        }

        public Vec2() : this(0, 0)
        {
        }

        public Vec2(Vec2 vec) : this(vec.X, vec.Y)
        {
        }

        public Vec2(double x, double y)
        {
            X = x; Y = y;
        }

        public static implicit operator Vec2(ValueTuple<double, double> tuple) => new Vec2(tuple.Item1, tuple.Item2);

        /// <summary>
        /// Returns the length of vector
        /// </summary>
        /// <param name="vec">The vector</param>
        /// <returns>The length of vector</returns>
        public static double operator ~(Vec2 vec) => Math.Sqrt(vec.X * vec.X + vec.Y * vec.Y);

        /// <summary>
        /// Normalize the vector
        /// </summary>
        /// <param name="vec">The vector</param>
        /// <returns>The normal of vector</returns>
        public static Vec2 operator !(Vec2 vec) => new Vec2(vec.X / ~vec, vec.Y / ~vec);

        public static Vec2 operator +(Vec2 vec) => new Vec2(vec.X, vec.Y);

        public static Vec2 operator -(Vec2 vec) => new Vec2(-vec.X, -vec.Y);

        public static Vec2 operator +(Vec2 a, Vec2 b) => new Vec2(a.X + b.X, a.Y + b.Y);

        public static Vec2 operator +(Vec2 vec, double d) => new Vec2(vec.X + d, vec.Y + d);

        public static Vec2 operator -(Vec2 a, Vec2 b) => new Vec2(a.X - b.X, a.Y - b.Y);

        public static Vec2 operator -(Vec2 vec, double d) => new Vec2(vec.X - d, vec.Y - d);

        /// <summary>
        /// IS NOT DOT MUL
        /// </summary>
        /// <param name="a">Vector A</param>
        /// <param name="b">Vector B</param>
        /// <returns>A new vector</returns>
        public static Vec2 operator *(Vec2 a, Vec2 b) => new Vec2(a.X * b.X, a.Y * b.Y);

        public static Vec2 operator *(Vec2 vec, double d) => new Vec2(vec.X * d, vec.Y * d);

        /// <summary>
        /// Rotate the vector
        /// </summary>
        /// <param name="vec">The vector</param>
        /// <param name="d">The angle in rad</param>
        /// <returns>The rotated vector</returns>
        public static Vec2 operator ^(Vec2 vec, double d)
        {
            return new Vec2(vec.X * Math.Cos(d) - vec.Y * Math.Sin(d), vec.X * Math.Sin(d) + vec.Y * Math.Cos(d));
        }
    }
}
