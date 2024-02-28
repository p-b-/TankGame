using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data.SqlTypes;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.Intrinsics;
using System.Security;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms.VisualStyles;
using TankGame.Maths;
using static System.Windows.Forms.DataFormats;

namespace TankGame.Engine
{
    abstract internal class BaseEntity
    {
        protected private class EntityPolygon
        {
            List<int> _vertices = new List<int>();
            public int VertexCount { get { return _vertices.Count; } }
            public EntityPolygon(int v1, int v2, int v3)
            {
                _vertices.Add(v1);
                _vertices.Add(v2);
                _vertices.Add(v3);
            }

            public void AddVertexIndex(int vertexIndex)
            {
                _vertices.Add(vertexIndex);
            }

            public int VertexIndex(int vertexIndex)
            {
                return _vertices[vertexIndex];
            }
        }
        protected internal Color LineColour { get; set; }

        private PointFloat3d _originInWorldSpace;
        virtual internal PointFloat3d OriginInWorldSpace
        {
            get
            {
                return this._originInWorldSpace;
            }
            set
            {
                this._originInWorldSpace = value;
                if (this._boundingBox != null)
                {
                    this._boundingBox.OriginInWorldSpace = value;
                }
                if (AttachedCamera != null)
                {
                    AttachedCamera.UpdateLocationFromAttachment(_originInWorldSpace, CameraOffset, new Angle(WorldYAxisRotation));
                }
            }
        }

        private Point3d _originInModelSpace;
        internal Point3d OriginInModelSpace {
            get
            {
                return this._originInModelSpace;
            }
            set
            {
                this._originInModelSpace = value;
                if (this._boundingBox != null)
                {
                    this._boundingBox.OriginInModelSpace = value;
                }
            }
        }

        private double _modelYAxisRotation;
        internal double ModelYAxisRotation
        {
            get
            {
                return this._modelYAxisRotation;
            }
            set
            {
                this._modelYAxisRotation = value;
                if (this._boundingBox != null)
                {
                    this._boundingBox.ModelYAxisRotation = value;
                }
            }
        }

        private double _modelXAxisRotation;
        internal double ModelXAxisRotation
        {
            get
            {
                return this._modelXAxisRotation;
            }
            set
            {
                this._modelXAxisRotation = value;
                if (this._boundingBox != null)
                {
                    this._boundingBox.ModelXAxisRotation = value;
                }
            }
        }

        private double _worldYAxisRotation;
        virtual internal double WorldYAxisRotation
        {
            get
            {
                return this._worldYAxisRotation;
            }
            set
            {
                this._worldYAxisRotation = value;
                if (this._boundingBox != null)
                {
                    this._boundingBox.WorldYAxisRotation = value;
                }
                if (AttachedCamera != null)
                {
                    Angle newCameraYRotation = this._worldYAxisRotation + CameraYRotationOffset;

                    AttachedCamera.RotationY = newCameraYRotation;
                    AttachedCamera.UpdateLocationFromAttachment(_originInWorldSpace, CameraOffset, new Angle(WorldYAxisRotation));
                }
            }
        }

        private bool _onScreen;
        internal bool OnScreen
        {
            get
            {
                return this._onScreen;
            }
            set
            {
                this._onScreen = value;
                if (this._boundingBox != null)
                {
                    this._boundingBox.OnScreen = value;
                }
            }
        }
        protected private List<EntityVertex> _vertices = new List<EntityVertex>();
        protected private List<EntityPolygon> _entityPolygons = new List<EntityPolygon>();
        protected private List<Polygon> _clippedPolygons = new List<Polygon>();

        static object _nextIdLock = new object();
        static int _nextId = 1;
        internal int EntityId { get; private set; }

        internal int VertexCount { get
            {
                return _vertices.Count;
            }
        }

        private EntityVertex _summedVertices;

        internal EntityVertex BoundingSphereCentre { get; private set; }
        internal double BoundingSphereRadius { get; private set; }

