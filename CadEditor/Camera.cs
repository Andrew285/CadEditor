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
		private Vector cameraPosition;
        private Vector rotationPoint;

		public double CameraDistance { get; set; } = 5.0f;
        public double rtri { get; set; }
        public double utri { get; set; }


		private Vertex position;
		private Matrix projectionMatrix = new Matrix(4, 4);
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


		public Vertex Position
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
            rotationPoint = _rotationPoint;
			position = new Vertex(-1f, -1f, 0.5f, gl);
			cameraPosition = new Vector(new double[] {0, 0, 5});
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
