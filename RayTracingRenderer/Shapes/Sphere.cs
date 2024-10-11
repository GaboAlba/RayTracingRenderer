namespace RayTracingRenderer.Shapes
{
    using RayTracingRenderer.Rays;
    using System.Numerics;

    // TODO: Look into making this a struct
    public class Sphere : IHittableObject
    {
        public Vector3 Center { get; init; }

        public float Radius { get; init; }

        public Sphere(Vector3 center, float radius)
        {
            Center = center;
            Radius = Math.Max(0, radius);
        }

        public bool HitObject(Ray ray, float rayTMin, float rayTMax, HitRecord record)
        {
            if (!this.IsSphereHit(ray, out var discriminant, out var a, out var h))
            {
                return false;
            }

            // TODO: Refactor this if statement
            var root = (h - (float)Math.Sqrt(discriminant)) / a;
            if (root <= rayTMin || root >= rayTMax)
            {
                root = (h + (float)Math.Sqrt(discriminant)) / a;
                if (root <= rayTMin || root >= rayTMax)
                {
                    return false;
                }
            }

            // Log the hit
            record.HitTime = root;
            record.HitPosition = ray.GetPosition(record.HitTime);
            record.HitNormal = (record.HitPosition - this.Center) / this.Radius;

            return true;
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
