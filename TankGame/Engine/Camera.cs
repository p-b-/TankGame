using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TankGame.Maths;

namespace TankGame.Engine
{
    internal class Camera
    {
        internal Angle RotationX { get; set ; }
        internal Angle RotationY { get; set; }

        internal PointFloat3d Location {  get; set; }

        internal int ClippingPlaneZ { get; private set; }

        internal int ProjectionMultiplier {  get; private set; }

        internal Frustrum ViewFrustrum { get; private set; }

        public Camera(int clippingPlaneZ, int projectionMultiplier, int viewportWidth, int viewportHeight)
        {
            RotationX = 0;
            RotationY = 0;
            ClippingPlaneZ = clippingPlaneZ;
            Location = new PointFloat3d(0, 0, 0);
            ClippingPlaneZ = clippingPlaneZ;
            ProjectionMultiplier = projectionMultiplier;

            ViewFrustrum = new Frustrum(projectionMultiplier,viewportWidth, viewportHeight);
        }

        internal void RotateAroundYAxis(double rotateBy)
        {
            RotationY += rotateBy;
        }

        internal void RotateAroundXAxis(double rotateBy)
        {
            RotationX += rotateBy;
        }

        internal void MoveForwardBy(double moveBy)
        {
            double newX = Location.X + moveBy * RotationY.Inverted().Sin;
            double newY = Location.Y;
            double newZ = Location.Z + moveBy * RotationY.Inverted().Cos;
            Location = new PointFloat3d(newX, newY, newZ);
        }

        internal void SetCameraHeight(int height)
        {
            double newX = Location.X;
            double newY = height;
            double newZ = Location.Z;
            Location = new PointFloat3d(newX, newY, newZ);

        }
         
        internal void UpdateLocationFromAttachment(Point3d attachedEntityWorldLocation, Point3d attachmentOffset, Angle attachedEntityYRotation)
        {
            PointFloat3d rotatedAttachment = attachmentOffset.RotatedAroundYAxis(attachedEntityYRotation);

            Location = attachedEntityWorldLocation + rotatedAttachment;
        }
    }
}
