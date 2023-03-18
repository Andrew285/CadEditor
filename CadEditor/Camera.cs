using SharpGL;
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
        private Vector cameraPosition;
        private Vector rotationPoint;

        public double rtri { get; set; }
        public double utri { get; set; }
        private OpenGL gl;


		private Vertex position = new Vertex(-1f, -1f, 0.5f);
		private Matrix projectionMatrix = new Matrix(4, 4);
		private double aspectRatio = 1.0f;
		private double fieldOfView = 60.0f;
		private double near = 0.5f;
		private double far = 9999.0f;

		[Description("The angle of the lense of the camera (60 degrees = human eye)."), Category("Camera (Perspective")]
		public double FieldOfView
		{
			get { return fieldOfView; }
			set { fieldOfView = value; }
		}

		[Description("The near clipping distance."), Category("Camera (Perspective")]
		public double Near
		{
			get { return near; }
			set { near = value; }
		}

		/// <summary>
		/// Gets or sets the far.
		/// </summary>
		/// <value>
		/// The far.
		/// </value>
		[Description("The far clipping distance."), Category("Camera (Perspective")]
		public double Far
		{
			get { return far; }
			set { far = value; }
		}


		protected Vertex target = new Vertex(0, 0, 0);
		protected Vertex upVector = new Vertex(0, 0, 1);

		public Vertex Position
		{
			get { return position; }
			set { position = value; }
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
            cameraPosition = new Vector(new double[] {0, 0, 5});
        }

        public void RotateAxisX()
        {
            //// Translate the scene to the origin
            //gl.Translate(-rotationPoint[0], -rotationPoint[1], -rotationPoint[2]);

			gl.Rotate(utri, 1.0f, 0.0f, 0.0f);

            //// Translate the scene back to its original position
            //gl.Translate(rotationPoint[0], rotationPoint[1], rotationPoint[2]);

        }

        public void RotateAxisY() 
        {
			//// Translate the scene to the origin
			//gl.Translate(-rotationPoint[0], -rotationPoint[1], -rotationPoint[2]);

			gl.Rotate(rtri, 0.0f, 1.0f, 0.0f);

			//// Translate the scene back to its original position
			//gl.Translate(rotationPoint[0], rotationPoint[1], rotationPoint[2]);

        }

        public void UpdatePosition()
        {
            // Get the current camera position
            double[] cameraPos = new double[3];

			// Update the camera position
			double[] modelview = new double[16];
			gl.GetDouble(OpenGL.GL_MODELVIEW_MATRIX, modelview);
			cameraPos[0] = -modelview[12];
			cameraPos[1] = -modelview[13];
			cameraPos[2] = -modelview[14];
            cameraPosition = new Vector(cameraPos);
		}

		public void UpdateCameraPosition(double horizontalAngle, double verticalAngle)
		{
			double radius = Math.Sqrt(Math.Pow(cameraPosition[0] - rotationPoint[0], 2) +
									  Math.Pow(cameraPosition[1] - rotationPoint[1], 2) +
									  Math.Pow(cameraPosition[2] - rotationPoint[2], 2));

			double x = rotationPoint[0] + radius * Math.Sin(verticalAngle) * Math.Cos(horizontalAngle);
			double y = rotationPoint[1] + radius * Math.Cos(verticalAngle);
			double z = rotationPoint[2] + radius * Math.Sin(verticalAngle) * Math.Sin(horizontalAngle);

			cameraPosition = new Vector(new double[] { x, y, z });
		}

		public void UpdateOpenGLCameraPosition(OpenGL gl, double cameraInclination, double cameraAzimuth)
		{
			double cameraRadius = 5;

			// calculate new camera position in spherical coordinates
			double x = cameraRadius * Math.Sin(cameraInclination) * Math.Cos(cameraAzimuth);
			double y = cameraRadius * Math.Cos(cameraInclination);
			double z = cameraRadius * Math.Sin(cameraInclination) * Math.Sin(cameraAzimuth);

			// update camera position
			gl.MatrixMode(OpenGL.GL_MODELVIEW);
			gl.LoadIdentity();
			gl.LookAt(x, y, z, 0, 0, 0, 0, 1, 0);

			cameraPosition = new Vector(new double[] { x, y, z });
		}

		public Vector GetPosition()
        {
            return cameraPosition;
        }

        public Vector GetDirection()
        {
			double[] modelview = new double[16];
			gl.GetDouble(OpenGL.GL_MODELVIEW_MATRIX, modelview);

			// Extract the camera direction from the model-view matrix
			double cameraDirX = -modelview[8];
			double cameraDirY = -modelview[9];
			double cameraDirZ = -modelview[10];

			return new Vector(new double[] { cameraDirX, cameraDirY, cameraDirZ });
		}




		public void TransformProjectionMatrix(OpenGL gl)
		{
			//  Perform the look at transformation.
			gl.Perspective(FieldOfView, AspectRatio, Near, Far);
			gl.LookAt((double)Position.X, (double)Position.Y, (double)Position.Z,
				(double)target.X, (double)target.Y, (double)target.Z,
				(double)upVector.X, (double)upVector.Y, (double)upVector.Z);
		}
	}
}