        protected private BoundingBox? _boundingBox;

        internal Camera? AttachedCamera { get; private set; }

        internal Angle CameraYRotationOffset { get; private set; }
        internal Angle CameraXRotationOffset { get; private set; }
        internal PointFloat3d? CameraOffset { get; private set; }
        internal bool Animates { get; private set; }
        internal int ShieldStrength { get; private set; }

        internal List<int> CollisionSpaceOffsets { get; set; }

        internal GameEngine ControllingEngine { get; set; }

        internal string EntityName { get; set; }


        //internal BaseEntity(Point3d origin, double modelYRotation, double worldYAxisRotation, bool animates)
        //{
        //    EntityId = GetNextEntityId();
        //    this._boundingBox = new BoundingBox();
        //    OriginInWorldSpace = origin;
        //    ModelYAxisRotation = modelYRotation;
        //    WorldYAxisRotation = worldYAxisRotation;
        //    _summedVertices = new EntityVertex(0, 0, 0);
        //    BoundingSphereCentre = new EntityVertex(0, 0, 0);
        //    Animates = animates;
        //}

        internal BaseEntity(string entityName, int shieldStrength, bool createBoundingBox, bool animates)
        {
            EntityId = GetNextEntityId();
            EntityName = entityName;
            if (createBoundingBox)
            {
                this._boundingBox = new BoundingBox(entityName);
            }
            else
            {
                this._boundingBox = null;
            }
            OriginInWorldSpace = new PointFloat3d(0, 0, 0); ;
            ModelYAxisRotation = 0;
            WorldYAxisRotation = 0;
            _summedVertices = new EntityVertex(0, 0, 0);
            BoundingSphereCentre = new EntityVertex(0, 0, 0);
            Animates = animates;
            ShieldStrength = shieldStrength;
        }


        internal BaseEntity(string entityName, bool createBoundingBox)
        {
            EntityId = GetNextEntityId();
            EntityName = entityName;
            if (createBoundingBox)
            {
                this._boundingBox = new BoundingBox(entityName);
            }
            else
            {
                this._boundingBox = null;
            }
            OriginInWorldSpace = new PointFloat3d(0, 0, 0); ;
            ModelYAxisRotation = 0;
            WorldYAxisRotation = 0;
            _summedVertices = new EntityVertex(0, 0, 0);
            BoundingSphereCentre = new EntityVertex(0, 0, 0);
        }

        static int GetNextEntityId()
        {
            int idToReturn;
            lock(_nextIdLock)
            {
                idToReturn = _nextId++;
            }
            return idToReturn;
        }

        virtual internal void TransformBoundingBoxToWorld()
        {
            if (this._boundingBox != null)
            {
                this._boundingBox.Transform();
            }
        }

        virtual internal void TransformBoundingBoxToCameraView(Camera camera)
        {
            if (this._boundingBox!=null)
            {
                this._boundingBox.Transform();
                this._boundingBox.CameraTransformWithFrustrumCull(camera, false);
            }
        }

        // If this were a real game, this would be implemented with a transformation matrix
        virtual internal void Transform()
        {
            _clippedPolygons.Clear();
            OnScreen = true;
            for (int vertexIndex = 0; vertexIndex < _vertices.Count; vertexIndex++)
            {
                EntityVertex vertex = _vertices[vertexIndex];
                vertex.ResetTransformation();
                vertex.ModelTransform(OriginInModelSpace, ModelXAxisRotation, ModelYAxisRotation);
                vertex.WorldTransform(WorldYAxisRotation, OriginInWorldSpace);
            }
            TransformBoundingSphere(OriginInWorldSpace);
        }

        internal void TransformBoundingSphere(PointFloat3d worldOrigin)
        {
            BoundingSphereCentre.ModelTransform(OriginInModelSpace, ModelXAxisRotation, ModelYAxisRotation);
            BoundingSphereCentre.WorldTransform(WorldYAxisRotation, worldOrigin);
        }

