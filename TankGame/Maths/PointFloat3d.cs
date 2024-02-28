using System;
using System.Collections.Generic;
using System.Drawing.Printing;
using System.Linq;
using System.Reflection.PortableExecutable;
using System.Text;
using System.Threading.Tasks;
using TankGame.GameEntities;

namespace TankGame.Maths
{
    internal class PointFloat3d
    {
        internal double X { get; set; }
        internal double Y { get; set; }
        internal double Z { get; set; }

        internal PointFloat3d()
        {
            X = 0;
            Y = 0;
            Z = 0;
        }

        internal PointFloat3d(double x, double y, double z)
        {
            X = x;
            Y = y;
            Z = z;
        }
        internal PointFloat3d(EntityVertex entityVertex)
        {
            X = entityVertex.X;
            Y = entityVertex.Y;
            Z = entityVertex.Z;
        }

        internal PointFloat3d(Point3d point)
        {
            X = point.X;
            Y = point.Y;
            Z = point.Z;
        }

        public static explicit operator PointFloat3d(Point3d v)
        {
            return new PointFloat3d(v);
        }

        static public PointFloat3d CrossProduct(PointFloat3d v1, PointFloat3d v2)
        {
            double x = v1.Y * v2.Z - v1.Z * v2.Y;
            double y = v1.Z * v2.X - v1.X * v2.Z;
            double z = v1.X * v2.Y - v1.Y * v2.X;

            return new PointFloat3d(x, y, z);
        }

        static public double DotProduct(PointFloat3d v1, PointFloat3d v2)
        {
            return v1.X * v2.X + v1.Y * v2.Y + v1.Z * v2.Z;
        }

        internal double Magnitude()
        {
            return Math.Sqrt(X * X + Y * Y + Z * Z);
        }

        internal void Normalise()
        {
            double magnitude = Magnitude();

            X /= magnitude;
            Y /= magnitude;
            Z /= magnitude;
        }

        static public PointFloat3d operator +(PointFloat3d lhs, PointFloat3d rhs)
        {
            PointFloat3d toReturn = new PointFloat3d(lhs.X + rhs.X, lhs.Y + rhs.Y, lhs.Z + rhs.Z);
            return toReturn;
        }
        static public PointFloat3d operator +(Point3d lhs, PointFloat3d rhs)
        {
            PointFloat3d toReturn = new PointFloat3d(lhs.X + rhs.X, lhs.Y + rhs.Y, lhs.Z + rhs.Z);
            return toReturn;
        }

        static public PointFloat3d operator -(PointFloat3d lhs, PointFloat3d rhs)
        {
            PointFloat3d toReturn = new PointFloat3d(lhs.X - rhs.X, lhs.Y - rhs.Y, lhs.Z - rhs.Z);
            return toReturn;
        }
        static public PointFloat3d operator *(PointFloat3d lhs, double rhs)
        {
            PointFloat3d toReturn = new PointFloat3d(lhs.X * rhs, lhs.Y * rhs, lhs.Z * rhs);
            return toReturn;
        }

        static public PointFloat3d operator /(PointFloat3d lhs, double rhs)
        {
            PointFloat3d toReturn = new PointFloat3d(lhs.X / rhs, lhs.Y / rhs, lhs.Z / rhs);
            return toReturn;
        }

        internal double SquareDistanceTo(PointFloat3d rhs)
        {
            double dx = rhs.X - this.X;
            double dy = rhs.Y - this.Y;
            double dz = rhs.Z - this.Z;
            return dx * dx + dy * dy + dz * dz;
        }

        public override string ToString()
        {
            return $"({X:0.#},{Y:0.#},{Z:0.#})";
        }
    }
}
