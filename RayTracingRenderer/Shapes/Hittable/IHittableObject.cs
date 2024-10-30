namespace RayTracingRenderer.Shapes.Hittable
{
    using RayTracingRenderer;
    using RayTracingRenderer.BoundingVolumeHierarchy;
    using RayTracingRenderer.Rays;
    using RayTracingRenderer.Utils;

    public interface IHittableObject
    {
        public bool HitObject(Ray ray, Interval interval, ref HitRecord record);
    }
}
