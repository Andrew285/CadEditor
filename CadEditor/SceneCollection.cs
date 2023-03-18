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
			sceneCollection.ExpandAll();
		}

		public void Add(CustomCube cube)
		{
			sceneCollection.Nodes[0].Nodes.Add(new TreeNode(cube.CubeName));
			sceneCollection.Update();
		}

		public void Remove(CustomCube cube)
		{
			for(int i = 0; i < sceneCollection.Nodes.Count; i++)
			{
				if (sceneCollection.Nodes[i].Nodes.Count > 0)
				{
					for(int j = 0; j < sceneCollection.Nodes[i].Nodes.Count; j++)
					{
						if (sceneCollection.Nodes[i].Nodes[j].Text == cube.CubeName)
						{
							sceneCollection.Nodes[i].Nodes.RemoveAt(j);
						}
					}
				}
				else
				{
					if (sceneCollection.Nodes[i].Text == cube.CubeName)
					{
						sceneCollection.Nodes.RemoveAt(i);
					}
				}
			}
			sceneCollection.Update();
		}
	}
}
