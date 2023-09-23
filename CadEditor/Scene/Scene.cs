using CadEditor.Graphics;
using CadEditor.MeshObjects;
using SharpGL;
using SharpGL.SceneGraph;
using SharpGL.SceneGraph.Primitives;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Runtime.InteropServices;

namespace CadEditor
{
    public class Scene
    {
        private Camera camera;
        public SceneCollection SceneCollection { get; private set; }
        private OpenGL GL;
        public List<ComplexCube> DrawableCubes { get; private set; }
		public Dictionary<ComplexCube, ComplexCube> NonDrawableCubes { get; private set; }
        public Ray ray;
		public Ray selectingRay;
		public float selectedObjAxisLength { get; set; }
		private Axis[] selectingCoordinateAxes;
		private AxisCube[] selectingCoordinateCubes;

		private readonly double facetTolerance = 0.08;
		private readonly double edgeTolerance = 0.08;

		public bool DrawFacets { get; set; }

		public SceneMode SceneMode { get; set; } = SceneMode.VIEW;

		public IGraphics SelectedCube;
		public IGraphics SelectedObject;
		public IGraphics SelectedPreviousObject;


		public Scene(OpenGL _gl, Camera _camera, SceneCollection _sceneCollection)
        {
			GL = _gl;
			camera = _camera;
			SceneCollection = _sceneCollection;
			DrawableCubes = new List<ComplexCube>();
			NonDrawableCubes = new Dictionary<ComplexCube, ComplexCube>();
        }

		public Camera Camera
		{
			get { return camera; }
		}

		#region --- Initializing ---

		public void InitializeObjects()
		{
			ComplexCube cube = new ComplexCube(GL, new Point(0, 0, 0, GL), 1, 1, 1, "Cube_1");
			DrawableCubes.Add(cube);
			SceneCollection.AddCube(cube);
		}

		public void InitSelectingCoordAxes(IGraphics obj, float lineWidth, double axisLength)
		{
			Point v = null;

			//Get a point where axis coordinates will be shown
			if (obj is Point)
			{
				v = (Point)obj;
			}
			else if (obj is Line)
			{
				v = (Point)((Line)obj).GetCenterPoint();
			}
			else if (obj is Plane)
			{
				v = (Point)((Plane)obj).GetCenterPoint();
			}
			else if(obj is ComplexCube)
			{
				v = ((ComplexCube)obj).CenterPoint;
				axisLength += 1;
			}


			//Create Axis Lines
			double[] axisXYZLengths = new double[3];   //axisXLength, axisYLength, axisZLength

			for(int i = 0; i < axisXYZLengths.Length; i++)
			{
				double multiplier = 1;

				if (v[i] < 0)
				{
					multiplier = -1;
				}
				else if (v[i] > 0)
				{
					multiplier = 1;
				}
				else
				{
					axisXYZLengths[i] = axisLength;
				}

				axisXYZLengths[i] = axisLength * multiplier;
			}

			Axis axisX = new Axis(new Point(v.X, v.Y, v.Z, GL), new Point(axisXYZLengths[0] + v.X, v.Y, v.Z, GL), CoordinateAxis.X, GL);
			Axis axisY = new Axis(new Point(v.X, v.Y, v.Z, GL), new Point(v.X, axisXYZLengths[1] + v.Y, v.Z, GL), CoordinateAxis.Y, GL);
			Axis axisZ = new Axis(new Point(v.X, v.Y, v.Z, GL), new Point(v.X, v.Y, axisXYZLengths[2] + v.Z, GL), CoordinateAxis.Z, GL);

			axisX.LineWidth = lineWidth;
			axisY.LineWidth = lineWidth;
			axisZ.LineWidth = lineWidth;
			selectingCoordinateAxes = new Axis[] { axisX, axisY, axisZ };

			//Create Axis Cubes
			AxisCube cubeX = new AxisCube(GL, new Point(axisXYZLengths[0] + v.X, v.Y, v.Z, GL), CoordinateAxis.X, 0.1, 0.1, 0.1, "cubeAxisX");
			AxisCube cubeY = new AxisCube(GL, new Point(v.X, axisXYZLengths[1] + v.Y, v.Z, GL), CoordinateAxis.Y, 0.1, 0.1, 0.1, "cubeAxisY");
			AxisCube cubeZ = new AxisCube(GL, new Point(v.X, v.Y, axisXYZLengths[2] + v.Z, GL), CoordinateAxis.Z, 0.1, 0.1, 0.1, "cubeAxisZ");

			//Set colors
			cubeX.FacetSelectedColor = Color.Red;
			cubeY.FacetSelectedColor = Color.Green;
			cubeZ.FacetSelectedColor = Color.Blue;

            cubeX.FacetNonSelectedColor = Color.Red;
            cubeY.FacetNonSelectedColor = Color.Green;
            cubeZ.FacetNonSelectedColor = Color.Blue;

            selectingCoordinateCubes = new AxisCube[] { cubeX, cubeY, cubeZ };
		}

