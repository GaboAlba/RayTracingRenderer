namespace RayTracingRenderer.PPMImage
{

    using log4net;
    using RayTracingRenderer.Rays;
    using RayTracingRenderer.Shapes;
    using RayTracingRenderer.Shapes.Hittable;
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
                cameraCenter: new Vector3(0, 0, 0));

            var world = new HittableObjectsList();
            world.objects.Add(new Sphere(new Vector3(0, 0, -1), 0.5f));
            world.objects.Add(new Sphere(new Vector3(0, -100.5f, -1), 100));


            Console.WriteLine("P3");
            Console.WriteLine($"{camera.ImageWidth} {camera.ImageHeight}");
            Console.WriteLine("255");
            for (var j = 0; j < camera.ImageHeight; j++)
            {
                log.Debug($"\rScanlines remaining {camera.ImageHeight - j}\n"); //TODO: Make the bar show
                for (var i = 0; i < camera.ImageWidth; i++)
                {
                    var pixelCenter = camera.Pixel100Location + (i * camera.PixelDeltaU) + (j * camera.PixelDeltaV);
                    var rayDirection = pixelCenter - camera.CameraCenter;
                    Ray ray = new Ray(camera.CameraCenter, rayDirection);

                    var pixelColor = Ray.RayColor(ray, world);
                    WriteColor(pixelColor);
                }
            }

            log.Debug("\rDone.            \n");
        }

        public static void WriteColor(Vector3 pixelColor)
        {
            var red = pixelColor.X;
            var green = pixelColor.Y;
            var blue = pixelColor.Z;

            // Translating the [0,1] range to default RGB
            var redByte = (int)255.999 * red;
            var greenByte = (int)255.999 * green;
            var blueByte = (int)255.999 * blue;

            Console.WriteLine($"{redByte} {greenByte} {blueByte}");
        }
    }
}
