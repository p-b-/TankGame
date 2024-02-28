using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Numerics;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;
using System.Xml.Linq;
using System.Security.Cryptography.Xml;

namespace TankGame.Maths
{
    internal struct Point3d
    {
        internal int X { get; set; }
        internal int Y { get; set; }
        internal int Z { get; set; }

        public Point3d()
        {
            X = 0;
            Y = 0;
            Z = 0;
        }

        internal Point3d(int x, int y, int z)
        {
            X = x;
            Y = y;
            Z = z;
        }

        internal Point3d(double x, double y, double z)
        {
            X = (int)x;
            Y = (int)y;
            Z = (int)z;
        }

        internal Point3d(PointFloat3d floatingPt)
        {
            X = (int)floatingPt.X;
            Y = (int)floatingPt.Y;
            Z = (int)floatingPt.Z;
        }

        internal Point3d(EntityVertex entityVertex)
        {
            X = (int)entityVertex.X;
            Y = (int)entityVertex.Y;
            Z = (int)entityVertex.Z;
        }

        internal PointFloat3d RotatedAroundYAxis(Angle yAxisRotation)
        {
            double newX = X * yAxisRotation.Cos - Z * yAxisRotation.Sin;
            double newZ = X * yAxisRotation.Sin + Z * yAxisRotation.Cos;
            return new PointFloat3d(newX, Y, newZ);
        }

        static public Point3d operator +(Point3d lhs, Point3d rhs)
        {
            Point3d toReturn = new Point3d(lhs.X + rhs.X, lhs.Y + rhs.Y, lhs.Z + rhs.Z);
            return toReturn;
        }
        static public Point3d operator -(Point3d lhs, Point3d rhs)
        {
            Point3d toReturn = new Point3d(lhs.X - rhs.X, lhs.Y - rhs.Y, lhs.Z - rhs.Z);
            return toReturn;
        }
        static public Point3d operator *(Point3d lhs, double rhs)
        {
            Point3d toReturn = new Point3d((int)(lhs.X * rhs), (int)(lhs.Y * rhs), (int)(lhs.Z * rhs));
            return toReturn;
        }

        static public Point3d operator/(Point3d lhs, double rhs)
        {
            Point3d toReturn = new Point3d((int)(lhs.X / rhs), (int)(lhs.Y / rhs), (int)(lhs.Z / rhs));
            return toReturn;
        }

        public override string ToString()
        {
            return $"({X},{Y},{Z})";
        }

        public static implicit operator Point3d(PointFloat3d rhs)
        {
            Point3d toReturn = new Point3d(rhs.X, rhs.Y, rhs.Z);
            return toReturn;
        }

        internal int SquareDistanceTo(Point3d rhs)
        {
            int dx = rhs.X - this.X;
            int dy = rhs.Y - this.Y;
            int dz = rhs.Z - this.Z;
            return dx*dx+dy*dy+dz*dz;
        }
    }
}