		#endregion

		#region --- Drawing ---

		public void DrawScene(int Width, int Height)
        {
			GL.Clear(OpenGL.GL_COLOR_BUFFER_BIT | OpenGL.GL_DEPTH_BUFFER_BIT);

			// Set up the projection matrix
			GL.MatrixMode(OpenGL.GL_PROJECTION);
			GL.LoadIdentity();
			GL.Perspective(45.0, (double)Width / (double)Height, 0.1, 100.0);

			// Set up the view matrix
			GL.MatrixMode(OpenGL.GL_MODELVIEW);
			GL.LoadIdentity();
			GL.LookAt(0, 0, camera.CameraDistance, 0, 0, 0, 0, 1, 0);

			//Rotate Camera
			camera.RotateAxisX();
			camera.RotateAxisY();

			//Draw Scene Grid
			DrawCordinateAxes(new Point(0, 0, 0, GL), 3.0, 20);

            //Draw Coordinate Axes if selected Vertex
            if (selectingCoordinateAxes != null)
			{
				DrawSelectingCoordAxes();
			}

            //Draw ray when user clicks with left button
            if (selectingRay != null && selectingRay.Direction != null)
			{
				DrawLine(selectingRay.Origin, selectingRay.Direction);
			}

            //Draw all objects
            foreach (var cube in DrawableCubes)
			{
				cube.DrawFacets = this.DrawFacets;
				cube.Draw();
			}
		}

        public void DrawCordinateAxes(Point v, double lineWidth, double axisLength)
        {
            GL.LineWidth((float)lineWidth);
            GL.Begin(OpenGL.GL_LINES);

			GL.Color(1f, 0, 0, 0);
			GL.Vertex(-axisLength + v.X, v.Y, v.Z);
			GL.Vertex(axisLength + v.X, v.Y, v.Z);


			GL.Color(0, 1f, 0, 0);
			GL.Vertex(v.X, -axisLength + v.Y, v.Z);
            GL.Vertex(v.X, axisLength + v.Y, v.Z);

			GL.Color(0, 0, 1f, 0);
			GL.Vertex(v.X, v.Y, -axisLength + v.Z);
            GL.Vertex(v.X, v.Y, axisLength + v.Z);

            GL.End();
            GL.Flush();
        }

        public void DrawSelectingCoordAxes()
		{
			GL.Begin(OpenGL.GL_LINES);

			selectingCoordinateAxes[0].NonSelectedColor = Color.Red;
			selectingCoordinateAxes[1].NonSelectedColor = Color.Green;
			selectingCoordinateAxes[2].NonSelectedColor = Color.Blue;

            selectingCoordinateAxes[0].Draw();
            selectingCoordinateAxes[1].Draw();
            selectingCoordinateAxes[2].Draw();

			selectingCoordinateCubes[0].Draw();
			selectingCoordinateCubes[1].Draw();
			selectingCoordinateCubes[2].Draw();

			GL.End();
			GL.Flush();
		}

		public void DrawLine(Vector v1, Vector v2)
        {
            GL.LineWidth(3.0f);
            GL.Begin(OpenGL.GL_LINES);

			GL.Color(1, 0, 0);
			GL.Vertex(v1[0], v1[1], v1[2]);

            GL.Color(1, 0, 0);
			GL.Vertex(v2[0], v2[1], v2[2]);

			GL.End();
		}

