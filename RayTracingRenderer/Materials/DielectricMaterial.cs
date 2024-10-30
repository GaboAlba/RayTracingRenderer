using RayTracingRenderer.Rays;
using RayTracingRenderer.Shared;
using System.Numerics;

namespace RayTracingRenderer.Materials
{
    public class DielectricMaterial : IMaterial
    {
        private readonly float materialRefractionIndex;

        public DielectricMaterial(float refractionIndex)
        {
            this.materialRefractionIndex = refractionIndex;
        }

        public bool Scatter(Ray incomingRay, HitRecord hitRecord, out Vector3 attenuation, out Ray scatteredRay)
        {
            attenuation = Vector3.One;
            var refractionIndex = hitRecord.FrontFace ? (1 / this.materialRefractionIndex) : this.materialRefractionIndex;

            var normalizedDirection = Vector3.Normalize(incomingRay.Direction);
            var cosTheta = Math.Min(Vector3.Dot(-normalizedDirection, hitRecord.HitNormal), 1);
            var scatteredVector = refractionIndex * (float)Math.Sqrt(1 - cosTheta * cosTheta) > 1 || GetReflectance(cosTheta, refractionIndex) > RayTracingHelper.RandomFloat() ?
                Vector3.Reflect(normalizedDirection, hitRecord.HitNormal) :
                Refract(normalizedDirection, hitRecord.HitNormal, refractionIndex);


            scatteredRay = new Ray(hitRecord.HitPosition, scatteredVector);

            return true;
        }

        private static Vector3 Refract(Vector3 uv, Vector3 n, float refractiveIndexRatio)
        {
            var cosTheta = Math.Min(Vector3.Dot(-uv, n), 1);
            var ROutPerpendicular = refractiveIndexRatio * (uv + cosTheta * n);
            var ROutParallel = (float)-Math.Sqrt(Math.Abs(1.0f - ROutPerpendicular.LengthSquared())) * n;
            return ROutPerpendicular + ROutParallel;
        }

        private static float GetReflectance(float cosine, float refractionIndex)
        {
            var r0 = (1 - refractionIndex) / (1 + refractionIndex);
            r0 = r0 * r0;
            return r0 + (1 - r0) * (float)Math.Pow((1 - cosine), 5);
        }
    }
}
