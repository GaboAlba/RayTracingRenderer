namespace RayTracingRenderer
{
    using RayTracingRenderer.Materials;
    using RayTracingRenderer.Rays;
    using System.Numerics;
    public record HitRecord
    {
        /// <summary>
        /// Gets or sets a vector indicating the hit coordinates in the world
        /// </summary>
        public Vector3 HitPosition { get; set; }

        /// <summary>
        /// Gets or sets the normal vector for a hit point on an object
        /// </summary>
        public Vector3 HitNormal { get; set; }

        /// <summary>
        /// Gets or sets the time at which a hit was recorded
        /// </summary>
        public float HitTime { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether a normal should go to the inside or outside of an object
        /// </summary>
        public bool FrontFace { get; set; }

        /// <summary>
        /// Gets or set the value for the material which was hit
        /// </summary>
        public IMaterial? Material { get; set; }

        /// <summary>
        /// Sets the Face Normal depending on the result of the front face calculation
        /// </summary>
        /// <param name="ray"></param>
        /// <param name="outwardNormal"></param>
        public void SetFaceNormal(Ray ray, Vector3 outwardNormal)
        {
            FrontFace = Vector3.Dot(ray.Direction, outwardNormal) < 0;
            HitNormal = FrontFace ? outwardNormal : -outwardNormal;
        }
    }
}
