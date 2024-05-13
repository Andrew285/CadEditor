using CadEditor.Controllers;
using CadEditor.MeshObjects;
using CadEditor.Models.Scene;
using CadEditor.Models.Scene.MeshObjects;
using SharpGL;
using SharpGL.SceneGraph.Raytracing;
using System;
using System.Collections.Generic;
using System.Numerics;

namespace CadEditor
{
    public class Scene
    {
		private static Scene instance;

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
		public ISceneObject previousSelectedObject;
		public ISceneObject SelectedObject { get; set; }
		private ISceneObject previousRealSelectedObject;
		//public AxisCube SelectedAxisCube { get; set; }

		public bool DrawFacets { get; set; }

		public SceneMode SceneMode { get; set; } = SceneMode.VIEW;

		public bool IsObjectRotate { get; set; } = false;

		public static Scene GetInstance()
		{
			if (instance == null)
			{
				return new Scene();
			}

			return instance;
		}

		public Scene()
		{
            ObjectCollection = new List<ISceneObject>();
            instance = this;
            grid = new SceneGrid(sceneGridDensity, sceneGridSize, sceneGridLineWidth);
        }

        #region --- Initializing ---

        public ISceneObject GetPreviousSelectedObject()
		{
			foreach (ISceneObject obj in ObjectCollection)
			{
				if ((obj).IsEqual(previousSelectedObject))
				{
					return obj;
				}
			}

			return null;
		}
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
            ISceneObject realSelectedObject = null;

            //previousSelectedObject = SelectedObject != null ? (ISceneObject)SelectedObject.Clone() : null;

			double minDistance = 0;
            foreach (ISceneObject obj in ObjectCollection)
			{
				//obj.Deselect();
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

                        if (realSelectedObject.ParentObject != null)
                        {
                            SelectedObject = SelectedObject.ParentObject;

							if (SelectedObject != null)
							{
								if (previousRealSelectedObject == null ||
									!previousRealSelectedObject.IsEqual(realSelectedObject))
								{
                                    ComplexStructure structure = ComplexStructureController.GetInstance().GetStructureOf((ComplexCube)SelectedObject);

                                    if (structure != null)
                                    {
                                        SelectedObject = structure;
                                    }
                                }
                            }
                        }
                    }

                    //SelectedObject.Select();
                    //SelectedObject.IsSelected = true;
					
