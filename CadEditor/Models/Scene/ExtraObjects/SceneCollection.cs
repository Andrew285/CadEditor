using CadEditor.MeshObjects;
using CadEditor.Models.Scene.MeshObjects;
using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace CadEditor
{
	public class SceneCollection
	{
		private TreeView sceneCollection;
		private TreeNode currentNode;
		
		public SceneCollection(TreeView treeView, string collectionName)
		{
			sceneCollection = treeView;
			TreeNode mainNode = new TreeNode(collectionName);
			sceneCollection.Nodes.Add(mainNode);
			sceneCollection.ExpandAll();
            currentNode = sceneCollection.Nodes[0];
        }

        public void AddCube(ComplexCube cube)
		{
			TreeNode newCube = new TreeNode(cube.Name);
            currentNode.Nodes.Add(newCube);
			int indexOfNode = currentNode.Nodes.IndexOf(newCube);

            currentNode.Nodes[indexOfNode].Nodes.Add(new TreeNode("Mesh"));
            currentNode.Nodes[indexOfNode].Nodes[0].Nodes.Add(new TreeNode("Vertices"));
			for(int i = 1; i <= cube.Mesh.Vertices.Count; i++)
			{
				currentNode.Nodes[indexOfNode].Nodes[0].Nodes[0].Nodes.Add(new TreeNode(String.Format("P_{0}", i-1)));
			}

			sceneCollection.Update();
		}

		public bool AddCube(ComplexCube cube, IUniqueable structure)
		{
			currentNode = GetNodeOf(structure);
			if (currentNode != null)
			{
                AddCube(cube);
                currentNode = sceneCollection.Nodes[0];
				return true;
            }

			return false;
        }

		public bool AddComplexStructure(ComplexStructure structure)
		{
            TreeNode newStructure = new TreeNode(structure.Name);
            currentNode.Nodes.Add(newStructure);

            foreach (ComplexCube cube in structure.GetCubes())
			{
				RemoveCube(cube);
				if (!AddCube(cube, structure))
				{
					return false;
				}
			}

			return true;
		}

		public void RemoveCube(IUniqueable obj)
		{
			TreeNode nodeToRemove = GetNodeOf(obj);
			if (nodeToRemove != null)
			{
				currentNode.Nodes.Remove(nodeToRemove);
			}
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

		public List<ISceneObject> FindObjectByTreeNode(TreeNode node, Scene scene)
		{
			string nodeString = node.Text;
			if (nodeString == "Collection")
			{
				List<ISceneObject> objects = new List<ISceneObject>();
				foreach (ComplexCube c in scene.ObjectCollection)
				{
					objects.Add(c);
				}
				return objects;
			}
			else if (nodeString.Contains("Cube_"))
			{
				int indexOfCube = Int32.Parse(nodeString.Substring(5));
				return new List<ISceneObject>() { scene.ObjectCollection[indexOfCube-1] };
			}
			else if (nodeString.Contains("P_"))
			{
				TreeNode cubeNode = node.Parent.Parent.Parent;
				int indexOfPoint = Int32.Parse(nodeString.Substring(2));
				int indexOfCube = sceneCollection.Nodes[0].Nodes.IndexOf(cubeNode);
				return new List<ISceneObject>() { ((MeshObject3D)scene.ObjectCollection[indexOfCube]).Mesh.Vertices[indexOfPoint] };
			}

			return null;
        }

		public TreeNode GetNodeOf(IUniqueable obj)
		{
            for (int i = 0; i < currentNode.Nodes.Count; i++)
            {
                if (currentNode.Nodes[i].Text == obj.Name)
                {
                    return currentNode.Nodes[i];
                }
            }

			return null;
        }

		public void Import(List<ISceneObject> collection)
		{

            foreach (ISceneObject obj in collection)
            {
                if (obj is ComplexCube)
                {
					AddCube((ComplexCube)obj);
                }
                else if (obj is ComplexStructure)
                {
                    AddComplexStructure((ComplexStructure)obj);
                }
            }
        }

		public TreeNode GetSelectedNode()
		{
			return sceneCollection.SelectedNode;
		}

		public ISceneObject GetObjectByNode(TreeNode node, List<ISceneObject> objects)
		{
			if (sceneCollection.Nodes[0] == node)
			{
				foreach (ISceneObject obj in objects)
				{
					obj.Select();
				}

				return null;
			}

			foreach (ISceneObject obj in objects)
			{
				if (obj is IUniqueable && ((IUniqueable)obj).Name == node.Text)
				{
					return obj;
				}
			}

			return null;
		}

		public void ClearAll()
		{
			sceneCollection.Nodes[0].Nodes.Clear();
        }
	}
}
