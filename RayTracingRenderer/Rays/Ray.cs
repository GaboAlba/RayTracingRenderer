
namespace RayTracingRenderer.Rays
{
    using RayTracingRenderer.Materials;
    using RayTracingRenderer.PPMImage;
    using RayTracingRenderer.Shapes.Hittable;
    using RayTracingRenderer.Shared;
    using RayTracingRenderer.Utils;
    using System.Numerics;

    public class Ray
    {
        public Vector3 Origin { get; init; }

        public Vector3 Direction { get; set; }

        public float DirectionLengthSquared { get; init; }

        public int MaxBounces { get; init; }

        public int CurrentBounces { get; set; }

        /// <summary>
        /// Default constructor
        /// </summary>
        public Ray() { }

        /// <summary>
        /// Creates a new instance of <cref=Ray>
        /// </summary>
        /// <param name="origin"></param>
        /// <param name="direction"></param>
        public Ray(Vector3 origin, Vector3 direction, int maxBounces = 10)
        {
            this.Origin = origin;
            this.Direction = direction;
            this.MaxBounces = maxBounces;
            this.DirectionLengthSquared = this.Direction.LengthSquared();
        }

        public Ray(Camera camera, int pixelXPosition, int pixelYposition, int maxBounces = 10)
        {
            var p = RayTracingHelper.RandomVectorInDefocusDisk();
            this.Origin = camera.DefocusAngle <= 0 ?
                camera.CameraCenter :
                camera.CameraCenter + (p[0] * camera.DefocusDiskU) + (p[1] * camera.DefocusDiskV);
            var offset = RayTracingHelper.SampleSquare();
            var pixelCenter = camera.Pixel100Location + ((pixelXPosition + offset.X) * camera.PixelDeltaU) + ((pixelYposition + offset.Y) * camera.PixelDeltaV);
            var rayDirection = pixelCenter - this.Origin;
            this.Direction = rayDirection;
            this.MaxBounces = maxBounces;
            this.DirectionLengthSquared = this.Direction.LengthSquared();
        }

        /// <summary>
        /// Get the origin position for an specific ray
        /// </summary>
        /// <returns></returns>
        public Vector3 GetOrigin() => this.Origin;

        /// <summary>
        /// Get the direction of an specific ray
        /// </summary>
        /// <returns></returns>
        public Vector3 GetDirection() => this.Direction;

        /// <summary>
        /// Get the position of a ray precisely after a certain amount of time
        /// </summary>
        /// <param name="timePassed"></param>
        /// <returns></returns>
        public Vector3 GetPosition(float timePassed) => this.Origin + timePassed * this.Direction;

        /// <summary>
        /// Gets the ray color.
        /// </summary>
        /// <param name="ray"></param>
        /// <returns></returns>
        public Vector3 RayColor(Ray ray, HittableObjectsList world, int depth)
        {
            // Stop if max amount of bounces has been achieved
            if (depth <= 0)
            {
                return new Vector3(0, 0, 0);
            }

            var record = new HitRecord();
            if (world.HitObject(
                ray,
                new Interval(0.001f, float.PositiveInfinity), // Adding 0.0001 to prevent floating point anomalies
                ref record))
            {
                record.Material = record.Material ?? new DiffuseMaterial(new Vector3(1, 1, 1)); // If material is not set, default to a diffuse material
                if (record.Material.Scatter(ray, record, out var attenuation, out var scatteredRay))
                {
                    return attenuation * this.RayColor(scatteredRay, world, depth - 1);
                }
                return new Vector3(0, 0, 0);
            }

            var unitDirection = Vector3.Normalize(this.Direction);
            var a = 0.5f * (unitDirection.Y + 1.0f);
            return (1.0f - a) * new Vector3(1.0f, 1.0f, 1.0f) + a * new Vector3(0.5f, 0.7f, 1.0f);
        }
    }
}
