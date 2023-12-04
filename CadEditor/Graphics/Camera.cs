using CadEditor.Graphics;
using CadEditor.MeshObjects;
using SharpGL;
using SharpGL.SceneGraph;
using SharpGL.SceneGraph.Cameras;
using SharpGL.SceneGraph.Core;
using System;
using System.Windows.Forms;

namespace CadEditor
{
    public class Camera: SceneElement
    {
		public static double ZoomSensitivity { get; set; } = 0.01f;
		public static double MinZoom { get; set; } = 1.0f;
		public static double MaxZoom { get; set; } = 40.0f;
        public double rtri { get; set; }
        public double utri { get; set; }


		public Vector Position { get; set; } = new Vector(0, 0, 5.0f);
		public Point3D Target { get; set; } = new Point3D(0, 0, 0);
		public Vector RotationAxis { get; set; } = new Vector(0, 1, 0);
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

		public double AspectRatio
		{
			get { return aspectRatio; }
			set { aspectRatio = value; }
		}

		public void Rotate()
		{
            GraphicsGL.GL.Rotate(utri, 1.0f, 0.0f, 0.0f);
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
			Position[2] = Math.Max(Position[2], MinZoom);
            Position[2] = Math.Min(Position[2], MaxZoom);
        }

		public void Zoom(double value)
		{
			Position[2] += -value * ZoomSensitivity;
        }

		public void SetViewByAxis(CoordinateAxis axis)
		{
			if(axis == CoordinateAxis.X)
			{
                utri = 0.0f;
                rtri = 0.0f;

            }
			else if(axis == CoordinateAxis.Y)
			{
                utri = 90.0f;
                rtri = 0.0f;
            }
			else if(axis == CoordinateAxis.Z)
            {
                utri = 0.0f;
                rtri = 90.0f;
            }

            Rotate();
        }

    }
}
