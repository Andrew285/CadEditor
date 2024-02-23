using CadEditor.MeshObjects;
using SharpGL;
using SharpGL.SceneGraph.Core;
using System;
using System.Numerics;

namespace CadEditor
{
    //  public class Camera: SceneElement
    //  {
    //public static double ZoomSensitivity { get; set; } = 0.01f;
    //public static double MinZoom { get; set; } = 1.0f;
    //public static double MaxZoom { get; set; } = 40.0f;
    //      public double rtri { get; set; }
    //      public double utri { get; set; }


    //public Vector Position { get; set; } = new Vector(0, 0, 5.0f);
    //public Point3D Target { get; set; } = new Point3D(0, 0, 0);
    //public Vector RotationAxis { get; set; } = new Vector(0, 1, 0);
    //private double aspectRatio = 1.0f;
    //private double fieldOfView = 60.0f;
    //private double near = 0.5f;
    //private double far = 9999.0f;


    //public Vector3 cameraDirection;
    //      public Vector3 cameraPosition = new Vector3(0, 0, 5);
    //      public Vector3 cameraTarget = new Vector3(0, 0, 0);
    //      public Vector3 up = new Vector3(0, 1, 0);
    //      public Vector3 cameraRight = new Vector3(0, 1, 0);
    //      public Vector3 cameraUp;

    //      public Matrix4x4 view;

    //public Camera()
    //{
    //	cameraDirection = Vector3.Normalize(cameraPosition - cameraTarget);
    //	cameraRight = Vector3.Normalize(Vector3.Cross(up, cameraDirection));
    //	cameraUp = Vector3.Normalize(Vector3.Cross(cameraDirection, cameraRight));

    //	view = Matrix4x4.CreateLookAt(cameraPosition, cameraTarget, cameraUp);
    //}

    //public double FieldOfView
    //{
    //	get { return fieldOfView; }
    //	set { fieldOfView = value; }
    //}

    //public double Near
    //{
    //	get { return near; }
    //	set { near = value; }
    //}

    //public double Far
    //{
    //	get { return far; }
    //	set { far = value; }
    //}

    //public double AspectRatio
    //{
    //	get { return aspectRatio; }
    //	set { aspectRatio = value; }
    //}

    //public void Rotate()
    //{
    //	//GraphicsGL.GL.Rotate(utri, 1.0f, 0.0f, 0.0f);
    //	//GraphicsGL.GL.Rotate(rtri, 0.0f, 1.0f, 0.0f);

    //          GraphicsGL.GL.Translate(-cameraTarget.X, -cameraTarget.Y, -cameraTarget.Z);

    //	////float radius = 10.0f;
    //	//float camX = (float)Math.Sin(rtri);
    //	//float camZ = (float)Math.Cos(utri);
    //	//view = Matrix4x4.CreateLookAt(new Vector3((float)utri, cameraPosition.Y, (float)rtri), cameraTarget, cameraUp);
    //	GraphicsGL.GL.Rotate(rtri, cameraUp.X, cameraUp.Y, cameraUp.Z);
    //	GraphicsGL.GL.Rotate(utri, cameraRight.X, cameraRight.Y, cameraRight.Z);

    //	GraphicsGL.GL.Translate(cameraTarget.X, cameraTarget.Y, cameraTarget.Z);
    //      }

    //public void UpdateAxisX(double cameraAngle)
    //{
    //	utri += cameraAngle;
    //}

    //public void UpdateAxisY(double cameraAngle)
    //{
    //	rtri += cameraAngle;
    //}

    //public void Update(int x, int y)
    //{
    //	double horizontalAngle = MouseController.GetHorizontalAngle(x);
    //	double verticalAngle = MouseController.GetVerticalAngle(y);

    //	UpdateAxisX(verticalAngle);
    //	UpdateAxisY(horizontalAngle);



    //	//GraphicsGL.GL.Rotate(utri, 1.0f, 0.0f, 0.0f);
    //	//GraphicsGL.GL.Rotate(rtri, 0.0f, 1.0f, 0.0f);
    //}

    //      public void LimitDistance()
    //{
    //          //Position[2] = Math.Max(Position[2], MinZoom);
    //          //         Position[2] = Math.Min(Position[2], MaxZoom);

    //          cameraPosition.Z = (float)Math.Max(cameraPosition.Z, MinZoom);
    //          cameraPosition.Z = (float)Math.Min(cameraPosition.Z, MaxZoom);
    //      }

    //public void Zoom(double value)
    //{
    //	//Position[2] += -value * ZoomSensitivity;
    //	cameraPosition.Z += (float)(-value * ZoomSensitivity);


    //      }

    //public void UpdateViewMatrix()
    //{
    //          view = Matrix4x4.CreateLookAt(new Vector3((float)utri, cameraPosition.Y, (float)rtri), cameraTarget, cameraUp);
    //      }

    //      public void SetViewByAxis(CoordinateAxis axis)
    //{
    //	if(axis == CoordinateAxis.X)
    //	{
    //              utri = 0.0f;
    //              rtri = 0.0f;

    //          }
    //	else if(axis == CoordinateAxis.Y)
    //	{
    //              utri = 90.0f;
    //              rtri = 0.0f;
    //          }
    //	else if(axis == CoordinateAxis.Z)
    //          {
    //              utri = 0.0f;
    //              rtri = 90.0f;
    //          }

    //          Rotate();
    //      }

    //  }


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
        }

        public void SetTarget(float x, float y, float z)
        {
            Target = new Vector3(x, y, z);
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
