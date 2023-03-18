using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace CadEditor
{
	public class SceneCollection
	{
		private TreeView sceneCollection;
		
		public SceneCollection(TreeView treeView, string collectionName)
		{
			sceneCollection = treeView;
			TreeNode mainNode = new TreeNode(collectionName);
			sceneCollection.Nodes.Add(mainNode);
		}

		public void Add(CustomCube cube)
		{
			sceneCollection.Nodes[0].Nodes.Add(new TreeNode(cube.Name));
		}
	}
}
