using CadEditor.Controllers;
using CadEditor.MeshObjects;
using CadEditor.Tools.Localization;
using CadEditor.View;
using SharpGL;
using System;
using System.Drawing;
using System.Windows.Forms;

namespace CadEditor
{
    public partial class Form1 : Form
    {
        private ApplicationController _applicationController;
        private MenuToolsControl _toolsMenuControl;
        private BaseControl[] mainControls;
        private static int KeyX_Clicks = 0;
        private static int KeyY_Clicks = 0;
        private static int KeyZ_Clicks = 0;

        public Form1()
        {
            InitializeComponent();
            KeyPreview = true;

            _applicationController = new ApplicationController(this, GetOpenGLControl());
            GLControl _openGLController = new GLControl(this, GetOpenGLControl(), _applicationController);
            _toolsMenuControl = new MenuToolsControl(this, _applicationController, GetOpenGLControl());
            SceneCollectionControl _sceneCollectionController = new SceneCollectionControl(this, _applicationController);
            PropertiesControl _propertiesController = new PropertiesControl(this, _applicationController);

            _applicationController.SetSceneCollection(_sceneCollectionController.GetSceneCollection());
            _applicationController.Initialize();

            mainControls = new BaseControl[] 
            {
                _openGLController,
                _toolsMenuControl,
                _sceneCollectionController,
                _propertiesController,
            };

            foreach (var control in mainControls)
            {
                control.Initialize();
            }
        }

        private void Form1_Resize(object sender, EventArgs e)
        {
            foreach (var control in mainControls)
            {
                control.Resize(sender, e);
            }
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            //this.TopMost = true;
            this.WindowState = FormWindowState.Maximized;
            this.Text = _applicationController.Localization.GetTranslationOf(Strings.form_name);

            UpdateForm();
        }

        public OpenGLControl GetOpenGLControl()
        {
            return openGLControl1;
        }

		private void Form1_KeyDown(object sender, KeyEventArgs e)
        {

            switch (e.KeyCode)
            {
                case Keys.R:
                    _applicationController.RotateSelectedObject();
                    break;

                case Keys.X:
                    KeyX_Clicks = _applicationController.SetObjectToAxis(CoordinateAxis.X, KeyX_Clicks);
                    break;

                case Keys.Y:
                    KeyY_Clicks = _applicationController.SetObjectToAxis(CoordinateAxis.Y, KeyY_Clicks);
                    break;

                case Keys.Z:
                    KeyZ_Clicks = _applicationController.SetObjectToAxis(CoordinateAxis.Z, KeyZ_Clicks);
                    break;

                case Keys.Space:
                    _applicationController.AttachElements();
                    break;


                case Keys.ControlKey:
                    _applicationController.SceneController.Scene.IsObjectRotate = true;
                    break;
            }
        }

        private void Form1_KeyUp(object sender, KeyEventArgs e)
        {
            switch (e.KeyCode)
            {
                case Keys.ControlKey:
                    _applicationController.SceneController.Scene.IsObjectRotate = false;
                    break;
            }
        }

        protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
        {
            bool baseResult = base.ProcessCmdKey(ref msg, keyData);
            if (keyData == Keys.Tab)
            {
                ComboBox sceneModeComboBox = _toolsMenuControl.GetSceneModeComboBox();
                int oppositeViewIndex = _applicationController.HandlePressTab(sceneModeComboBox.SelectedIndex);
                sceneModeComboBox.SelectedItem = sceneModeComboBox.Items[oppositeViewIndex];
                return true;
            }
            else if (keyData == (Keys.Control | Keys.Z))
            {
                _applicationController.HandlePressCtrlZ();
            }
            else if (keyData == (Keys.Control | Keys.Shift | Keys.Z))
            {
                _applicationController.HandlePressCtrlShiftZ();
            }
            else if (keyData == (Keys.Control | Keys.C))
            {
                _applicationController.CopySelectedElement();
            }
            else if (keyData == (Keys.Control | Keys.V))
            {
                _applicationController.PasteElement();
            }

            return baseResult;
        }



        //public void DeselectAndSaveCommand(Scene scene, ISceneObject obj)
        //{
        //    DeselectionCommand deselectionCommand = new DeselectionCommand(_applicationController, obj);
        //    deselectionCommand.Execute();
        //    _applicationController.CommandsHistory.Push(deselectionCommand);
        //}

        //public void SelectAndSaveCommand(Scene scene, ISceneObject obj)
        //{
        //    SelectionCommand selectionCommand = new SelectionCommand(_applicationController, obj);
        //    selectionCommand.Execute();
        //    _applicationController.CommandsHistory.Push(selectionCommand);
        //}


        public void UpdateFormColor(Color color)
        {
            this.BackColor = color;
        }

        public void UpdateMenuBackColor(Color color)
        {
            //this.menuStrip1.BackColor = color;
        }

        public void SettingsForm_LanguageChanged(object sender, EventArgs e)
        {
            //Set lang to other controls   
            foreach (var control in mainControls)
            {
                control.SetTextToControls();
            }
        }

        public void UpdateForm()
        {
            //Set lang to other controls   
            foreach (var control in mainControls)
            {
                control.SetTextToControls();
            }
        }
    }
}
