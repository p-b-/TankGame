using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TankGame.Engine;
using TankGame.Maths;

namespace TankGame.GameEntities
{
    internal class Shell : BaseEntity
    {
        PointFloat3d _velocity;
        int _life;
        internal void SetColour(Color c)
        {
            this.LineColour = c;
        }

        internal Shell(PointFloat3d direction, double speed, int life) : base("Shell", 50, true, true) 
        {
            _life = life;
            this.LineColour = Color.DarkGreen;
            int m = 10;
            AddVertex(-m, 0, -m);
            AddVertex(0, 0, m);
            AddVertex(m, 0, -m);
            AddVertex(0, m, 0);
            AddVertex(0, -m, 0);

            AddTriangle(0, 1, 3);
            AddTriangle(1, 2, 3);
            AddTriangle(0, 3, 2);
            AddTriangle(0, 2, 4);
            AddTriangle(0, 4, 1);
            AddTriangle(4, 2, 1);

            this._velocity = direction * speed;

            CalculateBoundingSphere();
            CreateBoundingBox();
        }

        override internal bool Animate(double gravityY)
        {
            --_life;
            if (_life == 0)
            {
                return false;
            }
            else
            {
                OriginInWorldSpace = new PointFloat3d(OriginInWorldSpace.X + _velocity.X * 0.1,
                    OriginInWorldSpace.Y + (_velocity.Y - gravityY) * 0.1,
                    OriginInWorldSpace.Z + _velocity.Z * 0.1);
            }
            return true;
        }

    }
}
