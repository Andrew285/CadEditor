using SharpGL;
using System;
using System.Collections.Generic;
using System.Drawing;

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

		public float selectedObjAxisLength { get; set; }
		private Axis[] selectingCoordinateAxes;
		private AxisCube[] selectingCoordinateCubes;

		private readonly double facetTolerance = 0.08;
		private readonly double edgeTolerance = 0.08;

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
			CustomCube cube = new CustomCube(gl, new Vertex(0, 0, 0, gl), 1f, "Cube_1");
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

			Axis axisX = new Axis(new Vertex(v.X, v.Y, v.Z, gl), new Vertex(axisXYZLengths[0] + v.X, v.Y, v.Z, gl), CoordinateAxis.X, gl);
			Axis axisY = new Axis(new Vertex(v.X, v.Y, v.Z, gl), new Vertex(v.X, axisXYZLengths[1] + v.Y, v.Z, gl), CoordinateAxis.Y, gl);
			Axis axisZ = new Axis(new Vertex(v.X, v.Y, v.Z, gl), new Vertex(v.X, v.Y, axisXYZLengths[2] + v.Z, gl), CoordinateAxis.Z, gl);

			axisX.LineWidth = lineWidth;
			axisY.LineWidth = lineWidth;
			axisZ.LineWidth = lineWidth;
			selectingCoordinateAxes = new Axis[] { axisX, axisY, axisZ };

			//Create Axis Cubes
			AxisCube cubeX = new AxisCube(gl, new Vertex(axisXYZLengths[0] + v.X, v.Y, v.Z, gl), CoordinateAxis.X, 0.1f, "cubeAxisX");
			AxisCube cubeY = new AxisCube(gl, new Vertex(v.X, axisXYZLengths[1] + v.Y, v.Z, gl), CoordinateAxis.Y, 0.1f, "cubeAxisY");
			AxisCube cubeZ = new AxisCube(gl, new Vertex(v.X, v.Y, axisXYZLengths[2] + v.Z, gl), CoordinateAxis.Z, 0.1f, "cubeAxisZ");

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
			DrawCordinateAxes(new Vertex(0, 0, 0, gl), 3.0, 20);

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

			ISelectable selectedCube = null;
			ISelectable selectedObject = null;

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
						return selectingCoordinateCubes[i];
					}
				}
			}

            // Iterate over each object in the scene
            foreach (CustomCube cube in cubes)
            {
                //deselect all facets, edges and vertices before another selecting
                cube.Deselect();

                //check if any facet is selected
                Facet selectedFacet = CheckSelectedFacet(cube, ray);
                if (selectedFacet != null)
                {
                    selectedObject = selectedFacet;
                    selectedCube = cube;
                    break;
                }
                Console.WriteLine("\nFacet: " + selectedFacet);

                //check if any edge is selected
                Edge selectedEdge = CheckSelectedEdge(cube, ray);
                if (selectedEdge != null)
                {
                    selectedObject = selectedEdge;
                    selectedCube = cube;
                    break;
                }

                //check if any vertex is selected
                Vertex selectedVertex = CheckSelectedVertex(cube, ray);
                if (selectedVertex != null)
                {
                    selectedObject = selectedVertex;
                    selectedCube = cube;
                    break;
                }
            }

            if (selectedObject != null && SceneMode == SceneMode.VIEW)
            {
                selectedObject = selectedCube;
            }

            return selectedObject;
        }

		public Facet CheckSelectedFacet(CustomCube cube, Ray ray)
		{
			double? minDistance = null; //minimal distance between facet and ray origin
			Facet selectedFacet = null; //facet that is selected
			Vertex minIntersectionPoint = null;

			for(int i = 0; i < cube.Mesh.Facets.Length; i++)
			{
				Facet currentFacet = cube.Mesh.Facets[i];
                Vertex intersectionPoint = ray.RayIntersectsPlane(currentFacet);

				//check if facet contains intersection point
				if (intersectionPoint != null && currentFacet.Contains(intersectionPoint))
				{
					//compare distances from ray origin to intersection point
                    double distanceToPoint = GetDistance(intersectionPoint, new Vertex(ray.Origin));
                    
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
                Facet facetArea = selectedFacet.GetClickableFacet(facetTolerance);
				if (!facetArea.Contains(minIntersectionPoint))
				{
					return null;
				}

				selectingRay.Direction = new Vector(minIntersectionPoint);
            }

            return selectedFacet;
		}

		public Edge CheckSelectedEdge(CustomCube cube, Ray ray)
		{
			double? minDistance = null; //minimal distance between edge and ray origin
			Edge selectedEdge = null; //edge that is selected
			Vertex minIntersectionPoint = null;

			for (int i = 0; i < cube.Mesh.Edges.Length; i++)
			{
                Edge currentEdge = cube.Mesh.Edges[i];
                Vertex intersectionPoint = ray.RayIntersectsLine(currentEdge);

                //check if edge contains intersection point
                if (intersectionPoint != null && currentEdge.Contains(intersectionPoint))
                {
                    //compare distances from ray origin to intersection point
                    double distanceToPoint = GetDistance(intersectionPoint, new Vertex(ray.Origin));
                    
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
                Edge edgeArea = selectedEdge.GetClickableEdge(edgeTolerance);
                if (!edgeArea.Contains(minIntersectionPoint))
                {
                    return null;
                }

                selectingRay.Direction = new Vector(minIntersectionPoint);
            }

            return selectedEdge;
		}

		public Vertex CheckSelectedVertex(CustomCube cube, Ray ray)
		{
			double? minDistance = null; //minimal distance between vertex and ray origin
			Vertex selectedVertex = null; //vertex that is selected
            Vertex minIntersectionPoint = null;

			for (int i = 0; i < cube.Mesh.Vertices.Length; i++)
			{
                Vertex currentVertex = cube.Mesh.Vertices[i];
                Vertex intersectionPoint = ray.RayIntersectsVertex(currentVertex);

                //check if edge contains intersection point
                if (intersectionPoint != null)
                {
                    //compare distances from ray origin to intersection point
                    double distanceToPoint = GetDistance(intersectionPoint, new Vertex(ray.Origin));

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
					Vertex intersectionPoint = ray.RayIntersectsLine(line);

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
			CustomCube cube = new CustomCube(gl, new Vertex(0, 0, 0, gl), null, "Cube2");
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

        private double GetDistance(Vertex v1, Vertex v2)
        {
            return Math.Sqrt(Math.Pow((v2.X - v1.X), 2) + Math.Pow((v2.Y - v1.Y), 2) + Math.Pow((v2.Z - v1.Z), 2));
        }

        #endregion
    }

    public enum SceneMode { VIEW, EDIT};
	public enum CoordinateAxis { X, Y, Z }
}
