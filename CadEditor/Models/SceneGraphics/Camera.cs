﻿using CadEditor.Models.Scene;
using GeometRi;
using System;
using System.Numerics;

namespace CadEditor
{
    public class Camera: IRotateable
    {
        public float xRotation { get; set; } = 0.0f;
        public float yRotation { get; set; } = 0.0f;
        public float zRotation { get; set; } = 0.0f;

        public Vector3 Position { get; set; }
        public float RotationSpeed { get; set; } = 1.0f;

        public static double ZoomSensitivity { get; set; } = 0.01f;
        public static double MinZoom { get; set; } = 1.0f;
        public static double MaxZoom { get; set; } = 40.0f;

        public Vector3 Target {  get; set; }
        public Vector3 Up {  get; set; }

        private float distance = 10.0f;
        private float azimuth = 0.0f; // Angle around the Y-axis
        private float elevation = 10.0f; // Angle above the XZ-plane

        public Camera()
        {
            Position = new Vector3(0, 0, distance);
            Target = new Vector3(0, 0, 0);
            Up = new Vector3(0, 1, 0);

            GraphicsGL.SetUpViewMatrix(this);
        }

        public float GetDistance()
        {
            return distance;
        }

        public void Rotate()
        {
            //GraphicsGL.GL.MatrixMode(SharpGL.Enumerations.MatrixMode.Modelview);
            //GraphicsGL.GL.PushMatrix();
            GraphicsGL.GL.Translate(Target.X, Target.Y, Target.Z);
            GraphicsGL.GL.Rotate(yRotation, 1.0f, 0.0f, 0.0f);
            GraphicsGL.GL.Rotate(xRotation, 0.0f, 1.0f, 0.0f);
            GraphicsGL.GL.Rotate(zRotation, 0.0f, 0.0f, 1.0f);
            GraphicsGL.GL.Translate(-Target.X, -Target.Y, -Target.Z);
        }

        public void UpdateRotation(double horizontalAngle, double verticalAngle, double rollAngle)
        {
            float xDelta = (float)horizontalAngle;
            float yDelta = (float)verticalAngle;
            float zDelta = (float)rollAngle;

            xRotation += xDelta * RotationSpeed;
            yRotation += yDelta * RotationSpeed;
            zRotation += zDelta * RotationSpeed;

            UpdateCameraPosition();
            GraphicsGL.Control.Invalidate();
        }

        public void SetCameraAt(double horizontalAngle, double verticalAngle, double rollAngle)
        {
            float xDelta = (float)horizontalAngle;
            float yDelta = (float)verticalAngle;
            float zDelta = (float)rollAngle;

            xRotation = xDelta * RotationSpeed;
            yRotation = yDelta * RotationSpeed;
            zRotation = zDelta * RotationSpeed;

            UpdateCameraPosition();
            GraphicsGL.Control.Invalidate();
        }


        public void SetTarget(double x, double y, double z)
        {
            xRotation = 0.0f;
            yRotation = 0.0f;
            Up = new Vector3(0, 1, 0);

            Target = new Vector3((float)x, (float)y, (float)z);
        }

        private void UpdateCameraPosition()
        {
            // Calculate camera position based on azimuth, elevation, and distance.
            double azimuthRad = azimuth * Math.PI / 180.0;
            double elevationRad = elevation * Math.PI / 180.0;

            float X = (float)(Target.X + distance * Math.Cos(elevationRad) * Math.Sin(azimuthRad));
            float Y = (float)(Target.Y + distance * Math.Sin(elevationRad));
            float Z = (float)(Target.Z + distance * Math.Cos(elevationRad) * Math.Cos(azimuthRad));

            Position = new Vector3(X, Y, Z);
        }

        public void ZoomBy(int value)
        {
            Zoom(value);
            LimitDistance();
            GraphicsGL.Invalidate();
        }

        public void Zoom(double value)
        {
            distance += (float)(-value * ZoomSensitivity);
        }

        public void LimitDistance()
        {
            float zValue = distance;
            zValue = (float)Math.Max(zValue, MinZoom);
            zValue = (float)Math.Min(zValue, MaxZoom);

            Position = new Vector3(Position.X, Position.Y, zValue);
        }
    }
}
