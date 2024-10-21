

namespace RayTracingRenderer.PPMImage
{
    using System.Numerics;

    public class Camera
    {
        public float AspectRatio { get; set; }

        public int ImageWidth { get; set; }

        public int ImageHeight => this.GetImageHeight();

        public float ViewportHeight { get; set; }

        public float ViewportWidth => this.ViewportHeight * (float)this.ImageWidth / this.ImageHeight;

        public float FocalLength { get; set; }

        public int SamplesPerPixel { get; set; }

        public float PixelSamplesScale => 1.0f / this.SamplesPerPixel;

        public Vector3 CameraCenter { get; set; }

        public Vector3 ViewportU => new Vector3(this.ViewportWidth, 0, 0);

        public Vector3 ViewportV => new Vector3(0, -this.ViewportHeight, 0);

        public Vector3 PixelDeltaU => this.ViewportU / this.ImageWidth;

        public Vector3 PixelDeltaV => this.ViewportV / this.ImageHeight;

        public Vector3 ViewportUpperLeftPixel => this.CameraCenter - new Vector3(0, 0, this.FocalLength) - this.ViewportU / 2 - this.ViewportV / 2;

        public Vector3 Pixel100Location => this.ViewportUpperLeftPixel + 0.5f * (this.PixelDeltaU + this.PixelDeltaV);

        public Camera(float aspectRatio, int imageWidth, float viewportHeight, float focalLength, Vector3 cameraCenter, int samplesPerPixel)
        {
            this.AspectRatio = aspectRatio;
            this.ImageWidth = imageWidth;
            this.ViewportHeight = viewportHeight;
            this.FocalLength = focalLength;
            this.CameraCenter = cameraCenter;
            this.SamplesPerPixel = samplesPerPixel;
        }

        private int GetImageHeight()
        {
            var imageHeight = (int)(this.ImageWidth / this.AspectRatio);
            imageHeight = imageHeight < 1 ? 1 : imageHeight;
            return imageHeight;
        }


    }
}
