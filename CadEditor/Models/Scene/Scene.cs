using CadEditor.Controllers;
using CadEditor.MeshObjects;
using CadEditor.Models.Scene;
using CadEditor.Models.Scene.MeshObjects;
using System;
using System.Collections.Generic;

namespace CadEditor
{
    public class Scene
    {

        public List<ISceneObject> ObjectCollection { get; private set; }
		public ISceneObject SelectedObject { get; set; }
		public ISceneObject PreviousSelectedObject { get; set; }
		public bool DrawFacets { get; set; }
		public SceneMode SceneMode { get; set; } = SceneMode.VIEW;
		public bool IsObjectRotate { get; set; } = false;

		public Scene()
		{
            ObjectCollection = new List<ISceneObject>();
        }

        #region --- Initializing ---

        //      public ISceneObject GetPreviousSelectedObject()
        //{
        //	foreach (ISceneObject obj in ObjectCollection)
        //	{
        //		if ((obj).IsEqual(previousSelectedObject))
        //		{
        //			return obj;
        //		}
        //	}

        //	return null;
        //}
        #endregion

        #region --- Drawing ---

        public void Draw()
        {

            //Draw all objects
            foreach (var obj in ObjectCollection)
            {
                if (obj is ComplexCube)
                {
                    ((ComplexCube)obj).DrawFacets = this.DrawFacets;
                }
                else if (obj is ComplexStructure)
                {
                    ((ComplexStructure)obj).DrawFacets = this.DrawFacets;
                }

                if (obj is IRotateable)
                {
                    RotateObject(obj);
                    continue;
                }

                obj.Draw();
            }
        }

        #endregion

        #region --- Selection ---

        public ISceneObject Select(int x, int y)
		{
			PreviousSelectedObject = SelectedObject;
            ISceneObject realSelectedObject = null;

			double minDistance = 0;
            foreach (ISceneObject obj in ObjectCollection)
			{
                (ISceneObject, double) result = obj.CheckSelected(x, y);
				if(result.Item1 != null)
				{
					if (minDistance == 0 || realSelectedObject == null)
					{
                        realSelectedObject = result.Item1;
                        minDistance = result.Item2;
                    }
					else
					{
						if (result.Item2 < minDistance)
						{
							minDistance = result.Item2;
							realSelectedObject = result.Item1;
						}
					}
				}
			}

			if(realSelectedObject != null)
			{
				if(!(realSelectedObject is AxisCube))
				{
                    //DeleteSelectingCoordAxes();

                    SelectedObject = realSelectedObject;
                    if (SceneMode == SceneMode.VIEW)
                    {
                        if (SelectedObject.ParentObject != null)
                        {
                            SelectedObject = SelectedObject.ParentObject;
                            ComplexStructure structure = GetComplexStructureByCube((ComplexCube)SelectedObject);
                            if (structure != null && PreviousSelectedObject != structure && PreviousSelectedObject != SelectedObject)
							{
								SelectedObject = structure;
                            }
                        }
                    }
                }
				else
				{
					SelectedObject = (AxisCube)realSelectedObject;
				}
            }
			else
			{
				SelectedObject = null;
                //DeleteSelectingCoordAxes();
            }
			return SelectedObject;
        }

		//public ISceneObject GetObjectByName(string name)
		//{
		//	foreach (ISceneObject obj in ObjectCollection)
		//	{
		//		if (obj is IUniqueable && ((IUniqueable)obj).Name == name)
		//		{
		//			return obj;
		//		}
		//	}
		//	return null;
		//}

        public ISceneObject GetObjectByName(string name, List<ISceneObject> objects)
        {
            foreach (ISceneObject obj in objects)
            {
                if (obj is IUniqueable && ((IUniqueable)obj).Name == name)
                {
                    return obj;
                }
            }
            return null;
        }

