namespace RayTracingRenderer.Shapes
{
    using RayTracingRenderer;
    using RayTracingRenderer.BoundingVolumeHierarchy;
    using RayTracingRenderer.Materials;
    using RayTracingRenderer.Rays;
    using RayTracingRenderer.Shapes.Hittable;
    using RayTracingRenderer.Utils;
    using System.Numerics;

    public class Sphere : IHittableObject
    {
        public Vector3 Center { get; init; }

        public float Radius { get; init; }

        public IMaterial Material { get; set; }

        public float RadiusSquared { get; init; }

        public Sphere(Vector3 center, float radius, IMaterial material)
        {
            this.Center = center;
            this.Radius = Math.Max(0, radius);
            this.Material = material;
            this.RadiusSquared = this.Radius * this.Radius;
        }

        public bool HitObject(Ray ray, Interval interval, ref HitRecord record)
        {
            if (!this.IsSphereHit(ray, out var discriminant, out var a, out var h))
            {
                return false;
            }

            var root = this.GetNearestRoot(discriminant, h, a, interval);
            if (root == null)
            {
                return false;
            }

            // Log the hit
            record.HitTime = (float)root;
            record.HitPosition = ray.GetPosition(record.HitTime);
            record.HitNormal = (record.HitPosition - this.Center) / this.Radius;
            record.SetFaceNormal(ray, record.HitNormal);
            record.Material = this.Material;

            return true;
        }

        private float? GetNearestRoot(float discriminant, float h, float a, Interval interval)
        {
            var sqrtDiscriminant = (float)Math.Sqrt(discriminant);
            var root = (h - sqrtDiscriminant) / a;
            if (!interval.Surrounds(root))
            {
                root = (h + sqrtDiscriminant) / a;
                if (!interval.Surrounds(root))
                {
                    return null;
                }
            }

            return root;
        }

        private static float DotProductManual(Vector3 v1, Vector3 v2)
        {
            return v1.X * v2.X + v1.Y * v2.Y + v1.Z * v2.Z;
        }

        private float GetDiscriminant(Ray ray, out float a, out float h)
        {
            var centerToRayVector = this.Center - ray.Origin;

            // Quadratic formula
            a = ray.DirectionLengthSquared;
            h = DotProductManual(ray.Direction, centerToRayVector);
            var c = centerToRayVector.LengthSquared() - this.RadiusSquared;
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

            discriminant = this.GetDiscriminant(ray, out a, out h);

            return discriminant >= 0;
        }
    }
}
