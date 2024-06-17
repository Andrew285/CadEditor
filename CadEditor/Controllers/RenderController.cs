using SharpGL;
using CadEditor.Models.Scene;
using System.Numerics;
using System.Collections.Generic;
using System;
using CadEditor.MeshObjects;
using CadEditor.Maths;
using System.Drawing;
using System.Windows.Forms;

namespace CadEditor.Controllers
{
    public class RenderController
    {
        private ApplicationController _controller;
        public Camera Camera;
        public GraphicsGL Graphics;
        private SceneGrid grid;
        private const int sceneGridSize = 20;
        private const int sceneGridDensity = sceneGridSize * 4;
        private const float sceneGridLineWidth = 0.1f;
        public static Ray selectingRay;
        private Scene _scene;
        public bool IsRayDrawable { get; set; } = false;
        public bool DrawFacets { get; set; }

        public void SetCamera(Camera camera)
        {
            Camera = camera;
        }

        public RenderController(ApplicationController appController, OpenGLControl control) 
        {
            _controller = appController;
            GraphicsGL.CreateInstance(control);
            GraphicsGL.GL.ClearColor(0.5f, 0.5f, 0.5f, 1.0f);
            grid = new SceneGrid(sceneGridDensity, sceneGridSize, sceneGridLineWidth);
            Camera = new Camera();
        }

        public void Initialize()
        {
            Point3D centerPoint = new Point3D(0, 0, 0);
            Camera.SetTarget(centerPoint.X, centerPoint.Y, centerPoint.Z);
        }

        public void Render(List<ISceneObject> objectsToDraw)
        {
            _scene = _controller.SceneController.Scene;
            GraphicsGL.GL.Clear(OpenGL.GL_COLOR_BUFFER_BIT | OpenGL.GL_DEPTH_BUFFER_BIT);

            // Set up the projection matrix
            GraphicsGL.SetUpProjectionMatrix();

            //// Set up the view matrix
            GraphicsGL.SetUpViewMatrix(Camera);

            //Rotate Camera
            Camera.Rotate();
            DrawCordinateAxes(new Point3D(0, 0, 0), 3.0, sceneGridSize);
            grid.Draw();

            //Draw all objects
            foreach (var obj in objectsToDraw)
            {
                if (obj is ComplexCube)
                {
                    ((ComplexCube)obj).DrawFacets = DrawFacets;
                }
                else if (obj is ComplexStructure)
                {
                    ((ComplexStructure)obj).DrawFacets = DrawFacets;
                }

                if (obj is IRotateable)
                {
                    RotateObject(obj);
                    continue;
                }

                obj.Draw();
            }

            DrawSelectingRay(Camera.Position);
        }

        public void RotateObject(ISceneObject obj)
        {
            if (obj is IRotateable)
            {
                GraphicsGL.GL.MatrixMode(SharpGL.Enumerations.MatrixMode.Modelview);
                GraphicsGL.GL.PushMatrix();
                Point3D p = obj.GetCenterPoint();
                GraphicsGL.GL.Translate(p.X, p.Y, p.Z);
                GraphicsGL.GL.Rotate(((IRotateable)obj).yRotation, 1.0f, 0.0f, 0.0f);
                GraphicsGL.GL.Rotate(((IRotateable)obj).xRotation, 0.0f, 1.0f, 0.0f);
                GraphicsGL.GL.Translate(-p.X, -p.Y, -p.Z);
                (obj).Draw();
                GraphicsGL.GL.PopMatrix();
            }
        }

        public void DrawSelectingRay(Vector3 cameraPosition)
        {
            if (selectingRay != null && IsRayDrawable)
            {
                GraphicsGL.GL.LineWidth(2f);
                GraphicsGL.GL.Begin(OpenGL.GL_LINES);

                GraphicsGL.GL.Color(1f, 0f, 0f, 0f);
                GraphicsGL.GL.Vertex(selectingRay.Origin[0], selectingRay.Origin[1], selectingRay.Origin[2]);
                GraphicsGL.GL.Vertex(selectingRay.Origin[0] + selectingRay.Direction[0] * cameraPosition.Z,
                selectingRay.Origin[1] + selectingRay.Direction[1] * cameraPosition.Z,
                selectingRay.Origin[2] + selectingRay.Direction[2] * cameraPosition.Z);

                GraphicsGL.GL.End();
                GraphicsGL.GL.Flush();
            }
        }

        public void DrawCordinateAxes(Point3D v, double lineWidth, double axisLength)
        {
            GraphicsGL.GL.LineWidth((float)lineWidth);
            GraphicsGL.GL.Begin(OpenGL.GL_LINES);

            GraphicsGL.GL.Color(1f, 0, 0, 0);
            GraphicsGL.GL.Vertex(-axisLength + v.X, v.Y, v.Z);
            GraphicsGL.GL.Vertex(axisLength + v.X, v.Y, v.Z);


            GraphicsGL.GL.Color(0, 1f, 0, 0);
            GraphicsGL.GL.Vertex(v.X, -axisLength + v.Y, v.Z);
            GraphicsGL.GL.Vertex(v.X, axisLength + v.Y, v.Z);

            GraphicsGL.GL.Color(0, 0, 1f, 0);
            GraphicsGL.GL.Vertex(v.X, v.Y, -axisLength + v.Z);
            GraphicsGL.GL.Vertex(v.X, v.Y, axisLength + v.Z);

            GraphicsGL.GL.End();
            GraphicsGL.GL.Flush();
        }

