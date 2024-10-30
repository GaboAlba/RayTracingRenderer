namespace RayTracingRenderer.Shapes.Hittable
{
    using RayTracingRenderer;
    using RayTracingRenderer.BoundingVolumeHierarchy;
    using RayTracingRenderer.Rays;
    using RayTracingRenderer.Utils;

    public class HittableObjectsList : IHittableObject
    {
        public List<IHittableObject> objects;
        private BVHNode Root;

        public HittableObjectsList()
        {
            objects = new List<IHittableObject>();
            //this.Root = new BVHNode(this.objects, 0, this.objects.Count());
        }
        public HittableObjectsList(IHittableObject hittableObject)
        {
            this.objects = new List<IHittableObject>();
            this.objects.Add(hittableObject);
            //this.Root = new BVHNode(this.objects, 0, this.objects.Count());
        }

        public void ClearList()
        {
            this.objects.Clear();
        }

        public bool HitObject(Ray ray, Interval interval, ref HitRecord record)
        {
            var tempRecord = new HitRecord();
            //outputRecord = tempRecord;
            var hitAnything = false;
            var closestRoot = interval.MaxValue;

            foreach (var hittableObject in this.objects)
            {
                if (hittableObject.HitObject(
                    ray,
                    new Interval(interval.MinValue, closestRoot),
                    ref tempRecord))
                {
                    hitAnything = true;
                    closestRoot = tempRecord.HitTime;
                    record = tempRecord;
                    //outputRecord = record;
                }
            }

            return hitAnything;
        }
    }
}
