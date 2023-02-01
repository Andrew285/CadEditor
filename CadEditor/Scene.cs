using SharpGL;
using SharpGL.SceneGraph;
using SharpGL.SceneGraph.Core;
using SharpGL.SceneGraph.Effects;
using SharpGL.SceneGraph.Lighting;
using SharpGL.SceneGraph.Primitives;
using System.Windows.Forms;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;

namespace CadEditor
{
    public class Scene
    {
        private ArcBallEffect arcBallEffect = new ArcBallEffect();
        SceneControl sceneControl1;
        System.Windows.Forms.TreeView treeView1;
        PropertyGrid propertyGrid1;

        public Scene(SceneControl _sceneControl1, System.Windows.Forms.TreeView _treeView1, PropertyGrid _propertyGrid1)
        {
            sceneControl1 = _sceneControl1;
            treeView1 = _treeView1;
            propertyGrid1 = _propertyGrid1;
            sceneControl1.MouseDown += new MouseEventHandler(Form1_MouseDown);
            sceneControl1.MouseMove += new MouseEventHandler(Form1_MouseMove);
            sceneControl1.MouseUp += new MouseEventHandler(sceneControl1_MouseUp);

            //  Add some design-time primitives.
            sceneControl1.Scene.SceneContainer.AddChild(new
                SharpGL.SceneGraph.Primitives.Grid());
            sceneControl1.Scene.SceneContainer.AddChild(new
                SharpGL.SceneGraph.Primitives.Axies());

            //  Create a light.
            Light light = new Light()
            {
                On = true,
                Position = new Vertex(3, 10, 3),
                GLCode = OpenGL.GL_LIGHT0
            };

            //  Add the light.
            sceneControl1.Scene.SceneContainer.AddChild(light);

           

            //  Add the root element to the tree.
            AddElementToTree(sceneControl1.Scene.SceneContainer, treeView1.Nodes);
        }

        public void AddElement(SceneElement element)
        {
            //  Create a sphere.
            element.AddEffect(arcBallEffect);

            //  Add it.
            sceneControl1.Scene.SceneContainer.AddChild(element);
        }

        /// <summary>
        /// Adds the element to tree.
        /// </summary>
        /// <param name="sceneElement">The scene element.</param>
        /// <param name="nodes">The nodes.</param>
        private void AddElementToTree(SceneElement sceneElement, TreeNodeCollection nodes)
        {
            //  Add the element.
            TreeNode newNode = new TreeNode()
            {
                Text = sceneElement.Name,
                Tag = sceneElement
            };
            nodes.Add(newNode);

            //  Add each child.
            foreach (var element in sceneElement.Children)
                AddElementToTree(element, newNode.Nodes);
        }

        /// <summary>
        /// Handles the AfterSelect event of the treeView1 control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.Forms.TreeViewEventArgs"/> instance containing the event data.</param>
        private void treeView1_AfterSelect(object sender, TreeViewEventArgs e)
        {
            SelectedSceneElement = e.Node.Tag as SceneElement;
        }

        /// <summary>
        /// Called when [selected scene element changed].
        /// </summary>
        private void OnSelectedSceneElementChanged()
        {
            propertyGrid1.SelectedObject = SelectedSceneElement;
        }

        /// <summary>
        /// The selected scene element.
        /// </summary>
        private SceneElement selectedSceneElement = null;

        /// <summary>
        /// Gets or sets the selected scene element.
        /// </summary>
        /// <value>
        /// The selected scene element.
        /// </value>
        public SceneElement SelectedSceneElement
        {
            get { return selectedSceneElement; }
            set
            {
                selectedSceneElement = value;
                OnSelectedSceneElementChanged();
            }
        }

        private void Form1_MouseMove(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                arcBallEffect.ArcBall.MouseMove(e.X, e.Y);
            }

        }

        private void Form1_MouseDown(object sender, MouseEventArgs e)
        {
            arcBallEffect.ArcBall.SetBounds(sceneControl1.Width, sceneControl1.Height);
            arcBallEffect.ArcBall.MouseDown(e.X, e.Y);
        }


        private void sceneControl1_MouseUp(object sender, MouseEventArgs e)
        {
            arcBallEffect.ArcBall.MouseUp(e.X, e.Y);

        }
    }
}