        virtual internal void CameraTransformWithFrustrumCull(Camera camera)
        {
            CameraTransformWithFrustrumCull(camera, true);
        }

        virtual protected private void CameraTransformWithFrustrumCull(Camera camera, bool cullToFrustrum)
        {
            BoundingSphereCentre.CameraTransform(camera.RotationX, camera.RotationY, camera.Location);

            Point3d sphereCentre = new Point3d(BoundingSphereCentre.CameraTransformedX, BoundingSphereCentre.CameraTransformedY, BoundingSphereCentre.CameraTransformedZ);
            if (cullToFrustrum &&
                camera.ViewFrustrum.SphereInsideFrustrum(sphereCentre,BoundingSphereRadius)== false)
            {
                OnScreen = false;
                return;
            }

            int verticesPastClippingPlaneCount = 0;
            for (int vertexIndex = 0; vertexIndex < _vertices.Count; vertexIndex++)
            {
                EntityVertex vertex = _vertices[vertexIndex];
                vertex.CameraTransform(camera.RotationX, camera.RotationY, camera.Location);

                if (vertex.CameraTransformedZ > camera.ClippingPlaneZ)
                {
                    verticesPastClippingPlaneCount++;
                }
            }
            if (verticesPastClippingPlaneCount==0)
            {
                OnScreen = false;
            }
        }

        virtual internal void ClipToViewport(int clippingPlaneZ, bool cullBackfaces)
        {
            if (OnScreen)
            {
                for (int entityPolygonIndex = 0; entityPolygonIndex < _entityPolygons.Count; entityPolygonIndex++)
                {
                    EntityPolygon entityPolygon = _entityPolygons[entityPolygonIndex];

                    if (cullBackfaces)
                    {
                        PointFloat3d normal = CalculateNormal(entityPolygon);

                        EntityVertex v1 = _vertices[entityPolygon.VertexIndex(0)];
                        double dotProduct = PointFloat3d.DotProduct(v1.PointInView, normal);

                        if (dotProduct >= 0)
                        {
                            continue;
                        }
                    }

                    Polygon? p = ClipPolygonToViewport(entityPolygon, clippingPlaneZ);
                    if (p != null)
                    {
                        p.EntityTriangle = entityPolygonIndex;
                        p.FromEntity = EntityId;
                        _clippedPolygons.Add(p);
                    }
                }
                OnScreen = _clippedPolygons.Count > 0;
            }
        }

        virtual internal void ClipBoundingBoxToViewPort(int clippingPlaneZ)
        {
            if (OnScreen && _boundingBox!=null)
            {
                _boundingBox.ClipToViewport(clippingPlaneZ, false);
            }
        }

        PointFloat3d CalculateNormal(EntityPolygon poly)
        {
            EntityVertex v1 = _vertices[poly.VertexIndex(0)];
            EntityVertex v2 = _vertices[poly.VertexIndex(1)];
            EntityVertex v3 = _vertices[poly.VertexIndex(2)];

            PointFloat3d v1ToV3 = v2.PointInView - v1.PointInView;
            PointFloat3d v2ToV3 = v3.PointInView - v1.PointInView;

            PointFloat3d normal = PointFloat3d.CrossProduct(v1ToV3, v2ToV3);
            normal.Normalise();

            return normal;
        }

