namespace RayTracingRenderer.Shapes
{
    using RayTracingRenderer.Rays;
    using RayTracingRenderer.Shapes.Hittable;
    using System.Numerics;

    public class Sphere : IHittableObject
    {
        public Vector3 Center { get; init; }

        public float Radius { get; init; }

        public Sphere(Vector3 center, float radius)
        {
            Center = center;
            Radius = Math.Max(0, radius);
        }

        public bool HitObject(Ray ray, float rayTMin, float rayTMax, HitRecord record, out HitRecord outputRecord)
        {
            if (!this.IsSphereHit(ray, out var discriminant, out var a, out var h))
            {
                outputRecord = record;
                return false;
            }

            var root = this.GetNearestRoot(discriminant, h, a, rayTMin, rayTMax);
            if (root == null)
            {
                outputRecord = record;
                return false;
            }

            // Log the hit
            record.HitTime = (float)root;
            record.HitPosition = ray.GetPosition(record.HitTime);
            record.HitNormal = (record.HitPosition - this.Center) / this.Radius;
            record.SetFaceNormal(ray, record.HitNormal);

            outputRecord = record;
            return true;
        }

        private float? GetNearestRoot(float discriminant, float h, float a, float rayTMin, float rayTMax)
        {
            var root = (h - (float)Math.Sqrt(discriminant)) / a;
            if (root <= rayTMin || root >= rayTMax)
            {
                root = (h + (float)Math.Sqrt(discriminant)) / a;
                if (root <= rayTMin || root >= rayTMax)
                {
                    return null;
                }
            }

            return root;
        }

        private float GetDiscriminant(Ray ray, out float a, out float h, out float c)
        {
            var centerToRayVector = this.Center - ray.Origin;

            // Quadratic formula
            a = ray.Direction.LengthSquared();
            h = Vector3.Dot(ray.Direction, centerToRayVector);
            c = centerToRayVector.LengthSquared() - this.Radius * this.Radius;
            var discriminant = (h * h) - (a * c);
            return discriminant;
        }

        private bool IsSphereHit(Ray ray, out float discriminant, out float a, out float h)
        {
            if (this == null)
            {
                discriminant = -1;
                a = -1;
                h = -1;
                return false;
            }

            discriminant = this.GetDiscriminant(ray, out a, out h, out _);

            return discriminant >= 0;
        }
    }
}