		public void DeleteSelectingCoordAxes()
		{
			selectingCoordinateAxes = null;
		}

		#endregion

		#region --- Selection ---

		public IGraphics CheckSelectedElement(int x, int y, OpenGL gl)
		{
			// Convert the mouse coordinates to world coordinates
			Vector near = new Vector(gl.UnProject(x, y, 0));
			Vector far = new Vector(gl.UnProject(x, y, 1));
			Vector direction = (far - near).Normalize();

			//initialize a ray
			ray = new Ray(near, direction, gl);
			selectingRay = new Ray(gl);
			selectingRay.Origin = near;

            IGraphics selectedCube = null;
            IGraphics selectedObject = null;

			//Check if any coordinate axis is selected
			if (selectingCoordinateCubes != null)
			{
				foreach (AxisCube cube in selectingCoordinateCubes)
				{
					cube.Deselect();
				}

				for(int i = 0; i < selectingCoordinateCubes.Length; i++)
				{

					Plane selectedFacet = CheckSelectedFacet(selectingCoordinateCubes[i], ray);
					if (selectedFacet != null)
					{
						selectingCoordinateCubes[i].IsSelected = true;
						return selectingCoordinateCubes[i];
					}
				}
			}

            // Iterate over each object in the scene
            foreach (ComplexCube cube in DrawableCubes)
            {
				CheckCubeElements(cube, ref selectedObject, ref selectedCube);
            }

            if (selectedObject != null && SceneMode == SceneMode.VIEW)
            {
                selectedObject = selectedCube;
            }

            return selectedObject;
        }

		private void CheckCubeElements(ComplexCube cube, ref IGraphics selectedObject, ref IGraphics selectedCube)
		{
			//deselect all facets, edges and vertices before another selecting
            cube.Deselect();

			//check if any vertex is selected
			Point selectedVertex = CheckSelectedVertex(cube, ray);
			if (selectedVertex != null)
			{
				selectedObject = selectedVertex;
				selectedCube = cube;
				return;
			}

			//check if any edge is selected
			Line selectedEdge = CheckSelectedEdge(cube, ray);
            if (selectedEdge != null)
            {
                selectedObject = selectedEdge;
                selectedCube = cube;
				return;
            }

			//check if any facet is selected
			Plane selectedFacet = CheckSelectedFacet(cube, ray);
			if (selectedFacet != null)
			{
				selectedObject = selectedFacet;
				selectedCube = cube;
				return;
			}
		}

		public Plane CheckSelectedFacet(CustomCube cube, Ray ray)
		{
			double? minDistance = null; //minimal distance between facet and ray origin
			Plane selectedFacet = null; //facet that is selected
			Point minIntersectionPoint = null;

			for(int i = 0; i < cube.Mesh.Facets.Count; i++)
			{
				Plane currentFacet = cube.Mesh.Facets[i];
                Point intersectionPoint = ray.RayIntersectsPlane(currentFacet);

				//check if facet contains intersection point
				if (intersectionPoint != null && currentFacet.Contains(intersectionPoint))
				{
					//compare distances from ray origin to intersection point
                    double distanceToPoint = GetDistance(intersectionPoint, new Point(ray.Origin));
                    
					if (minDistance != null)
					{
						if(distanceToPoint < minDistance)
						{
							minDistance = distanceToPoint;
                            selectedFacet = currentFacet;
							minIntersectionPoint = intersectionPoint;
                        }
                    }
					else
					{
						minDistance = distanceToPoint;
                        selectedFacet = currentFacet;
                        minIntersectionPoint = intersectionPoint;
                    }
				}
            }

			//check if clickable area of facet contains the intersection point
			if(selectedFacet != null && minIntersectionPoint != null)
			{
                Plane facetArea = selectedFacet.GetClickableArea(facetTolerance);
				if (!facetArea.Contains(minIntersectionPoint))
				{
					return null;
				}

				selectingRay.Direction = new Vector(minIntersectionPoint);
            }

            return selectedFacet;
		}