        private Polygon? ClipPolygonToViewport(EntityPolygon poly, int clippingPaneZ)
        {
            int vertexCount = poly.VertexCount;

            Polygon? toReturn = null;
            int behindClippingPaneCount = 0;
            for (int vertexIndex = 0; vertexIndex < vertexCount; ++vertexIndex)
            {
                EntityVertex vertex = _vertices[poly.VertexIndex(vertexIndex)];
                if (vertex.CameraTransformedZ <= clippingPaneZ)
                {
                    ++behindClippingPaneCount;
                }
            }
            if (behindClippingPaneCount == vertexCount)
            {
                return null;
            }
            else if (behindClippingPaneCount == 0)
            {
                toReturn = new Polygon();
                for (int vertexIndex = 0; vertexIndex < vertexCount; vertexIndex++)
                {
                    EntityVertex vertex = _vertices[poly.VertexIndex(vertexIndex)];
                    toReturn.AddVertex(vertex);
                }
                return toReturn;
            }

            toReturn = new Polygon();
            bool[] behindClipPlaneFlags = new bool[vertexCount];
            for (int polyVertexIndex = 0; polyVertexIndex < vertexCount; polyVertexIndex++)
            {
                bool lastVertex = polyVertexIndex == vertexCount - 1;
                int vertexIndex = poly.VertexIndex(polyVertexIndex);
                EntityVertex vertex = _vertices[vertexIndex];
                if (vertex.CameraTransformedZ > clippingPaneZ)
                {
                    behindClipPlaneFlags[polyVertexIndex] = false;

                    if (polyVertexIndex > 0 && behindClipPlaneFlags[polyVertexIndex - 1])
                    {
                        // line between this vertex and previous, crosses the clip plane boundary
                        //  add a new vertex
                        toReturn.AddVertex(IntersectLineWithClippingPlane(_vertices[poly.VertexIndex(polyVertexIndex - 1)], vertex, clippingPaneZ));
                    }

                    toReturn.AddVertex(vertex);
                    if (lastVertex && behindClipPlaneFlags[0] == true)
                    {
                        // Line between this vertex and first vertex, crosses the clip plane boundary
                        toReturn.AddVertex(IntersectLineWithClippingPlane(_vertices[poly.VertexIndex(0)], vertex, clippingPaneZ));
                    }
                }
                else
                {
                    behindClipPlaneFlags[polyVertexIndex] = true;
                    if (polyVertexIndex > 0 && behindClipPlaneFlags[polyVertexIndex - 1] == false)
                    {
                        // line between this vertex and previous, crosses the clip plane boundary
                        //  add a new vertex
                        toReturn.AddVertex(IntersectLineWithClippingPlane(vertex, _vertices[poly.VertexIndex(polyVertexIndex - 1)], clippingPaneZ));
                    }
                    if (lastVertex && behindClipPlaneFlags[0] == false)
                    {
                        // line from this last vertex, to first vertex, crossed boundary
                        toReturn.AddVertex(IntersectLineWithClippingPlane(vertex, _vertices[poly.VertexIndex(0)], clippingPaneZ));
                    }
                }
            }

            return toReturn;
        }

        private EntityVertex IntersectLineWithClippingPlane(EntityVertex v1, EntityVertex v2, int clippingPlaneZ)
        {
            double v1ToClipZ = clippingPlaneZ - v1.CameraTransformedZ;
            double ratioOfV1ToV2Z = v1ToClipZ / (v2.CameraTransformedZ - v1.CameraTransformedZ);
            double newX = (v2.CameraTransformedX - v1.CameraTransformedX) * ratioOfV1ToV2Z + v1.CameraTransformedX;
            double newY = (v2.CameraTransformedY - v1.CameraTransformedY) * ratioOfV1ToV2Z + v1.CameraTransformedY;
            EntityVertex toReturn = new EntityVertex();
            toReturn.CameraTransformedX = newX;
            toReturn.CameraTransformedY = newY;
            toReturn.CameraTransformedZ = clippingPlaneZ;

            toReturn.WorldTransformedX = (v2.WorldTransformedX - v1.WorldTransformedX) * ratioOfV1ToV2Z + v1.WorldTransformedX;
            toReturn.WorldTransformedY = (v2.WorldTransformedY - v1.WorldTransformedY) * ratioOfV1ToV2Z + v1.WorldTransformedY;
            toReturn.WorldTransformedZ = (v2.WorldTransformedZ - v1.WorldTransformedZ) * ratioOfV1ToV2Z + v1.WorldTransformedZ;

            toReturn.ModelTransformedX = (v2.ModelTransformedX - v1.ModelTransformedX) * ratioOfV1ToV2Z + v1.ModelTransformedX;
            toReturn.ModelTransformedY = (v2.ModelTransformedY - v1.ModelTransformedY) * ratioOfV1ToV2Z + v1.ModelTransformedY;
            toReturn.ModelTransformedZ = (v2.ModelTransformedZ - v1.ModelTransformedZ) * ratioOfV1ToV2Z + v1.ModelTransformedZ;

            toReturn.X = (int)((v2.X - v1.X) * ratioOfV1ToV2Z) + v1.X;
            toReturn.Y = (int)((v2.Y - v1.Y) * ratioOfV1ToV2Z) + v1.Y;
            toReturn.Z = (int)((v2.Z - v1.Z) * ratioOfV1ToV2Z) + v1.Z;

            return toReturn;
        }

