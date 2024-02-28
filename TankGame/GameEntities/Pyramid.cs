using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TankGame.Engine;
using TankGame.Maths;

namespace TankGame.GameEntities
{
    internal class Pyramid : BaseEntity
    {
        internal void SetColour(Color c)
        {
            this.LineColour = c;
        }
        internal Pyramid() : base("Pyramid", 100000, true, false)
        {
            this.LineColour = Color.DarkGreen;
            AddVertex(-70, 0, -70);
            AddVertex(0, 0, 70);
            AddVertex(70, 0, -70);
            AddVertex(0, 70, 0);

            AddTriangle(0, 1, 3);
            AddTriangle(1, 2, 3);
            AddTriangle(0, 3, 2);
            AddTriangle(0, 2, 1);

            CalculateBoundingSphere();
            CreateBoundingBox();
        }
    }
}
