using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using TankGame.Maths;

namespace TankGame.Engine
{
    internal class BoundingBox : BaseEntity
    {
        internal enum BoundingBoxCorner
        {
            NearLeftBottom = 0,
            NearLeftTop = 1,
            NearRightTop = 2,
            NearRightBottom = 3,
            FarLeftBottom = 4,
            FarLeftTop = 5,
            FarRightTop = 6,
            FarRightBottom =7
        }
        //EntityVertex?[] _vertices = new EntityVertex?[8];

        double _leftX;
        double _rightX;
        double _nearZ;
        double _farZ;
        double _bottomY;
        double _topY;
        bool _firstVertex = true;
        internal bool IsAABB { get; private set; }

        internal BoundingBox(string boundedEntityName) : base($"boundingbox_{boundedEntityName}", false)
        {
            LineColour = Color.White;

            //for(int i=0;i<7;++i)
            //{
            //    _vertices[i] = null;
            //}
            IsAABB = false;
        }

        internal BoundingBox(string boundedEntityName, double leftX, double rightX,double bottomY, double topY, double nearZ, double farZ) : base($"boundingbox_{boundedEntityName}", false)
        {
            _leftX = leftX;
            _rightX = rightX;
            _nearZ = nearZ;
            _farZ = farZ;
            _bottomY = bottomY;
            _topY = topY;
            IsAABB = true;
        }

        internal void AddUnderlyingModelVertex(EntityVertex vertex)
        {
            AddUnderlyingModelVertex(vertex.PointInLocalSpace);
        }

        internal void AddUnderlyingModelVertex(Point3d vertex)
        {
            if (_firstVertex)
            {
                SetFirstVertex(vertex);
            }
            else
            {
                if (vertex.X < _leftX)
                {
                    _leftX = vertex.X;
                }
                else if (vertex.X > _rightX)
                {
                    _rightX = vertex.X;
                }
                if (vertex.Y > _topY)
                {
                    _topY = vertex.Y;
                }
                else if (vertex.Y < _bottomY)
                {
                    _bottomY = vertex.Y;
                }
                if (vertex.Z>_farZ)
                {
                    _farZ= vertex.Z;
                }
                else if (vertex.Z<_nearZ)
                {
                    _nearZ = vertex.Z;
                }
            }
        }

        void SetFirstVertex(Point3d vertex)
        {
            _leftX = vertex.X;
            _rightX = vertex.X;
            _nearZ = vertex.Z;
            _farZ = vertex.Z;
            _topY = vertex.Y;
            _bottomY = vertex.Y;

            _firstVertex = false;
        }

        internal void CreateBoundingBoxFromExtemes()
        {
            AddVertex(_leftX, _bottomY, _nearZ);
            AddVertex(_leftX, _topY, _nearZ);
            AddVertex(_rightX, _topY, _nearZ);
            AddVertex(_rightX, _bottomY, _nearZ);

            AddVertex(_leftX, _bottomY, _farZ);
            AddVertex(_leftX, _topY, _farZ);
            AddVertex(_rightX, _topY, _farZ);
            AddVertex(_rightX, _bottomY, _farZ);

            AddQuadrangle(0, 1, 2, 3);
            AddQuadrangle(0, 4, 5, 1);
            AddQuadrangle(4, 7, 6, 5);
            AddQuadrangle(3, 2, 6, 7);
            AddQuadrangle(0, 3, 7, 4);
            AddQuadrangle(1, 5, 6, 2);
        }

        internal BoundingBox? GetAxisAlignedBoundingBox()
        {
            if (_vertices == null)
            {
                return null;
            }
            double leftX=_vertices![0].WorldTransformedX;
            double rightX= _vertices![0].WorldTransformedX;
            double bottomY = _vertices[0].WorldTransformedY;
            double topY = _vertices[0].WorldTransformedY;
            double nearZ = _vertices[0].WorldTransformedZ;
            double farZ = _vertices[0].WorldTransformedZ;

            for(int index=1;index<8;++index)
            {
                if (_vertices[index].WorldTransformedX<leftX)
                {
                    leftX = _vertices[index].WorldTransformedX;
                }
                else if (_vertices[index].WorldTransformedX > rightX)
                {
                    rightX = _vertices[index].WorldTransformedX;
                }

                if (_vertices[index].WorldTransformedY < bottomY)
                {
                    bottomY = _vertices[index].WorldTransformedY;
                }
                else if (_vertices[index].WorldTransformedY > topY)
                {
                    topY = _vertices[index].WorldTransformedY;
                }

                if (_vertices[index].WorldTransformedZ < nearZ)
                {
                    nearZ = _vertices[index].WorldTransformedZ;
                }
                else if (_vertices[index].WorldTransformedZ > farZ)
                {
                    farZ = _vertices[index].WorldTransformedZ;
                }
            }
            return new BoundingBox(EntityName, leftX, rightX,bottomY, topY, nearZ, farZ);
        }

        internal bool IntersectsWithAABB(BoundingBox aabb)
        {
            if (IsAABB == false || aabb.IsAABB==false)
            {
                return false;
            }

            if (_leftX>aabb._rightX ||
                _rightX<aabb._leftX)
            {
                return false;
            }
            if (_topY < aabb._bottomY ||
                _bottomY > aabb._topY)
            {
                return false;
            }
            if (_nearZ > aabb._farZ ||
                _farZ < aabb._nearZ)
            {
                return false;
            }
            return true;
        }

        override internal void AddPolygonsToSortedList(SortedList<int, List<Polygon>> depthOrderedPolygons)
        {
            AddPolygonsToSortedList(depthOrderedPolygons, true);
        }
    }
}
