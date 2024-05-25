using SharpGL.SceneGraph.Cameras;
using SharpGL.SceneGraph;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SharpGL;
using System.Windows.Forms;
using CadEditor.Models.Scene;
using SharpGL.SceneGraph.Primitives;
using System.Numerics;

namespace CadEditor.Controllers
{
    public class RenderController
    {
        private ApplicationController _controller;
        public Camera Camera;
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

        public RenderController(ApplicationController appController) 
        {
            _controller = appController;
            //_scene = _controller.Scene;
            Camera = new Camera();
            grid = new SceneGrid(sceneGridDensity, sceneGridSize, sceneGridLineWidth);
        }

        public void Initialize()
        {
            Point3D centerPoint = new Point3D(0, 0, 0);
            Camera.SetTarget(centerPoint.X, centerPoint.Y, centerPoint.Z);
        }

        public void Render()
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
            _scene.Draw();
            DrawSelectingRay(Camera.Position);
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

        public void UpdateRotation(int x, int y)
        {
            if (_controller.MouseController.IsMiddleButtonPressed && _scene.IsObjectRotate && _scene.SelectedObject != null && _scene.SelectedObject.IsSelected && _scene.SelectedObject is IRotateable)
            {
                _controller.UpdateRotation((IRotateable)_scene.SelectedObject, x, y);
            }
            else if (_controller.MouseController.IsMiddleButtonPressed)
            {
                _controller.UpdateRotation(Camera, x, y);
            }
        }
    }
}
