using log4net;
using RayTracingRenderer.Rays;
using System.Numerics;

namespace RayTracingRenderer.PPMImage
{
    public static class ImageGenerator
    {
        private static readonly ILog log = LogManager.GetLogger(typeof(ImageGenerator));
        
        public static void RenderImage(int width, int height)
        {
            var aspectRatio = 16.0f / 9.0f;
            var imageWidth = 1920;

            // Calculate height
            var imageHeight = (int)(imageWidth / aspectRatio);
            imageHeight = imageHeight < 1 ? 1 : imageHeight;

            // Compute viewport size
            var viewportHeight = 2.0f;
            var viewportWidth = viewportHeight * (float)imageWidth / imageHeight;

            // Compute camera
            var focalLength = 1.0f;
            var cameraCenter = new Vector3(0, 0, 0);

            // Vertical and Horizontal viewport vectors
            var viewportU = new Vector3(viewportWidth, 0, 0);
            var viewportV = new Vector3(0, -viewportHeight, 0);

            // Pixel delta
            var pixelDeltaU = viewportU / imageWidth;
            var pixelDeltaV = viewportV / imageHeight;

            // Location of upper left pixel
            var viewportUpperLeft = cameraCenter - new Vector3(0, 0, focalLength) - viewportU/2 - viewportV/2;
            var pixel100_loc = viewportUpperLeft + 0.5f * (pixelDeltaU + pixelDeltaV);


            Console.WriteLine("P3");
            Console.WriteLine($"{imageWidth} {imageHeight}");
            Console.WriteLine("255");
            for (var j = 0; j < imageHeight; j++)
            {
                log.Debug($"\rScanlines remaining {imageHeight - j}\n"); //TODO: Make the bar show
                for (var i = 0; i < imageWidth; i++)
                {
                    var pixelCenter = pixel100_loc + (i * pixelDeltaU) + (j * pixelDeltaV);
                    var rayDirection = pixelCenter - cameraCenter;
                    Ray ray = new Ray(cameraCenter, rayDirection);

                    var pixelColor = Ray.RayColor(ray);
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
