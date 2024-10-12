namespace RayTracingRenderer.Shapes.Hittable
{
    using RayTracingRenderer.Rays;

    public interface IHittableObject
    {
        public bool HitObject(Ray ray, float rayTMin, float rayTMax, HitRecord record, out HitRecord outputRecord);
    }
}