		public Line CheckSelectedEdge(ComplexCube cube, Ray ray)
		{
			double? minDistance = null; //minimal distance between edge and ray origin
			Line selectedEdge = null; //edge that is selected
			Point minIntersectionPoint = null;

			for (int i = 0; i < cube.Mesh.Edges.Count; i++)
			{
                Line currentEdge = cube.Mesh.Edges[i];
                Point intersectionPoint = ray.RayIntersectsLine(currentEdge);

                //check if edge contains intersection point
                if (intersectionPoint != null && currentEdge.Contains(intersectionPoint))
                {
                    //compare distances from ray origin to intersection point
                    double distanceToPoint = GetDistance(intersectionPoint, new Point(ray.Origin));
                    
					if (minDistance != null)
                    {
                        if (distanceToPoint < minDistance)
                        {
                            minDistance = distanceToPoint;
                            selectedEdge = currentEdge;
                            minIntersectionPoint = intersectionPoint;
                        }
                    }
                    else
                    {
                        minDistance = distanceToPoint;
                        selectedEdge = currentEdge;
                        minIntersectionPoint = intersectionPoint;
                    }
                }
            }

            //check if clickable area of facet contains the intersection point
            if (selectedEdge != null && minIntersectionPoint != null)
            {
                Line edgeArea = selectedEdge.GetClickableArea(edgeTolerance);
                if (!edgeArea.Contains(minIntersectionPoint))
                {
                    return null;
                }

                selectingRay.Direction = new Vector(minIntersectionPoint);
            }

            return selectedEdge;
		}

		public Point CheckSelectedVertex(ComplexCube cube, Ray ray)
		{
			double? minDistance = null; //minimal distance between vertex and ray origin
			Point selectedVertex = null; //vertex that is selected
            Point minIntersectionPoint = null;

			for (int i = 0; i < cube.Mesh.Vertices.Count; i++)
			{
                Point currentVertex = cube.Mesh.Vertices[i];
                Point intersectionPoint = ray.RayIntersectsVertex(currentVertex);

                //check if edge contains intersection point
                if (intersectionPoint != null)
                {
                    //compare distances from ray origin to intersection point
                    double distanceToPoint = GetDistance(intersectionPoint, new Point(ray.Origin));

                    if (minDistance != null)
                    {
                        if (distanceToPoint < minDistance)
                        {
                            minDistance = distanceToPoint;
                            selectedVertex = currentVertex;
                            minIntersectionPoint = intersectionPoint;
                        }
                    }
                    else
                    {
                        minDistance = distanceToPoint;
                        selectedVertex = currentVertex;
                        minIntersectionPoint = intersectionPoint;
                    }
                }
            }

			if(minIntersectionPoint != null)
			{
                selectingRay.Direction = new Vector(minIntersectionPoint);
            }

            return selectedVertex;
		}

		public Axis CheckSelectedCoordinateAxes()
		{
			if(selectingCoordinateAxes != null)
			{
				foreach (Axis line in selectingCoordinateAxes)
				{
					Point intersectionPoint = ray.RayIntersectsLine(line);

					if (intersectionPoint != null)
					{
						line.IsSelected = true;
						return line;
					}
				}
			}

			return null;
		}

		public ComplexCube GetSelectedCube(int x, int y, OpenGL gl)
		{
			// Convert the mouse coordinates to world coordinates
			Vector near = new Vector(gl.UnProject(x, y, 0));
			Vector far = new Vector(gl.UnProject(x, y, 1));
			Vector direction = (far - near).Normalize();

			ray = new Ray(near, direction, gl);
			selectingRay = new Ray(gl);
			selectingRay.Origin = near;

			// Iterate over each object in the scene
			foreach (ComplexCube cube in DrawableCubes)
			{
				Plane selectedFacet = CheckSelectedFacet(cube, ray);
				if (selectedFacet != null)
				{
					return cube;
				}
			}

			return null;
		}

