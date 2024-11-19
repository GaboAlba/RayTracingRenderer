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
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;

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
        private readonly Camera Camera;
        private HittableObjectsList world;
        private volatile bool moved;
        private int samplesProcessed;
        private IEnumerable<(int i, int j)> pixelPositions;

        public MainWindow()
        {
            this.Camera = new Camera(
                    aspectRatio: 16.0f / 9.0f,
                    imageWidth: 1200,
                    verticalFieldfOfView: 20,
                    focalLength: 1.0f,
                    cameraCenter: new Vector3(0, 0, 2),
                    target: new Vector3(0, 0, 0),
                    cameraUp: new Vector3(0, 1, 0),
                    samplesPerPixel: 1,
                    defocusAngle: 0f,
                    focusDistance: 100f,
                    movementSpeed: 800000);
            this.InitializeComponent();

            // Initialize enumarable for the pixel positions
            this.pixelPositions = Enumerable.Range(0, this.Camera.ImageWidth)
                                   .SelectMany(i => Enumerable.Range(0, this.Camera.ImageHeight)
                                                              .Select(j => (i, j)));

            // Initialize WriteableBitmap
            writeableBitmap = new WriteableBitmap(this.Camera.ImageWidth, this.Camera.ImageHeight, 96, 96, PixelFormats.Bgra32, null);
            this.displayImage.Source = writeableBitmap;
            this.samplesProcessed = 1;

            // Initialize pixel buffer
            this.pixelBuffer = new byte[this.Camera.ImageWidth * this.Camera.ImageHeight * bytesPerPixel];
            this.accumulatedBuffer = new float[this.Camera.ImageWidth * this.Camera.ImageHeight * bytesPerPixel];

            // Initialize and cache the world objects
            this.world = new HittableObjectsList();
            this.CreateWorld(this.world, false);

            // Add key press events to move the camera
            this.KeyDown += this.ReadPosition;

            // Start a rendering loop to continuously update the bitmap
            Task.Run(() => RenderLoop());
        }

        private void RenderLoop()
        {
            while (true)
            {
                // Update pixel buffer with ray-tracing results
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
            if (this.moved)
            {
                this.samplesProcessed = 1;
                Array.Clear(this.accumulatedBuffer, 0, this.accumulatedBuffer.Length);
            }

            this.moved = false;

            Parallel.ForEach(this.pixelPositions, position =>
            {
                var index = (position.j * this.Camera.ImageWidth + position.i) * bytesPerPixel;

                var pixelColor = new Vector3(0, 0, 0);
                var ray = new Ray(this.Camera, position.i, position.j, 50);
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
            });
            this.samplesProcessed++;
        }

        // Need to move this into the camera and make it so it moves with respect to the target
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
                case Key.Up:
                    this.Camera.Pixel100Location += Vector3.UnitY * this.Camera.MovementSpeed * 10 * (1 / (float)TimeSpan.TicksPerSecond);
                    this.moved = true;
                    break;
                case Key.Down:
                    this.Camera.Pixel100Location -= Vector3.UnitY * this.Camera.MovementSpeed * 10 * (1 / (float)TimeSpan.TicksPerSecond);
                    this.moved = true;
                    break;
                case Key.Right:
                    this.Camera.Pixel100Location += Vector3.UnitX * this.Camera.MovementSpeed * 10 * (1 / (float)TimeSpan.TicksPerSecond);
                    this.moved = true;
                    break;
                case Key.Left:
                    this.Camera.Pixel100Location -= Vector3.UnitX * this.Camera.MovementSpeed * 10 * (1 / (float)TimeSpan.TicksPerSecond);
                    this.moved = true;
                    break;
            }
            Console.WriteLine($"New Camera Center: {this.Camera.CameraCenter}");
        }

        private void CreateWorld(HittableObjectsList world, bool isRandom = false)
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