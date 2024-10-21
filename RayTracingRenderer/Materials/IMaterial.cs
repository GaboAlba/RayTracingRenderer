using RayTracingRenderer.Rays;
using System.Numerics;

namespace RayTracingRenderer.Materials
{
    /// <summary>
    /// Defined an specific material
    /// </summary>
    public interface IMaterial
    {
        /// <summary>
        /// Defines the behaviour of a ray based on the attenuation of the material
        /// </summary>
        /// <param name="incomingRay"></param>
        /// <param name="hitRecord"></param>
        /// <param name="attenuation"></param>
        /// <param name="scatteredRay"></param>
        /// <returns></returns>
        public bool Scatter(Ray incomingRay, HitRecord hitRecord, out Vector3 attenuation, out Ray scatteredRay);
    }
}
