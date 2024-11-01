using RayTracingRenderer.Materials;
using RayTracingRenderer.PPMImage;
using RayTracingRenderer.Rays;
using RayTracingRenderer.Shapes;
using RayTracingRenderer.Shapes.Hittable;
using RayTracingRenderer.Shared;
using RayTracingRenderer.Utils;
using System.Diagnostics;
using System.Numerics;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using Timer = System.Windows.Forms.Timer;

namespace RealTimeRayTracer
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private WriteableBitmap writeableBitmap;
        private int bytesPerPixel = 4; // For PixelFormats.Bgra32
        private byte[] pixelBuffer;
        private float[] accumulatedBuffer;
        private Timer RenderTimer = new();
        private Camera Camera;
        private Random Randomizer = new Random();
        private HittableObjectsList world;
        private bool moved;
        private int samplesProcessed;
        private IEnumerable<(int i, int j)> pixelPositions;

        public MainWindow()
        {
            this.Camera = new Camera(
                    aspectRatio: 16.0f / 9.0f,
                    imageWidth: 1200,
                    verticalFieldfOfView: 20,
                    focalLength: 1.0f,
                    cameraCenter: new Vector3(13, 2, 3),
                    target: new Vector3(0, 0, 0),
                    cameraUp: new Vector3(0, 1, 0),
                    samplesPerPixel: 1,
                    defocusAngle: 0f,
                    focusDistance: 10f,
                    movementSpeed: 400000);
            this.InitializeComponent();

            this.pixelPositions = Enumerable.Range(0, this.Camera.ImageWidth)
                                   .SelectMany(i => Enumerable.Range(0, this.Camera.ImageHeight)
                                                              .Select(j => (i, j)));
            this.pixelPositions = this.pixelPositions.OrderBy(_ => Randomizer.Next());

            // Initialize WriteableBitmap
            writeableBitmap = new WriteableBitmap(this.Camera.ImageWidth, this.Camera.ImageHeight, 96, 96, PixelFormats.Bgra32, null);
            this.displayImage.Source = writeableBitmap;
            this.samplesProcessed = 1;

            // Set WriteableBitmap as the source for an Image control in the XAML
            Image displayImage = new Image
            {
                Source = writeableBitmap,
                Width = this.Camera.ImageWidth,
                Height = this.Camera.ImageHeight,
            };

            // Initialize pixel buffer
            this.pixelBuffer = new byte[this.Camera.ImageWidth * this.Camera.ImageHeight * bytesPerPixel];
            this.accumulatedBuffer = new float[this.Camera.ImageWidth * this.Camera.ImageHeight * bytesPerPixel];

            // Initialize and cache the world objects
            this.world = new HittableObjectsList();
            this.CreateWorld(this.world, false);

            this.KeyDown += this.ReadPosition;

            // Start a rendering loop to continuously update the bitmap
            Task.Run(() => RenderLoop());
        }

        private void RenderLoop()
        {
            while (true)
            {
                // Update pixel buffer with random color data or ray-tracing results
                var stopwatch = Stopwatch.StartNew();
                this.CalculatePixels();
                stopwatch.Stop();
                var renderTime = stopwatch.ElapsedMilliseconds;

                // Apply pixel buffer to WriteableBitmap
                Dispatcher.Invoke(() =>
                {
                    UpdateWriteableBitmap();
                    this.renderTimeText.Text = $"RenderTime: {renderTime} ms";
                });
            }
        }

        private void CalculatePixels()
        {
            this.moved = false;

            Parallel.ForEach(this.pixelPositions, position =>
            {
                var index = (position.j * this.Camera.ImageWidth + position.i) * bytesPerPixel;

                var pixelColor = new Vector3(0, 0, 0);
                var ray = new Ray(this.Camera, position.i, position.j, 10);
                pixelColor = ray.RayColor(ray, this.world, ray.MaxBounces);

                // Accumulate color
                this.accumulatedBuffer[index + 0] += pixelColor.Z;
                this.accumulatedBuffer[index + 1] += pixelColor.Y;
                this.accumulatedBuffer[index + 2] += pixelColor.X;
                var accumulatedColor = new Vector3(this.accumulatedBuffer[index + 2], this.accumulatedBuffer[index + 1], this.accumulatedBuffer[index]);
                accumulatedColor /= this.samplesProcessed;

                (var red, var green, var blue) = this.WriteColor(accumulatedColor * this.Camera.PixelSamplesScale);

                this.pixelBuffer[index + 0] = blue;
                this.pixelBuffer[index + 1] = green;
                this.pixelBuffer[index + 2] = red;
                this.pixelBuffer[index + 3] = 255;

                if (this.moved)
                {
                    this.samplesProcessed = 1;
                    this.accumulatedBuffer[index + 0] = 0;
                    this.accumulatedBuffer[index + 1] = 0;
                    this.accumulatedBuffer[index + 2] = 0;
                }
            });
            this.samplesProcessed++;
        }

        private void ReadPosition(object sender, System.Windows.Input.KeyEventArgs e)
        {
            Console.WriteLine("Reading positions");
            switch (e.Key)
            {
                case Key.W:
                    this.Camera.CameraCenter -= Vector3.UnitZ * this.Camera.MovementSpeed * (1 / (float)TimeSpan.TicksPerSecond);
                    this.moved = true;
                    break;
                case Key.S:
                    this.Camera.CameraCenter += Vector3.UnitZ * this.Camera.MovementSpeed * (1 / (float)TimeSpan.TicksPerSecond);
                    this.moved = true;
                    break;
                case Key.D:
                    this.Camera.CameraCenter += Vector3.UnitX * this.Camera.MovementSpeed * (1 / (float)TimeSpan.TicksPerSecond);
                    this.moved = true;
                    break;
                case Key.A:
                    this.Camera.CameraCenter -= Vector3.UnitX * this.Camera.MovementSpeed * (1 / (float)TimeSpan.TicksPerSecond);
                    this.moved = true;
                    break;
                case Key.Q:
                    this.Camera.CameraCenter -= Vector3.UnitY * this.Camera.MovementSpeed * (1 / (float)TimeSpan.TicksPerSecond);
                    this.moved = true;
                    break;
                case Key.E:
                    this.Camera.CameraCenter += Vector3.UnitY * this.Camera.MovementSpeed * (1 / (float)TimeSpan.TicksPerSecond);
                    this.moved = true;
                    break;
            }
            Console.WriteLine($"New Camera Center: {this.Camera.CameraCenter}");
        }

        private void CreateWorld(HittableObjectsList world, bool isRandom = false, int numberOfObjects = 100)
        {
            // Use random as false for development purposes
            if (!isRandom)
            {
                // Define materials' albedo
                var groundMaterial = new DiffuseMaterial(new Vector3(0.5f, 0.5f, 0.5f));
                var centerSphereMaterial = new DiffuseMaterial(new Vector3(0.1f, 0.2f, 0.5f));
                var leftSphereMaterial = new DielectricMaterial(1.5f);
                var leftBubbleMaterial = new DielectricMaterial(1.0f / 1.5f);
                var rightSphereMaterial = new MetalMaterial(new Vector3(0.8f, 0.6f, 0.2f), 0.0f);

                // Create objects
                world.objects.Add(new Sphere(new Vector3(0, 0, -1f), 0.5f, centerSphereMaterial));
                world.objects.Add(new Sphere(new Vector3(0, -100.5f, -1.2f), 100, groundMaterial));
                world.objects.Add(new Sphere(new Vector3(-1.0f, 0, -1), 0.5f, leftSphereMaterial));
                world.objects.Add(new Sphere(new Vector3(-1.0f, 0, -1), 0.4f, leftBubbleMaterial));
                world.objects.Add(new Sphere(new Vector3(1.0f, 0, -1), 0.5f, rightSphereMaterial));
            }
            else
            {
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
                                world.objects.Append(new Sphere(center, 0.2f, material));
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

            }
        }

        private (byte, byte, byte) WriteColor(Vector3 pixelColor)
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
            var redByte = (byte)(256 * intensity.Clamp(red));
            var greenByte = (byte)(256 * intensity.Clamp(green));
            var blueByte = (byte)(256 * intensity.Clamp(blue));

            return (redByte, greenByte, blueByte);
        }

        private Vector3 ReadColor(float red, float green, float blue)
        {
            red = red / 256;
            green = green/256;
            blue = blue / 256;

            var intensity = new Interval(0.000f, 0.999f);
            red = intensity.Clamp(red);
            green = intensity.Clamp(green);
            blue = intensity.Clamp(blue);

            red = RayTracingHelper.GammaToLinear(red);
            green = RayTracingHelper.GammaToLinear(green);
            blue = RayTracingHelper.GammaToLinear(blue);

            return new Vector3(red, green, blue);
        }

        private void UpdatePixelBuffer()
        {
            // Modify the pixel buffer, setting color data
            Parallel.For(0, this.Camera.ImageHeight, y =>
            {
                for (int x = 0; x < this.Camera.ImageWidth; x++)
                {
                    int index = (y * this.Camera.ImageWidth + x) * bytesPerPixel;

                    // Example data (replace this with your actual pixel data)
                    pixelBuffer[index + 0] = (byte)(x % 256); // Blue
                    pixelBuffer[index + 1] = (byte)(y % 256); // Green
                    pixelBuffer[index + 2] = 128;             // Red
                    pixelBuffer[index + 3] = 255;             // Alpha
                }
            });
        }

        private void UpdateWriteableBitmap()
        {
            // Lock the WriteableBitmap for direct access to the back buffer
            writeableBitmap.Lock();

            // Copy pixel data to the back buffer
            Marshal.Copy(pixelBuffer, 0, writeableBitmap.BackBuffer, pixelBuffer.Length);

            // Mark the whole bitmap as dirty to refresh the display
            writeableBitmap.AddDirtyRect(new Int32Rect(0, 0, this.Camera.ImageWidth, this.Camera.ImageHeight));

            // Unlock the WriteableBitmap to apply the updates
            writeableBitmap.Unlock();
        }
    }
}