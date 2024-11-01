namespace RayTracingRenderer.Shapes.Hittable
{
    using RayTracingRenderer;
    using RayTracingRenderer.BoundingVolumeHierarchy;
    using RayTracingRenderer.Rays;
    using RayTracingRenderer.Utils;
    using System.Numerics;

    public interface IHittableObject
    {
        public Vector3 Center { get; init; }
        public bool HitObject(Ray ray, Interval interval, ref HitRecord record);
    }
}
