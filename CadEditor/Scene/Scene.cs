using CadEditor.Graphics;
using CadEditor.MeshObjects;
using Microsoft.SqlServer.Server;
using SharpGL;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;

namespace CadEditor
{
    public class Scene
    {
        public Camera Camera { get; private set; }
        public SceneCollection SceneCollection { get; private set; }
        public List<ISceneObject> ObjectCollection { get; private set; }
		public List<Plane> AttachingFacetsPair { get; private set; }
		public AxisSystem AttachingAxisSystem { get; private set; }
		//public Ray selectingRay;
		private AxisSystem axisSystem;
		private SceneGrid grid;

		public static Vector MovingVector;
		public static CoordinateAxis ActiveMovingAxis;
		public ISceneObject SelectedObject { get; set; }
		public AxisCube SelectedAxisCube { get; set; }

		public bool DrawFacets { get; set; }

		public SceneMode SceneMode { get; set; } = SceneMode.VIEW;

		public AttachingController AttachingController { get; private set; }

		public Scene(Camera _camera, SceneCollection _sceneCollection)
        {
			Camera = _camera;
			SceneCollection = _sceneCollection;
			ObjectCollection = new List<ISceneObject>();
			AttachingController = new AttachingController();
        }

		#region --- Initializing ---

		public void InitializeObjects()
		{
			//ComplexCube cube = new ComplexCube(new Point3D(0, 0, 0), new Vector(1, 1, 1), "Cube_1");
			ComplexCube cube = new ComplexCube(new Point3D(6, 0, 5), new Vector(1, 1, 1), "Cube_1");
			ComplexCube cube2 = new ComplexCube(new Point3D(5, 5, 8), new Vector(1, 1, 1), "Cube_2");
			ObjectCollection.Add(cube);
			ObjectCollection.Add(cube2);
			SceneCollection.AddCube(cube);
			SceneCollection.AddCube(cube2);
            //Camera.Target = cube.GetCenterPoint();
            Camera.Target = new Point3D(0, 0, 0);
			grid = new SceneGrid();
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
			DrawCordinateAxes(new Point3D(0, 0, 0), 3.0, 20);
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
			ISceneObject selectedObject = null;

			foreach (ISceneObject obj in ObjectCollection)
			{
                selectedObject = obj.CheckSelected();
				if(selectedObject != null)
				{
					break;
				}
			}

			if(selectedObject != null)
			{

				if(!(selectedObject is AxisCube))
				{
                    DeleteSelectingCoordAxes();

                    SelectedObject = selectedObject;

                    if (SceneMode == SceneMode.VIEW)
                    {
                        if (selectedObject.ParentObject != null)
                        {
                            SelectedObject = SelectedObject.ParentObject;
                        }
                    }

                    SelectedObject.Select();
                    SelectedObject.IsSelected = true;
					
                    axisSystem = new AxisSystem(SelectedObject);
                    ObjectCollection.Insert(0, axisSystem);
                }
				else
				{
					SelectedAxisCube = (AxisCube)selectedObject;
					SelectedAxisCube.Select();
					SelectedAxisCube.IsSelected = true;
				}
            }
			else
			{
				SelectedObject = null;
                DeleteSelectingCoordAxes();
            }
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
			ObjectCollection.Remove((ComplexCube)cube);
			SceneCollection.RemoveCube((ComplexCube)cube);
		}

		public void AddCube()
		{
			ComplexCube cube = new ComplexCube(new Point3D(0, 0, 0), new Vector(1, 1, 1), "Cube2");
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
			MeshObject3D attachingObject = (MeshObject3D)AttachingController.GetAttachingObject();

			//Create AttachingFacetsPair
			AttachingFacetsPair = new List<Plane>();
			foreach(Plane facet in ((MeshObject3D)AttachingController.GetTargetObject()).Mesh.Facets)
			{
				if(facet.GetCenterPoint() == axis.P1)
				{
					facet.IsAttached = true;
					AttachingFacetsPair.Add(facet);
					break;
				}
			}

			CoordinateAxisType oppositeType = AxisSystem.GetOppositeAxisType(AttachingFacetsPair[0].AxisType);
            foreach (Plane facet in ((MeshObject3D)AttachingController.GetAttachingObject()).Mesh.Facets)
            {
                if (facet.AxisType == oppositeType)
                {
                    facet.IsAttached = true;
                    AttachingFacetsPair.Add(facet);
                    break;
                }
            }

            Vector distanceVector = attachingObject.GetCenterPoint() - pointToMove;
			attachingObject.Move(distanceVector*(-1));
		}

