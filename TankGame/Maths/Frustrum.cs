using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace TankGame.Maths
{
    internal class Frustrum
    {
        enum FrustrumPlane
        {
            Left = 0,
            Top = 1,
            Right = 2,
            Bottom = 3,
        }
        internal Plane[] _planes = new Plane[4];

        internal Frustrum(double cameraProjectionMultiplier, int viewportWidth, int viewportHeight)
        {
            double horizontalAngle = Math.Atan2(viewportWidth / 2, cameraProjectionMultiplier) - 0.0001;
            double verticalAngle= Math.Atan2(viewportHeight / 2, cameraProjectionMultiplier) - 0.0001;

            double sinHoriz = Math.Sin(horizontalAngle);
            double sinVert = Math.Sin(verticalAngle);
            double cosHoriz = Math.Cos(horizontalAngle);
            double cosVert = Math.Cos(verticalAngle);

            _planes[(int)FrustrumPlane.Left] = new Plane(new PointFloat3d(cosHoriz, 0, sinHoriz), 0);
            _planes[(int)FrustrumPlane.Top] = new Plane(new PointFloat3d(0,-cosVert,sinVert),0);
            _planes[(int)FrustrumPlane.Right] = new Plane(new PointFloat3d(-cosHoriz, 0, sinHoriz), 0);
            _planes[(int)FrustrumPlane.Bottom] = new Plane(new PointFloat3d(0, cosVert, sinVert), 0);
        }

        internal bool SphereInsideFrustrum(Point3d centre, double radius)
        {
            if (PointInsideFrustrumPlane(FrustrumPlane.Left, centre+new Point3d(radius,0,0)) == false)
            {
                return false;
            }
            if (PointInsideFrustrumPlane(FrustrumPlane.Top, centre + new Point3d(0, -radius, 0)) == false)
            {
                return false;
            }
            if (PointInsideFrustrumPlane(FrustrumPlane.Right, centre + new Point3d(-radius, 0, 0)) == false)
            {
                return false;
            }
            if (PointInsideFrustrumPlane(FrustrumPlane.Bottom, centre + new Point3d(0, radius, 0)) == false)
            {
                return false;
            }
            return true;
        }

        private bool PointInsideFrustrumPlane(FrustrumPlane checkAgainstPlane, Point3d pt)
        {
            Plane plane = this._planes[(int)checkAgainstPlane];
            double distance = PointFloat3d.DotProduct((PointFloat3d)pt, plane.Normal) - plane.DistanceToOrigin;
            return distance >= 0;
        }

        internal bool PointInsideFrustrum(Point3d pt)
        {
            if (PointInsideFrustrumPlane(FrustrumPlane.Left, pt) == false ||
                PointInsideFrustrumPlane(FrustrumPlane.Top, pt) == false ||
                PointInsideFrustrumPlane(FrustrumPlane.Right, pt) == false ||
                PointInsideFrustrumPlane(FrustrumPlane.Bottom, pt) == false)
            {
                return false;
            }
            return true;
        }
    }
}
