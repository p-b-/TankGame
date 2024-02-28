using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TankGame.Engine;
using TankGame.Maths;

namespace TankGame.GameEntities
{
    internal class TankGun : BaseEntity
    {
        static int s_unitSize = 60;
        static int s_gunLength = 3* s_unitSize;
        int _gunStVertex1;
        int _gunStVertex2;
        int _gunEndVertex1;
        int _gunEndVertex2;
        internal void SetColour(Color c)
        {
            this.LineColour = c;
        }

        internal TankGun() : base("TankGun", 0,true,false)
        {
            this.LineColour = Color.DarkGreen;

            double topOfBaseHalfWidthFront = s_unitSize * 1.5;
            double topOfBaseHalfWidthRear = s_unitSize * 2;
            double topOfBaseDepthFront = s_unitSize * 0.5;
            double topOfBaseDepthRear = s_unitSize * 2;
            double topOfBaseHeight = s_unitSize * 2;

            double bottomOfTurretRear = topOfBaseDepthRear * 0.75;
            double bottomOfTurretFront = topOfBaseDepthFront * 0.5;
            double bottomOfTurretHalfWidth = topOfBaseHalfWidthFront * 0.5;
            double topOfTurretDepthRear = topOfBaseDepthRear * 0.6;
            double heightOfTurret = s_unitSize * 0.8;

            double bottomOfGunHeight = heightOfTurret * 0.5;
            double topOfGunHeight = heightOfTurret * 0.7;
            double gunHalfWidth = bottomOfTurretHalfWidth * 0.2;
            double bottomOfGunDepthRear = topOfTurretDepthRear - s_unitSize * 0.2;
            double lengthOfGun = s_unitSize*3;
            double bottomOfGunDepthFront = bottomOfGunDepthRear + lengthOfGun;


            
            AddVertex(-gunHalfWidth, bottomOfGunHeight, -bottomOfGunDepthRear);
            AddVertex(-gunHalfWidth, bottomOfGunHeight, bottomOfGunDepthFront);
            AddVertex(gunHalfWidth, bottomOfGunHeight, bottomOfGunDepthFront);
            AddVertex(gunHalfWidth, bottomOfGunHeight, -bottomOfGunDepthRear);
            _gunStVertex1 = AddVertex(-gunHalfWidth, topOfGunHeight, -bottomOfGunDepthRear);
            _gunEndVertex1 = AddVertex(-gunHalfWidth, topOfGunHeight, bottomOfGunDepthFront);
            _gunEndVertex2 = AddVertex(gunHalfWidth, topOfGunHeight, bottomOfGunDepthFront);
            _gunStVertex2 = AddVertex(gunHalfWidth, topOfGunHeight, -bottomOfGunDepthRear);

            AddQuadrangle(0,1, 5, 4);
            AddQuadrangle(1, 0, 3, 2); // bottom
            AddQuadrangle(1, 2, 6, 5);
            AddQuadrangle(2, 3, 7, 6);
            AddQuadrangle(0, 4, 7, 3);
            AddQuadrangle(5, 6, 7, 4);
            CalculateBoundingSphere();
        }

        internal Shell Fire()
        {
            double shellX = (_vertices[_gunEndVertex1].WorldTransformedX + _vertices[_gunEndVertex2].WorldTransformedX) / 2;
            double shellY = _vertices[_gunEndVertex1].WorldTransformedY;
            double shellZ = (_vertices[_gunEndVertex1].WorldTransformedZ + _vertices[_gunEndVertex2].WorldTransformedZ) / 2;

            double gunStX = (_vertices[_gunStVertex1].WorldTransformedX + _vertices[_gunStVertex2].WorldTransformedX) / 2;
            double gunStY = _vertices[_gunStVertex1].WorldTransformedY;
            double gunStZ = (_vertices[_gunStVertex1].WorldTransformedZ + _vertices[_gunStVertex2].WorldTransformedZ) / 2;

            PointFloat3d shellOrigin = new PointFloat3d(shellX, shellY, shellZ);
            PointFloat3d dir = shellOrigin - new PointFloat3d(gunStX, gunStY, gunStZ);

            Shell s = new Shell(dir, 10,150);
            s.OriginInWorldSpace = shellOrigin;

            return s;
        }
    }
}
