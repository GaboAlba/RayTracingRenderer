

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

        public float PixelSamplesScale { get; init; }

        public int ImageHeight { get; init; }

        public float ViewportHeight { get; init; }

        public float ViewportWidth { get; init; }

        public float DefocusRadius { get; init; }

        public Vector3 DefocusDiskU { get; init; }

        public Vector3 DefocusDiskV { get; init; }

        public Vector3 ViewportU { get; init; }

        public Vector3 ViewportV { get; init; }

        public Vector3 PixelDeltaU { get; init; }

        public Vector3 PixelDeltaV { get; init; }

        public Vector3 ViewportUpperLeftPixel { get; init; }

        public Vector3 Pixel100Location { get; set; }

        public Vector3 W { get; init; }

        public Vector3 U { get; init; }

        public Vector3 V { get; init; }

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

            this.W = Vector3.Normalize(this.CameraCenter - this.Target);
            this.U = Vector3.Normalize(Vector3.Cross(this.CameraUp, this.W));
            this.V = Vector3.Cross(this.W, this.U);
            this.PixelSamplesScale = 1.0f / this.SamplesPerPixel;
            this.ImageHeight = this.GetImageHeight();
            this.ViewportHeight = 2 * (float)Math.Tan(this.VerticalFieldOfView / 2) * this.FocusDistance;
            this.ViewportWidth = this.ViewportHeight * (float)this.ImageWidth / this.ImageHeight;
            this.DefocusRadius = this.FocusDistance * (float)Math.Tan(RayTracingHelper.Deg2Rad(this.DefocusAngle / 2));
            this.DefocusDiskU = this.U * this.DefocusRadius;
            this.DefocusDiskV = this.V * this.DefocusRadius;
            this.ViewportU = this.ViewportWidth * this.U;
            this.ViewportV = this.ViewportHeight * -this.V;
            this.PixelDeltaU = this.ViewportU / this.ImageWidth;
            this.PixelDeltaV = this.ViewportV / this.ImageHeight;
            this.ViewportUpperLeftPixel = this.CameraCenter - (this.FocusDistance * this.W) - this.ViewportU / 2 - this.ViewportV / 2;
            this.Pixel100Location = this.ViewportUpperLeftPixel + 0.5f * (this.PixelDeltaU + this.PixelDeltaV);
        }

        private int GetImageHeight()
        {
            var imageHeight = (int)(this.ImageWidth / this.AspectRatio);
            imageHeight = imageHeight < 1 ? 1 : imageHeight;
            return imageHeight;
        }


    }
}
