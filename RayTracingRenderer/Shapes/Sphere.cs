namespace RayTracingRenderer.Shapes
{
    using System.Numerics;

    // TODO: Look into making this a struct
    public class Sphere
    {
        public Vector3 Center { get; set; }

        public float Radius { get; set; }

        public Sphere(Vector3 center, float radius)
        {
            Center = center;
            Radius = radius;
        }
    }
}
