namespace RayTracingRenderer.Shapes.Hittable
{
    using RayTracingRenderer.Rays;
    public class HittableObjectsList : IHittableObject
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

        public bool HitObject(Ray ray, float rayTMin, float rayTMax, HitRecord record, out HitRecord outputRecord)
        {
            var tempRecord = new HitRecord();
            outputRecord = tempRecord;
            var hitAnything = false;
            var closestRoot = rayTMax;

            foreach (var hittableObject in this.objects)
            {
                if (hittableObject.HitObject(ray, rayTMin, closestRoot, tempRecord, out outputRecord))
                {
                    hitAnything = true;
                    closestRoot = tempRecord.HitTime;
                    record = tempRecord;
                    outputRecord = record;
                }
            }

            return hitAnything;
        }
    }
}
