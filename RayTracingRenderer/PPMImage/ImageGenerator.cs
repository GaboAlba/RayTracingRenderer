namespace RayTracingRenderer.PPMImage
{

    using log4net;
    using RayTracingRenderer.Materials;
    using RayTracingRenderer.Rays;
    using RayTracingRenderer.Shapes;
    using RayTracingRenderer.Shapes.Hittable;
    using RayTracingRenderer.Shared;
    using RayTracingRenderer.Utils;
    using System.Diagnostics;
    using System.Numerics;
    [Obsolete("Image Generator is single threaded and not optimized, use the RealTimeRayTracer project")]
    public static class ImageGenerator
    {
        private static readonly ILog log = LogManager.GetLogger(typeof(ImageGenerator));

        public static void RenderImage()
        {
            var camera = new Camera(
                    aspectRatio: 16.0f / 9.0f,
                    imageWidth: 1200,
                    verticalFieldfOfView: 20,
                    focalLength: 1.0f,
                    cameraCenter: new Vector3(13, 2, 3),
                    target: new Vector3(0, 0, 0),
                    cameraUp: new Vector3(0, 1, 0),
                    samplesPerPixel: 1,
                    defocusAngle: 0f,
                    focusDistance: 0f,
                    movementSpeed: 400000);

            // Small Scene

            // Define materials' albedo
            //var groundMaterial = new DiffuseMaterial(new Vector3(0.8f, 0.8f, 0));
            //var centerSphereMaterial = new DiffuseMaterial(new Vector3(0.1f, 0.2f, 0.5f));
            //var leftSphereMaterial = new MetalMaterial(new Vector3(0.8f, 0.8f, 0.8f), 0.3f);
            //var rightSphereMaterial = new MetalMaterial(new Vector3(0.8f, 0.6f, 0.2f), 1.0f);

            //// Initialize the scene
            //var world = new HittableObjectsList();
            //world.objects.Add(new Sphere(new Vector3(0, 0, -1), 0.5f, centerSphereMaterial));
            //world.objects.Add(new Sphere(new Vector3(0, -100.5f, -1.2f), 100, groundMaterial));
            //world.objects.Add(new Sphere(new Vector3(-1.0f, 0, -1), 0.5f, leftSphereMaterial));
            //world.objects.Add(new Sphere(new Vector3(1.0f, 0, -1), 0.5f, rightSphereMaterial));

            // Big scene

            //Initialize the scene
           var world = new HittableObjectsList();
            var groundMaterial = new DiffuseMaterial(new Vector3(0.5f, 0.5f, 0.5f));
            world.objects.Add(new Sphere(new Vector3(0, -1000, 0), 1000, groundMaterial));
            for (int a = -11; a < 11; a++)
            {
                for (int b = -11; b < 11; b++)
                {
                    var chooseMaterial = RayTracingHelper.RandomFloat();
                    var center = new Vector3(a + 0.9f * RayTracingHelper.RandomFloat(), 0.2f, b + 0.9f * RayTracingHelper.RandomFloat());

                    if ((center - new Vector3(4, 0.2f, 0)).Length() > 0.9f)
                    {
                        IMaterial material;
                        if (chooseMaterial < 0.8f)
                        {
                            // Diffuse material
                            var albedo = RayTracingHelper.RandomVector3() * RayTracingHelper.RandomVector3();
                            material = new DiffuseMaterial(albedo);
                            world.objects.Add(new Sphere(center, 0.2f, material));
                        }
                        else if (chooseMaterial < 0.95)
                        {
                            // Metal
                            var albedo = RayTracingHelper.RandomVector3(0.5f, 1);
                            var fuzz = RayTracingHelper.RandomFloat(0, 0.3f);
                            material = new MetalMaterial(albedo, fuzz);
                            world.objects.Add(new Sphere(center, 0.2f, material));
                        }
                        else
                        {
                            // Translucent
                            material = new DielectricMaterial(1.5f);
                            world.objects.Add(new Sphere(center, 0.2f, material));
                        }
                    }
                }
            }

            // Add big spheres
            var material1 = new DielectricMaterial(1.5f);
            var material2 = new DiffuseMaterial(new Vector3(0.4f, 0.2f, 0.1f));
            var material3 = new MetalMaterial(new Vector3(0.7f, 0.6f, 0.5f), 0);

            world.objects.Add(new Sphere(new Vector3(0, 1, 0), 1, material1));
            world.objects.Add(new Sphere(new Vector3(-4, 1, 0), 1, material2));
            world.objects.Add(new Sphere(new Vector3(4, 1, 0), 1, material3));

            using (var ppmWriter = new StreamWriter("image.ppm"))
            {
                ppmWriter.WriteLine("P3");
                ppmWriter.WriteLine($"{camera.ImageWidth} {camera.ImageHeight}");
                ppmWriter.WriteLine("255");
                var stopwatch = new Stopwatch();
                stopwatch.Start();
                for (var j = 0; j < camera.ImageHeight; j++)
                {
                    Console.WriteLine($"\rScanlines remaining {camera.ImageHeight - j}\n"); //TODO: Make the bar show
                    for (var i = 0; i < camera.ImageWidth; i++)
                    {
                        var pixelColor = new Vector3(0, 0, 0);
                        for (var sample = 0; sample < camera.SamplesPerPixel; sample++)
                        {
                            var ray = new Ray(camera, i, j);

                            pixelColor += ray.RayColor(ray, world, ray.MaxBounces);
                        }
                        WriteColor(pixelColor * camera.PixelSamplesScale, ppmWriter);
                    }
                }
                stopwatch.Stop();

                Console.WriteLine("\rDone.            \n");
                Console.WriteLine($"Time taken: {stopwatch.ElapsedMilliseconds}ms");
            }
        }

        public static void WriteColor(Vector3 pixelColor, StreamWriter writer)
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

            writer.WriteLine($"{redByte} {greenByte} {blueByte}");
        }
    }
}