		public void AttachCubes()
		{
			if(AttachingFacetsPair != null && AttachingFacetsPair.Count ==  2)
			{
				Plane targetFacet = AttachingFacetsPair[0];
				Plane attachingFacet = AttachingFacetsPair[1];

                Mesh targetMesh = ((MeshObject3D)AttachingController.GetTargetObject()).Mesh;
                Mesh attachingMesh = ((MeshObject3D)AttachingController.GetAttachingObject()).Mesh;

				//find closest point to attaching cube
				double minDistance = 0;
				Vector minVector = null;
				int indexOfMinPoint = 0;
				for(int i = 0; i < targetFacet.Points.Count; i++)
				{
					Point3D p = targetFacet.Points[i];
					Vector distanceVector = attachingFacet.GetCenterPoint() - p;
                    double distance = distanceVector.Length();

					if (minDistance == 0 || distance < minDistance)
					{
						minDistance = distance;
						minVector = distanceVector;
						indexOfMinPoint = i;
					}
				}

				//move to target cube
				Vector pointToPoint = attachingFacet.Points[indexOfMinPoint] - targetFacet.Points[indexOfMinPoint];
                Vector centerToPoint = minVector - pointToPoint;
				Point3D resultCenterPoint = new Point3D(targetFacet.Points[indexOfMinPoint] + new Point3D(centerToPoint));
				Vector resultVector = attachingFacet.GetCenterPoint() - resultCenterPoint;

                for (int i = 0; i < attachingMesh.Vertices.Count; i++)
				{
					attachingMesh.Vertices[i].Move(resultVector * (-1));
                }
                ((ComplexCube)AttachingController.GetAttachingObject()).GetCenterPoint().Move(resultVector * (-1));

                //attach facet
				for (int i = 0; i < attachingFacet.Points.Count; i++)
				{
					int index1 = targetMesh.GetIndexOfPoint(targetFacet[i]);
					Point3D targetPoint = targetMesh.Vertices[index1];

					int index2 = attachingMesh.GetIndexOfPoint(attachingFacet[i]);
					Point3D attachingPoint = attachingMesh.Vertices[index2];

					Vector v = attachingPoint - targetPoint;
					attachingPoint.Move(v * (-1));
					targetMesh.Vertices[index1] = attachingPoint;
				}

                ((ComplexCube)AttachingController.GetTargetObject()).UpdateObject();

				int targetFacetIndex = ((ComplexCube)AttachingController.GetTargetObject()).Mesh.GetIndexOfFacet(targetFacet);
                ((ComplexCube)AttachingController.GetTargetObject()).Mesh.attachedFacets.Add(targetFacetIndex);

                int attachingFacetIndex = ((ComplexCube)AttachingController.GetAttachingObject()).Mesh.GetIndexOfFacet(attachingFacet);
                ((ComplexCube)AttachingController.GetAttachingObject()).Mesh.attachedFacets.Add(attachingFacetIndex);
				//axisSystem.Move(v * (-1));

				ObjectCollection.Remove(AttachingAxisSystem);
				InitializeAttachingAxes((MeshObject3D)AttachingController.GetTargetObject());
            }
        }
        
		#endregion

		#region --- Export/Import ---

		public string Export()
		{
			string exportString = "";

			foreach (ComplexCube cube in ObjectCollection)
			{
				exportString += cube.Export();
			}

			return exportString;
		}

		public void Import(string[] importStrings)
		{
			List<ISceneObject> cubes = new List<ISceneObject>();
			List<Point3D> currentPoints = new List<Point3D>();
			List<Line> currentLines = new List<Line>();
			string name = "";

			for (int i = 0; i < importStrings.Length; i++)
			{
				if (importStrings[i].Contains("Cube"))
				{
					if(currentLines.Count > 0 && currentPoints.Count > 0)
					{
						Mesh currentMesh = new Mesh();
						currentMesh.Vertices = currentPoints;
						currentMesh.Edges = currentLines;
						ComplexCube newCube = new ComplexCube(currentMesh);
						newCube.Name = name;
						cubes.Add(newCube);
						currentPoints = new List<Point3D>();
						currentLines = new List<Line>();
					}
					else
					{
						name = importStrings[i];
					}
				}
				else if (importStrings[i].Contains("("))
				{
					string trimmedString = importStrings[i].Substring(1, importStrings[i].Length-2);
					string[] splited = trimmedString.Split(' ');
					double[] coords = new double[splited.Length];

					for(int j = 0; j < splited.Length; j++)
					{
						coords[j] = double.Parse(splited[j]);
					}

					currentPoints.Add(new Point3D(coords));
				}
				else if (importStrings[i].Contains('L'))
				{
					string trimmedString = importStrings[i].Substring(2);
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
				ComplexCube newCube = new ComplexCube(currentMesh);
				newCube.Name = name;
				cubes.Add(newCube);
				currentPoints = new List<Point3D>();
				currentLines = new List<Line>();
			}

			ObjectCollection = cubes;
			foreach(ComplexCube cube in cubes)
			{
				SceneCollection.AddCube(cube);
			}
		}

		#endregion
	}

	public enum SceneMode { VIEW, EDIT};
}
