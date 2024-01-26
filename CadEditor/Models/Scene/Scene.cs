using CadEditor.Controllers;
using CadEditor.MeshObjects;
using CadEditor.Models.Scene.MeshObjects;
using MathNet.Numerics.LinearAlgebra.Factorization;
using Microsoft.SqlServer.Server;
using SharpGL;
using SharpGL.SceneGraph;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace CadEditor
{
    public class Scene
    {
		private const int SCENE_GRID_SIZE = 20;
		private const int SCENE_GRID_DENSITY = SCENE_GRID_SIZE * 4;
		private const float SCENE_GRID_LINE_WIDTH = 0.1f;
        public Camera Camera { get; private set; }
        public SceneCollection SceneCollection { get; private set; }
        public List<ISceneObject> ObjectCollection { get; private set; }
		public AxisSystem AttachingAxisSystem { get; private set; }
		//public Ray selectingRay;
		private AxisSystem axisSystem;
		private SceneGrid grid;

		public static Vector MovingVector;
		public static CoordinateAxis ActiveMovingAxis;
		public ISceneObject previousSelectedObject;
		public ISceneObject SelectedObject { get; set; }
		private ISceneObject previousRealSelectedObject;
		private ISceneObject realSelectedObject;
		public AxisCube SelectedAxisCube { get; set; }

		public bool DrawFacets { get; set; }

		public SceneMode SceneMode { get; set; } = SceneMode.VIEW;

		public AttachingController AttachingController { get; private set; }
		public ComplexStructureController StructureController { get; private set; }

		public Scene(Camera _camera, SceneCollection _sceneCollection)
        {
			Camera = _camera;
			SceneCollection = _sceneCollection;
			ObjectCollection = new List<ISceneObject>();
			AttachingController = new AttachingController();
			StructureController = ComplexStructureController.GetInstance();
        }

		#region --- Initializing ---

		public void InitializeObjects()
		{
            ComplexCube cube = new ComplexCube(new Point3D(6, 0, 5), new Vector(1, 1, 1), NameController.GetNextCubeName());
            ComplexCube cube2 = new ComplexCube(new Point3D(5, 5, 8), new Vector(1, 1, 1), NameController.GetNextCubeName());
            ObjectCollection.Add(cube);
            ObjectCollection.Add(cube2);
            SceneCollection.AddCube(cube);
            SceneCollection.AddCube(cube2);

            Camera.Target = new Point3D(0, 0, 0);
			grid = new SceneGrid(SCENE_GRID_DENSITY, SCENE_GRID_SIZE, SCENE_GRID_LINE_WIDTH);
        }

		public void InitializeAttachingAxes(MeshObject3D obj)
		{
			AttachingAxisSystem = new AxisSystem();
            AttachingAxisSystem.AxisLength = 5.0f;
            for (int i = 0; i < obj.Mesh.Facets.Count; i++)
            {
                if (!obj.Mesh.attachedFacets.Contains(i))
                {
					AttachingAxisSystem.CreateAxis(obj.Mesh.Facets[i].AxisType, obj.Mesh.Facets[i].GetCenterPoint());
                }
            }
			ObjectCollection.Add(AttachingAxisSystem);
        }

		//public void InitializeSelectingAxes(ISceneObject obj)
		//{
		//	AttachingAxisSystem = new AxisSystem();
		//	AttachingAxisSystem.AxisLength = 3.0f;

		//	MeshObject3D meshObject = null;

		//	if (obj is ComplexStructure)
		//	{
		//		meshObject = ((ComplexStructure)obj).GetCubes()[0];
		//	}
		//	for (int i = 0; i < meshObject.Mesh.Facets.Count; i++)
		//	{
		//		if (!obj.Mesh.attachedFacets.Contains(i))
		//		{
		//			AttachingAxisSystem.CreateAxis(obj.Mesh.Facets[i].AxisType, obj.Mesh.Facets[i].GetCenterPoint());
		//		}
		//	}
		//	ObjectCollection.Add(AttachingAxisSystem);
		//}

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
			GraphicsGL.GL.Clear(OpenGL.GL_COLOR_BUFFER_BIT | OpenGL.GL_DEPTH_BUFFER_BIT);

			// Set up the projection matrix
			GraphicsGL.SetUpProjectionMatrix();

			// Set up the view matrix
			GraphicsGL.SetUpViewMatrix(Camera);

			//Rotate Camera
			Camera.Rotate();

			//Draw Scene Grid
			DrawCordinateAxes(new Point3D(0, 0, 0), 3.0, SCENE_GRID_SIZE);
			grid.Draw();

   //         //Draw ray when user clicks with left button
   //         if (selectingRay != null && selectingRay.Direction != null)
			//{
			//	DrawLine(selectingRay.Origin, selectingRay.Direction);
			//}

            //Draw all objects
            foreach (var obj in ObjectCollection)
			{
				if (obj is ComplexCube)
                {
					((ComplexCube)obj).DrawFacets = this.DrawFacets;
                }

                obj.Draw();
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

		public void DeleteSelectingCoordAxes()
		{
			ObjectCollection.Remove(axisSystem);
		}

		#endregion

		#region --- Selection ---

		public void Select()
		{
			//selectingRay = new Ray();
			//selectingRay.Origin = ray.Origin;

            ISceneObject realSelectedObject = null;

            previousSelectedObject = SelectedObject != null ? (ISceneObject)SelectedObject.Clone() : null;


            foreach (ISceneObject obj in ObjectCollection)
			{
				obj.Deselect();
                realSelectedObject = obj.CheckSelected();
				if(realSelectedObject != null)
				{
					break;
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
                                    ComplexStructure structure = StructureController.GetStructureOf((ComplexCube)SelectedObject);

                                    if (structure != null)
                                    {
                                        SelectedObject = structure;
                                    }
                                }
                            }
                        }
                    }

                    SelectedObject.Select();
                    SelectedObject.IsSelected = true;
					
                    axisSystem = new AxisSystem(SelectedObject);
                    ObjectCollection.Insert(0, axisSystem);
                }
				else
				{
					SelectedAxisCube = (AxisCube)realSelectedObject;
					SelectedAxisCube.Select();
					SelectedAxisCube.IsSelected = true;
				}
            }
			else
			{
				SelectedObject = null;
                DeleteSelectingCoordAxes();
            }

            previousRealSelectedObject = realSelectedObject != null ? (ISceneObject)realSelectedObject.Clone() : null;
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

		public void DeleteCompletely(ISceneObject cube)
		{
			ObjectCollection.Remove(cube);
			SceneCollection.RemoveCube((IUniqueable)cube);
			DeleteSelectingCoordAxes();
        }

        public void AddCube()
		{
			ComplexCube cube = new ComplexCube(new Point3D(0, 0, 0), new Vector(1, 1, 1), NameController.GetNextCubeName());
			ObjectCollection.Add(cube);
			SceneCollection.AddCube(cube);
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

		public void SetAttachingObjectToAxis(Axis axis)
		{
			Point3D pointToMove = axis.P2;
			MeshObject3D attachingObject = AttachingController.GetAttachingObject();

			//Create AttachingFacetsPair
			foreach(Plane facet in AttachingController.GetTargetObject().Mesh.Facets)
			{
				if(facet.GetCenterPoint() == axis.P1)
				{
					facet.IsAttached = true;
					AttachingController.AddTargetFacet(facet);
					break;
				}
			}

			CoordinateAxisType oppositeType = AxisSystem.GetOppositeAxisType(AttachingController.GetTargetFacet().AxisType);
            foreach (Plane facet in AttachingController.GetAttachingObject().Mesh.Facets)
            {
                if (facet.AxisType == oppositeType)
                {
                    facet.IsAttached = true;
					AttachingController.AddAttachingFacet(facet);
                    break;
                }
            }

            Vector distanceVector = attachingObject.GetCenterPoint() - pointToMove;
			attachingObject.Move(distanceVector*(-1));
		}

		public void AttachCubes()
		{
			if(AttachingController.IsFacetsInitialized())
			{
				Plane targetFacet = AttachingController.GetTargetFacet();
				Plane attachingFacet = AttachingController.GetAttachingFacet();

                ComplexCube targetCube = ((ComplexCube)AttachingController.GetTargetObject());
                ComplexCube attachingCube = ((ComplexCube)AttachingController.GetAttachingObject());

				//find closest point to attaching cube
				(int, Vector) closestDistance = AttachingController.GetClosestDistanceToAttach();
				int indexOfMinPoint = closestDistance.Item1;
				Vector minVector = closestDistance.Item2;

				//move to target cube
				Vector pointToPoint = attachingFacet.Points[indexOfMinPoint] - targetFacet.Points[indexOfMinPoint];
                Vector centerToPoint = minVector - pointToPoint;
				Point3D resultCenterPoint = new Point3D(targetFacet.Points[indexOfMinPoint] + new Point3D(centerToPoint));
				Vector resultVector = attachingFacet.GetCenterPoint() - resultCenterPoint;
				AttachingController.GetAttachingObject().Move(resultVector * (-1));

				//attach facet
				AttachingController.AttachFacets();
				AttachingController.UpdateObjects();
                AttachingController.Clear();

                ObjectCollection.Remove(AttachingAxisSystem);

                //Creating complex structure or adding cube to it
                ComplexStructure structure = StructureController.AddCubes(attachingCube, targetCube);
				structure.AttachingDetailsList.Add(new ComplexStructure.AttachingDetails(

					targetCube,
					targetFacet,
					attachingCube,
					attachingFacet
				));
				ObjectCollection.Remove(targetCube);
				ObjectCollection.Remove(attachingCube);
				
				if (structure != null && !ObjectCollection.Contains(structure))
				{
					ObjectCollection.Add(structure);
                    SceneCollection.AddComplexStructure(structure);
                }
				else
				{
                    SceneCollection.RemoveCube(attachingCube);
                    SceneCollection.AddCube(attachingCube, structure);
                }
			}
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
			ComplexStructure currentComplexStructure = null;
			string name = "";

			for (int i = 0; i < importStrings.Length; i++)
			{
				if (importStrings[i].Contains("End of Structure"))
				{
                    cubes.Add(currentComplexStructure);
                    StructureController.AddStructure(currentComplexStructure);
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
                        ComplexCube newCube = new ComplexCube(currentMesh);
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


                    }

                    //Attach cubes
                    string[] splitted = importStrings[i].Split(' ');
					ComplexCube targetCube1 = (ComplexCube)currentComplexStructure.GetObjectByName(splitted[1]);
                    AttachingController.AddTargetCube(targetCube1);
					AttachingController.AddTargetFacet(targetCube1.Mesh.Facets[Int32.Parse(splitted[2])]);

                    ComplexCube attachingCube1 = (ComplexCube)currentComplexStructure.GetObjectByName(splitted[3]);
                    AttachingController.AddAttachingCube(attachingCube1);
                    AttachingController.AddAttachingFacet(attachingCube1.Mesh.Facets[Int32.Parse(splitted[4])]);

                    Plane targetFacet = AttachingController.GetTargetFacet();
                    Plane attachingFacet = AttachingController.GetAttachingFacet();

                    ComplexCube targetCube = ((ComplexCube)AttachingController.GetTargetObject());
                    ComplexCube attachingCube = ((ComplexCube)AttachingController.GetAttachingObject());

                    //find closest point to attaching cube
                    (int, Vector) closestDistance = AttachingController.GetClosestDistanceToAttach();
                    int indexOfMinPoint = closestDistance.Item1;
                    Vector minVector = closestDistance.Item2;

                    //move to target cube
                    Vector pointToPoint = attachingFacet.Points[indexOfMinPoint] - targetFacet.Points[indexOfMinPoint];
                    Vector centerToPoint = minVector - pointToPoint;
                    Point3D resultCenterPoint = new Point3D(targetFacet.Points[indexOfMinPoint] + new Point3D(centerToPoint));
                    Vector resultVector = attachingFacet.GetCenterPoint() - resultCenterPoint;
                    AttachingController.GetAttachingObject().Move(resultVector * (-1));

                    //attach facet
                    AttachingController.AttachFacets();
                    AttachingController.UpdateObjects();

                    currentComplexStructure.AttachingDetailsList.Add(new ComplexStructure.AttachingDetails(

						targetCube,
						targetFacet,
						attachingCube,
						attachingFacet
					));

                    AttachingController = new AttachingController();
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
						ComplexCube newCube = new ComplexCube(currentMesh);
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
				
			}

			if (currentLines.Count > 0 && currentPoints.Count > 0)
			{
				Mesh currentMesh = new Mesh();
				currentMesh.Vertices = currentPoints;
				currentMesh.Edges = currentLines;
				currentMesh.Facets = currentPlanes;
				ComplexCube newCube = new ComplexCube(currentMesh);
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
            }


			ObjectCollection = cubes;
			foreach (ISceneObject cube in cubes)
			{
				if (cube is ComplexCube)
				{
                    SceneCollection.AddCube((ComplexCube)cube);
                }
				else if (cube is ComplexStructure)
				{
                    SceneCollection.AddComplexStructure((ComplexStructure)cube);
                }
            }
			NameController.GetValuesOf(ObjectCollection);
		}

		#endregion
	}

	public enum SceneMode { VIEW, EDIT};
}