        virtual internal void ProjectToViewport(Point viewportOrigin, double cameraProjectionMultiplier, Size viewportSize)
        {
            if (OnScreen)
            {
                for (int polygonIndex = 0; polygonIndex < _clippedPolygons.Count; ++polygonIndex)
                {
                    Polygon poly = _clippedPolygons[polygonIndex];
                    for (int vertexIndex = 0; vertexIndex < poly.VertexCount(); ++vertexIndex)
                    {
                        EntityVertex vertex = poly.VertexAtIndex(vertexIndex);
                        vertex.ViewportTransform(viewportOrigin, cameraProjectionMultiplier);
                    }
                }
            }
        }

        virtual internal void ProjectBoundingBoxToViewPort(Point viewportOrigin, double cameraProjectionMultiplier, Size viewportSize)
        {
            if (OnScreen && _boundingBox != null)
            {
                _boundingBox.ProjectToViewport(viewportOrigin, cameraProjectionMultiplier, viewportSize);
            }
        }

        virtual internal void AddPolygonsToSortedList(SortedList<int, List<Polygon>> depthOrderedPolygons)
        {
            AddPolygonsToSortedList(depthOrderedPolygons, false);
        }

        protected private void AddPolygonsToSortedList(SortedList<int, List<Polygon>> depthOrderedPolygons, bool boundingPolygon)
        {
            if (OnScreen)
            {
                for (int polygonIndex = 0; polygonIndex < _clippedPolygons.Count; ++polygonIndex)
                {
                    Polygon poly = _clippedPolygons[polygonIndex];
                    poly.CalcAverageSquaredDistanceToOrigin();
                    int sqDist = -poly.SquaredDistanceToOrigin;

                    List<Polygon>? polyList;
                    if (!depthOrderedPolygons.TryGetValue(sqDist, out polyList))
                    {
                        polyList = new List<Polygon>();
                        depthOrderedPolygons[sqDist] = polyList;
                    }
                    poly.LineColour = LineColour;
                    poly.BoundingPolygon = boundingPolygon;
                    polyList.Add(poly);
                }
            }
        }

        virtual internal void AddBoundingBoxPolygonsToSortedList(SortedList<int, List<Polygon>> depthOrderedPolygons)
        {
            if (OnScreen && _boundingBox!=null)
            {
                _boundingBox.AddPolygonsToSortedList(depthOrderedPolygons);
            }
        }

        internal int AddVertex(EntityVertex vertex)
        {
            _vertices.Add(vertex);
            _summedVertices = vertex + _summedVertices;
            if (this._boundingBox != null)
            {
                this._boundingBox.AddUnderlyingModelVertex(vertex);
            }

            return _vertices.Count - 1;
        }

        internal int AddVertex(int x, int y, int z)
        {
            EntityVertex vertex = new EntityVertex(x, y, z);
            _vertices.Add(vertex);
            _summedVertices = vertex + _summedVertices;

            if (this._boundingBox != null)
            {
                this._boundingBox.AddUnderlyingModelVertex(vertex);
            }
            return _vertices.Count - 1;
        }

