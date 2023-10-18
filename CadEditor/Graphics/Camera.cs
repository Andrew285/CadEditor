using SharpGL;
using SharpGL.SceneGraph.Cameras;
using SharpGL.SceneGraph.Core;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CadEditor
{
    public class Camera: SceneElement
    {
		private OpenGL gl;

		public double CameraDistance { get; set; } = 5.0f;
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

		public Camera(OpenGL _gl, Vector _rotationPoint)
        {
            gl = _gl;
			position = new Point3D(-1f, -1f, 0.5f, gl);
        }

        public void RotateAxisX()
        {
			gl.Rotate(utri, 1.0f, 0.0f, 0.0f);
        }

        public void RotateAxisY() 
        {
			gl.Rotate(rtri, 0.0f, 1.0f, 0.0f);
        }

		public void UpdateAxisX(double cameraAngle)
		{
			utri += cameraAngle;
		}

		public void UpdateAxisY(double cameraAngle)
		{
			rtri += cameraAngle;
		}

	}
}
