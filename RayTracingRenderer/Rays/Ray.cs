
namespace RayTracingRenderer.Rays
{
    using RayTracingRenderer.Shapes;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Numerics;
    using System.Text;
    using System.Threading.Tasks;

    public class Ray
    {
        private Vector3 Origin {  get; init; }

        private Vector3 Direction { get; init; }

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
        public static Vector3 RayColor(Ray ray)
        {
            var sphere = new Sphere(center: new Vector3(0, 0, -1), radius: 0.5f);
            if (IsSphereHit(sphere, ray))
            {
                return new Vector3(1, 0, 0); // Draw red if there is an intersection
            }

            var unitDirection = Vector3.Normalize(ray.Direction);
            var a = 0.5f * (unitDirection.Y + 1.0f);
            return (1.0f - a) * new Vector3(1.0f, 1.0f, 1.0f) + a * new Vector3(0.5f, 0.7f, 1.0f);
        }

        public static bool IsSphereHit(Sphere sphere, Ray ray)
        {
            if (sphere == null)
            {
                return false;
            }

            var centerToRayVector = sphere.Center - ray.Origin;

            // Quadratic formula
            var a = Vector3.Dot(ray.Direction, ray.Direction);
            var b = -2.0f * Vector3.Dot(ray.Direction, centerToRayVector);
            var c = Vector3.Dot(centerToRayVector, centerToRayVector) - sphere.Radius * sphere.Radius;
            var discriminant = (b * b) - (4 * a * c);
            return discriminant >= 0;
        }
    }
}
