using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TankGame.Engine;
using TankGame.Maths;

namespace TankGame.GameEntities
{
    internal class TankBase : BaseEntity
    {
        internal void SetColour(Color c)
        {
            this.LineColour = c;
        }

        internal TankBase() : base("TankBase", 0, true, false)
        {
            this.LineColour = Color.DarkGreen;

            int m = 60;

            double bottomHalfWidth = m * 2;
            double bottomDepthRear = m * 2;
            double bottomDepthFront = m * 2;

            double outsideBaseHalfWidth = m * 3;
            double outsideBaseDepthFront = m * 4;
            double outsideBaseDepthRear = m * 3;
            double outsideBaseHeight = m * 1;

            double topOfBaseHalfWidthFront = m * 1.5;
            double topOfBaseHalfWidthRear = m * 2;
            double topOfBaseDepthFront = m * 0.5;
            double topOfBaseDepthRear = m * 2;
            double topOfBaseHeight = m * 2;

            AddVertex(-bottomHalfWidth, 0, -bottomDepthRear);
            AddVertex(-bottomHalfWidth, 0,  bottomDepthFront);
            AddVertex( bottomHalfWidth, 0,  bottomDepthFront);
            AddVertex( bottomHalfWidth, 0, -bottomDepthRear);

            AddVertex(-outsideBaseHalfWidth, outsideBaseHeight, -outsideBaseDepthRear);
            AddVertex(-outsideBaseHalfWidth, outsideBaseHeight,  outsideBaseDepthFront);
            AddVertex( outsideBaseHalfWidth, outsideBaseHeight,  outsideBaseDepthFront);
            AddVertex( outsideBaseHalfWidth, outsideBaseHeight, -outsideBaseDepthRear);

            AddVertex(-topOfBaseHalfWidthRear, topOfBaseHeight,-topOfBaseDepthRear);
            AddVertex(-topOfBaseHalfWidthFront, topOfBaseHeight, topOfBaseDepthFront);
            AddVertex( topOfBaseHalfWidthFront, topOfBaseHeight, topOfBaseDepthFront);
            AddVertex( topOfBaseHalfWidthRear, topOfBaseHeight,-topOfBaseDepthRear);

            AddTriangle(0, 3, 2);
            AddTriangle(0, 2, 1);

            AddQuadrangle(0, 4, 7, 3);
            AddQuadrangle(0, 1, 5, 4);
            AddQuadrangle(1, 2, 6, 5);
            AddQuadrangle(3, 7, 6, 2);


            AddQuadrangle(4, 8, 11, 7);
            AddQuadrangle(4, 5, 9, 8);
            AddQuadrangle(5, 6, 10, 9);
            AddQuadrangle(6, 7, 11, 10);

            AddQuadrangle(8, 9, 10, 11);
            CalculateBoundingSphere();
        }
    }
}