  //      public void SetObjectByName(string name, ISceneObject objToSet)
		//{
		//	ISceneObject obj = GetObjectByName(name);
  //          int index = ObjectCollection.IndexOf(obj);
		//	if (index != -1)
		//	{
		//		ObjectCollection[index] = objToSet;
		//	}
  //      }

		public List<ComplexStructure> GetAllComplexStructures()
		{
			List<ComplexStructure> resultComplexStructures = new List<ComplexStructure>();
            for (int i = 0; i < ObjectCollection.Count; i++)
            {
                if (ObjectCollection[i] is ComplexStructure)
                {
					resultComplexStructures.Add(ObjectCollection[i] as ComplexStructure);
                }
            }
			return resultComplexStructures;
        }

		public ComplexStructure GetComplexStructureByCube(ComplexCube cube)
		{
			List<ComplexStructure> structures = GetAllComplexStructures();
			for (int i = 0; i < structures.Count; i++)
			{
				if (structures[i].Contains(cube))
				{
					return structures[i];
				}
			}
			return null;
		}

		public bool Select(ISceneObject obj)
		{
			obj.Select();
			return true;
        }

        public bool Deselect(ISceneObject obj)
		{
			obj.Deselect();
			return true;
        }

        public void DeselectAll()
		{
			foreach (ISceneObject c in ObjectCollection)
			{
				c.Deselect();
			}
		}
		
        #endregion

        #region --- Manipulation ---

        public void AddObject(ISceneObject obj)
		{
			ObjectCollection.Add(obj);
		}

		public void RotateObject(ISceneObject obj)
		{
			if (obj is IRotateable)
			{
                GraphicsGL.GL.MatrixMode(SharpGL.Enumerations.MatrixMode.Modelview);
                GraphicsGL.GL.PushMatrix();
                Point3D p = obj.GetCenterPoint();
                GraphicsGL.GL.Translate(p.X, p.Y, p.Z);
                GraphicsGL.GL.Rotate(((IRotateable)obj).yRotation, 1.0f, 0.0f, 0.0f);
                GraphicsGL.GL.Rotate(((IRotateable)obj).xRotation, 0.0f, 1.0f, 0.0f);
                GraphicsGL.GL.Translate(-p.X, -p.Y, -p.Z);
                (obj).Draw();
                GraphicsGL.GL.PopMatrix();
            }
        }

		public bool Remove(ISceneObject obj)
		{
			return ObjectCollection.Remove(obj);
		}
        
		public bool Contains(ISceneObject obj)
		{
			return ObjectCollection.Contains(obj);
		}

		public void Add(ISceneObject obj)
		{
			ObjectCollection.Add(obj);
		}

		public void Insert(int index, ISceneObject obj)
		{
			ObjectCollection.Insert(index, obj);
		}

		#endregion

		#region --- Export/Import ---

		public string Export()
		{
			string exportString = "";

			foreach (IExportable cube in ObjectCollection)
			{
				exportString += cube.Export();
			}

			return exportString;
		}

