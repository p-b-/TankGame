using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TankGame.Maths
{
    internal class Polygon
    {
        List<EntityVertex> _vertices = new List<EntityVertex>();

        internal int FromEntity { get; set; }
        internal int EntityTriangle { get; set; }

        public int SquaredDistanceToOrigin { get; set; }
        public Color LineColour { get; set; }

        public bool BoundingPolygon { get; set; }
        public Polygon()
        {
            BoundingPolygon = false;
        }

        public Polygon(Polygon p)
        {
            BoundingPolygon = p.BoundingPolygon;
            FromEntity = p.FromEntity;
            EntityTriangle = p.EntityTriangle;
            SquaredDistanceToOrigin= p.SquaredDistanceToOrigin;
            LineColour = p.LineColour;
            foreach(EntityVertex v in p._vertices)
            {
                _vertices.Add(new EntityVertex(v));
            }
        }

        public Polygon(List<EntityVertex> verticies)
        {
            BoundingPolygon = false;
            _vertices = verticies;
        }

        public void AddVertex(EntityVertex v)
        {
            _vertices.Add(v);
        }

        public int VertexCount()
        {
            return _vertices.Count;
        }

        public EntityVertex VertexAtIndex(int index)
        {
            return _vertices[index];
        }

        public void CalcAverageSquaredDistanceToOrigin()
        {
            if (_vertices.Count > 0)
            {
                double x = 0;
                double y = 0;
                double z = 0;
                for (int vertexIndex = 0; vertexIndex < _vertices.Count; vertexIndex++)
                {
                    x += _vertices[vertexIndex].CameraTransformedX;
                    y += _vertices[vertexIndex].CameraTransformedY;
                    z += _vertices[vertexIndex].CameraTransformedZ;
                }

                x /= _vertices.Count;
                y /= _vertices.Count;
                z /= _vertices.Count;

                SquaredDistanceToOrigin = (int)(x * x + y * y + z * z);
            }
        }
    }
}
