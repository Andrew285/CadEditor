using SharpGL;
using SharpGL.SceneGraph;
using SharpGL.SceneGraph.Core;
using SharpGL.SceneGraph.Primitives;
using SharpGL.SceneGraph.Raytracing;
using System;
using System.CodeDom;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Net.Http.Headers;
using System.Runtime.Remoting;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml.Linq;

namespace CadEditor
{
    public class Scene
    {
        private Camera camera;
        private SceneCollection sceneCollection;
        private OpenGL gl;
        public List<CustomCube> cubes;

        public Ray ray;
		public Ray selectingRay;

		public Vertex centerPoint;
		public bool Selected { get; set; }
		private ISelectable selectedAxis { get; set; }

		public float selectedObjAxisLength { get; set; }
		private Axis[] selectingCoordinateAxes;
		private AxisCube[] selectingCoordinateCubes;

		public SceneMode SceneMode { get; set; } = SceneMode.VIEW;


		public Scene(OpenGL _gl, Camera _camera, SceneCollection _sceneCollection)
        {
			gl = _gl;
			camera = _camera;
			sceneCollection = _sceneCollection;
			cubes = new List<CustomCube>();
        }

		public Camera Camera
		{
			get { return camera; }
		}

		#region --- Initializing ---

		public void InitializeObjects()
		{
			CustomCube cube = new CustomCube(gl, new Vertex(gl, 0, 0, 0), 1f, "Cube_1");
			cubes.Add(cube);
			sceneCollection.Add(cube);
		}

