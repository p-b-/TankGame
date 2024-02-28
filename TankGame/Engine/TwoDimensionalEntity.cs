//using System;
//using System.Collections.Generic;
//using System.ComponentModel;
//using System.Linq;
//using System.Net.NetworkInformation;
//using System.Runtime.CompilerServices;
//using System.Security.AccessControl;
//using System.Text;
//using System.Threading.Tasks;
//using System.Transactions;
//using System.Windows.Forms;
//using TankGame.Maths;

//namespace TankGame.Engine
//{
//    internal class TwoDimensionalEntity : BaseEntity
//    {
//        private struct LineDescription
//        {
//            public int Point1 { get; set; }
//            public int Point2 { get; set; }
//            public LineDescription(int p1, int p2)
//            {
//                Point1 = p1;
//                Point2 = p2;
//            }
//        }

//        private bool _singlePolygon;
//        private bool _onHorizon = true;
//        private int _farDistance = 20000;
//        private double _scale = 1.0;
//        private List<Point> _outlinePoints = new List<Point>();
//        private List<Point> _transformedPoints = new List<Point>();

//        private Point3d _transformedCentreVertex;
//        private EntityVertex _cameraTransformedCentreVertex;
//        private double _transformedScaleX;
//        private double _transformedScaleZ;
//        private double _transformedScale;
//        private List<LineDescription> _outlineLines = new List<LineDescription>();

//        internal TwoDimensionalEntity(Point3d origin, bool singlePolygon, double scale = 1.0) : base(origin, 0.0, 0.0)
//        {
//            LineColour = Color.DarkGreen;

//            _onHorizon = false;
//            _scale = scale;
//            _singlePolygon = singlePolygon;
//        }

//        internal TwoDimensionalEntity(double worldYAxisRotation, bool singlePolygon, double scale = 1.0) : base(new Point3d(0, 0, 0), 0.0, worldYAxisRotation)
//        {
//            LineColour = Color.DarkGreen;

//            _onHorizon = true;
//            _scale = scale;
//            _singlePolygon = singlePolygon;
//        }

//        private protected void AddOutlinePoint(Point point)
//        {
//            _outlinePoints.Add(point);
//        }

//        private protected void AddOutlineLine(int point1, int point2)
//        {
//            _outlineLines.Add(new LineDescription(point1, point2));
//        }

//        //override internal void DrawEntity(Graphics g, Point viewportOrigin, int viewportWidth, int viewportHeight)
//        //{
//        //    if (!OnScreen)
//        //    {
//        //        return;
//        //    }
//        //    Pen pen = new Pen(LineColour);
//        //    Brush brush = new SolidBrush(Color.Black);

//        //    if (_singlePolygon)
//        //    {
//        //        g.FillPolygon(brush, _transformedPoints.ToArray());
//        //        g.DrawPolygon(pen, _transformedPoints.ToArray());
//        //    }
//        //    else
//        //    {
//        //        foreach (Triangle t in _triangles)
//        //        {
//        //            int ptIndex1 = t.V1;
//        //            int ptIndex2 = t.V2;
//        //            int ptIndex3 = t.V3;
//        //            Point[] pts = {
//        //            _transformedPoints[ptIndex1],
//        //            _transformedPoints[ptIndex2],
//        //            _transformedPoints[ptIndex3]};

//        //            g.FillPolygon(brush, pts);
//        //            g.DrawPolygon(pen, pts);
//        //        }
//        //    }
//        //}

//        override internal void Transform()
//        {
//            double x = OriginInWorldSpace.X;
//            double y = OriginInWorldSpace.Y;
//            double z = OriginInWorldSpace.Z;
//            if (_onHorizon)
//            {
//                z = _farDistance;
//            }
//            double rot = WorldYAxisRotation;
//            double transformedX = x * Math.Cos(rot) - z * Math.Sin(rot);
//            double transformedY = y;
//            double transformedZ = x * Math.Sin(rot) + z * Math.Cos(rot);
//            _transformedCentreVertex = new Point3d((int)transformedX, (int)transformedY, (int)transformedZ);

//            if (_onHorizon)
//            {
//                _transformedScaleX = 0.0;
//                _transformedScaleZ = _scale;
//            }
//            else
//            {
//                x = 0.0;
//                z = _scale;
//                _transformedScaleX = x * Math.Cos(rot) - z * Math.Sin(rot);
//                _transformedScaleZ = x * Math.Sin(rot) + z * Math.Cos(rot);
//            }
//        }

//        override internal void CameraTransformWithFrustrumCull(Camera camera)
//        {
//            OnScreen = true;
//            double rotatedX = _transformedCentreVertex.X * camera.RotationY.Inverted().Cos - _transformedCentreVertex.Z * camera.RotationY.Inverted().Sin;
//            double rotatedY = _transformedCentreVertex.Y;
//            double rotatedZ = _transformedCentreVertex.X * camera.RotationY.Inverted().Sin + _transformedCentreVertex.Z * camera.RotationY.Inverted().Cos;

