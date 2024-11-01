namespace RayTracingRenderer.PPMImage
{
    using RayTracingRenderer.Materials;
    using RayTracingRenderer.Rays;
    using RayTracingRenderer.Shapes;
    using RayTracingRenderer.Shapes.Hittable;
    using RayTracingRenderer.Shared;
    using RayTracingRenderer.Utils;
    using System;
    using System.Diagnostics;
    using System.Drawing;
    using System.Numerics;
    using System.Threading.Tasks;
    using System.Windows.Forms;

    [Obsolete("Please use the RealTimeRayTracer Project")]
    public class RealTimeRenderingRayTracer : Form
    {
        private Bitmap Bitmap;
        private object BitmapLock = new();
        private Timer RenderTimer = new();
        private Camera Camera;
        private Random Randomizer = new Random();
        private Action MoveCamera;

        public RealTimeRenderingRayTracer()
        {
            this.DoubleBuffered = true;
            this.Camera = new Camera(
                    aspectRatio: 16.0f / 9.0f,
                    imageWidth: 1200,
                    verticalFieldfOfView: 20,
                    focalLength: 1.0f,
                    cameraCenter: new Vector3(13, 2, 3),
                    target: new Vector3(0, 0, 0),
                    cameraUp: new Vector3(0, 1, 0),
                    samplesPerPixel: 10,
                    defocusAngle: 0f,
                    focusDistance: 10f);
            this.Bitmap = new Bitmap(this.Camera.ImageWidth, this.Camera.ImageHeight);
            this.KeyDown += this.ReadPosition;

            // Timer to trigger form repaint
            this.RenderTimer.Interval = 5;
            this.RenderTimer.Tick += (sender, e) => this.Invalidate(); // Repaint form
            this.RenderTimer.Start();

            // Run the pixel calculation in a background task
            Task.Run(() => CalculatePixels());
        }

        // Override OnPaint to render the bitmap on the form
        protected override void OnPaint(PaintEventArgs e)
        {
            lock (BitmapLock)
            {
                if (Bitmap != null)
                {
                    e.Graphics.DrawImage(Bitmap, 0, 0, Bitmap.Width, Bitmap.Height);
                }
            }
        }

        private void CalculatePixels()
        {
            // Create the scene
            var world = new HittableObjectsList();
            this.CreateWorld(world, false);
            Console.WriteLine("Loaded world");

            // Create Enumerable
            var pixelPositions = from i in Enumerable.Range(0, this.Camera.ImageWidth)
                                 from j in Enumerable.Range(0, this.Camera.ImageHeight)
                                 select new { i, j };
            pixelPositions = pixelPositions.OrderBy(_ => Randomizer.Next());

            var pixelBuffer = new Color[this.Camera.ImageWidth, this.Camera.ImageHeight];

            while (true)
            {
                Stopwatch.StartNew();
                var initialTime = Stopwatch.GetTimestamp();
                Parallel.ForEach(pixelPositions, position =>
                {
                    var pixelColor = new Vector3(0, 0, 0);
                    for (var sample = 0; sample < this.Camera.SamplesPerPixel; sample++)
                    {
                        var ray = new Ray(this.Camera, position.i, position.j, 10);
                        pixelColor += ray.RayColor(ray, world, ray.MaxBounces);
                    }

                    (var red, var green, var blue) = this.WriteColor(pixelColor * this.Camera.PixelSamplesScale);
                    pixelBuffer[position.i, position.j] = Color.FromArgb(red, green, blue);

                    lock (this.BitmapLock)
                    {
                        this.UpdateBitmapWithLockBits(position.i, position.j, pixelBuffer[position.i, position.j]);
                    }


                });
                var endTime = Stopwatch.GetElapsedTime(initialTime).TotalMilliseconds;
                Console.WriteLine($"Execution took: {endTime}ms");

                this.ApplyBufferToBitmap(pixelBuffer);
                lock (this.BitmapLock)
                {
                    this.Bitmap.Save("image.bmp");
                }
            }
        }

        private void ReadPosition(object sender, KeyEventArgs e)
        {
            Console.WriteLine("Reading positions");
            switch (e.KeyCode)
            {
                case Keys.W:
                    this.Camera.CameraCenter -= Vector3.UnitZ * this.Camera.MovementSpeed * (1 / (float)TimeSpan.TicksPerSecond); ;
                    break;
                case Keys.S:
                    this.Camera.CameraCenter += Vector3.UnitZ * this.Camera.MovementSpeed * (1 / (float)TimeSpan.TicksPerSecond);
                    break;
                case Keys.D:
                    this.Camera.CameraCenter += Vector3.UnitX * this.Camera.MovementSpeed * (1 / (float)TimeSpan.TicksPerSecond);
                    break;
                case Keys.A:
                    this.Camera.CameraCenter -= Vector3.UnitX * this.Camera.MovementSpeed * (1 / (float)TimeSpan.TicksPerSecond);
                    break;
                case Keys.Q:
                    this.Camera.CameraCenter -= Vector3.UnitY * this.Camera.MovementSpeed * (1 / (float)TimeSpan.TicksPerSecond);
                    break;
                case Keys.E:
                    this.Camera.CameraCenter += Vector3.UnitY * this.Camera.MovementSpeed * (1 / (float)TimeSpan.TicksPerSecond);
                    break;
            }
            Console.WriteLine($"New Camera Center: {this.Camera.CameraCenter}");
        }

        private void ApplyBufferToBitmap(Color[,] buffer)
        {
            lock (BitmapLock)
            {
                for (int i = 0; i < this.Camera.ImageWidth; i++)
                {
                    for (int j = 0; j < this.Camera.ImageHeight; j++)
                    {
                        Bitmap.SetPixel(i, j, buffer[i, j]);
                    }
                }
            }
        }

        /// <summary>
        /// This method updates the bitmap using raw pointers for added efficiency
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="color"></param>
        private void UpdateBitmapWithLockBits(int x, int y, Color color)
        {
            Rectangle rect = new Rectangle(0, 0, this.Bitmap.Width, this.Bitmap.Height);
            System.Drawing.Imaging.BitmapData bitmapData = this.Bitmap.LockBits(rect, System.Drawing.Imaging.ImageLockMode.ReadWrite, this.Bitmap.PixelFormat);

            int bytesPerPixel = System.Drawing.Bitmap.GetPixelFormatSize(this.Bitmap.PixelFormat) / 8;
            IntPtr ptrFirstPixel = bitmapData.Scan0;

            // Calculate the pixel's byte offset in memory
            int byteOffset = y * bitmapData.Stride + x * bytesPerPixel;

            unsafe
            {
                byte* pixelPtr = (byte*)ptrFirstPixel + byteOffset;
                pixelPtr[0] = color.B;  // Blue
                pixelPtr[1] = color.G;  // Green
                pixelPtr[2] = color.R;  // Red
                if (bytesPerPixel == 4)
                {
                    pixelPtr[3] = color.A;  // Alpha, if present
                }
            }

            this.Bitmap.UnlockBits(bitmapData);
        }

        private void CreateWorld(HittableObjectsList world, bool isRandom = false, int numberOfObjects = 100)
        {
            // Use random as false for development purposes
            if (!isRandom)
            {
                // Define materials' albedo
                var groundMaterial = new DiffuseMaterial(new Vector3(0.8f, 0.8f, 0));
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

        private (int, int, int) WriteColor(Vector3 pixelColor)
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

            return (redByte, greenByte, blueByte);
        }
    }
}
