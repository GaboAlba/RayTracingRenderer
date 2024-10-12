
namespace RayTracingRenderer.Rays
{
    using RayTracingRenderer.Shapes;
    using RayTracingRenderer.Shapes.Hittable;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Numerics;
    using System.Text;
    using System.Threading.Tasks;

    public class Ray
    {
        public Vector3 Origin {  get; init; }

        public Vector3 Direction { get; init; }

        /// <summary>
        /// Default constructor
        /// </summary>
        public Ray() { }

        /// <summary>
        /// Creates a new instance of <cref=Ray>
        /// </summary>
        /// <param name="origin"></param>
        /// <param name="direction"></param>
        public Ray(Vector3 origin, Vector3 direction)
        {
            this.Origin = origin;
            this.Direction = direction;
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
        /// Gets the ray color. TODO: Change it to the actual color
        /// </summary>
        /// <param name="ray"></param>
        /// <returns></returns>
        public static Vector3 RayColor(Ray ray, IHittableObject world)
        {
            var record = new HitRecord();
            if (world.HitObject(ray, 0, float.PositiveInfinity, record, out var outRecord))
            {
                return 0.5f * (outRecord.HitNormal + new Vector3(1, 1, 1));
            }

            var unitDirection = Vector3.Normalize(ray.Direction);
            var a = 0.5f * (unitDirection.Y + 1.0f);
            return (1.0f - a) * new Vector3(1.0f, 1.0f, 1.0f) + a * new Vector3(0.5f, 0.7f, 1.0f);
        }
    }
}
