using CadEditor.Controllers;
using CadEditor.Tools;
using SharpGL;
using System;
using System.Windows.Forms;

namespace CadEditor.View
{
    public class GLControl: BaseControl
    {
        private OpenGL _gl;
        private OpenGLControl _glControl;
        private ApplicationController _applicationController;
        private MouseController _mouseController;
        private Form1 _mainForm;
        private GroupBox _glGroupBox;

        public GLControl(Form1 mainForm, OpenGLControl control, ApplicationController applicationController) 
        {
            GraphicsGL.CreateInstance(control);
            _gl = GraphicsGL.GL;
            _glControl = control;
            _mainForm = mainForm;
            _glGroupBox = new GroupBox();
            _applicationController = applicationController;
            _mouseController = new MouseController();

            _glControl.OpenGLInitialized += new EventHandler(openGLControl1_OpenGLInitialized_1);
            _glControl.OpenGLDraw += new RenderEventHandler(openGLControl1_OpenGLDraw_1);
            _glControl.MouseDown += new MouseEventHandler(openGLControl1_MouseDown);
            _glControl.MouseMove += new MouseEventHandler(openGLControl1_MouseMove);
            _glControl.MouseUp += new MouseEventHandler(openGLControl1_MouseUp);
            _glControl.MouseWheel += new MouseEventHandler(openGLControl_MouseWheel);
            _glControl.Resize += new EventHandler(Resize);
        }

        public void Initialize()
        {
            // 
            // openGLControl1
            // 
            _glControl.DrawFPS = true;
            _glControl.Location = new System.Drawing.Point(18, 73);
            _glControl.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            _glControl.Name = "openGLControl1";
            _glControl.OpenGLVersion = SharpGL.Version.OpenGLVersion.OpenGL2_1;
            _glControl.RenderContextType = SharpGL.RenderContextType.DIBSection;
            _glControl.RenderTrigger = SharpGL.RenderTrigger.TimerBased;
            int newWidth = (int)UITools.ScaleBy(_mainForm.Width, 70);
            int newHeight = (int)UITools.ScaleBy(_mainForm.Height, 70);
            _glControl.Size = new System.Drawing.Size(newWidth, newHeight);
            _glControl.TabIndex = 0;

            // 
            // groupBox1
            // 
            _glGroupBox.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            _glGroupBox.Location = new System.Drawing.Point(12, 54);
            _glGroupBox.Name = "groupBox1";
            _glGroupBox.Size = new System.Drawing.Size(826, 834);
            _glGroupBox.TabIndex = 5;
            _glGroupBox.TabStop = false;
            _glGroupBox.Text = "Scene";

            _mainForm.Controls.Add(this._glGroupBox);
        }

        private void openGLControl1_OpenGLInitialized_1(object sender, EventArgs e)
        {
        }

        private void openGLControl1_OpenGLDraw_1(object sender, RenderEventArgs args)
        {
            Scene scene = _applicationController.SceneController.Scene;
            _applicationController.RenderController.Render(scene.ObjectCollection);
        }

        public void Resize(object sender, EventArgs e)
        {
            int newWidth = _mainForm.Width - SceneCollectionControl.collectionGroupBoxWidth - 3 * UITools.GAP;
            int newHeight = (int)UITools.ScaleBy(_mainForm.Height, UITools.WORKSPACE_SCALE_HEIGHT);
            _glControl.Size = new System.Drawing.Size(newWidth, newHeight);
            _glGroupBox.Size = new System.Drawing.Size(newWidth + UITools.GAP, newHeight + UITools.GAP);
        }

        private void openGLControl1_MouseDown(object sender, MouseEventArgs e)
        {
            _mouseController.UpdateMousePosition(e.X, e.Y);

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
                _mouseController.IsMiddleButtonPressed = true;
            }
        }

        private void openGLControl1_MouseMove(object sender, MouseEventArgs e)
        {
            double horizontalAngle = _mouseController.GetHorizontalAngle(e.X);
            double verticalAngle = _mouseController.GetVerticalAngle(e.Y);
            _applicationController.HandleMouseMove(horizontalAngle, verticalAngle, _mouseController.IsMiddleButtonPressed);
            _mouseController.UpdateMousePosition(e.X, e.Y);
        }

        private void openGLControl1_MouseUp(object sender, MouseEventArgs e)
        {
            _mouseController.IsMiddleButtonPressed = false;
            _applicationController.HandleMouseUp();
        }

        private void openGLControl_MouseWheel(object sender, MouseEventArgs e)
        {
            _applicationController.HandleMouseWheel(e.Delta);
        }
    }
}
