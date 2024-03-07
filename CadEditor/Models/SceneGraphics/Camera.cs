using System;
using System.Numerics;

namespace CadEditor
{
    public class Camera
    {
        private float xRotation = 0.0f;
        private float yRotation = 0.0f;

        public Vector3 Position { get; set; }
        public float RotationSpeed { get; set; } = 1.0f;

        public static double ZoomSensitivity { get; set; } = 0.01f;
        public static double MinZoom { get; set; } = 1.0f;
        public static double MaxZoom { get; set; } = 40.0f;

        public Vector3 Target {  get; set; }
        public Vector3 Up {  get; set; }

        public Camera()
        {
            Position = new Vector3(0, 0, 10);
            Target = new Vector3(0, 0, 0);
            Up = new Vector3(0, 1, 0);

            GraphicsGL.SetUpViewMatrix(this);
        }

        public void Rotate()
        {
            GraphicsGL.GL.Translate(0.0f, 0.0f, -Position.Z);
            GraphicsGL.GL.Rotate(yRotation, 1.0f, 0.0f, 0.0f);
            GraphicsGL.GL.Rotate(xRotation, 0.0f, 1.0f, 0.0f);
            GraphicsGL.GL.Translate(-Target.X, -Target.Y, -Target.Z);
        }

        public void UpdateRotation(int x, int y)
        {
            float xDelta = (float)MouseController.GetHorizontalAngle(x);
            float yDelta = (float)MouseController.GetVerticalAngle(y);

            xRotation += xDelta * RotationSpeed;
            yRotation += yDelta * RotationSpeed;

            // Calculate the new position after rotation
            //CalculatePosition();
        }

        private void CalculatePosition()
        {
            // Convert degrees to radians
            float xRad = xRotation * (float)Math.PI / 180.0f;
            float yRad = yRotation * (float)Math.PI / 180.0f;

            // Calculate the new position based on the rotation angles
            float x = Target.X + Position.Z * (float)Math.Sin(yRad) * (float)Math.Cos(xRad);
            float y = Target.Y + Position.Z * (float)Math.Sin(xRad);
            //float z = Target.Z + Position.Z * (float)Math.Cos(yRad) * (float)Math.Cos(xRad);

            // Update the camera position
            Position = new Vector3(x, y, Position.Z);
        }

        public void SetTarget(double x, double y, double z)
        {
            xRotation = 0.0f;
            yRotation = 0.0f;
            Position = new Vector3(0, 0, 10);
            Target = new Vector3(0, 0, 0);
            Up = new Vector3(0, 1, 0);

            Target = new Vector3((float)x, (float)y, (float)z);
            GraphicsGL.SetUpViewMatrix(this);

        }

        public void LimitDistance()
        {
            float zValue = Position.Z;
            zValue = (float)Math.Max(zValue, MinZoom);
            zValue = (float)Math.Min(zValue, MaxZoom);

            Position = new Vector3(Position.X, Position.Y, zValue);
        }

        public void Zoom(double value)
        {
            float zValue = Position.Z;
            zValue += (float)(-value * ZoomSensitivity);
            Position = new Vector3(Position.X, Position.Y, zValue);
        }
    }
}
