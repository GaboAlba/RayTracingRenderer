using System.Numerics;

namespace RayTracingRenderer.Shared
{
    public class RayTracingHelper
    {
        public static float Deg2Rad(float degrees)
        {
            return degrees * float.Pi / 180;
        }

        public static float RandomFloat()
        {
            return (float)Random.Shared.NextDouble();
        }

        public static float RandomFloat(float min, float max)
        {
            return min + (max - min) * RandomFloat();
        }

        /// <summary>
        /// This is used for anti aliasing to get the pixels in a sample square of 0.5f, 0.5f sides
        /// </summary>
        /// <returns></returns>
        public static Vector3 SampleSquare()
        {
            return new Vector3(RandomFloat() - 0.5f, RandomFloat() - 0.5f, 0);
        }

        public static Vector3 RandomVector3()
        {
            return new Vector3(RandomFloat(), RandomFloat(), RandomFloat());
        }

        public static Vector3 RandomVector3(float min, float max)
        {
            return new Vector3(RandomFloat(min, max), RandomFloat(min, max), RandomFloat(min, max));
        }

        /// <summary>
        /// Method that uses rejection to find a random vector inside the unit sphere. TODO: Change it to an analytical method for more efficiency
        /// </summary>
        /// <returns></returns>
        public static Vector3 RandomUnitVector()
        {
            var hasBeenFound = false;
            Vector3 vector = new Vector3(0, 0, 0);
            float lensq = 0;
            while (!hasBeenFound)
            {
                vector = RandomVector3(-1, 1);
                lensq = vector.LengthSquared();
                if (lensq <= 1)
                {
                    hasBeenFound = true;
                }
            }

            return vector / (float)Math.Sqrt(lensq == 0 ? 1e-80 : lensq);
        }

        public static Vector3 RandomVectorInDefocusDisk()
        {
            while (true)
            {
                var p = new Vector3(RandomFloat(-1, 1), RandomFloat(-1, 1), 0);
                if (p.LengthSquared() <= 1)
                {
                    return p;
                }
            }
        }

        public static Vector3 RandomVectorOnHemisphere(Vector3 normal)
        {
            var OnUnitSphere = RandomUnitVector();
            if (Vector3.Dot(OnUnitSphere, normal) > 0) // Same hemisphere as normal
            {
                return OnUnitSphere;
            }

            return -1 * OnUnitSphere;
        }

        public static float LinearToGamma(float linearComponent)
        {
            if (linearComponent > 0)
            {
                return (float)Math.Sqrt(linearComponent);
            }

            return 0;
        }

        public static float GammaToLinear(float gammaComponent)
        {
            return gammaComponent * gammaComponent;
        }
    }
}
