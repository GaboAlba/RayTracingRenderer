namespace RayTracingRenderer
{
    using RayTracingRenderer.PPMImage;
    class Program
    {
        [STAThread]
        public static int Main()
        {
            Application.EnableVisualStyles();
            Application.Run(new RealTimeRenderingRayTracer());

            // Only use this for experimental purposes, it isvery slow and produces a not friendly PPM Image
            // ImageGenerator.RenderImage();

            return 0;
        }
    }
}

