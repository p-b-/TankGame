using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TankGame.Engine;
using TankGame.Maths;

namespace TankGame.GameEntities
{
    internal class Tank : CompositeEntity
    {
        int _baseEntityIndex;
        int _turretEntityIndex;
        int _gunEntityIndex;
        double _maxYRotation;
        double _maxXRotation;

        internal Tank() : base("Tank", 100, true)
        {
            TankBase tb = new TankBase();
            TankTurret tt = new TankTurret();
            TankGun tg = new TankGun();

            _maxYRotation = Math.PI / 2;
            _maxXRotation = Math.PI / 4;

            _baseEntityIndex = AddCompositingEntity(tb, 0, 0, new Point3d());
            _turretEntityIndex = AddCompositingEntity(tt, 0, 0, new Point3d(0, 120, 0));
            _gunEntityIndex = AddCompositingEntity(tg, 0, 0, new Point3d(0, 120, 0));

            CalculateBoundingSphere();
            CreateBoundingBox();
        }

        internal void GetTurrentRotation(out double rotationAroundXAxis, out double rotationAroundYAxis)
        {
            TankTurret? tt = GetCompositingEntity(_turretEntityIndex) as TankTurret;
            if (tt != null)
            {
                rotationAroundXAxis = tt!.ModelXAxisRotation;
                rotationAroundYAxis = tt!.ModelYAxisRotation;
            }
            else
            {
                rotationAroundXAxis = 0;
                rotationAroundYAxis = 0;
            }
        }

        internal void RotateTurretRightOrLeft(double rotateBy)
        {
            TankTurret? tt = GetCompositingEntity(_turretEntityIndex) as TankTurret;
            TankGun? tg = GetCompositingEntity(_gunEntityIndex) as TankGun;
            if (tt != null && tg != null)
            {
                double rotationAroundYAxis = tt!.ModelYAxisRotation;
                rotationAroundYAxis += rotateBy;
                if (rotationAroundYAxis > _maxYRotation)
                {
                    rotationAroundYAxis = _maxYRotation;
                }
                else if (rotationAroundYAxis < -_maxYRotation)
                {
                    rotationAroundYAxis = -_maxYRotation;
                }
                tt.ModelYAxisRotation = rotationAroundYAxis;
                tg.ModelYAxisRotation = rotationAroundYAxis;
            }
        }

        internal void RotateGunUpOrDown(double rotateBy)
        {
            TankGun? tg = GetCompositingEntity(_gunEntityIndex) as TankGun;
            if (tg != null)
            {
                double rotationAroundXAxis = tg!.ModelXAxisRotation;
                rotationAroundXAxis -= rotateBy;
                if (rotationAroundXAxis < -_maxXRotation)
                {
                    rotationAroundXAxis =-_maxXRotation;
                }
                else if (rotationAroundXAxis > 0)
                {
                    rotationAroundXAxis = 0;
                }
                tg.ModelXAxisRotation = rotationAroundXAxis;
            }
        }

        internal void MoveForward(double moveBy, int forFrameIndex)
        {
            // 0 degrees is along the x-axis.
            // tank points along z-axis
            Angle moveAlongThisRotation = WorldYAxisRotation + Math.PI / 2;
            double newX = OriginInWorldSpace.X + moveBy * moveAlongThisRotation.Cos;
            double newZ = OriginInWorldSpace.Z + moveBy * moveAlongThisRotation.Sin;
            PointFloat3d nextOrigin = new PointFloat3d(newX, OriginInWorldSpace.Y, newZ);

            if (ControllingEngine.FloorEntityCanMoveTo(this, nextOrigin))
            {
                OriginInWorldSpace = nextOrigin;
            }
        }

        internal void Rotate(double rotateBy)
        {
            Angle newRotation = WorldYAxisRotation + rotateBy;

            if (ControllingEngine.EntityCanRotateTo(this, newRotation))
            {
                WorldYAxisRotation = newRotation.Value;
            }
        }

        internal Shell Fire()
        {
            TankGun? tg = GetCompositingEntity(_gunEntityIndex) as TankGun;
            Shell shell = tg.Fire();

            return shell;
        }
    }
}