//            _transformedCentreVertex = new Point3d((int)rotatedX, (int)rotatedY, (int)rotatedZ);

//            if (!_onHorizon)
//            {
//                rotatedX = _transformedScaleX * Math.Cos(-camera.RotationY) - _transformedScaleZ * Math.Sin(-camera.RotationY);
//                rotatedZ = _transformedScaleX * Math.Sin(-camera.RotationY) + _transformedScaleZ * Math.Cos(-camera.RotationY);

//                _transformedScaleX = rotatedX;
//                _transformedScaleZ = rotatedZ;

//            }
//            _transformedScale = _transformedScaleZ;
//            if (_transformedCentreVertex.Z < 0)
//            {
//                OnScreen = false;
//            }

//        }
//        override internal void ClipToViewport(int clippingPlaneZ, bool cullBackfaces)
//        {
//        }

//        override internal void ProjectToViewport(Point viewportOrigin, double focalLength, Size viewportSize)
//        {
//            if (!OnScreen)
//            {
//                return;
//            }
//            _transformedPoints.Clear();
//            _cameraTransformedCentreVertex = new EntityVertex(_transformedCentreVertex);
//            _cameraTransformedCentreVertex.CameraTransformedX = _transformedCentreVertex.X;
//            _cameraTransformedCentreVertex.CameraTransformedY = _transformedCentreVertex.Y;
//            _cameraTransformedCentreVertex.CameraTransformedZ = _transformedCentreVertex.Z;
//            _cameraTransformedCentreVertex.ViewportTransform(viewportOrigin, focalLength);
//            int countToLeft = 0;
//            int countToRight = 0;
//            int countAbove = 0;
//            int countBelow = 0;

//            for (int vertexIndex = 0; vertexIndex < _outlinePoints.Count; vertexIndex++)
//            {
//                Point v = _outlinePoints[vertexIndex];
//                v = new Point((int)(v.X * _transformedScale), (int)(v.Y * _transformedScale));

//                Point transformedPoint = new Point(v.X + _cameraTransformedCentreVertex.ViewportX, _cameraTransformedCentreVertex.ViewportY - v.Y);

//                _transformedPoints.Add(transformedPoint);

//                if (transformedPoint.X<0)
//                {
//                    ++countToLeft;
//                }
//                if (transformedPoint.X >= viewportSize.Width)
//                {
//                    ++countToRight;
//                }
//                if (transformedPoint.Y < 0)
//                {
//                    ++countAbove;
//                }
//                if (transformedPoint.Y >= viewportSize.Height)
//                {
//                    ++countBelow;
//                }
//            }
//            if (countToLeft == _transformedPoints.Count ||
//                countToRight == _transformedPoints.Count ||
//                countAbove == _transformedPoints.Count ||
//                countBelow == _transformedPoints.Count)
//            {
//                OnScreen = false;
//            }
//        }

//        override internal void AddPolygonsToSortedList(SortedList<int, List<Polygon>> depthOrderedPolygons)
//        {
//            if (!OnScreen)
//            {
//                return;
//            }
//            int sqDist;
//            if (_onHorizon)
//            {
//                sqDist = -_farDistance * _farDistance;
//            }
//            else
//            {
//                double z = _cameraTransformedCentreVertex.CameraTransformedZ;
//                sqDist = (int)(z * z);
//            }

//            List<Polygon>? polyList;
//            if (!depthOrderedPolygons.TryGetValue(sqDist, out polyList))
//            {
//                polyList = new List<Polygon>();
//                depthOrderedPolygons[sqDist] = polyList;
//            }

//            if (_singlePolygon)
//            {
//                Polygon poly = new Polygon();
//                for (int vertexIndex = 0; vertexIndex < _transformedPoints.Count; vertexIndex++)
//                {
//                    Point p = _transformedPoints[vertexIndex];
//                    EntityVertex vertex = new EntityVertex();
//                    vertex.ViewportX = p.X;
//                    vertex.ViewportY = p.Y;
//                    poly.AddVertex(vertex);
//                }
//                poly.LineColour = LineColour;
//                poly.SquaredDistanceToOrigin = _farDistance * _farDistance;
//                polyList.Add(poly);
//            }
//            else
//            {
//                for(int triIndex = 0; triIndex< _entityPolygons.Count; ++triIndex)
//                {
//                    EntityPolygon t = _entityPolygons[triIndex];
//                    Polygon poly = new Polygon();
//                    for(int triPointIndex=0;triPointIndex<3;++triPointIndex)
//                    {
//                        int vertexIndex = t.VertexIndex(triPointIndex);
//                        Point p = _transformedPoints[vertexIndex];

//                        EntityVertex vertex = new EntityVertex();
//                        vertex.ViewportX = p.X;
//                        vertex.ViewportY = p.Y;
//                        poly.AddVertex(vertex);
//                    }
//                    poly.LineColour = LineColour;
//                    poly.SquaredDistanceToOrigin = _farDistance * _farDistance;
//                    polyList.Add(poly);
//                }
//            }
//        }
//    }
//}
