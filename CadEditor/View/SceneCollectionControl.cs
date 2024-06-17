using CadEditor.Controllers;
using CadEditor.Tools;
using CadEditor.Tools.Localization;
using System;
using System.Windows.Forms;

namespace CadEditor.View
{
    public class SceneCollectionControl: BaseControl
    {
        private Form1 _mainForm;
        private ApplicationController _applicationController;
        private TreeView treeView1;
        private GroupBox _collectionGroupBox;
        private SceneCollection _sceneCollection;
        private string collectionRootNodeTitle = "Collection";
        public static int collectionGroupBoxWidth = 242;
        public static int collectionGroupBoxHeight = 313;
        public static int collectionGroupBoxStartX = 854;
        public static int collectionGroupBoxStartY = 54;

        public SceneCollectionControl(Form1 mainForm, ApplicationController applicationController) 
        {
            _mainForm = mainForm;
            _applicationController = applicationController;
            treeView1 = new TreeView();
            _collectionGroupBox = new GroupBox();
            _sceneCollection = new SceneCollection(treeView1, collectionRootNodeTitle);
        }

        public SceneCollection GetSceneCollection()
        {
            return _sceneCollection;
        }

        public void Initialize()
        {
            // 
            // treeView1
            // 
            this.treeView1.Location = new System.Drawing.Point(5, 18);
            this.treeView1.Name = "treeView1";
            this.treeView1.Size = new System.Drawing.Size(225, 290);
            this.treeView1.TabIndex = 2;
            this.treeView1.NodeMouseClick += new System.Windows.Forms.TreeNodeMouseClickEventHandler(this.treeView1_NodeMouseClick);

            // 
            // groupBox2
            // 
            this._collectionGroupBox.Controls.Add(this.treeView1);
            this._collectionGroupBox.Location = new System.Drawing.Point(collectionGroupBoxStartX, collectionGroupBoxStartY);
            this._collectionGroupBox.Margin = new System.Windows.Forms.Padding(2);
            this._collectionGroupBox.Name = "groupBox2";
            this._collectionGroupBox.Padding = new System.Windows.Forms.Padding(2);
            this._collectionGroupBox.Size = new System.Drawing.Size(collectionGroupBoxWidth, collectionGroupBoxHeight);
            this._collectionGroupBox.TabIndex = 8;
            this._collectionGroupBox.TabStop = false;
            this._collectionGroupBox.Resize += new EventHandler(Resize);

            _collectionGroupBox.Dock = DockStyle.Bottom;
            _collectionGroupBox.Anchor = AnchorStyles.Bottom & AnchorStyles.Left;
            _mainForm.Controls.Add(_collectionGroupBox);

            SetTextToControls();
        }

        public void Resize(object sender, EventArgs e)
        {
            int newX = _mainForm.Width - collectionGroupBoxWidth - UITools.GAP;
            int newY = UITools.MENU_TOOLS_HEIGHT;
            _collectionGroupBox.Location = new System.Drawing.Point(newX, newY);
        }

        private void treeView1_NodeMouseClick(object sender, TreeNodeMouseClickEventArgs e)
        {
            //        scene.DeselectAll();

            //        TreeNode selectedTreeNode = sceneCollection.GetSelectedNode();
            //        if(selectedTreeNode != null)
            //        {
            //            ISceneObject obj = sceneCollection.GetObjectByNode(selectedTreeNode, scene.ObjectCollection);
            //            if(obj != null)
            //            {
            //                scene.SelectedObject = obj;
            //                obj.Select();
            //                //scene.InitSelectingCoordAxes(nodeObjects[0], 2.8f, 1.0);
            //                AxisSystem axisSystem = new AxisSystem(obj.GetCenterPoint(), RenderController.selectingRay);
            //                scene.ObjectCollection.Insert(0, axisSystem);
            //}
            //        }
        }

        public void SetTextToControls()
        {
            _collectionGroupBox.Text = _applicationController.Localization.GetTranslationOf(Strings.scene_collection_group_box_title);
        }
    }
}
