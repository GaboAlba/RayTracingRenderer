

namespace RayTracingRenderer.PPMImage
{
    using RayTracingRenderer.Shared;
    using System.Numerics;

    public class Camera
    {
        public float AspectRatio { get; set; }

        public int ImageWidth { get; set; }

        public int SamplesPerPixel { get; set; }

        public float VerticalFieldOfView { get; set; }

        public Vector3 CameraCenter { get; set; }

        public Vector3 CameraUp { get; set; }

        public Vector3 Target { get; set; }

        public float MovementSpeed { get; set; }

        public float DefocusAngle { get; set; }

        public float FocusDistance { get; set; }

        public float PixelSamplesScale => 1.0f / this.SamplesPerPixel;

        public int ImageHeight => this.GetImageHeight();

        public float ViewportHeight => 2 * (float)Math.Tan(this.VerticalFieldOfView / 2) * this.FocusDistance;

        public float ViewportWidth => this.ViewportHeight * (float)this.ImageWidth / this.ImageHeight;

        public float DefocusRadius => this.FocusDistance * (float)Math.Tan(RayTracingHelper.Deg2Rad(this.DefocusAngle / 2));

        public Vector3 DefocusDiskU => this.U * this.DefocusRadius;

        public Vector3 DefocusDiskV => this.V * this.DefocusRadius;

        public Vector3 ViewportU => this.ViewportWidth * this.U;

        public Vector3 ViewportV => this.ViewportHeight * -this.V;

        public Vector3 PixelDeltaU => this.ViewportU / this.ImageWidth;

        public Vector3 PixelDeltaV => this.ViewportV / this.ImageHeight;

        public Vector3 ViewportUpperLeftPixel => this.CameraCenter - (this.FocusDistance * this.W) - this.ViewportU / 2 - this.ViewportV / 2;

        public Vector3 Pixel100Location => this.ViewportUpperLeftPixel + 0.5f * (this.PixelDeltaU + this.PixelDeltaV);

        public Vector3 W => Vector3.Normalize(this.CameraCenter - this.Target);

        public Vector3 U => Vector3.Normalize(Vector3.Cross(this.CameraUp, this.W));

        public Vector3 V => Vector3.Cross(this.W, this.U);

        public Camera(float aspectRatio, int imageWidth, float verticalFieldfOfView, float focalLength, Vector3 cameraCenter, Vector3 target, Vector3 cameraUp, int samplesPerPixel, float defocusAngle, float focusDistance, float movementSpeed = 40000f)
        {
            this.AspectRatio = aspectRatio;
            this.ImageWidth = imageWidth;

            // Determine Viewport Height
            this.VerticalFieldOfView = verticalFieldfOfView;
            this.VerticalFieldOfView = RayTracingHelper.Deg2Rad(this.VerticalFieldOfView);

            this.CameraCenter = cameraCenter;
            this.Target = target;
            this.CameraUp = cameraUp;
            this.SamplesPerPixel = samplesPerPixel;
            this.DefocusAngle = defocusAngle;
            this.FocusDistance = focusDistance;
            this.MovementSpeed = movementSpeed;
        }

        private int GetImageHeight()
        {
            var imageHeight = (int)(this.ImageWidth / this.AspectRatio);
            imageHeight = imageHeight < 1 ? 1 : imageHeight;
            return imageHeight;
        }


    }
}
