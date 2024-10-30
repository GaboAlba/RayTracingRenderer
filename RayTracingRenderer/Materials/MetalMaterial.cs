using RayTracingRenderer.Rays;
using RayTracingRenderer.Shared;
using System.Numerics;

namespace RayTracingRenderer.Materials
{
    public class MetalMaterial : IMaterial
    {
        public Vector3 Albedo { get; init; }
        public float Fuzz { get; init; }

        public MetalMaterial(Vector3 albedo, float fuzz)
        {
            this.Albedo = albedo;
            this.Fuzz = fuzz < 1 ? fuzz : 1;
        }

        public bool Scatter(Ray incomingRay, HitRecord hitRecord, out Vector3 attenuation, out Ray scatteredRay)
        {
            var reflected = Vector3.Reflect(incomingRay.Direction, hitRecord.HitNormal);
            reflected = Vector3.Normalize(reflected) + (this.Fuzz * RayTracingHelper.RandomUnitVector());
            scatteredRay = new Ray(hitRecord.HitPosition, reflected);
            attenuation = this.Albedo;
            return Vector3.Dot(scatteredRay.Direction, hitRecord.HitNormal) > 0;
        }
    }
}