		public void Import(string[] importStrings)
		{
			List<ISceneObject> sceneObjects = new List<ISceneObject>();
			List<ISceneObject> structureCubes = new List<ISceneObject>();
			List<Point3D> currentPoints = new List<Point3D>();
			List<Line> currentLines = new List<Line>();
			List<Plane> currentPlanes = new List<Plane>();
			List<Point3D> currentOuterVertices = new List<Point3D>();
			ComplexStructure currentComplexStructure = null;
			ComplexCube currentComplexCube;
			string name = "";

			for (int i = 0; i < importStrings.Length; i++)
			{
				if (importStrings[i].Contains("End of Structure"))
				{
					structureCubes.Clear();
                    sceneObjects.Add(currentComplexStructure);
                    currentComplexStructure = null;
                }
				else if (importStrings[i].Contains("Connection"))
				{
                    string[] splitted = importStrings[i].Split(' ');
					ComplexCube targetCube = (ComplexCube)GetObjectByName(splitted[1], structureCubes);
					ComplexCube attachingCube = (ComplexCube)GetObjectByName(splitted[2], structureCubes);

					CoordinateAxisType targetAxisType = (CoordinateAxisType)Enum.Parse(typeof(CoordinateAxisType), splitted[3]);
					CoordinateAxisType attachingAxisType = (CoordinateAxisType)Enum.Parse(typeof(CoordinateAxisType), splitted[3]);

					currentComplexStructure.AttachCubes(targetCube, targetAxisType, attachingCube, attachingAxisType);
                }
                else if (importStrings[i].Contains("ComplexStructure"))
				{
					currentComplexStructure = new ComplexStructure();
				}
                else if (importStrings[i].Contains("Facet"))
                {
                    string trimmedString = importStrings[i].Substring("Facet".Length + 1);
                    string[] splitted = trimmedString.Split(' ');

                    List<Point3D> points = new List<Point3D>();
                    for (int j = 0; j < 8; j++)
                    {
                        Point3D p = currentPoints[Int32.Parse(splitted[j])];
                        points.Add(p);
                    }

                    Plane plane = new Plane(points);
                    plane.AxisType = (CoordinateAxisType)Enum.Parse(typeof(CoordinateAxisType), splitted[8]);
                    currentPlanes.Add(plane);
                }
                else if (importStrings[i].Contains("Cube"))
				{
					currentComplexCube = null;
                    currentPoints = new List<Point3D>();
					currentLines = new List<Line>();
					currentPlanes = new List<Plane>();
                    currentOuterVertices = new List<Point3D>();
                    name = importStrings[i];
				}
				else if (importStrings[i].Contains("("))
				{
					string trimmedString = importStrings[i].Substring(1, importStrings[i].Length - 2);
					string[] splited = trimmedString.Split(' ');
					double[] coords = new double[splited.Length];

					for (int j = 0; j < splited.Length; j++)
					{
						coords[j] = double.Parse(splited[j]);
					}

					currentPoints.Add(new Point3D(coords));
				}
				else if (importStrings[i].Contains("Edge"))
				{
					string trimmedString = importStrings[i].Substring("Edge".Length + 1);
					string[] splited = trimmedString.Split(' ');
					Point3D p1 = currentPoints[Int32.Parse(splited[0])];
					Point3D p2 = currentPoints[Int32.Parse(splited[1])];

					currentLines.Add(new Line(p1, p2));
				}
				else if (importStrings[i].Contains("OuterVertices"))
				{
                    string trimmedString = importStrings[i].Substring("OuterVertices".Length + 1);
                    string[] splitted = trimmedString.Split(' ');

					for (int j = 0; j < splitted.Length-1; j++)
					{
						currentOuterVertices.Add(currentPoints[Int32.Parse(splitted[j])]);
					}

                    Mesh currentMesh = new Mesh();
                    currentMesh.Vertices = currentPoints;
                    currentMesh.Edges = currentLines;
                    currentMesh.Facets = currentPlanes;
                    currentComplexCube = new ComplexCube(currentMesh, currentOuterVertices.ToArray());
                    currentComplexCube.Name = name;
					currentComplexCube.Size = new Vector(1, 1, 1);
                    structureCubes.Add(currentComplexCube);
                }
				
			}

			//add new object to SceneCollection
			//SceneCollection.ClearAll();
			ObjectCollection = sceneObjects;
			foreach (ISceneObject obj in ObjectCollection)
			{
				if (obj is ComplexCube)
				{
					//SceneCollection.AddCube((ComplexCube)obj);
				}
				else if (obj is ComplexStructure)
				{
					//SceneCollection.AddComplexStructure((ComplexStructure)obj);
				}
            }
			ModelNameProvider.GetInstance().GetValuesOf(ObjectCollection);
		}

		#endregion
	}

	public enum SceneMode { VIEW, EDIT};
}
