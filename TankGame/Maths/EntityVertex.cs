using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using TankGame.GameEntities;

namespace TankGame.Maths
{
    internal class EntityVertex
    {
        internal double X { get; set; }
        internal double Y { get; set; }
        internal double Z { get; set; }
        internal double ModelTransformedX { get; set; }
        internal double ModelTransformedY { get; set; }
        internal double ModelTransformedZ { get; set; }
        internal double WorldTransformedX { get; set; }
        internal double WorldTransformedY { get; set; }
        internal double WorldTransformedZ { get; set; }
        internal double CameraTransformedX { get; set; }
        internal double CameraTransformedY { get; set; }
        internal double CameraTransformedZ { get; set; }
        internal int ViewportX { get; set; }
        internal int ViewportY { get; set; }
        internal Point ViewportPoint { get { return new Point(ViewportX, ViewportY); } }

        internal Point3d PointInModelSpace
        {
            get
            {
                return new Point3d(ModelTransformedX, ModelTransformedY, ModelTransformedZ);
            }
        }

        internal Point3d PointInLocalSpace
        {
            get
            {
                return new Point3d(X, Y, Z);
            }
        }

        internal PointFloat3d PointInWorld
        {
            get
            {
                return new PointFloat3d(WorldTransformedX, WorldTransformedY, WorldTransformedZ);
            }
        }

        internal PointFloat3d PointInView
        {
            get
            {
                return new PointFloat3d(CameraTransformedX, CameraTransformedY, CameraTransformedZ);
            }
        }
        internal EntityVertex()
        {

        }

        internal EntityVertex(int x, int y, int z)
        {
            X = x;
            Y = y;
            Z = z;
        }
        internal EntityVertex(double x, double y, double z)
        {
            X = (int)x;
            Y = (int)y;
            Z = (int)z;
        }

        internal EntityVertex(Point3d pt)
        {
            X = pt.X;
            Y = pt.Y;
            Z = pt.Z;
        }

        internal EntityVertex(EntityVertex rhs)
        {
            X = rhs.X;
            Y = rhs.Y;
            Z = rhs.Z;
            ModelTransformedX = rhs.ModelTransformedX;
            ModelTransformedY = rhs.ModelTransformedY;
            ModelTransformedZ = rhs.ModelTransformedZ;
            WorldTransformedX = rhs.WorldTransformedX;
            WorldTransformedY = rhs.WorldTransformedY;
            WorldTransformedZ = rhs.WorldTransformedZ;
            CameraTransformedX = rhs.CameraTransformedX;
            CameraTransformedY = rhs.CameraTransformedY;
            CameraTransformedZ = rhs.CameraTransformedZ;
            ViewportX = rhs.ViewportX;
            ViewportY = rhs.ViewportY;
        }

        internal EntityVertex(Point pt, int z)
        {
            X = pt.X;
            Y = pt.Y;
            Z = z;
        }

        internal EntityVertex(Point pt, int otherAxis, bool otherAxisIsY)
        {
            X = pt.X;
            if (otherAxisIsY)
            {
                Y = otherAxis;
                Z = pt.Y;
            }
            else
            {
                Y = pt.Y;
                Z = otherAxis;
            }
        }

        internal void ResetTransformation()
        {
            ModelTransformedX = X;
            ModelTransformedY = Y;
            ModelTransformedZ = Z;
            WorldTransformedX = X;
            WorldTransformedY = Y;
            WorldTransformedZ = Z;
            CameraTransformedX = X;
            CameraTransformedY = Y;
            CameraTransformedZ = Z;
            ViewportX = 0;
            ViewportY = 0;
        }

        internal void ModelTransform(Point3d modelTranslation, double rotateAroundXAxis, double rotateAroundYAxis)
        {
            double rotatingX = X;
            double rotatingY = Y * Math.Cos(rotateAroundXAxis) - Z * Math.Sin(rotateAroundXAxis);
            double rotatingZ = Y * Math.Sin(rotateAroundXAxis) + Z * Math.Cos(rotateAroundXAxis);

            ModelTransformedX = rotatingX * Math.Cos(rotateAroundYAxis) - rotatingZ * Math.Sin(rotateAroundYAxis);
            ModelTransformedY = rotatingY;
            ModelTransformedZ = rotatingX * Math.Sin(rotateAroundYAxis) + rotatingZ * Math.Cos(rotateAroundYAxis);

            ModelTransformedX += modelTranslation.X;
            ModelTransformedY += modelTranslation.Y;
            ModelTransformedZ += modelTranslation.Z;
        }

