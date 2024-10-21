namespace RayTracingRenderer.Shapes.Hittable
{
    using RayTracingRenderer;
    using RayTracingRenderer.Rays;
    using RayTracingRenderer.Utils;

    public interface IHittableObject
    {
        public bool HitObject(Ray ray, Interval interval, HitRecord record, out HitRecord outputRecord);
    }
}
