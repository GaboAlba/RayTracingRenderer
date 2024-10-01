using log4net;
using System.Numerics;

namespace RayTracingRenderer.PPMImage
{
    public static class ImageGenerator
    {
        private static readonly ILog log = LogManager.GetLogger(typeof(ImageGenerator));
        
        public static void RenderImage(int width, int height)
        {
            Console.WriteLine("P3");
            Console.WriteLine($"{width} {height}");
            Console.WriteLine("255");
            for (var j = 0; j < height; j++)
            {
                log.Debug($"\rScanlines remaining {height - j}\n"); //TODO: Make the bar show
                for (var i = 0; i < width; i++)
                {
                    var red = (float)i / (width - 1);
                    var green = (float)j / (height - 1);
                    var blue = 0.0f;

                    var pixelColor = new Vector3(red, green, blue);
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
