using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using TankGame.Maths;

namespace TankGame.Engine
{
    internal class GameEngine
    {
        Dictionary<Color, Pen> _pens = new Dictionary<Color, Pen>();
        Brush _brush = new SolidBrush(Color.Black);
        SortedList<int, List<Polygon>> _depthOrderedPolygons = new SortedList<int, List<Polygon>>();
        object _depthOrderedPolygonsScreenReadyLock = new object();
        SortedList<int, List<Polygon>> _depthOrderedPolygonsScreenReady;

        Font _statsFont;
        Brush _statsBrush;

        List<BaseEntity> _entities = new List<BaseEntity>();
        internal bool ReplayingFrames { get; set; } = false;

        double _frameRate;
        bool _frameRateValid = false;

        int _sightSize = 150;
        int _sightOffset = 30;
        Color sightColour = Color.ForestGreen;

        int _viewportWidth;
        int _viewportHeight;
        double _cameraProjectionMultiplier = 512.0;
        int _clippingPlaneZ = 100;
        Camera? _camera;

        List<SortedList<int, List<Polygon>>> _frames = new List<SortedList<int, List<Polygon>>>();
        internal int RedrawFrameIndex { get; set; }

        internal int FrameIndex { get; private set; }

        int _visibleEntitiesCount = 0;
        CollisionSpace _collisionSpace;
        internal bool DrawBoundingBoxes
        {
            get;
            set;
        }

