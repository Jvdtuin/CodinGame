using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodersStrikeBack.Simulation
{
    public class Vector
    {
        public Vector(double x, double y)
        {
            X = x;
            Y = y;
        }

        public static Vector CreateVectorAngleSizeRad(double angleRad, double size)
        {
            return new Vector(Math.Cos(angleRad) * size, Math.Sin(angleRad) * size);
        }

        public double X { get; private set; }

        public double Y { get; private set; }

        public double AngleRad
        {
            get
            {
                double a = 0.0;
                double sx = X > 0 ? 1.0 : -1.0;
                double sy = Y > 0 ? 1.0 : -1.0;
                if (sx * X > sy * Y)
                {
                    if (X > 0)
                    {
                        a = Math.Atan(Y / X);
                    }
                    else if (X < 0)
                    {
                        a = Math.PI + Math.Atan(Y / X); ;
                    }
                }
                else
                {
                    if (Y > 0)
                    {
                        //B
                        a = Math.PI * 0.5 - Math.Atan(X / Y);
                    }
                    else if (Y < 0)
                    {
                        //D
                        a = Math.PI * 1.5 - Math.Atan(X / Y);
                    }
                }
                if (a > Math.PI)
                {
                    a -= 2 * Math.PI;
                }
                return a;
            }
        }

        public double AngleDeg
        {
            get
            {
                return AngleRad / Math.PI * 180.0;
            }
        }

        public double Size2
        {
            get
            {
                return X * X + Y * Y;
            }
        }

        public double Size
        {
            get { return Math.Sqrt(Size2); }
        }

        public void Round()
        {
            X = Math.Round(X);
            Y = Math.Round(Y);
        }

        public void Truncate()
        {
            X = Math.Truncate(X);
            Y = Math.Truncate(Y);
        }

        public Vector Rotate(double angle)
        {
            double fc = Math.Cos(angle);
            double fs = Math.Sin(angle);
            double x = fc * X - fs * Y;
            double y = fs * X + fc * Y;
            return new Vector(x, y);
        } 

        public static Vector operator +(Vector v1, Vector v2)
        {
            return new Vector(v1.X + v2.X, v1.Y + v2.Y);
        }

        public static Vector operator -(Vector v1, Vector v2)
        {
            return new Vector(v1.X - v2.X, v1.Y - v2.Y);
        }

        public static Vector operator *(Vector v1, double x)
        {
            return new Vector(v1.X * x, v1.Y * x);
        }

        public static Vector operator /(Vector v1, double x)
        {
            return new Vector(v1.X / x, v1.Y / x);
        }

        public static double operator *(Vector v1, Vector v2)
        {
            return v1.X * v2.X + v1.Y * v2.Y;
        }

        public void Normalize(double newSize)
        {
            double factor = newSize / Size;
            X *= factor;
            Y *= factor;
        }

        public override string ToString()
        {
            return string.Format("({0:0.00} {1:0.00})", X, Y);
        }
    }
}
