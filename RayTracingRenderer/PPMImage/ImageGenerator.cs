namespace RayTracingRenderer.PPMImage
{

    using log4net;
    using RayTracingRenderer.Materials;
    using RayTracingRenderer.Rays;
    using RayTracingRenderer.Shapes;
    using RayTracingRenderer.Shapes.Hittable;
    using RayTracingRenderer.Shared;
    using RayTracingRenderer.Utils;
    using System.Numerics;
    public static class ImageGenerator
    {
        private static readonly ILog log = LogManager.GetLogger(typeof(ImageGenerator));

        public static void RenderImage(int width, int height)
        {
            var camera = new Camera(
                aspectRatio: 16.0f / 9.0f,
                imageWidth: 400,
                viewportHeight: 2.0f,
                focalLength: 1.0f,
                cameraCenter: new Vector3(0, 0, 0),
                samplesPerPixel: 100);

            // Define materials' albedo
            var groundMaterial = new DiffuseMaterial(new Vector3(0.8f, 0.8f, 0));
            var centerSphereMaterial = new DiffuseMaterial(new Vector3(0.1f, 0.2f, 0.5f));
            var leftSphereMaterial = new MetalMaterial(new Vector3(0.8f, 0.8f, 0.8f));
            var rightSphereMaterial = new MetalMaterial(new Vector3(0.8f, 0.6f, 0.2f));

            // Initialize the scene
            var world = new HittableObjectsList();
            world.objects.Add(new Sphere(new Vector3(0, 0, -1), 0.5f, centerSphereMaterial));
            world.objects.Add(new Sphere(new Vector3(0, -100.5f, -1.2f), 100, groundMaterial));
            world.objects.Add(new Sphere(new Vector3(-1.0f, 0, -1), 0.5f, leftSphereMaterial));
            world.objects.Add(new Sphere(new Vector3(1.0f, 0, -1), 0.5f, rightSphereMaterial));

            Console.WriteLine("P3");
            Console.WriteLine($"{camera.ImageWidth} {camera.ImageHeight}");
            Console.WriteLine("255");
            for (var j = 0; j < camera.ImageHeight; j++)
            {
                log.Debug($"\rScanlines remaining {camera.ImageHeight - j}\n"); //TODO: Make the bar show
                for (var i = 0; i < camera.ImageWidth; i++)
                {
                    var pixelColor = new Vector3(0, 0, 0);
                    for (var sample = 0; sample < camera.SamplesPerPixel; sample++)
                    {
                        var ray = new Ray(camera, i, j);

                        pixelColor += ray.RayColor(ray, world, ray.MaxBounces);
                    }
                    WriteColor(pixelColor * camera.PixelSamplesScale);
                }
            }

            log.Debug("\rDone.            \n");
        }

        public static void WriteColor(Vector3 pixelColor)
        {
            var red = pixelColor.X;
            var green = pixelColor.Y;
            var blue = pixelColor.Z;

            // Apply gamma correction
            red = RayTracingHelper.LinearToGamma(red);
            green = RayTracingHelper.LinearToGamma(green);
            blue = RayTracingHelper.LinearToGamma(blue);

            // Translating the [0,1] range to default RGB
            var intensity = new Interval(0.000f, 0.999f);
            var redByte = (int)(256 * intensity.Clamp(red));
            var greenByte = (int)(256 * intensity.Clamp(green));
            var blueByte = (int)(256 * intensity.Clamp(blue));

            Console.WriteLine($"{redByte} {greenByte} {blueByte}");
        }
    }
}