        internal GameEngine()
        {
            this._statsFont = new System.Drawing.Font("Consolas", 14F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            Color c = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(192)))), ((int)(((byte)(0))))); ;
            this._statsBrush = new SolidBrush(c);
            this._collisionSpace = new CollisionSpace(40000, 40000, 20000, 20000, 1000);
            DrawBoundingBoxes = false;
            FrameIndex = 0;
        }

        internal void GameLoop(Graphics graphics, int width, int height, Camera camera)
        {
            _viewportWidth = width;
            _viewportHeight = height;
            _camera = camera;
            _cameraProjectionMultiplier = camera.ProjectionMultiplier;
            _clippingPlaneZ = camera.ClippingPlaneZ;

            AnimateEntities();
            TransformBoundingBoxes();
            TransformEntities();
            DrawEntities(graphics, new Point(width / 2, height / 2));
//            DrawSight(graphics);
            DrawStats(graphics);
            ++FrameIndex;
        }

        private void DrawSight(Graphics g)
        {
            int middleX = _viewportWidth / 2;
            int middleY = _viewportHeight / 2;

            Pen sightPen = new Pen(this.sightColour);
            g.DrawLine(sightPen, middleX - _sightSize / 2, middleY, middleX - _sightOffset, middleY);
            g.DrawLine(sightPen, middleX + _sightSize / 2, middleY, middleX + _sightOffset, middleY);
            g.DrawLine(sightPen, middleX, middleY - _sightSize / 2, middleX, middleY - _sightOffset);
            g.DrawLine(sightPen, middleX, middleY + _sightSize / 2, middleX, middleY + _sightOffset);
        }

        private void AnimateEntities()
        {
            List<BaseEntity> toDelete = new List<BaseEntity>();
            for(int beIndex = 0;beIndex<this._entities.Count;++beIndex)
            {
                BaseEntity be = _entities[beIndex];
                if (be.Animates)
                {
                    if (be.Animate(20.0) == false)
                    {
                        toDelete.Add(be);
                    }
                }
            }
            foreach(BaseEntity be in toDelete)
            {
                this._entities.Remove(be);
            }
        }

        private void TransformBoundingBoxes()
        {
            foreach (BaseEntity be in this._entities)
            {
                be.TransformBoundingBoxToCameraView(_camera!);
            }
        }

        private void TransformEntities()
        {
            _depthOrderedPolygons.Clear();
            Point viewportOrigin = new Point(_viewportWidth / 2, _viewportHeight / 2);
            Size viewportSize = new Size(_viewportWidth, _viewportHeight);
            foreach (BaseEntity be in this._entities)
            {
                be.Transform();
                be.CameraTransformWithFrustrumCull(_camera!);
                be.ClipToViewport(_clippingPlaneZ, true);
                be.ProjectToViewport(viewportOrigin, _cameraProjectionMultiplier, viewportSize);
                if (DrawBoundingBoxes)
                {
                    be.ClipBoundingBoxToViewPort(_clippingPlaneZ);
                    be.ProjectBoundingBoxToViewPort(viewportOrigin, _cameraProjectionMultiplier, viewportSize);
                    be.AddBoundingBoxPolygonsToSortedList(_depthOrderedPolygons);
                }
                be.AddPolygonsToSortedList(_depthOrderedPolygons);
            }
            lock(_depthOrderedPolygonsScreenReadyLock)
            {
                _depthOrderedPolygonsScreenReady = _depthOrderedPolygons;
                _depthOrderedPolygons = new SortedList<int, List<Polygon>>();
            }
        }

        private void DrawEntitiesForFrame(int frameIndex, Graphics g, Point origin)
        {
            DrawPolygonsInDepthSortedList(g, _frames[frameIndex]);
        }

        private void DrawEntities(Graphics g, Point origin)
        {
            if (ReplayingFrames)
            {
                DrawEntitiesForFrame(RedrawFrameIndex, g, origin);
                return;
            }
            _visibleEntitiesCount = 0;
            foreach (BaseEntity be in this._entities)
            {
                if (be.OnScreen)
                {
                    _visibleEntitiesCount++;
                }
            }
            lock (_depthOrderedPolygonsScreenReadyLock)
            {
                CopyFrameToList(_depthOrderedPolygonsScreenReady);
                DrawPolygonsInDepthSortedList(g, this._depthOrderedPolygonsScreenReady);
            }
        }

        private void CopyFrameToList(SortedList<int, List<Polygon>> polygons)
        {
            SortedList<int, List<Polygon>> newSortedList = new SortedList<int, List<Polygon>>();
            foreach (int d in polygons.Keys)
            {
                List<Polygon> newPolygonList = new List<Polygon>();
                foreach (Polygon p in polygons[d])
                {
                    newPolygonList.Add(new Polygon(p));
                }
                newSortedList.Add(d, newPolygonList);
            }
            _frames.Add(newSortedList);
            if (_frames.Count > 240)
            {
                _frames.RemoveAt(0);
            }
        }
        private void DrawPolygonsInDepthSortedList(Graphics g, SortedList<int, List<Polygon>> polygons)
        {
            foreach (List<Polygon> polyList in polygons.Values)
            {
                for (int polygonIndex = 0; polygonIndex < polyList.Count; polygonIndex++)
                {
                    Polygon poly = polyList[polygonIndex];

                    Pen pen;
                    if (!_pens.TryGetValue(poly.LineColour, out pen))
                    {
                        pen = new Pen(poly.LineColour);
                        _pens.Add(poly.LineColour, pen);
                    }
                    Point[] pts = new Point[poly.VertexCount()];

                    int countBottomLeft = 0;
                    int countAntiBottomLeft = 0;
                    for (int vertexIndex = 0; vertexIndex < poly.VertexCount(); ++vertexIndex)
                    {
                        pts[vertexIndex] = poly.VertexAtIndex(vertexIndex).ViewportPoint;
                        if (pts[vertexIndex].X < 50 && pts[vertexIndex].Y < 50)
                        {
                            countBottomLeft++;
                        }
                        else if (pts[vertexIndex].X > 400 && pts[vertexIndex].Y > 400)
                        {
                            countAntiBottomLeft++;
                        }

                    }
                    if (countBottomLeft == 1 && countAntiBottomLeft >= 2)
                    {
                        System.Diagnostics.Debug.WriteLine("Potential error");
                    }
                    if (poly.BoundingPolygon == false)
                    {
                        g.FillPolygon(_brush, pts);
                    }
                    g.DrawPolygon(pen, pts);
                }
            }
        }

        internal void SetFrameRate(double frameRate)
        {
            _frameRate = frameRate;
            _frameRateValid = true;
        }

        void DrawStats(Graphics g)
        {
            int height = 30;
            g.DrawString($"{_camera!.RotationY}", _statsFont, _statsBrush, _viewportWidth - 150, height * 1);
            g.DrawString($"{_camera!.Location.X}, {_camera!.Location.Z}", _statsFont, _statsBrush, _viewportWidth - 150, height * 2);

            if (_frameRateValid)
            {
                g.DrawString($"FPS {_frameRate:0}", _statsFont, _statsBrush, _viewportWidth - 150, height * 3);
            }
            if (ReplayingFrames == false)
            {
                g.DrawString($"Entities {_visibleEntitiesCount}", _statsFont, _statsBrush, _viewportWidth - 150, height * 4);
            }
            else
            {
                g.DrawString($"Frame {RedrawFrameIndex}", _statsFont, _statsBrush, _viewportWidth - 150, height * 4);
            }
        }

        internal void AddEntity(BaseEntity e)
        {
            e.ControllingEngine = this;
            e.Transform();
            this._entities.Add(e);
            this._collisionSpace.AddEntity(e);
        }

        internal bool FloorEntityCanMoveTo(BaseEntity entity, PointFloat3d newWorldOriginForEntity)
        {
            PointFloat3d currentOrigin = entity.OriginInWorldSpace;
            entity.OriginInWorldSpace = newWorldOriginForEntity;
            entity.Transform();
            entity.TransformBoundingBoxToWorld();
            List<BaseEntity> entitiesWithinSphere = _collisionSpace.EntitiesWithinSphere(entity, newWorldOriginForEntity);
            List<BaseEntity>? entitiesIntersectingAABB = null;
            List<BaseEntity>? entitiesIntersecting = null;
            if (entitiesWithinSphere.Count>0)
            {
                entitiesIntersectingAABB = EntitiesWithIntersectingAABB(entity, entitiesWithinSphere);
            }
            if (entitiesIntersectingAABB!=null && entitiesIntersectingAABB.Count>0)
            {
                entitiesIntersecting = EntitiesIntersectingAtFloorLevel(entity, entitiesIntersectingAABB);
            }
            if (entitiesIntersecting!=null && entitiesIntersecting.Count>0)
            {
                entity.OriginInWorldSpace = currentOrigin;
                return false;
            }
            _collisionSpace.RemoveEntity(entity, false);
            _collisionSpace.AddEntity(entity);
            return true;
        }

        List<BaseEntity> EntitiesWithIntersectingAABB(BaseEntity entity, List<BaseEntity> entitiesToCheck)
        {
            List<BaseEntity> entitiesIntersecting = new List<BaseEntity>();
            BoundingBox? bb = entity.GetAxisAlignedBoundingBox();

            for (int index = 0; index < entitiesToCheck.Count; ++index)
            {
                BoundingBox? otherBB = entitiesToCheck[index].GetAxisAlignedBoundingBox();
                if (bb.IntersectsWithAABB(otherBB))
                {
                    entitiesIntersecting.Add(entitiesToCheck[index]);
                }
            }
            return entitiesIntersecting;
        }

        List<BaseEntity> EntitiesIntersectingAtFloorLevel(BaseEntity entity, List<BaseEntity> entitiesToCheck)
        {
            List<BaseEntity> entitiesIntersecting = new List<BaseEntity>();

            for (int index = 0; index < entitiesToCheck.Count; ++index)
            {
                if (entity.IntersectsAtFloorLevel(entitiesToCheck[index]))
                {
                    entitiesIntersecting.Add(entitiesToCheck[index]);
                }
            }
            return entitiesIntersecting;
        }

        internal bool EntityCanRotateTo(BaseEntity entity, Angle newRotation)
        {
            return true;
        }

        internal void RedrawLastFrame()
        {
            RedrawFrameIndex--;
            if (RedrawFrameIndex < 0)
            {
                RedrawFrameIndex = _frames.Count - 1;
            }
        }

        internal void RedrawNextFrame()
        {
            RedrawFrameIndex++;
            if (RedrawFrameIndex == _frames.Count)
            {
                RedrawFrameIndex = 0;
            }
        }
    }
}