		public bool IsSelectedAxisCube(int x, int y, AxisCube cube, OpenGL gl)
		{
			// Convert the mouse coordinates to world coordinates
			Vector near = new Vector(gl.UnProject(x, y, 0));
			Vector far = new Vector(gl.UnProject(x, y, 1));
			Vector direction = (far - near).Normalize();

			ray = new Ray(near, direction, gl);
			selectingRay = new Ray(gl);
			selectingRay.Origin = near;

			Plane selectedFacet = CheckSelectedFacet(cube, ray);
			if (selectedFacet != null)
			{
				return true;
			}

			return false;
		}

		public void DeselectAll()
		{
			foreach (CustomCube c in DrawableCubes)
			{
				c.Deselect();
			}
		}

		#endregion

		#region --- Manipulation ---

		public void DeleteCompletely(ComplexCube cube)
		{
			DrawableCubes.Remove(cube);
			SceneCollection.RemoveCube(cube);
		}

		public void AddCube()
		{
			ComplexCube cube = new ComplexCube(GL, new Point(0, 0, 0, GL), null, null, null, "Cube2");
			DrawableCubes.Add(cube);
			SceneCollection.AddCube(cube);
		}

		public void MoveCoordinateAxes(double x, double y, double z)
		{
			for(int i = 0; i < selectingCoordinateCubes.Length; i++)
			{
				selectingCoordinateCubes[i].Move(x, y, z);
				selectingCoordinateAxes[i].Move(x, y, z);
			}

		}

		public void Update()
		{
			foreach(ComplexCube cube in DrawableCubes)
			{
				cube.Deselect();
			}

			if(selectingCoordinateCubes != null)
			{
				DeleteSelectingCoordAxes();
			}
		}

        private double GetDistance(Point v1, Point v2)
        {
            return Math.Sqrt(Math.Pow((v2.X - v1.X), 2) + Math.Pow((v2.Y - v1.Y), 2) + Math.Pow((v2.Z - v1.Z), 2));
        }

		public int GetIndexOfComplexCube(ComplexCube cube)
		{
			for(int i = 0; i < DrawableCubes.Count; i++)
			{
				if (DrawableCubes[i].Equals(cube))
				{
					return i;
				}
			}

			return -1;
		}

		public ComplexCube FindNonDrawableCube(ComplexCube drawableCube)
		{
			if (NonDrawableCubes.ContainsKey(drawableCube))
			{
				return NonDrawableCubes[drawableCube];
			}

			return null;
		}

		#endregion

		#region --- Export/Import ---

		public string Export()
		{
			string exportString = "";

			foreach (ComplexCube cube in DrawableCubes)
			{
				exportString += cube.Export();
			}

			return exportString;
		}

		public void Import(string[] importStrings)
		{
			List<ComplexCube> cubes = new List<ComplexCube>();
			List<Point> currentPoints = new List<Point>();
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
						ComplexCube newCube = new ComplexCube(currentMesh, GL);
						newCube.Name = name;
						cubes.Add(newCube);
						currentPoints = new List<Point>();
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

					currentPoints.Add(new Point(coords, GL));
				}
				else if (importStrings[i].Contains('L'))
				{
					string trimmedString = importStrings[i].Substring(2);
					string[] splited = trimmedString.Split(' ');
					Point p1 = currentPoints[Int32.Parse(splited[0])];
					Point p2 = currentPoints[Int32.Parse(splited[1])];

					currentLines.Add(new Line(p1, p2, GL));
				}
			}

			if (currentLines.Count > 0 && currentPoints.Count > 0)
			{
				Mesh currentMesh = new Mesh();
				currentMesh.Vertices = currentPoints;
				currentMesh.Edges = currentLines;
				ComplexCube newCube = new ComplexCube(currentMesh, GL);
				newCube.Name = name;
				cubes.Add(newCube);
				currentPoints = new List<Point>();
				currentLines = new List<Line>();
			}

			DrawableCubes = cubes;
			foreach(ComplexCube cube in cubes)
			{
				SceneCollection.AddCube(cube);
			}
		}

		#endregion
	}

	public enum SceneMode { VIEW, EDIT};
	public enum CoordinateAxis { X, Y, Z }
}
