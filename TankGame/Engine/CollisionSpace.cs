using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Http.Headers;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using TankGame.Maths;

namespace TankGame.Engine
{
    internal class CollisionSpace
    {
        int _width;
        int _depth;
        PointFloat3d _centre;
        int _granularity;

        Dictionary<int, Dictionary<int, BaseEntity>> _collisionSpace;
        public CollisionSpace(int width, int depth, int centreX, int centreZ, int granularity) 
        { 
            _width = width/granularity;
            _depth = depth/granularity;
            _granularity = granularity;
            _centre = new PointFloat3d(centreX, 0, centreZ);

            _collisionSpace = new Dictionary<int, Dictionary<int, BaseEntity>>(_width*_depth);
        }

        internal void AddEntity(BaseEntity entity)
        {
            Debug.WriteLine($"Adding entity {entity.EntityName} to collision space");
            PointFloat3d centrePt = entity.BoundingSphereCentre.PointInWorld + _centre;
            double sphereRadius = entity.BoundingSphereRadius;
            Debug.WriteLine($"  centre: {centrePt}, radius: {sphereRadius: 0.#}");
            AddEntityAtCollisionPoint(entity, entity.BoundingSphereCentre.PointInWorld);
            AddEntityAtCollisionPoint(entity, entity.BoundingSphereCentre.PointInWorld + new PointFloat3d(-sphereRadius, 0, 0));
            AddEntityAtCollisionPoint(entity, entity.BoundingSphereCentre.PointInWorld + new PointFloat3d( sphereRadius, 0, 0));
            AddEntityAtCollisionPoint(entity, entity.BoundingSphereCentre.PointInWorld + new PointFloat3d(0, 0, sphereRadius));
            AddEntityAtCollisionPoint(entity, entity.BoundingSphereCentre.PointInWorld + new PointFloat3d(0, 0, -sphereRadius));

            sphereRadius = sphereRadius * Math.Cos(Math.PI / 4);
            AddEntityAtCollisionPoint(entity, entity.BoundingSphereCentre.PointInWorld + new PointFloat3d(-sphereRadius, 0, -sphereRadius));
            AddEntityAtCollisionPoint(entity, entity.BoundingSphereCentre.PointInWorld + new PointFloat3d(-sphereRadius, 0, sphereRadius));
            AddEntityAtCollisionPoint(entity, entity.BoundingSphereCentre.PointInWorld + new PointFloat3d( sphereRadius, 0, sphereRadius));
            AddEntityAtCollisionPoint(entity, entity.BoundingSphereCentre.PointInWorld + new PointFloat3d(-sphereRadius, 0, -sphereRadius));
        }

        internal void RemoveEntity(BaseEntity entity, bool useCoordsToRemove)
        {
            if (useCoordsToRemove)
            {
                PointFloat3d centrePt = entity.BoundingSphereCentre.PointInWorld + _centre;
                double sphereRadius = entity.BoundingSphereRadius;
                RemoveEntityFromCollisionPoint(entity, entity.BoundingSphereCentre.PointInWorld);
                RemoveEntityFromCollisionPoint(entity, entity.BoundingSphereCentre.PointInWorld + new PointFloat3d(-sphereRadius, 0, 0));
                RemoveEntityFromCollisionPoint(entity, entity.BoundingSphereCentre.PointInWorld + new PointFloat3d(sphereRadius, 0, 0));
                RemoveEntityFromCollisionPoint(entity, entity.BoundingSphereCentre.PointInWorld + new PointFloat3d(0, 0, sphereRadius));
                RemoveEntityFromCollisionPoint(entity, entity.BoundingSphereCentre.PointInWorld + new PointFloat3d(0, 0, sphereRadius));

                sphereRadius = sphereRadius * Math.Cos(Math.PI / 4);
                RemoveEntityFromCollisionPoint(entity, entity.BoundingSphereCentre.PointInWorld + new PointFloat3d(-sphereRadius, 0, -sphereRadius));
                RemoveEntityFromCollisionPoint(entity, entity.BoundingSphereCentre.PointInWorld + new PointFloat3d(-sphereRadius, 0, sphereRadius));
                RemoveEntityFromCollisionPoint(entity, entity.BoundingSphereCentre.PointInWorld + new PointFloat3d(sphereRadius, 0, sphereRadius));
                RemoveEntityFromCollisionPoint(entity, entity.BoundingSphereCentre.PointInWorld + new PointFloat3d(-sphereRadius, 0, -sphereRadius));
            }
            else
            {
                for(int coIndex = 0;coIndex<entity.CollisionSpaceOffsets.Count;++coIndex)
                {
                    RemoveEntityFromCollisionPoint(entity, entity.CollisionSpaceOffsets[coIndex]);
                }
            }
        }