                    //axisSystem = new AxisSystem(SelectedObject.GetCenterPoint(), selectingRay);
                    //ObjectCollection.Insert(0, axisSystem);
                }
				else
				{
					//SelectedAxisCube = (AxisCube)realSelectedObject;
					SelectedObject = (AxisCube)realSelectedObject;

					//SelectedAxisCube.Select();
					//SelectedAxisCube.IsSelected = true;
				}
            }
			else
			{
				SelectedObject = null;
                DeleteSelectingCoordAxes();
            }

            previousRealSelectedObject = realSelectedObject != null ? (ISceneObject)realSelectedObject.Clone() : null;

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

		public void SetObjectByName(string name, ISceneObject objToSet)
		{
			ISceneObject obj = GetObjectByName(name);
            int index = ObjectCollection.IndexOf(obj);
			if (index != -1)
			{
				ObjectCollection[index] = objToSet;
			}
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
			List<ISceneObject> cubes = new List<ISceneObject>();
			List<ISceneObject> tempCubes = new List<ISceneObject>();
			List<Point3D> currentPoints = new List<Point3D>();
			List<Line> currentLines = new List<Line>();
			List<Plane> currentPlanes = new List<Plane>();
			List<Point3D> currentOuterVertices = new List<Point3D>();
			ComplexStructure currentComplexStructure = null;
			string name = "";
			AttachingController attachingController = new AttachingController();

			for (int i = 0; i < importStrings.Length; i++)
			{
				if (importStrings[i].Contains("End of Structure"))
				{
                    cubes.Add(currentComplexStructure);
					ComplexStructureController.GetInstance().AddStructure(currentComplexStructure);
                    currentComplexStructure = null;
                }
				else if (importStrings[i].Contains("Attaching"))
				{
                    if (currentLines.Count > 0 && currentPoints.Count > 0)
                    {
                        Mesh currentMesh = new Mesh();
                        currentMesh.Vertices = currentPoints;
                        currentMesh.Edges = currentLines;
                        currentMesh.Facets = currentPlanes;
                        ComplexCube newCube = new ComplexCube(currentMesh, currentOuterVertices.ToArray());
                        newCube.DrawFacets = true;
                        newCube.Name = name;
                        if (currentComplexStructure != null)
                        {
                            currentComplexStructure.AddCube(newCube);
                        }
                        else
                        {
                            cubes.Add(newCube);
                        }

                        currentPoints = new List<Point3D>();
                        currentLines = new List<Line>();
                        currentPlanes = new List<Plane>();
						currentOuterVertices = new List<Point3D>();
                    }

                    //Attach cubes
                    string[] splitted = importStrings[i].Split(' ');
					ComplexCube targetCube1 = (ComplexCube)currentComplexStructure.GetObjectByName(splitted[1]);
                    attachingController.AddTargetCube(targetCube1);
                    attachingController.AddTargetFacet(targetCube1.Mesh.Facets[Int32.Parse(splitted[2])]);

                    ComplexCube attachingCube1 = (ComplexCube)currentComplexStructure.GetObjectByName(splitted[3]);
                    attachingController.AddAttachingCube(attachingCube1);
                    attachingController.AddAttachingFacet(attachingCube1.Mesh.Facets[Int32.Parse(splitted[4])]);

                    Plane targetFacet = attachingController.GetTargetFacet();
                    Plane attachingFacet = attachingController.GetAttachingFacet();

                    ComplexCube targetCube = ((ComplexCube)attachingController.GetTargetObject());
                    ComplexCube attachingCube = ((ComplexCube)attachingController.GetAttachingObject());

                    //find closest point to attaching cube
                    (int, Vector) closestDistance = attachingController.GetClosestDistanceToAttach();
                    int indexOfMinPoint = closestDistance.Item1;
                    Vector minVector = closestDistance.Item2;

                    //move to target cube
                    Vector pointToPoint = attachingFacet.Points[indexOfMinPoint] - targetFacet.Points[indexOfMinPoint];
                    Vector centerToPoint = minVector - pointToPoint;
                    Point3D resultCenterPoint = new Point3D(targetFacet.Points[indexOfMinPoint] + new Point3D(centerToPoint));
                    Vector resultVector = attachingFacet.GetCenterPoint() - resultCenterPoint;
                    attachingController.GetAttachingObject().Move(resultVector * (-1));

                    //attach facet
                    attachingController.AttachFacets();
                    attachingController.UpdateObjects();

                    currentComplexStructure.AttachingDetailsList.Add(new ComplexStructure.AttachingDetails(

						targetCube,
						targetFacet,
						attachingCube,
						attachingFacet
					));

                    attachingController = new AttachingController();
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
					if (currentLines.Count > 0 && currentPoints.Count > 0)
					{
						Mesh currentMesh = new Mesh();
						currentMesh.Vertices = currentPoints;
						currentMesh.Edges = currentLines;
						currentMesh.Facets = currentPlanes;
						ComplexCube newCube = new ComplexCube(currentMesh, currentOuterVertices.ToArray());
                        newCube.DrawFacets = true;
                        newCube.Name = name;

						if (currentComplexStructure != null)
						{
							currentComplexStructure.AddCube(newCube);
						}
						else
						{
                            cubes.Add(newCube);
                        }
                        currentPoints = new List<Point3D>();
						currentLines = new List<Line>();
						currentPlanes = new List<Plane>();
                        currentOuterVertices = new List<Point3D>();
                    }

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
                }
				
			}

			if (currentLines.Count > 0 && currentPoints.Count > 0)
			{
				Mesh currentMesh = new Mesh();
				currentMesh.Vertices = currentPoints;
				currentMesh.Edges = currentLines;
				currentMesh.Facets = currentPlanes;
                ComplexCube newCube = new ComplexCube(currentMesh, currentOuterVertices.ToArray());
                newCube.DrawFacets = true;
				newCube.Name = name;
				if (currentComplexStructure != null)
				{
					currentComplexStructure.AddCube(newCube);
				}
				else
				{
					cubes.Add(newCube);
				}
            }

			//add new object to SceneCollection
			ObjectCollection = cubes;
			//SceneCollection.ClearAll();
			foreach (ISceneObject cube in cubes)
			{
				if (cube is ComplexCube)
				{
                    //SceneCollection.AddCube((ComplexCube)cube);
                }
				else if (cube is ComplexStructure)
				{
                    //SceneCollection.AddComplexStructure((ComplexStructure)cube);
                }
            }
			NameController.GetValuesOf(ObjectCollection);
		}

		#endregion
	}

	public enum SceneMode { VIEW, EDIT};
}
