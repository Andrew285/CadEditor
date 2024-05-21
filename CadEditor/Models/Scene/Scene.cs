using CadEditor.Controllers;
using CadEditor.MeshObjects;
using CadEditor.Models.Scene;
using CadEditor.Models.Scene.MeshObjects;
using SharpGL;
using System;
using System.Collections.Generic;
using System.Numerics;
using static CadEditor.ComplexStructure;

namespace CadEditor
{
    public class Scene
    {
		private const int sceneGridSize = 20;
		private const int sceneGridDensity = sceneGridSize * 4;
		private const float sceneGridLineWidth = 0.1f;
        public List<ISceneObject> ObjectCollection { get; private set; }
		public bool IsRayDrawable { get; set; } = false;
		public static Ray selectingRay;
		private AxisSystem axisSystem;
		private SceneGrid grid;
		public static Vector MovingVector;
		public static CoordinateAxis ActiveMovingAxis;
		public ISceneObject SelectedObject { get; set; }
		public ISceneObject PreviousSelectedObject { get; set; }
		public bool DrawFacets { get; set; }
		public SceneMode SceneMode { get; set; } = SceneMode.VIEW;
		public bool IsObjectRotate { get; set; } = false;
		private SceneCollection SceneCollection;

		public Scene(SceneCollection sceneCollection)
		{
            ObjectCollection = new List<ISceneObject>();
            grid = new SceneGrid(sceneGridDensity, sceneGridSize, sceneGridLineWidth);
			this.SceneCollection = sceneCollection;
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
            //Draw Scene Grid
            DrawCordinateAxes(new Point3D(0, 0, 0), 3.0, sceneGridSize);
			grid.Draw();

			//Draw all objects
			foreach (var obj in ObjectCollection)
			{
				if (obj is ComplexCube)
                {
                    ((ComplexCube)obj).DrawFacets = this.DrawFacets;
                }

				if (obj is IRotateable)
				{
                    RotateObject(obj);
					continue;
				}

                obj.Draw();
			}
        }

        public void DrawSelectingRay(Vector3 cameraPosition)
        {
			if (selectingRay != null && IsRayDrawable)
			{
                GraphicsGL.GL.LineWidth(2f);
                GraphicsGL.GL.Begin(OpenGL.GL_LINES);

                GraphicsGL.GL.Color(1f, 0f, 0f, 0f);
                GraphicsGL.GL.Vertex(selectingRay.Origin[0], selectingRay.Origin[1], selectingRay.Origin[2]);
                GraphicsGL.GL.Vertex(selectingRay.Origin[0] + selectingRay.Direction[0] * cameraPosition.Z,
                selectingRay.Origin[1] + selectingRay.Direction[1] * cameraPosition.Z,
                selectingRay.Origin[2] + selectingRay.Direction[2] * cameraPosition.Z);

                GraphicsGL.GL.End();
                GraphicsGL.GL.Flush();
            }
        }

        public void DrawCordinateAxes(Point3D v, double lineWidth, double axisLength)
        {
            GraphicsGL.GL.LineWidth((float)lineWidth);
            GraphicsGL.GL.Begin(OpenGL.GL_LINES);

            GraphicsGL.GL.Color(1f, 0, 0, 0);
            GraphicsGL.GL.Vertex(-axisLength + v.X, v.Y, v.Z);
            GraphicsGL.GL.Vertex(axisLength + v.X, v.Y, v.Z);


            GraphicsGL.GL.Color(0, 1f, 0, 0);
            GraphicsGL.GL.Vertex(v.X, -axisLength + v.Y, v.Z);
            GraphicsGL.GL.Vertex(v.X, axisLength + v.Y, v.Z);

            GraphicsGL.GL.Color(0, 0, 1f, 0);
            GraphicsGL.GL.Vertex(v.X, v.Y, -axisLength + v.Z);
            GraphicsGL.GL.Vertex(v.X, v.Y, axisLength + v.Z);

            GraphicsGL.GL.End();
            GraphicsGL.GL.Flush();
        }

		#endregion

		#region --- Selection ---

		public ISceneObject Select()
		{
			PreviousSelectedObject = SelectedObject;
            ISceneObject realSelectedObject = null;

			double minDistance = 0;
            foreach (ISceneObject obj in ObjectCollection)
			{
                (ISceneObject, double) result = obj.CheckSelected();
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
                    DeleteSelectingCoordAxes();

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
                DeleteSelectingCoordAxes();
            }
			return SelectedObject;
        }

		public ISceneObject GetObjectByName(string name)
		{
			foreach (ISceneObject obj in ObjectCollection)
			{
				if (obj is IUniqueable && ((IUniqueable)obj).Name == name)
				{
					return obj;
				}
			}
			return null;
		}

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

        public void SetObjectByName(string name, ISceneObject objToSet)
		{
			ISceneObject obj = GetObjectByName(name);
            int index = ObjectCollection.IndexOf(obj);
			if (index != -1)
			{
				ObjectCollection[index] = objToSet;
			}
        }

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
		
		public bool CreateAxes(ISceneObject obj)
		{
			if (axisSystem != null)
			{
				ObjectCollection.Remove(axisSystem);
            }

            axisSystem = new AxisSystem(obj.GetCenterPoint(), selectingRay);
            ObjectCollection.Insert(0, axisSystem);
            return true;
        }

        public bool DeleteSelectingCoordAxes()
        {
            ObjectCollection.Remove(axisSystem);
			return true;
        }

        #endregion

        #region --- Manipulation ---

        public void DeleteCompletely(ISceneObject obj)
		{
			ObjectCollection.Remove(obj);
			DeleteSelectingCoordAxes();
        }

        public void AddObject(ISceneObject obj)
		{
			ObjectCollection.Add(obj);
		}

		public void MoveCoordinateAxes(Vector vector)
		{
			axisSystem.Move(vector);
		}

		public void Update()
		{
			foreach(ISceneObject cube in ObjectCollection)
			{
				cube.Deselect();
			}

			if(axisSystem != null)
			{
				DeleteSelectingCoordAxes();
			}
		}

		public void UpdateObjectRotation(IRotateable rotateable, int x, int y)
		{
            float xDelta = (float)MouseController.GetHorizontalAngle(x);
            float yDelta = (float)MouseController.GetVerticalAngle(y);

            rotateable.xRotation += xDelta * 1f;
            rotateable.yRotation += yDelta * 1f;

            GraphicsGL.Control.Invalidate();
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

		public void Remove(ISceneObject obj)
		{
			ObjectCollection.Remove(obj);
		}
        
		public bool Contains(ISceneObject obj)
		{
			return ObjectCollection.Contains(obj);
		}

		public void Add(ISceneObject obj)
		{
			ObjectCollection.Add(obj);
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
					SceneCollection.AddCube((ComplexCube)obj);
				}
				else if (obj is ComplexStructure)
				{
					SceneCollection.AddComplexStructure((ComplexStructure)obj);
				}
            }
			NameController.GetValuesOf(ObjectCollection);
		}

		#endregion
	}

	public enum SceneMode { VIEW, EDIT};
}
