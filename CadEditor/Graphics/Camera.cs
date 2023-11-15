using CadEditor.Graphics;
using SharpGL;
using SharpGL.SceneGraph;
using SharpGL.SceneGraph.Cameras;
using SharpGL.SceneGraph.Core;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace CadEditor
{
    public class Camera: SceneElement
    {
		public double CameraDistance { get; set; } = 5.0f;
		public static double ZoomSensitivity { get; set; } = 0.01f;
		public static double MinZoom { get; set; } = 1.0f;
		public static double MaxZoom { get; set; } = 20.0f;
        public double rtri { get; set; }
        public double utri { get; set; }


		private Point3D position;
		private double aspectRatio = 1.0f;
		private double fieldOfView = 60.0f;
		private double near = 0.5f;
		private double far = 9999.0f;

		public double FieldOfView
		{
			get { return fieldOfView; }
			set { fieldOfView = value; }
		}

		public double Near
		{
			get { return near; }
			set { near = value; }
		}

		public double Far
		{
			get { return far; }
			set { far = value; }
		}

		public Point3D Position
		{
			get { return position; }
		}

		public double AspectRatio
		{
			get { return aspectRatio; }
			set { aspectRatio = value; }
		}

		public Camera(Vector _rotationPoint)
        {
			position = new Point3D(-1f, -1f, 0.5f);
        }

        public void RotateAxisX()
        {
			GraphicsGL.GL.Rotate(utri, 1.0f, 0.0f, 0.0f);
        }

        public void RotateAxisY() 
        {
            GraphicsGL.GL.Rotate(rtri, 0.0f, 1.0f, 0.0f);
        }

		public void UpdateAxisX(double cameraAngle)
		{
			utri += cameraAngle;
		}

		public void UpdateAxisY(double cameraAngle)
		{
			rtri += cameraAngle;
		}

		public void Update(int x, int y)
		{
            double horizontalAngle = MouseController.GetHorizontalAngle(x);
            double verticalAngle = MouseController.GetVerticalAngle(y);

            UpdateAxisX(verticalAngle);
			UpdateAxisY(horizontalAngle);
		}

		public void LimitDistance()
		{
            CameraDistance = Math.Max(CameraDistance, MinZoom);
            CameraDistance = Math.Min(CameraDistance, MaxZoom);
        }

		public void Zoom(double value)
		{
            CameraDistance += value * ZoomSensitivity;
        }

	}
}
