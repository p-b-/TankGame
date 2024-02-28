using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TankGame.Engine;
using TankGame.Maths;

namespace TankGame.GameEntities
{
    internal class Mountain3d : BaseEntity
    {
        internal void SetColour(Color c)
        {
            this.LineColour = c;
        }
        internal Mountain3d(int mountainTypeIndex) : base("3dmountain", 100000, true, false)
        {
            this.LineColour = Color.DarkGreen;
            //AddVertex(-1400, 0, -1400);
            //AddVertex(0, 0, 1400);
            //AddVertex(1400, 0, -1400);
            //AddVertex(0, 1400, 0);

            //AddTriangle(0, 1, 3);
            //AddTriangle(1, 2, 3);
            //AddTriangle(0, 3, 2);
            //AddTriangle(0, 2, 1);

            AddVertex(-1400, 0, -1400);
            AddVertex(0, 0, 1400);
            AddVertex(1400, 0, -1400);
            AddVertex(-700, 0, -1200);
            AddVertex(0, 1400, 0);

            AddTriangle(0, 1, 4);
            AddTriangle(1, 2, 4);
            AddTriangle(3, 4, 2);
            AddTriangle(0, 4, 3);
            //AddTriangle(0, 2, 1);


            CalculateBoundingSphere();
            CreateBoundingBox();
        }
    }
}
