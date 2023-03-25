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
		public bool Selected { get; set; }
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
			DrawCordinateAxes(3.0f, 20);

			//DrawLine();
			if (selectingRay != null)
			{
				DrawLine(selectingRay.Origin, selectingRay.Direction);
			}

			foreach (var cube in cubes)
			{
				cube.Draw();
			}
		}

        public void DrawCordinateAxes(float lineWidth, float axisLength)
        {
            gl.LineWidth(lineWidth);
            gl.Begin(OpenGL.GL_LINES);
            gl.Color(1f, 0, 0, 0);
            gl.Vertex(-axisLength, 0, 0);
            gl.Vertex(axisLength, 0, 0);

            gl.Color(0, 1f, 0, 0);
            gl.Vertex(0, -axisLength, 0);
            gl.Vertex(0, axisLength, 0);

            gl.Color(0, 0, 1f, 0);
            gl.Vertex(0, 0, -axisLength);
            gl.Vertex(0, 0, axisLength);

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

		public void InitializeObjects()
		{
			CustomCube cube = new CustomCube(gl, "Cube_1");
			cubes.Add(cube);
			sceneCollection.Add(cube);
		}

		#endregion

		#region --- Object Selection ---

		public ISelectable SelectElement(int x, int y, OpenGL gl)
		{
			// Convert the mouse coordinates to world coordinates
			Vector near = new Vector(gl.UnProject(x, y, 0));
			Vector far = new Vector(gl.UnProject(x, y, 1));
			Vector direction = (far - near).Normalize();

			ray = new Ray(near, direction, gl);
			selectingRay = new Ray(gl);
			selectingRay.Origin = near;

			ISelectable selectedObject = null;

			// Iterate over each object in the scene
			foreach (CustomCube cube in cubes)
			{

				//deselect all facets, edges and vertices before another selecting
				cube.DeselectAll();


				//check if any vertex is selected
				Vertex selectedVertex = CheckSelectedVertex(cube, ray);
				if (selectedVertex != null)
				{
					selectedObject = selectedVertex;
				}
				else
				{
					//check if any edge is selected
					Edge selectedEdge = CheckSelectedEdge(cube, ray);
					if (selectedEdge != null)
					{
						selectedObject = selectedEdge;
					}
					else
					{
						//check if any facet is selected
						Facet selectedFacet = CheckSelectedFacet(cube, ray);
						if (selectedFacet != null)
						{
							selectedObject = selectedFacet;
						}
					}
				}


				//check if any object is selected
				if(selectedObject != null)
				{
					if(SceneMode == SceneMode.VIEW)
					{
						foreach(Edge edge in cube.Mesh.Edges)
						{
							edge.IsSelected = true;
						}
					}
					else if(SceneMode == SceneMode.EDIT)
					{
						selectedObject.IsSelected = true;
					}
				}

			}

			return selectedObject;
		}

		public Facet CheckSelectedFacet(CustomCube cube, Ray ray)
		{
			double? minDistance = null; //minimal distance between facet and ray origin
			Facet selectedFacet = null; //facet that is selected

			for(int i = 0; i < cube.Mesh.Facets.Length; i++)
			{
				double? currentDistance = ray.RayIntersectsFacet(cube.Mesh.Facets[i], selectingRay);
				if (minDistance != null)
				{
					if(currentDistance < minDistance)
					{
						selectedFacet = cube.Mesh.Facets[i];
						minDistance = currentDistance;
					}
				}
				else if(currentDistance != null)
				{
					selectedFacet = cube.Mesh.Facets[i];
					minDistance = currentDistance;
				}
			}
			return selectedFacet;
		}

		public Edge CheckSelectedEdge(CustomCube cube, Ray ray)
		{
			double? minDistance = null; //minimal distance between edge and ray origin
			Edge selectedEdge = null; //edge that is selected
			Vertex intersectionPoint = null;

			for (int i = 0; i < cube.Mesh.Edges.Length; i++)
			{
				double? currentDistance = ray.RayIntersectsEdge(cube.Mesh.Edges[i], out intersectionPoint);
				if (minDistance != null)
				{
					if (currentDistance < minDistance)
					{
						selectedEdge = cube.Mesh.Edges[i];
						minDistance = currentDistance;
					}
				}
				else if (currentDistance != null)
				{
					selectedEdge = cube.Mesh.Edges[i];
					minDistance = currentDistance;
				}
			}

			if(intersectionPoint != null)
			{
				selectingRay.Direction = new Vector(intersectionPoint);
			}
			else
			{
				selectingRay.Direction = selectingRay.Origin;
			}
			return selectedEdge;
		}

		public Vertex CheckSelectedVertex(CustomCube cube, Ray ray)
		{
			double? minDistance = null; //minimal distance between vertex and ray origin
			Vertex selectedVertex = null; //vertex that is selected

			for (int i = 0; i < cube.Mesh.Vertices.Length; i++)
			{
				double? currentDistance = ray.RayIntersectsVertex(cube.Mesh.Vertices[i]);
				if (minDistance != null)
				{
					if (currentDistance < minDistance)
					{
						selectedVertex = cube.Mesh.Vertices[i];
						minDistance = currentDistance;
					}
				}
				else if (currentDistance != null)
				{
					selectedVertex = cube.Mesh.Vertices[i];
					minDistance = currentDistance;
				}
			}

			if (selectedVertex != null)
			{
				selectingRay.Direction = new Vector(selectedVertex);
			}
			else
			{
				selectingRay.Direction = selectingRay.Origin;
			}
			return selectedVertex;
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

		#endregion


		public void DeleteCompletely(CustomCube cube)
		{
			cubes.Remove(cube);
			sceneCollection.Remove(cube);
		}

		public void AddCube()
		{
			CustomCube cube = new CustomCube(gl, "Cube2");
			cubes.Add(cube);
			sceneCollection.Add(cube);
		}
	}

	public enum SceneMode { VIEW, EDIT};
}