		public void InitSelectingCoordAxes(ISelectable obj, float lineWidth, double axisLength)
		{
			Vertex v = null;

			//Get a point where axis coordinates will be shown
			if (obj is Vertex)
			{
				v = (Vertex)obj;
			}
			else if (obj is Edge)
			{
				v = ((Edge)obj).GetCenterPoint();
			}
			else if (obj is Facet)
			{
				v = ((Facet)obj).GetCenterPoint();
			}
			else if(obj is CustomCube)
			{
				v = ((CustomCube)obj).GetCenterPoint();
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

			Axis axisX = new Axis(gl, new Vertex(gl, v.X, v.Y, v.Z), new Vertex(gl, axisXYZLengths[0] + v.X, v.Y, v.Z), CoordinateAxis.X);
			Axis axisY = new Axis(gl, new Vertex(gl, v.X, v.Y, v.Z), new Vertex(gl, v.X, axisXYZLengths[1] + v.Y, v.Z), CoordinateAxis.Y);
			Axis axisZ = new Axis(gl, new Vertex(gl, v.X, v.Y, v.Z), new Vertex(gl, v.X, v.Y, axisXYZLengths[2] + v.Z), CoordinateAxis.Z);

			axisX.LineWidth = lineWidth;
			axisY.LineWidth = lineWidth;
			axisZ.LineWidth = lineWidth;
			selectingCoordinateAxes = new Axis[] { axisX, axisY, axisZ };

			//Create Axis Cubes
			AxisCube cubeX = new AxisCube(gl, new Vertex(gl, axisXYZLengths[0] + v.X, v.Y, v.Z), CoordinateAxis.X, 0.1f, "cubeAxisX");
			AxisCube cubeY = new AxisCube(gl, new Vertex(gl, v.X, axisXYZLengths[1] + v.Y, v.Z), CoordinateAxis.Y, 0.1f, "cubeAxisY");
			AxisCube cubeZ = new AxisCube(gl, new Vertex(gl, v.X, v.Y, axisXYZLengths[2] + v.Z), CoordinateAxis.Z, 0.1f, "cubeAxisZ");

			//Set colors
			cubeX.NonSelectedColor = Color.Red;
			cubeY.NonSelectedColor = Color.Green;
			cubeZ.NonSelectedColor = Color.Blue;

			selectingCoordinateCubes = new AxisCube[] { cubeX, cubeY, cubeZ };
		}

		#endregion

		#region --- Drawing ---

		public void DrawScene(int Width, int Height)
        {
			gl.Clear(OpenGL.GL_COLOR_BUFFER_BIT | OpenGL.GL_DEPTH_BUFFER_BIT);

			// Set up the projection matrix
			gl.MatrixMode(OpenGL.GL_PROJECTION);
			gl.LoadIdentity();
			gl.Perspective(45.0, (double)Width / (double)Height, 0.1, 100.0);

			// Set up the view matrix
			gl.MatrixMode(OpenGL.GL_MODELVIEW);
			gl.LoadIdentity();
			gl.LookAt(0, 0, camera.CameraDistance, 0, 0, 0, 0, 1, 0);

			//Rotate Camera
			camera.RotateAxisX();
			camera.RotateAxisY();

			//Draw Scene Grid
			DrawCordinateAxes(new Vertex(gl, 0, 0, 0), 3.0, 20);

			//Draw Coordinate Axes if selected Vertex
			if(selectingCoordinateAxes != null)
			{
				DrawSelectingCoordAxes();
			}

			//Draw ray when user clicks with left button
			if (selectingRay != null && selectingRay.Direction != null)
			{
				DrawLine(selectingRay.Origin, selectingRay.Direction);
			}

			//Draw all objects
			foreach (var cube in cubes)
			{
				cube.Draw();
			}
		}

        public void DrawCordinateAxes(Vertex v, double lineWidth, double axisLength)
        {
            gl.LineWidth((float)lineWidth);
            gl.Begin(OpenGL.GL_LINES);

			gl.Color(1f, 0, 0, 0);
			gl.Vertex(-axisLength + v.X, v.Y, v.Z);
			gl.Vertex(axisLength + v.X, v.Y, v.Z);


			gl.Color(0, 1f, 0, 0);
			gl.Vertex(v.X, -axisLength + v.Y, v.Z);
            gl.Vertex(v.X, axisLength + v.Y, v.Z);

			gl.Color(0, 0, 1f, 0);
			gl.Vertex(v.X, v.Y, -axisLength + v.Z);
            gl.Vertex(v.X, v.Y, axisLength + v.Z);

            gl.End();
            gl.Flush();
        }

		public void DrawSelectingCoordAxes()
		{
			gl.Begin(OpenGL.GL_LINES);

			selectingCoordinateAxes[0].Draw(new double[] {1, 0, 0});
			selectingCoordinateAxes[1].Draw(new double[] { 0, 1, 0 });
			selectingCoordinateAxes[2].Draw(new double[] { 0, 0, 1 });

			selectingCoordinateCubes[0].Draw();
			selectingCoordinateCubes[1].Draw();
			selectingCoordinateCubes[2].Draw();

			gl.End();
			gl.Flush();
		}

		public void DrawLine(Vector v1, Vector v2)
        {
			gl.Begin(OpenGL.GL_LINES);

			gl.Color(1, 0, 0);
			gl.Vertex(v1[0], v1[1], v1[2]);

            gl.Color(1, 0, 0);
			gl.Vertex(v2[0], v2[1], v2[2]);

			gl.End();
		}

		public void DeleteSelectingCoordAxes()
		{
			selectingCoordinateAxes = null;
		}

		#endregion

		#region --- Selection ---

		public ISelectable CheckSelectedElement(int x, int y, OpenGL gl)
		{
			// Convert the mouse coordinates to world coordinates
			Vector near = new Vector(gl.UnProject(x, y, 0));
			Vector far = new Vector(gl.UnProject(x, y, 1));
			Vector direction = (far - near).Normalize();

			//initialize a ray
			ray = new Ray(near, direction, gl);
			selectingRay = new Ray(gl);
			selectingRay.Origin = near;

			ISelectable selectedObject = null;
			selectedAxis = null;

			List<ISelectable> selectedObjects = new List<ISelectable>();

			//Check if any coordinate axis is selected
			if (selectingCoordinateCubes != null)
			{
				foreach (CustomCube cube in selectingCoordinateCubes)
				{
					cube.Deselect();
				}

				for(int i = 0; i < selectingCoordinateCubes.Length; i++)
				{

					Facet selectedFacet = CheckSelectedFacet(selectingCoordinateCubes[i], ray);
					if (selectedFacet != null)
					{
						selectingCoordinateCubes[i].IsSelected = true;
						selectedObjects.Add(selectingCoordinateCubes[i]);
						//return selectingCoordinateCubes[i];
					}
				}
			}

			if(selectedObjects.Count == 0)
			{
				// Iterate over each object in the scene
				foreach (CustomCube cube in cubes)
				{
					//deselect all facets, edges and vertices before another selecting
					cube.Deselect();


					////check if any vertex is selected
					//List<ISelectable> selectedObjects = new List<ISelectable>();

					Console.WriteLine("\n------------------\n");
					//check if any facet is selected
					Facet selectedFacet = CheckSelectedFacet(cube, ray);
					if (selectedFacet != null)
					{
						selectedObjects.Add(selectedFacet);
					}
					Console.WriteLine("\nFacet: " + selectedFacet);

					//check if any edge is selected
					Edge selectedEdge = CheckSelectedEdge(cube, ray);
					if (selectedEdge != null)
					{
						selectedObjects.Add(selectedEdge);

					}

					Vertex selectedVertex = CheckSelectedVertex(cube, ray);
					if (selectedVertex != null)
					{
						selectedObjects.Add(selectedVertex);
					}

					if(selectedObjects.Count > 0)
					{
						if(SceneMode == SceneMode.VIEW)
						{
							selectedObjects = new List<ISelectable> { cube };
						}
					}

					//if(selectedObjects.Count > 0)
					//{
					//	selectedObject = GetNearestObject(selectedObjects, ray);
					//}
					//else
					//{
					//	selectedObject = null;
					//}

					////Check SceneMode
					//if (selectedObject != null)
					//{
					//	if (SceneMode == SceneMode.VIEW)
					//	{
					//		cube.Select();
					//		return cube;
					//	}
					//	else if (SceneMode == SceneMode.EDIT)
					//	{
					//		selectedObject.IsSelected = true;
					//		return selectedObject;
					//	}
					//}

				}
			}

			if (selectedObjects.Count > 1)
			{
				selectedObject = GetNearestObject(selectedObjects, ray);
				selectedObject.Select();

			}
			else if (selectedObjects.Count == 1)
			{
				selectedObject = selectedObjects[0];
				selectedObject.Select();
			}
			else
			{
				selectedObject = null;
			}

			return selectedObject;
		}

		private ISelectable GetNearestObject(List<ISelectable> selectedObjects, Ray _ray)
		{
			Vertex minVertex = null;
			int index = 0;
			for (int i = 0; i < selectedObjects.Count; i++)
			{
				//Get a point where axis coordinates will be shown
				Vertex v = null;
				if (selectedObjects[i] is Vertex)
				{
					v = (Vertex)selectedObjects[i];
				}
				else if (selectedObjects[i] is Edge)
				{
					v = ((Edge)selectedObjects[i]).GetCenterPoint();
				}
				else if (selectedObjects[i] is Facet)
				{
					v = ((Facet)selectedObjects[i]).GetCenterPoint();
				}

				if (minVertex != null)
				{
					double dist1 = GetDistance(v, new Vertex(ray.Origin));
					double dist2 = GetDistance(minVertex, new Vertex(ray.Origin));
					if (Math.Abs(dist1 - dist2) > 0.001 && dist1 < dist2)
					{
						minVertex = v;
						index = i;
					}
				}
				else
				{
					minVertex = v;
				}
			}

			return selectedObjects[index];
		}

		private double GetDistance(Vertex v1, Vertex v2)
		{
			return Math.Sqrt(Math.Pow((v2.X - v1.X), 2) + Math.Pow((v2.Y - v1.Y), 2) + Math.Pow((v2.Z - v1.Z), 2));
		}

		public Facet CheckSelectedFacet(CustomCube cube, Ray ray)
		{
			double? minDistance = null; //minimal distance between facet and ray origin
			Facet selectedFacet = null; //facet that is selected

			Vertex minPoint = null;
			for(int i = 0; i < cube.Mesh.Facets.Length; i++)
			{
				Vertex intersectionPoint = ray.RayIntersectsFacet(cube.Mesh.Facets[i]);

				if(intersectionPoint != null)
				{
					Console.WriteLine("\nPoint: " + intersectionPoint);
				}
				//if (minDistance != null)
				//{
				//	if(currentDistance < minDistance)
				//	{
				//		selectedFacet = cube.Mesh.Facets[i];
				//		minDistance = currentDistance;
				//	}
				//}
				//else if(currentDistance != null)
				//{
				//	selectedFacet = cube.Mesh.Facets[i];
				//	minDistance = currentDistance;
				//}

				if (intersectionPoint != null)
				{
					if (minPoint != null)
					{
						double dist1 = GetDistance(intersectionPoint, new Vertex(ray.Origin));
						double dist2 = GetDistance(minPoint, new Vertex(ray.Origin));
						if (dist1 < dist2)
						{
							minPoint = intersectionPoint;
							selectedFacet = cube.Mesh.Facets[i];
						}
					}
					else
					{
						minPoint = intersectionPoint;
						selectedFacet = cube.Mesh.Facets[i];
					}
				}
			}

			if(minPoint != null)
			{
				selectingRay.Direction = new Vector(minPoint);
				Console.WriteLine("\nResult Min Point: " + minPoint);
			}
			return selectedFacet;
		}

		public Edge CheckSelectedEdge(CustomCube cube, Ray ray)
		{
			double? minDistance = null; //minimal distance between edge and ray origin
			Edge selectedEdge = null; //edge that is selected

			Vertex minPoint = null;
			for (int i = 0; i < cube.Mesh.Edges.Length; i++)
			{
				Vertex intersectionPoint = ray.RayIntersectsEdge(cube.Mesh.Edges[i]);
				
				//if (minDistance != null)
				//{
				//	if (currentDistance < minDistance)
				//	{
				//		selectedEdge = cube.Mesh.Edges[i];
				//		minDistance = currentDistance;
				//	}
				//}
				//else if (currentDistance != null)
				//{
				//	selectedEdge = cube.Mesh.Edges[i];
				//	minDistance = currentDistance;
				//}

				if (intersectionPoint != null)
				{
					if (minPoint != null)
					{
						double dist1 = GetDistance(intersectionPoint, new Vertex(ray.Origin));
						double dist2 = GetDistance(minPoint, new Vertex(ray.Origin));
						if (dist1 < dist2)
						{
							minPoint = intersectionPoint;
							selectedEdge = cube.Mesh.Edges[i];
						}
					}
					else
					{
						minPoint = intersectionPoint;
						selectedEdge = cube.Mesh.Edges[i];
					}
				}
			}

			//if(intersectionPoint != null)
			//{
			//	selectingRay.Direction = new Vector(intersectionPoint);
			//}
			//else
			//{
			//	selectingRay.Direction = selectingRay.Origin;
			//}
			return selectedEdge;
		}

		public Vertex CheckSelectedVertex(CustomCube cube, Ray ray)
		{
			double? minDistance = null; //minimal distance between vertex and ray origin
			Vertex selectedVertex = null; //vertex that is selected

			Vertex minPoint = null;
			for (int i = 0; i < cube.Mesh.Vertices.Length; i++)
			{
				Vertex intersectionPoint = ray.RayIntersectsVertex(cube.Mesh.Vertices[i]);

				//if (minDistance != null)
				//{
				//	if (currentDistance < minDistance)
				//	{
				//		selectedVertex = cube.Mesh.Vertices[i];
				//		minDistance = currentDistance;
				//	}
				//}
				//else if (currentDistance != null)
				//{
				//	selectedVertex = cube.Mesh.Vertices[i];
				//	minDistance = currentDistance;
				//}

				if (intersectionPoint != null)
				{
					if (minPoint != null)
					{
						double dist1 = GetDistance(intersectionPoint, new Vertex(ray.Origin));
						double dist2 = GetDistance(minPoint, new Vertex(ray.Origin));
						if (dist1 < dist2)
						{
							minPoint = intersectionPoint;
							selectedVertex = cube.Mesh.Vertices[i];
						}
					}
					else
					{
						minPoint = intersectionPoint;
						selectedVertex = cube.Mesh.Vertices[i];
					}
				}
			}

			//if (selectedVertex != null)
			//{
			//	selectingRay.Direction = new Vector(selectedVertex);
			//}
			//else
			//{
			//	selectingRay.Direction = selectingRay.Origin;
			//}
			return selectedVertex;
		}

		public Axis CheckSelectedCoordinateAxes()
		{
			if(selectingCoordinateAxes != null)
			{
				foreach (Axis line in selectingCoordinateAxes)
				{
					Vertex intersectionPoint = ray.RayIntersectsEdge(line);

					if (intersectionPoint != null)
					{
						line.IsSelected = true;
						return line;
					}
				}
			}

			return null;
		}

		public CustomCube GetSelectedCube(int x, int y, OpenGL gl)
		{
			// Convert the mouse coordinates to world coordinates
			Vector near = new Vector(gl.UnProject(x, y, 0));
			Vector far = new Vector(gl.UnProject(x, y, 1));
			Vector direction = (far - near).Normalize();

			ray = new Ray(near, direction, gl);
			selectingRay = new Ray(gl);
			selectingRay.Origin = near;

			// Iterate over each object in the scene
			foreach (CustomCube cube in cubes)
			{
				Facet selectedFacet = CheckSelectedFacet(cube, ray);
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

			Facet selectedFacet = CheckSelectedFacet(cube, ray);
			if (selectedFacet != null)
			{
				return true;
			}

			return false;
		}

		#endregion

		#region --- Manipulation ---

		public void DeleteCompletely(CustomCube cube)
		{
			cubes.Remove(cube);
			sceneCollection.Remove(cube);
		}

		public void AddCube()
		{
			CustomCube cube = new CustomCube(gl, new Vertex(gl, 0, 0, 0), null, "Cube2");
			cubes.Add(cube);
			sceneCollection.Add(cube);
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
			foreach(CustomCube cube in cubes)
			{
				cube.Deselect();
			}

			if(selectingCoordinateCubes != null)
			{
				DeleteSelectingCoordAxes();
			}
		}

		#endregion
	}

	public enum SceneMode { VIEW, EDIT};
	public enum CoordinateAxis { X, Y, Z }
}