        void AddEntityAtCollisionPoint(BaseEntity entity, PointFloat3d pt)
        {
            int offset = CollisionSpaceOffsetFromPoint(pt);
            if (offset != -1)
            {
//                Debug.WriteLine($"Adding entity {entity.EntityName} at {pt} to collision pt {offset} ({pt.X/_granularity:0.#},{pt.Z/_granularity:0.#})");
                Dictionary<int, BaseEntity>? entitiesAtCollisionArea;
                if (_collisionSpace.TryGetValue(offset,out entitiesAtCollisionArea)==false)
                {
                    entitiesAtCollisionArea = new Dictionary<int, BaseEntity>();
                    _collisionSpace[offset] = entitiesAtCollisionArea;
                }
                else
                {
                    entitiesAtCollisionArea= _collisionSpace[offset];
                }
                if (entitiesAtCollisionArea.ContainsKey(entity.EntityId))
                {
                    offset = -1;
                }
                else
                {
                    entitiesAtCollisionArea[entity.EntityId] = entity;
                }
            }
            if (offset!=-1)
            {
                entity.AddCollisionOffset(offset);
            }
        }

        void RemoveEntityFromCollisionPoint(BaseEntity entity, PointFloat3d pt)
        {
            int offset = CollisionSpaceOffsetFromPoint(pt);
            if (offset != -1)
            {
                Dictionary<int, BaseEntity>? entitiesAtCollisionArea;
                if (_collisionSpace.TryGetValue(offset, out entitiesAtCollisionArea))
                {
                    entitiesAtCollisionArea = _collisionSpace[offset];
                    entitiesAtCollisionArea.Remove(entity.EntityId);
                }
            }
        }

        void RemoveEntityFromCollisionPoint(BaseEntity entity, int collisionOffset)
        {
            if (collisionOffset  != -1)
            {
                Dictionary<int, BaseEntity>? entitiesAtCollisionArea;
                if (_collisionSpace.TryGetValue(collisionOffset, out entitiesAtCollisionArea))
                {
                    entitiesAtCollisionArea = _collisionSpace[collisionOffset];
                    if (entity.EntityId==1)
                    {
                        Debug.WriteLine("Removing tower from collision space");
                    }
                    entitiesAtCollisionArea.Remove(entity.EntityId);
                }
            }
        }

        int CollisionSpaceOffsetFromPoint(PointFloat3d point)
        {
            point += _centre;
            int x = (int)(point.X / _granularity);
            int z = (int)(point.Z / _granularity);

            Debug.WriteLine($"  Collision offset coords: ({x}, {z}), offset: {x+z*_width}");

            if (x<0 || x>=_width ||
                z<0 || z>=_depth)
            {
                return -1;
            }

            return x + z * _width;
        }

        internal List<BaseEntity> EntitiesWithinSphere(BaseEntity entity, PointFloat3d newWorldOriginForEntity)
        {
            int offset = CollisionSpaceOffsetFromPoint(newWorldOriginForEntity);
            Dictionary<int, BaseEntity>? entitiesAtCollisionArea;
            List<BaseEntity> toReturn = new List<BaseEntity>();
            //System.Diagnostics.Debug.WriteLine($" Tank offset {offset} Tank centre {newWorldOriginForEntity}");
            if (_collisionSpace.TryGetValue(offset, out entitiesAtCollisionArea))
            {
                foreach(BaseEntity be in entitiesAtCollisionArea.Values)
                {
                    if (be.EntityId != entity.EntityId)
                    {
                        double sqDist = be.BoundingSphereCentre.PointInWorld.SquareDistanceTo(entity.BoundingSphereCentre.PointInWorld);
                        double sqBoundingSpheresRadius = (be.BoundingSphereRadius + entity.BoundingSphereRadius)* (be.BoundingSphereRadius + entity.BoundingSphereRadius);
                        if (sqDist< sqBoundingSpheresRadius)
                        {
                            toReturn.Add(be);
                        }
                    }
                }
            }

            return toReturn;
        }
    }
}
