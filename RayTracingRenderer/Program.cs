using RayTracingRenderer.PPMImage;

namespace RayTracingRenderer
{
    class Program
    {
        public static int Main()
        {
            ImageGenerator.RenderImage(256, 256);
            return 0;
        }
    }
}

