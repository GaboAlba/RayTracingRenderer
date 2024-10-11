namespace RayTracingRenderer.Shapes
{
    using RayTracingRenderer.Rays;
    public interface IHittableObject
    {
        public bool HitObject(Ray ray, float rayTMin, float rayTMax, HitRecord record);
    }
}
