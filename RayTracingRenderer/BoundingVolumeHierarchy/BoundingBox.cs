using RayTracingRenderer.Rays;
using System.Numerics;

namespace RayTracingRenderer.BoundingVolumeHierarchy
{
    public class BoundingBox
    {
        public Vector3 Min { get; private set; }
        public Vector3 Max { get; private set; }

        public BoundingBox(Vector3 min, Vector3 max)
        {
            this.Min = min;
            this.Max = max;
        }

        // Calculate if the ray intersects this bounding box
        public bool Hit(Ray ray, float tMin, float tMax)
        {
            for (int i = 0; i < 3; i++)
            {
                float invD = 1.0f / ray.Direction[i];
                float t0 = (Min[i] - ray.Origin[i]) * invD;
                float t1 = (Max[i] - ray.Origin[i]) * invD;

                if (invD < 0.0f) (t0, t1) = (t1, t0);

                tMin = Math.Max(t0, tMin);
                tMax = Math.Min(t1, tMax);
                if (tMax <= tMin) return false;
            }
            return true;
        }
    }
}
