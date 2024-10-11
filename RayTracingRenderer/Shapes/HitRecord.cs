namespace RayTracingRenderer.Shapes
{
    using System.Numerics;
    public record HitRecord
    {
        /// <summary>
        /// Gets or sets a vector indicating the hit coordinates in the world
        /// </summary>
        public Vector3 HitPosition {  get; set; }

        /// <summary>
        /// Gets or sets the normal vector for a hit point on an object
        /// </summary>
        public Vector3 HitNormal { get; set; }

        /// <summary>
        /// Gets or sets the time at which a hit was recorded
        /// </summary>
        public float HitTime { get; set; }
    }
}
