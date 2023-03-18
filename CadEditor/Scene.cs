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
        private List<CustomCube> cubes;
        public Ray ray;
		public Ray selectingRay;

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
			gl.LookAt(0, 0, 5, 0, 0, 0, 0, 1, 0);

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

		public void UpdateObject(CustomCube cube)
		{
			for(int i = 0; i < cube.Mesh.Facets.Length; i++)
			{
				cube.Mesh.Facets[i].IsSelected = false;
			}
		}

		#endregion

		#region --- Object Selection ---

		public void SelectElement(int x, int y, OpenGL gl)
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
				UpdateObject(cube);

				
				Facet selectedFacet = CheckSelectedFacet(cube, ray);
				if(selectedFacet != null)
				{
					selectedFacet.IsSelected = true;
					//selectedFacet.SelectedColor = Color.Pink;
				}
				
			}
		}

		public Facet CheckSelectedFacet(CustomCube cube, Ray ray)
		{
			double? minDistance = null; //minimal distance between facet and ray origin
			Facet selectedFacet = null; //facet that is selected

			for(int i = 0; i < cube.Mesh.Facets.Length; i++)
			{
				double? currentDistance = ray.RayIntersectsPlane(cube.Mesh.Facets[i], selectingRay);
				if (minDistance != null)
				{
					if(currentDistance < minDistance)
					{
						UpdateObject(cube);
						selectedFacet = cube.Mesh.Facets[i];
						minDistance = currentDistance;
					}
				}
				else if(currentDistance != null)
				{
					UpdateObject(cube);
					selectedFacet = cube.Mesh.Facets[i];
					minDistance = currentDistance;
				}
			}
			return selectedFacet;
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

		//Not implemented yet
		public void CheckSelectedEdge(CustomCube cube, Ray ray)
		{

		}

		public void CheckSelectedVertex(CustomCube cube, Ray ray)
		{

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
}