        public void SetCameraTarget(ISceneObject obj)
        {
            Point3D centerPoint = obj.GetCenterPoint();
            Camera.SetTarget(centerPoint.X, centerPoint.Y, centerPoint.Z);
        }

        public void UpdateRotation(double horizontalAngle, double verticalAngle, double rollAngle, bool isMiddleBtnPressed)
        {
            if (isMiddleBtnPressed)
            {
                if (_scene.IsObjectRotate && _scene.SelectedObject != null && _scene.SelectedObject.IsSelected && _scene.SelectedObject is IRotateable)
                {
                    UpdateRotation((IRotateable)_scene.SelectedObject, horizontalAngle, verticalAngle);
                }
                else
                {
                    //_controller.UpdateRotation(Camera, x, y);
                    Camera.UpdateRotation(horizontalAngle, verticalAngle, rollAngle);
                }
            }
        }
        public void UpdateRotation(IRotateable rotateable, double horizontalAngle, double verticalAngle)
        {
            float xDelta = (float)horizontalAngle;
            float yDelta = (float)verticalAngle;

            rotateable.xRotation += xDelta * 1f;
            rotateable.yRotation += yDelta * 1f;

            float rotationAngleX = TransformMesh.DegreesToRadians(rotateable.xRotation);
            float rotationAngleY = TransformMesh.DegreesToRadians(rotateable.yRotation);
            float rotationAngleZ = TransformMesh.DegreesToRadians(0);
            RotateVertices(rotationAngleX, rotationAngleY, rotationAngleZ, rotateable);

            GraphicsGL.Control.Invalidate();
        }

        void RotateVertices(float angleX, float angleY, float angleZ, IRotateable obj)
        {
            float cosX = (float)Math.Cos(angleX);
            float sinX = (float)Math.Sin(angleX);
            Matrix4x4 rotationMatrixX = new Matrix4x4(
                 1, 0, 0, 0,
                 0, cosX, -sinX, 0,
                 0, sinX, cosX, 0,
                 0, 0, 0, 1
            );

            float cosY = (float)Math.Cos(angleY);
            float sinY = (float)Math.Sin(angleY);
            Matrix4x4 rotationMatrixY = new Matrix4x4(
                 cosY, 0, sinY, 0,
                 0, 1, 0, 0,
                 -sinY, 0, cosY, 0,
                 0, 0, 0, 1
            );

            float cosZ = (float)Math.Cos(angleZ);
            float sinZ = (float)Math.Sin(angleZ);
            Matrix4x4 rotationMatrixZ = new Matrix4x4(
                cosZ, -sinZ, 0, 0,
                sinZ, cosZ, 0, 0,
                0, 0, 1, 0,
                0, 0, 0, 1
            );


            Matrix4x4 combinedRotation = rotationMatrixZ * rotationMatrixY * rotationMatrixX;

            if (obj is ComplexStructure)
            {
                ComplexStructure complexStructure = (ComplexStructure)obj;
                for (int i = 0; i < complexStructure.GetCubes().Count; i++)
                {
                    for (int j = 0; j < complexStructure.GetCubes()[i].Mesh.Vertices.Count; j++)
                    {
                        Vector3 vertex = new Vector3((float)complexStructure.GetCubes()[i].Mesh.Vertices[j].X,
                                                     (float)complexStructure.GetCubes()[i].Mesh.Vertices[j].Y,
                                                     (float)complexStructure.GetCubes()[i].Mesh.Vertices[j].Z);
                        Vector3 result3 = Vector3.Transform(vertex, combinedRotation);
                        complexStructure.GetCubes()[i].Mesh.Vertices[j].X = result3.X;
                        complexStructure.GetCubes()[i].Mesh.Vertices[j].Y = result3.Y;
                        complexStructure.GetCubes()[i].Mesh.Vertices[j].Z = result3.Z;
                    }
                }
            }
            else if (obj is MeshObject3D)
            {
                MeshObject3D meshObj = (MeshObject3D)obj;
                for (int j = 0; j < meshObj.Mesh.Vertices.Count; j++)
                {
                    Vector3 vertex = new Vector3((float)meshObj.Mesh.Vertices[j].X,
                                                    (float)meshObj.Mesh.Vertices[j].Y,
                                                    (float)meshObj.Mesh.Vertices[j].Z);
                    Vector3 result = Vector3.Transform(vertex, rotationMatrixZ);
                    meshObj.Mesh.Vertices[j].X = result.X;
                    meshObj.Mesh.Vertices[j].Y = result.Y;
                    meshObj.Mesh.Vertices[j].Z = result.Z;
                }
            }
        }

        public Bitmap CaptureScreen()
        {
            Control c = GraphicsGL.Control;
            Bitmap bmp = new System.Drawing.Bitmap(c.Width, c.Height);
            c.DrawToBitmap(bmp, c.ClientRectangle);
            return bmp;
        }
    }
}
