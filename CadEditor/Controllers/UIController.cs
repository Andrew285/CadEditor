using System.Windows.Forms;
using CadEditor.Properties;
using CadEditor.Settings;
using CadEditor.View.Forms;

namespace CadEditor.Controllers
{
    public class UIController
    {
        private ApplicationController _applicationController;
        private ContextMenuStrip contextMenuStrip;
        private static ToolStripMenuItem selectItem = new ToolStripMenuItem("Select Object");
        private static ToolStripMenuItem deselectItem = new ToolStripMenuItem("Deselect Object");
        private static ToolStripMenuItem deleteItem = new ToolStripMenuItem("Delete Object");
        private static ToolStripMenuItem attachItem = new ToolStripMenuItem("Attach Object");
        private static ToolStripMenuItem detachItem = new ToolStripMenuItem("Detach Object");
        private static ToolStripMenuItem setTargetItem = new ToolStripMenuItem("Set as Target");
        private static ToolStripMenuItem notSetTargetItem = new ToolStripMenuItem("Deselect Target");
        private static ToolStripMenuItem divideItem = new ToolStripMenuItem("Divide");
        private static ToolStripMenuItem uniteItem = new ToolStripMenuItem("Unite");
        private static ToolStripMenuItem setTarget = new ToolStripMenuItem("Set Camera Target");
        public Form1 MainForm { get; set; }

        public UIController(ApplicationController appController, Form1 mainForm)
        {
            _applicationController = appController;
            Scene scene = _applicationController.SceneController.Scene;
            MainForm = mainForm;
            contextMenuStrip = GraphicsGL.Control.ContextMenuStrip;
            contextMenuStrip = new ContextMenuStrip();
            contextMenuStrip.Items.AddRange(new ToolStripMenuItem[]
            {
                selectItem, deselectItem, deleteItem, divideItem, uniteItem, attachItem, detachItem, setTargetItem, notSetTargetItem, setTarget
            });

            selectItem.Click += (sender, e) => _applicationController.SelectElement();
            selectItem.Image = Resources.select_object;

            deselectItem.Click += (sender, e) => _applicationController.DeselectElement();
            deselectItem.Image = Resources.deselect_object;

            deleteItem.Click += (sender, e) => _applicationController.DeleteElement();
            deleteItem.Image = Resources.remove;

            attachItem.Click += (sender, e) => _applicationController.MakeAttachableElement();
            detachItem.Click += (sender, e) => _applicationController.MakeNonAttachableElement();
            setTargetItem.Click += (sender, e) => _applicationController.MakeTargetableElement();
            notSetTargetItem.Click += (sender, e) => _applicationController.MakeNonAttachableElement();
            divideItem.Click += (sender, e) => _applicationController.DivideElement();
            uniteItem.Click += (sender, e) => _applicationController.UniteElement();
            setTarget.Click += (sender, e) => _applicationController.RenderController.SetCameraTarget(scene.SelectedObject);
        }

        public void Initialize()
        {
            //Load Settings
            MainForm.BackColor = ThemeSettings.MainThemeColor;
            MainForm.GetMenuStrip().BackColor = ThemeSettings.MenuStripBackColor;
        }

        public void InitContextMenu(int x, int y)
        {
            Scene scene = _applicationController.SceneController.Scene;
            ComplexCube attachingCube = _applicationController.AttachingController.GetAttachingCube();
            ComplexCube targetCube = _applicationController.AttachingController.GetTargetCube();

            if (scene.SelectedObject != null)
            {
                setTarget.Visible = true;

                if (scene.SelectedObject is IDivideable && !(scene.SelectedObject as IDivideable).IsDivided)
                {
                    divideItem.Visible = true;
                    uniteItem.Visible = false;
                }
                else
                {
                    divideItem.Visible = false;
                    uniteItem.Visible = true;
                }


                if (scene.SelectedObject == attachingCube)
                {
                    detachItem.Visible = true;
                    attachItem.Visible = false;
                    setTargetItem.Visible = false;
                    notSetTargetItem.Visible = false;
                }
                else if (attachingCube == null && targetCube == null)
                {
                    attachItem.Visible = true;
                    setTargetItem.Visible = true;
                    detachItem.Visible = false;
                    notSetTargetItem.Visible = false;
                }
                else if (scene.SelectedObject == targetCube)
                {
                    notSetTargetItem.Visible = true;
                    setTargetItem.Visible = false;
                    attachItem.Visible = false;
                    detachItem.Visible = false;
                }
                else if (scene.SelectedObject != targetCube &&
                         targetCube == null)
                {
                    setTargetItem.Visible = true;
                    notSetTargetItem.Visible = false;
                    attachItem.Visible = false;
                    detachItem.Visible = false;
                }
                else if (scene.SelectedObject != attachingCube &&
                         attachingCube == null)
                {
                    attachItem.Visible = true;
                    detachItem.Visible = false;
                    notSetTargetItem.Visible = false;
                    setTargetItem.Visible = false;
                }
            }

            contextMenuStrip.Show(GraphicsGL.Control, new System.Drawing.Point(x, y));
        }

        public void CreateSettingsForm()
        {
            SettingsForm settingsForm = new SettingsForm(_applicationController);
            settingsForm.ShowDialog();
        }

        public DividingCubeForm CreateDividingForm()
        {
            DividingCubeForm form = new DividingCubeForm();
            form.TopMost = true;
            form.FormBorderStyle = FormBorderStyle.FixedDialog;
            form.StartPosition = FormStartPosition.CenterScreen;
            form.MinimizeBox = false;
            form.MaximizeBox = false;

            return form;
        }
    }
}
