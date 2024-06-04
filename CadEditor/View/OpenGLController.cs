using CadEditor.Controllers;
using SharpGL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace CadEditor.View
{
    public class OpenGLController
    {
        private OpenGL _gl;
        private OpenGLControl _glControl;
        private ApplicationController _applicationController;
        public MouseController MouseController { get; private set; }


        public OpenGLController(OpenGLControl control, ApplicationController applicationController) 
        {
            GraphicsGL.CreateInstance(control);
            _gl = GraphicsGL.GL;
            _glControl = control;
            _applicationController = applicationController;
            MouseController = new MouseController();

            _glControl.OpenGLInitialized += new System.EventHandler(this.openGLControl1_OpenGLInitialized_1);
            _glControl.OpenGLDraw += new SharpGL.RenderEventHandler(this.openGLControl1_OpenGLDraw_1);
            _glControl.MouseDown += new System.Windows.Forms.MouseEventHandler(this.openGLControl1_MouseDown);
            _glControl.MouseMove += new System.Windows.Forms.MouseEventHandler(this.openGLControl1_MouseMove);
            _glControl.MouseUp += new System.Windows.Forms.MouseEventHandler(this.openGLControl1_MouseUp);
            _glControl.MouseWheel += new System.Windows.Forms.MouseEventHandler(this.openGLControl_MouseWheel);
        }

        private void openGLControl1_OpenGLInitialized_1(object sender, EventArgs e)
        {


        }

        private void openGLControl1_OpenGLDraw_1(object sender, RenderEventArgs args)
        {
            Scene scene = _applicationController.SceneController.Scene;
            _applicationController.RenderController.Render(scene.ObjectCollection);
        }

        private void openGLControl1_MouseDown(object sender, MouseEventArgs e)
        {
            MouseController.UpdateMousePosition(e.X, e.Y);

            if (e.Button == MouseButtons.Left)
            {
                _applicationController.HandleMouseDown(e.X, e.Y);
                _applicationController.HandleLeftMouseButtonDown();
            }
            else if (e.Button == MouseButtons.Right)
            {
                _applicationController.HandleMouseDown(e.X, e.Y);
                bool result = _applicationController.HandleRightMouseButtonDown();
                if (result)
                {
                    _applicationController.InitializeContextMenu(e.X, e.Y);
                }
            }
            else if (e.Button == MouseButtons.Middle)
            {
                MouseController.IsMiddleButtonPressed = true;
            }
        }

        private void openGLControl1_MouseMove(object sender, MouseEventArgs e)
        {
            double horizontalAngle = MouseController.GetHorizontalAngle(e.X);
            double verticalAngle = MouseController.GetVerticalAngle(e.Y);
            _applicationController.HandleMouseMove(horizontalAngle, verticalAngle, MouseController.IsMiddleButtonPressed);
            MouseController.UpdateMousePosition(e.X, e.Y);
        }

        private void openGLControl1_MouseUp(object sender, MouseEventArgs e)
        {
            MouseController.IsMiddleButtonPressed = false;
            _applicationController.HandleMouseUp();
        }

        private void openGLControl_MouseWheel(object sender, MouseEventArgs e)
        {
            _applicationController.RenderController.Camera.ZoomBy(e.Delta);
        }
    }
}
