namespace RayTracingRenderer.Shapes.Hittable
{
    using RayTracingRenderer;
    using RayTracingRenderer.Rays;
    using RayTracingRenderer.Utils;

    public class HittableObjectsList
    {
        public List<IHittableObject> objects;

        public HittableObjectsList()
        {
            objects = new List<IHittableObject>();
        }
        public HittableObjectsList(IHittableObject hittableObject)
        {
            this.objects = new List<IHittableObject>();
            this.objects.Add(hittableObject);
        }

        public void ClearList()
        {
            this.objects.Clear();
        }

        public bool HitObject(Ray ray, Interval interval, ref HitRecord record)
        {
            var tempRecord = new HitRecord();
            var hitAnything = false;
            var closestRoot = interval.MaxValue;

            foreach (var hittableObject in this.objects)
            {
                if ((hittableObject.Center - ray.Origin).LengthSquared() > 0 && hittableObject.HitObject(
                    ray,
                    new Interval(interval.MinValue, closestRoot),
                    ref tempRecord))
                {
                    hitAnything = true;
                    closestRoot = tempRecord.HitTime;
                    record = tempRecord;
                }
            }

            return hitAnything;
        }
    }
}