        internal void ModelTransform(double rotateAroundXAxis, double rotateAroundYAxis)
        {
            double rotatingX = X;
            double rotatingY = Y * Math.Cos(rotateAroundXAxis) - Z * Math.Sin(rotateAroundXAxis);
            double rotatingZ = Y * Math.Sin(rotateAroundXAxis) + Z * Math.Cos(rotateAroundXAxis);

            ModelTransformedX = rotatingX * Math.Cos(rotateAroundYAxis) - rotatingZ * Math.Sin(rotateAroundYAxis);
            ModelTransformedY = rotatingY;
            ModelTransformedZ = rotatingX * Math.Sin(rotateAroundYAxis) + rotatingZ * Math.Cos(rotateAroundYAxis);

            //ModelTransformedX = X * Math.Cos(rotateAroundYAxis) - Z * Math.Sin(rotateAroundYAxis);
            //ModelTransformedY = Y;
            //ModelTransformedZ = X * Math.Sin(rotateAroundYAxis) + Z * Math.Cos(rotateAroundYAxis);
        }

        internal void WorldTransform(double rotateAroundYAxis, Point3d translateBy)
        {
            WorldTransformedX = ModelTransformedX * Math.Cos(rotateAroundYAxis) - ModelTransformedZ * Math.Sin(rotateAroundYAxis);
            WorldTransformedY = ModelTransformedY;
            WorldTransformedZ = ModelTransformedX * Math.Sin(rotateAroundYAxis) + ModelTransformedZ * Math.Cos(rotateAroundYAxis);
            WorldTransformedX += translateBy.X;
            WorldTransformedY += translateBy.Y;
            WorldTransformedZ += translateBy.Z;
        }

        internal void CameraTransform(Angle rotateAroundXAxis, Angle rotateAroundYAxis, Point3d cameraLocation)
        {
            rotateAroundXAxis = rotateAroundXAxis.Inverted();
            rotateAroundYAxis= rotateAroundYAxis.Inverted();

            double x = WorldTransformedX - cameraLocation.X;
            double y = WorldTransformedY - cameraLocation.Y;
            double z = WorldTransformedZ - cameraLocation.Z;

            CameraTransformedX = x;
            CameraTransformedY = y * rotateAroundXAxis.Cos - z * rotateAroundXAxis.Sin;
            CameraTransformedZ = y * rotateAroundXAxis.Sin + z * rotateAroundXAxis.Cos;

            x = CameraTransformedX;
            z = CameraTransformedZ;

            CameraTransformedX = x * rotateAroundYAxis.Cos - z * rotateAroundYAxis.Sin;
            CameraTransformedZ = x * rotateAroundYAxis.Sin + z * rotateAroundYAxis.Cos;
        }

        internal void ViewportTransform(Point viewportOrigin, double focalLength)
        {
            if (Math.Abs(CameraTransformedZ) < 0.001)
            {
                CameraTransformedZ = 0.001;
            }
            ViewportX = viewportOrigin.X + (int)(focalLength * CameraTransformedX / CameraTransformedZ);
            ViewportY = viewportOrigin.Y - (int)(focalLength * CameraTransformedY / CameraTransformedZ);
        }

        public static EntityVertex operator +(EntityVertex lhs, EntityVertex rhs)
        {
            EntityVertex toReturn;

            toReturn = new EntityVertex(lhs.X + rhs.X, lhs.Y + rhs.Y, lhs.Z + rhs.Z);
            return toReturn;
        }

        public static EntityVertex operator /(EntityVertex lhs, int divideBy)
        {
            EntityVertex toReturn;

            toReturn = new EntityVertex(lhs.X / divideBy, lhs.Y / divideBy, lhs.Z / divideBy);
            return toReturn;
        }

        public double SquaredDistanceTo(EntityVertex v)
        {
            double diffX = v.X - X;
            double diffY = v.Y - Y;
            double diffZ = v.Z - Z;
            return diffX * diffY + diffY * diffY + diffZ * diffZ;
        }

        public double DistanceTo(EntityVertex v)
        {
            return Math.Sqrt(SquaredDistanceTo(v));
        }
    }
}
