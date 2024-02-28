using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TankGame.Engine;
using TankGame.Maths;

namespace TankGame.GameEntities
{
    internal class Tower : BaseEntity
    {
        internal void SetColour(Color c)
        {
            this.LineColour = c;
        }

        internal Tower() : base("Tower", 1000000,true, false)
        {
            this.LineColour = Color.DarkGreen;

            double towerRadius = 70.0f;
            int pointCount = 0;
            int height = 0;
            int maxLevel = 4;
            int verticesPerLevel = 8;
            for (int towerLevel = 0; towerLevel < maxLevel; ++towerLevel)
            {
                switch (towerLevel)
                {
                    case 0:
                        towerRadius = 200.0f;
                        height = 0;
                        break;
                    case 1:
                        towerRadius = 100.0f;
                        height = 200;
                        break;
                    case 2:
                        towerRadius = 80.0f;
                        height = 600;
                        break;
                    case 3:
                        towerRadius = 80.0f;
                        height = 1200;
                        break;

                }
                for (double a = 0.0; a < Math.PI * 2; a += Math.PI / (verticesPerLevel / 2))
                {
                    AddVertex((int)(towerRadius * Math.Cos(a)), height, (int)(towerRadius * Math.Sin(a)));
                    pointCount++;
                }
                if (towerLevel < maxLevel - 1)
                {
                    int baseVertexIndex = towerLevel * verticesPerLevel;

                    for (int vertexIndexOffset = 0; vertexIndexOffset < verticesPerLevel; ++vertexIndexOffset)
                    {
                        int index = baseVertexIndex + vertexIndexOffset;
                        int v1 = index + 0;
                        int v2 = index + 1;
                        int v3 = index + verticesPerLevel + 1;
                        int v4 = index + verticesPerLevel;

                        if (vertexIndexOffset == verticesPerLevel - 1)
                        {
                            v2 -= verticesPerLevel;
                            v3 -= verticesPerLevel;
                        }
                        AddTriangle(v3,v2,v1);
                        AddTriangle(v4,v3,v1);
                    }
                }
            }
            CalculateBoundingSphere();
            CreateBoundingBox();
        }
    }
}
