using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace TankGame.Maths
{
    internal class Plane
    {
        internal PointFloat3d Normal { get; private set; }
        internal double DistanceToOrigin { get; private set; }

        internal Plane(PointFloat3d normal, double distanceToOrigin)
        {
            Normal = normal;
            DistanceToOrigin = distanceToOrigin;
        }

        internal Plane(Point3d p1, Point3d p2, Point3d p3) 
            : this((PointFloat3d)p1, (PointFloat3d) p2, (PointFloat3d) p3)
        {

        }
        internal Plane(PointFloat3d p1, PointFloat3d p2, PointFloat3d p3)
        {
            PointFloat3d p1ToP2 = p2 - p1;
            PointFloat3d p3ToP2 = p2 - p3;

            Normal = PointFloat3d.CrossProduct(p1ToP2, p3ToP2);
            Normal.Normalise();

            DistanceToOrigin = PointFloat3d.DotProduct(Normal, p1);
        }
    }
}