        internal int AddVertex(double x, double y, double z)
        {
            EntityVertex vertex = new EntityVertex(x, y, z);
            _vertices.Add(vertex);
            _summedVertices = vertex + _summedVertices;

            if (this._boundingBox != null)
            {
                this._boundingBox.AddUnderlyingModelVertex(vertex);
            }

            return _vertices.Count - 1;
        }

        internal int AddTriangle(int v1, int v2, int v3)
        {
            EntityPolygon t = new EntityPolygon(v1, v2, v3);
            _entityPolygons.Add(t);
            return _entityPolygons.Count - 1;
        }

        internal int AddQuadrangle(int v1, int v2, int v3, int v4)
        {
            EntityPolygon t = new EntityPolygon(v1, v2, v3);
            t.AddVertexIndex(v4);
            _entityPolygons.Add(t);
            return _entityPolygons.Count - 1;
        }

        internal EntityVertex GetVertex(int vertexIndex)
        {
            return this._vertices[vertexIndex];
        }

        protected private void CalculateBoundingSphere()
        {
            BoundingSphereCentre = GetCentre();
            BoundingSphereRadius = GetBoundingRadius(BoundingSphereCentre);
        }

        virtual protected void CreateBoundingBox()
        {
            this._boundingBox!.CreateBoundingBoxFromExtemes();
        }

        virtual internal BoundingBox? GetAxisAlignedBoundingBox()
        {
            if (this._boundingBox == null)
            {
                return null;
            }
            return this._boundingBox.GetAxisAlignedBoundingBox();
        }

        virtual internal EntityVertex GetCentre()
        {
            return _summedVertices / _vertices.Count;
        }

        protected internal void SumVertices(ref EntityVertex summedVertex, ref int summedVertexCount)
        {
            summedVertex += _summedVertices;
            summedVertexCount += _vertices.Count;
        }

        virtual internal double GetBoundingRadius(EntityVertex fromVertex)
        {
            double maxSquaredDistance = 0;
            foreach (EntityVertex v in _vertices)
            {
                double squaredDistance = v.SquaredDistanceTo(fromVertex);
                if (squaredDistance > maxSquaredDistance)
                {
                    maxSquaredDistance = squaredDistance;
                }
            }

            return Math.Sqrt(maxSquaredDistance);
        }

        virtual internal bool IntersectsAtFloorLevel(BaseEntity otherEntity)
        {
            return false;
        }

        internal void AttachCamera(Camera camera, PointFloat3d cameraOffset, Angle cameraXRotationOffset, Angle cameraYRotationOffset)
        {
            AttachedCamera = camera;
            CameraOffset = cameraOffset;
            CameraXRotationOffset = cameraXRotationOffset;
            CameraYRotationOffset = cameraYRotationOffset;

            AttachedCamera.Location = this._originInWorldSpace + CameraOffset;
            Angle newCameraYRotation = CameraYRotationOffset + _worldYAxisRotation;
            AttachedCamera.RotationY = newCameraYRotation;
        }

        internal void AddCollisionOffset(int offset)
        {
            if (CollisionSpaceOffsets==null)
            {
                CollisionSpaceOffsets = new List<int>();
            }
            CollisionSpaceOffsets.Add(offset);
        }

        internal void AttachedCameraRotateAroundYAxis(double rotateBy)
        {
            CameraYRotationOffset += rotateBy;
            Angle newCameraYRotation = this._worldYAxisRotation + CameraYRotationOffset;

            if (AttachedCamera != null)
            {
                AttachedCamera.RotationY = newCameraYRotation;
                AttachedCamera.UpdateLocationFromAttachment(_originInWorldSpace, CameraOffset, new Angle(WorldYAxisRotation));
            }
        }

        virtual internal bool Animate(double gravityY)
        {
            return true;
        }
    }
}