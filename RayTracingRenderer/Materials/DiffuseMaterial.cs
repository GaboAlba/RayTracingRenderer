using RayTracingRenderer.Rays;
using RayTracingRenderer.Shared;
using System.Numerics;

namespace RayTracingRenderer.Materials
{
    public class DiffuseMaterial : IMaterial
    {
        public Vector3 Albedo { get; init; }

        public DiffuseMaterial(Vector3 albedo)
        {
            this.Albedo = albedo;
        }

        public bool Scatter(Ray incomingRay, HitRecord hitRecord, out Vector3 attenuation, out Ray scatteredRay)
        {
            var randomUnitVector = RayTracingHelper.RandomUnitVector();

            // In the case of the random vctor being exactly equal to the normal we need to return the normal
            var scatterDirection = randomUnitVector != -1 * hitRecord.HitNormal ? hitRecord.HitNormal + randomUnitVector : hitRecord.HitNormal;
            scatteredRay = new Ray(hitRecord.HitPosition, scatterDirection);
            attenuation = this.Albedo;
            return true;
        }
    }
}
