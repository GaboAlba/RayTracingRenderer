using RayTracingRenderer.Rays;
using System.Numerics;

namespace RayTracingRenderer.Materials
{
    public class MetalMaterial : IMaterial
    {
        public Vector3 Albedo { get; init; }

        public MetalMaterial(Vector3 albedo)
        {
            this.Albedo = albedo;
        }

        public bool Scatter(Ray incomingRay, HitRecord hitRecord, out Vector3 attenuation, out Ray scatteredRay)
        {
            var reflected = Vector3.Reflect(incomingRay.Direction, hitRecord.HitNormal);
            scatteredRay = new Ray(hitRecord.HitPosition, reflected);
            attenuation = this.Albedo;
            return true;
        }
    }
}
