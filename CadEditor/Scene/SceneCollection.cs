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

		public void AddCube(ComplexCube cube)
		{
			TreeNode newCube = new TreeNode(cube.Name);
			sceneCollection.Nodes[0].Nodes.Add(newCube);
			int indexOfNode = sceneCollection.Nodes[0].Nodes.IndexOf(newCube);

			sceneCollection.Nodes[0].Nodes[indexOfNode].Nodes.Add(new TreeNode("Mesh"));
			sceneCollection.Nodes[0].Nodes[indexOfNode].Nodes[0].Nodes.Add(new TreeNode("Vertices"));
			for(int i = 1; i <= cube.Mesh.Vertices.Count; i++)
			{
				sceneCollection.Nodes[0].Nodes[indexOfNode].Nodes[0].Nodes[0].Nodes.Add(new TreeNode(String.Format("P_{0}", i-1)));
			}

			sceneCollection.Update();
		}

		public void RemoveCube(ComplexCube cube)
		{
			for(int i = 0; i < sceneCollection.Nodes.Count; i++)
			{
				if (sceneCollection.Nodes[i].Nodes.Count > 0)
				{
					for(int j = 0; j < sceneCollection.Nodes[i].Nodes.Count; j++)
					{
						if (sceneCollection.Nodes[i].Nodes[j].Text == cube.Name)
						{
							sceneCollection.Nodes[i].Nodes.RemoveAt(j);
						}
					}
				}
				else
				{
					if (sceneCollection.Nodes[i].Text == cube.Name)
					{
						sceneCollection.Nodes.RemoveAt(i);
					}
				}
			}
			sceneCollection.Update();
		}

		public TreeNode CheckSelectedNode()
		{
			foreach(TreeNode treeNode in sceneCollection.Nodes)
			{
				if(treeNode.Nodes.Count > 1)
				{
					CheckSelectedNode();
				}
				else
				{
					if (treeNode.Nodes[0].IsSelected)
					{
						return treeNode.Nodes[0];
					}
				}

				if (treeNode.IsSelected)
				{
					return treeNode;
				}
			}

			return null;
		}

		public List<Object3D> FindObjectByTreeNode(TreeNode node, Scene scene)
		{
			string nodeString = node.Text;
			if (nodeString == "Collection")
			{
				List<Object3D> objects = new List<Object3D>();
				foreach (ComplexCube c in scene.DrawableCubes)
				{
					objects.Add(c);
				}
				return objects;
			}
			else if (nodeString.Contains("Cube_"))
			{
				int indexOfCube = Int32.Parse(nodeString.Substring(5));
				return new List<Object3D>() { scene.DrawableCubes[indexOfCube-1] };
			}
			else if (nodeString.Contains("P_"))
			{
				TreeNode cubeNode = node.Parent.Parent.Parent;
				int indexOfPoint = Int32.Parse(nodeString.Substring(2));
				int indexOfCube = sceneCollection.Nodes[0].Nodes.IndexOf(cubeNode);
				return new List<Object3D>() { scene.DrawableCubes[indexOfCube].Mesh.Vertices[indexOfPoint] };
			}

			return null;
		}
	}
}
