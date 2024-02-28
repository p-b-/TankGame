using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TankGame.Engine;
using TankGame.Maths;

namespace TankGame.GameEntities
{
    internal class TankTurret : BaseEntity
    {
        internal void SetColour(Color c)
        {
            this.LineColour = c;
        }

        internal TankTurret() : base("TankTurret", 0, true, false)
        {
            this.LineColour = Color.DarkGreen;

            int m = 60;

            double topOfBaseHalfWidthFront = m * 1.5;
            double topOfBaseHalfWidthRear = m * 2;
            double topOfBaseDepthFront = m * 0.5;
            double topOfBaseDepthRear = m * 2;
            double topOfBaseHeight = m * 2;

            double bottomOfTurretRear = topOfBaseDepthRear * 0.75;
            double bottomOfTurretFront = topOfBaseDepthFront * 0.5;
            double bottomOfTurretHalfWidth = topOfBaseHalfWidthFront * 0.5;
            double topOfTurretDepthRear = topOfBaseDepthRear * 0.6;
            double heightOfTurret = m *0.8;

            double bottomOfGunHeight = heightOfTurret * 0.5;
            double topOfGunHeight = heightOfTurret * 0.7;
            double gunHalfWidth = bottomOfTurretHalfWidth * 0.2;
            double bottomOfGunDepthRear = topOfTurretDepthRear - m * 0.2;
            double lengthOfGun = m;
            double bottomOfGunDepthFront = bottomOfGunDepthRear + lengthOfGun;

            AddVertex(-bottomOfTurretHalfWidth, 0, -bottomOfTurretRear);
            AddVertex(-bottomOfTurretHalfWidth, 0,  bottomOfTurretFront);
            AddVertex( bottomOfTurretHalfWidth, 0,  bottomOfTurretFront);
            AddVertex( bottomOfTurretHalfWidth, 0, -bottomOfTurretRear);
            AddVertex(-bottomOfTurretHalfWidth, heightOfTurret, -topOfTurretDepthRear);
            AddVertex( bottomOfTurretHalfWidth, heightOfTurret, -topOfTurretDepthRear);

            //AddVertex(-gunHalfWidth, bottomOfGunHeight, -bottomOfGunDepthRear);
            //AddVertex(-gunHalfWidth, bottomOfGunHeight, bottomOfGunDepthFront);
            //AddVertex( gunHalfWidth, bottomOfGunHeight, bottomOfGunDepthFront);
            //AddVertex( gunHalfWidth, bottomOfGunHeight, -bottomOfGunDepthRear);
            //AddVertex(-gunHalfWidth, topOfGunHeight,  -bottomOfGunDepthRear);
            //AddVertex(-gunHalfWidth, topOfGunHeight, bottomOfGunDepthFront);
            //AddVertex(gunHalfWidth, topOfGunHeight, bottomOfGunDepthFront);
            //AddVertex(gunHalfWidth, topOfGunHeight, -bottomOfGunDepthRear);

            AddTriangle(0, 1, 4);
            AddTriangle(2, 3, 5);

            AddQuadrangle(1, 2, 5, 4);
            AddQuadrangle(0, 4, 5, 3);

            //AddQuadrangle(6, 7, 11, 10);
            //AddQuadrangle(7, 6, 9, 8); // bottom
            //AddQuadrangle(7, 8, 12, 11);
            //AddQuadrangle(8, 9, 13, 12);
            //AddQuadrangle(6, 10, 13, 9);
            //AddQuadrangle(11, 12, 13, 10);
            CalculateBoundingSphere();
        }
    }
}