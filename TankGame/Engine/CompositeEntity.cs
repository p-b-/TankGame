using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TankGame.Maths;

namespace TankGame.Engine
{
    internal class CompositeEntity : BaseEntity
    {
        protected List<BaseEntity> _compositeEntities = new List<BaseEntity>();

        override internal PointFloat3d OriginInWorldSpace
        {
            get
            {
                return this._compositeEntities[0].OriginInWorldSpace;
            }
            set
            {
                for (int entityIndex = 0; entityIndex < _compositeEntities.Count; entityIndex++)
                {
                    BaseEntity entity = _compositeEntities[entityIndex];
                    entity.OriginInWorldSpace = value;
                }
                if (this._boundingBox != null)
                {
                    this._boundingBox.OriginInWorldSpace = value;
                }
                base.OriginInWorldSpace = value;
            }
        }

        override internal double WorldYAxisRotation
        {
            get
            {
                return this._compositeEntities[0].WorldYAxisRotation;
            }
            set
            {
                for (int entityIndex = 0; entityIndex < _compositeEntities.Count; entityIndex++)
                {
                    BaseEntity entity = _compositeEntities[entityIndex];
                    entity.WorldYAxisRotation = value;
                }
                if (this._boundingBox != null)
                {
                    this._boundingBox.WorldYAxisRotation = value;
                }
                base.WorldYAxisRotation = value;
            }
        }
        internal void SetColour(Color c)
        {
            for (int entityIndex = 0; entityIndex < _compositeEntities.Count; entityIndex++)
            {
                BaseEntity entity = _compositeEntities[entityIndex];
                entity.LineColour = c;
            }
        }

        internal CompositeEntity(string entityName, int shieldStrength, bool animates) : base(entityName, shieldStrength, true, animates)
        {

        }

        protected int AddCompositingEntity(BaseEntity entity, double xRotation, double yRotation, Point3d modelTranslation)
        {
            entity.ModelXAxisRotation = xRotation;
            entity.ModelYAxisRotation = yRotation;
            entity.OriginInModelSpace = modelTranslation;

            this._compositeEntities.Add(entity);
            return _compositeEntities.Count - 1;
        }

        protected BaseEntity GetCompositingEntity(int entityIndex)
        {
            return this._compositeEntities[entityIndex] as BaseEntity;
        }

        override internal void Transform()
        {
            for (int entityIndex = 0; entityIndex < _compositeEntities.Count; entityIndex++)
            {
                BaseEntity entity = _compositeEntities[entityIndex];
                entity.Transform();
            }
            TransformBoundingSphere(OriginInWorldSpace);
        }

        override internal void CameraTransformWithFrustrumCull(Camera camera)
        {
            bool atLeastOneOnScreen = false;
            for (int entityIndex = 0; entityIndex < _compositeEntities.Count; entityIndex++)
            {
                BaseEntity entity = _compositeEntities[entityIndex];
                entity.CameraTransformWithFrustrumCull(camera);
                if (entity.OnScreen)
                {
                    atLeastOneOnScreen = true;
                }
            }
            OnScreen = atLeastOneOnScreen;
        }

        override internal void ClipToViewport(int clippingPlaneZ, bool cullBackfaces)
        {
            if (OnScreen)
            {
                bool atLeastOneOnScreen = false;
                for (int entityIndex = 0; entityIndex < _compositeEntities.Count; entityIndex++)
                {
                    BaseEntity entity = _compositeEntities[entityIndex];
                    entity.ClipToViewport(clippingPlaneZ, cullBackfaces);
                    if (entity.OnScreen)
                    {
                        atLeastOneOnScreen = true;
                    }
                }
                if (atLeastOneOnScreen)
                {
                    OnScreen = true;
                }
            }
        }

        override internal void ProjectToViewport(Point viewportOrigin, double focalLength, Size viewpointSize)
        {
            for (int entityIndex = 0; entityIndex < _compositeEntities.Count; entityIndex++)
            {
                BaseEntity entity = _compositeEntities[entityIndex];
                entity.ProjectToViewport(viewportOrigin, focalLength, viewpointSize);
            }
        }

        override internal void AddPolygonsToSortedList(SortedList<int, List<Polygon>> depthOrderedPolygons)
        {
            for (int entityIndex = 0; entityIndex < _compositeEntities.Count; entityIndex++)
            {
                BaseEntity entity = _compositeEntities[entityIndex];
                entity.AddPolygonsToSortedList(depthOrderedPolygons);
            }
        }
        override internal EntityVertex GetCentre()
        {
            EntityVertex summedVertex = new EntityVertex();
            int vertexCount = 0;
            for (int entityIndex = 0; entityIndex < _compositeEntities.Count; entityIndex++)
            {
                BaseEntity entity = _compositeEntities[entityIndex];
                entity.SumVertices(ref summedVertex, ref vertexCount);
            }

            return summedVertex / vertexCount;
        }

        override internal double GetBoundingRadius(EntityVertex fromVertex)
        {
            double maxRadius = 0;
            for (int entityIndex = 0; entityIndex < _compositeEntities.Count; entityIndex++)
            {
                BaseEntity entity = _compositeEntities[entityIndex];
                double radius = entity.GetBoundingRadius(fromVertex);
                if (radius > maxRadius)
                {
                    maxRadius = radius;
                }
            }
            return maxRadius;
        }

        override protected void CreateBoundingBox()
        {
            for (int entityIndex = 0; entityIndex < _compositeEntities.Count; entityIndex++)
            {
                BaseEntity entity = _compositeEntities[entityIndex];
                for (int vertexIndex = 0; vertexIndex < entity.VertexCount; ++vertexIndex)
                {
                    EntityVertex vertex = entity.GetVertex(vertexIndex);
                    vertex.ModelTransform(entity.OriginInModelSpace,entity.ModelXAxisRotation,entity.ModelYAxisRotation);

                    _boundingBox.AddUnderlyingModelVertex(vertex.PointInModelSpace);
                }
            }
            _boundingBox.CreateBoundingBoxFromExtemes();
        }
    }
}
