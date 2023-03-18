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
        private OpenGL gl;
        private List<CustomCube> cubes;
        public double[,] line;
        public Ray ray;
		public Ray lineRay;
        private double cameraDistance = 6;

		public Camera Camera
		{
			get { return camera; }
		}

		public Scene(OpenGL _gl, Camera _camera)
        {
			gl = _gl;
			camera = _camera;
			//Get the modelview
			double[] modelview = new double[16];
			gl.GetDouble(OpenGL.GL_MODELVIEW_MATRIX, modelview);

            cubes = new List<CustomCube>();

            line = new double[2, 3]
            {
                {2, 2, 6 },
                {0, 0, 0 }
            };

        }

        public void RotateCameraX()
        {
            camera.RotateAxisX();
        }

        public void RotateCameraY()
        {
            camera.RotateAxisY();
        }

        public void ChangeAxisX(double cameraAngle)
        {
            camera.utri += cameraAngle;
		}

        public void ChangeAxisY(double cameraAngle)
        {
            camera.rtri += cameraAngle;
		}

		public void DrawScene(int Width, int Height)
        {
            gl.Clear(OpenGL.GL_COLOR_BUFFER_BIT | OpenGL.GL_DEPTH_BUFFER_BIT);
            gl.LoadIdentity();
            gl.Translate(0.0f, 0.0f, 0.0f);

            RotateCameraX();
            RotateCameraY();

            DrawCordinateAxes(3.0f, 20);
            DrawCube(Width, Height);
            if (line[1, 0] != 0)
            {
                DrawLine();
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

        public void DrawObjects()
        {
            foreach (CustomCube cube in cubes)
            {
                cube.Draw();
            }
        }

        public void InitObjects()
        {
            cubes.Add(new CustomCube(gl));
        }

        public void AddObject(CustomCube c)
        {
            cubes.Add(c);
        }

        public void DrawLine()
        {
            gl.Color(0, 0, 1);
            gl.Begin(OpenGL.GL_LINES);

            for (int i = 0; i < line.GetLength(0); i++)
            {
                gl.Vertex(line[i, 0], line[i, 1], line[i, 2]);
            }
            gl.End();
        }


        public void CreateRay(int mouseX, int mouseY, Camera camera)
        {
			// Get the viewport dimensions
			int[] viewport = new int[4];
			gl.GetInteger(OpenGL.GL_VIEWPORT, viewport);

            //Get the modelview
			double[] modelview = new double[16];
			gl.GetDouble(OpenGL.GL_MODELVIEW_MATRIX, modelview);

            //Get the projection view
			double[] projection = new double[16];
			gl.GetDouble(OpenGL.GL_PROJECTION_MATRIX, projection);

            // Calculate the mouse coordinates in world space
            double mouseWorldX = 0;
            double mouseWorldY = 0;
            double mouseWorldZ = 0;
			gl.UnProject(mouseX, viewport[3] - mouseY, 0.0, modelview, projection, viewport, ref mouseWorldX, ref mouseWorldY, ref mouseWorldZ);

            double[] mouseWorld = new double[] { mouseWorldX, mouseWorldY, mouseWorldZ };
            // Create a ray from the camera position to the mouse position


            //double cameraX = -modelview[12];
            //double cameraY = -modelview[13];
            //double cameraZ = -modelview[14];

            //double cameraAngle = ;
            //double cameraDistance = 6;
            //double newX = cameraDistance * Math.Sin(cameraAngle);
            //double newZ = cameraDistance * Math.Cos(cameraAngle);

            //// Update the camera position
            //cameraX = newX;
            //cameraZ = newZ;

            Vector cameraPosition = camera.GetPosition();
            Console.WriteLine("Camera pos: ({0}, {1}, {2})", cameraPosition[0], cameraPosition[1], cameraPosition[2]);

			double[] rayDirection = new double[] { mouseWorld[0] - cameraPosition[0], mouseWorld[1] - cameraPosition[1], mouseWorld[2] - cameraPosition[2]};
			Normalize(rayDirection);

			line[0, 0] = cameraPosition[0];
			line[0, 1] = cameraPosition[1];
			line[0, 2] = cameraPosition[2];

			line[1, 0] = rayDirection[0];
            line[1, 1] = rayDirection[1];
            line[1, 2] = rayDirection[2];
		}

		private void Normalize(double[] v)
		{
			double length = Math.Sqrt(v[0] * v[0] + v[1] * v[1] + v[2] * v[2]);
			v[0] /= length;
			v[1] /= length;
			v[2] /= length;
		}


		public void DrawCube(int Width, int Height)
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

			RotateCameraX();
			RotateCameraY();

			DrawCordinateAxes(3.0f, 20);
            //DrawLine();
            if(lineRay != null)
            {
				DrawLine(lineRay.GetStartPosition(), lineRay.GetEndPosition());
			}

			foreach(var cube in cubes)
			{
				cube.DrawComplete();
			}

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

        public Vector GetMouseWorldCoords(int mouseX, int mouseY)
        {
			// Get the viewport dimensions
			int[] viewport = new int[4];
			gl.GetInteger(OpenGL.GL_VIEWPORT, viewport);

			// Get the projection matrix
			double[] projection = new double[16];
			gl.GetDouble(OpenGL.GL_PROJECTION_MATRIX, projection);

			// Get the model-view matrix
			double[] modelview = new double[16];
			gl.GetDouble(OpenGL.GL_MODELVIEW_MATRIX, modelview);

			// Initialize the mouse coordinates with depth value of 0.5
			double mouseXf = mouseX;
			double mouseYf = viewport[3] - mouseY - 1;
			double mouseZf = 5;

            // Use the unprojection function to convert the mouse coordinates to world space
            double worldX = 0, worldY = 0, worldZ = 0;
			gl.UnProject(mouseXf, mouseYf, mouseZf, modelview, projection, viewport, ref worldX, ref worldY, ref worldZ);

            return new Vector(new double[] { worldX, worldY, worldZ });
        }

  //      public CustomCube SelectObject(int mouseX, int mouseY, Camera camera, OpenGLControl gLControl)
  //      {
  //          Console.WriteLine("Mouse Click Coord: {0}, {1}", mouseX, mouseY);
		//	// Get the camera position and direction vectors
		//	Vector cameraPosition = camera.GetPosition();
		//	Vector cameraDirection = camera.GetDirection();

		//	// Convert mouse coordinates to world coordinates
		//	Vector mouseWorldCoords = GetMouseWorldCoords(mouseX, mouseY);
		//	Console.WriteLine("World Mouse Coord: {0}, {1}", mouseWorldCoords[0], mouseWorldCoords[1], mouseWorldCoords[2]);

  //          // Create a ray from the camera position and the mouse click position
  //          Vector direction = mouseWorldCoords - cameraPosition;
		//	ray = new Ray(cameraPosition, new Vector(new double[] { direction[0], direction[1], direction[2] }), gl);
  //          ray.Normalize();
  //          Console.WriteLine("Ray origin: " + ray.GetStartPosition());

  //          Vector cameraPos = new Vector(3);
		//	double[] modelview = new double[16];
		//	gl.GetDouble(OpenGL.GL_MODELVIEW_MATRIX, modelview);
		//	cameraPos[0] = -modelview[12];
		//	cameraPos[1] = -modelview[13];
		//	cameraPos[2] = -modelview[14];
  //          Console.Write(cameraPos);


		//	// Iterate over all objects in the scene and check for intersections
		//	foreach (CustomCube cube in cubes)
  //          {
  //              if (ray.IntersectsCube(cube))
  //              {
  //                  return cube;
  //              }
  //          }

  //          return null;
		//}


		public void DrawRayToMouse(int mouseX, int mouseY)
		{

			// Get the current position of the camera
			double[] cameraPosition = new double[3];
			gl.GetDouble(OpenGL.GL_MODELVIEW_MATRIX, cameraPosition);

			// Get the viewport and projection matrices
			int[] viewport = new int[4];
			double[] projection = new double[16];
			gl.GetInteger(OpenGL.GL_VIEWPORT, viewport);
			gl.GetDouble(OpenGL.GL_PROJECTION_MATRIX, projection);

			// Calculate the mouse position in 3D space
			mouseY = viewport[3] - mouseY - 1;
			double mousePosX = 0;
			double mousePosY = 0;
			double mousePosZ = 0;
			gl.UnProject(mouseX, mouseY, 0, cameraPosition, projection, viewport, ref mousePosX, ref mousePosY, ref mousePosZ);

			// Draw the ray from the camera position to the mouse position
			ray = new Ray(new Vector(new double[] { cameraPosition[0], cameraPosition[1], cameraPosition[2] }),
							new Vector(new double[] { mousePosX, mousePosY, mousePosZ }), gl);
		}



		//public virtual IEnumerable<SceneElement> DoHitTest(int x, int y)
		//{
		//	//  Create a result set.
		//	List<SceneElement> resultSet = new List<SceneElement>();

		//	//  Create a hitmap.
		//	Dictionary<uint, SceneElement> hitMap = new Dictionary<uint, SceneElement>();

		//	//	If we don't have a current camera, we cannot hit test.
		//	if (camera == null)
		//		return resultSet;

		//	//	Create an array that will be the viewport.
		//	int[] viewport = new int[4];

		//	//	Get the viewport, then convert the mouse point to an opengl point.
		//	gl.GetInteger(OpenGL.GL_VIEWPORT, viewport);
		//	y = viewport[3] - y;

		//	//	Create a select buffer.
		//	uint[] selectBuffer = new uint[512];
		//	gl.SelectBuffer(512, selectBuffer);

		//	//	Create a feedback buffer.
		//	//float[] feedbackBuffer = new float[512];
		//	//gl.FeedbackBuffer(512, OpenGL.GL_3D,feedbackBuffer);

		//	//	Enter select mode.
		//	gl.RenderMode(OpenGL.GL_SELECT);

		//	//	Initialise the names, and add the first name.
		//	gl.InitNames();
		//	gl.PushName(0);

		//	//	Push matrix, set up projection, then load matrix.
		//	gl.MatrixMode(OpenGL.GL_PROJECTION);
		//	gl.PushMatrix();
		//	gl.LoadIdentity();
		//	gl.PickMatrix(x, y, 4, 4, viewport);
		//	camera.TransformProjectionMatrix(gl);
		//	gl.MatrixMode(OpenGL.GL_MODELVIEW);
		//	gl.LoadIdentity();

		//	//  Create the name.
		//	uint currentName = 1;

		//	//  Render the root for hit testing.
		//	RenderElementForHitTest(SceneContainer, hitMap, ref currentName);

		//	//	Pop matrix and flush commands.
		//	gl.MatrixMode(OpenGL.GL_PROJECTION);
		//	gl.PopMatrix();
		//	gl.MatrixMode(OpenGL.GL_MODELVIEW);
		//	gl.Flush();

		//	//	End selection.
		//	int hits = gl.RenderMode(OpenGL.GL_RENDER);
		//	uint posinarray = 0;

		//	//  Go through each name.
		//	for (int hit = 0; hit < hits; hit++)
		//	{
		//		uint nameCount = selectBuffer[posinarray++];
		//		uint z1 = selectBuffer[posinarray++];
		//		uint z2 = selectBuffer[posinarray++];

		//		if (nameCount == 0)
		//			continue;

		//		//	Add each hit element to the result set to the array.
		//		for (int name = 0; name < nameCount; name++)
		//		{
		//			uint hitName = selectBuffer[posinarray++];
		//			resultSet.Add(hitMap[hitName]);
		//		}
		//	}

		//	//  Return the result set.
		//	return resultSet;
		//}


		private void RenderElementForHitTest(SceneElement sceneElement,
			Dictionary<uint, SceneElement> hitMap, ref uint currentName)
		{
			//  If the element is disabled, we're done.
			//  Also, never hit test the current camera.
			if (sceneElement.IsEnabled == false || sceneElement == camera)
				return;

			//  Push each effect.
			foreach (var effect in sceneElement.Effects)
				if (effect.IsEnabled)
					effect.Push(gl, sceneElement);

			//  If the element has an object space, transform into it.
			if (sceneElement is IHasObjectSpace)
				((IHasObjectSpace)sceneElement).PushObjectSpace(gl);

			//  If the element is volume bound, render the volume.
			if (sceneElement is IVolumeBound)
			{
				//  Load and map the name.
				gl.LoadName(currentName);
				hitMap[currentName] = sceneElement;

				//  Render the bounding volume.
				((IVolumeBound)sceneElement).BoundingVolume.Render(gl, RenderMode.HitTest);

				//  Increment the name.
				currentName++;
			}

			//  Recurse through the children.
			foreach (var childElement in sceneElement.Children)
				RenderElementForHitTest(childElement, hitMap, ref currentName);

			//  If the element has an object space, transform out of it.
			if (sceneElement is IHasObjectSpace)
				((IHasObjectSpace)sceneElement).PopObjectSpace(gl);

			//  Pop each effect.
			for (int i = sceneElement.Effects.Count - 1; i >= 0; i--)
				if (sceneElement.Effects[i].IsEnabled)
					sceneElement.Effects[i].Pop(gl, sceneElement);
		}


		//public void AddObjectsToSceneContainer(SceneContainer _sceneContainer)
		//{
		//	sceneContainer = _sceneContainer;
		//	CustomCube cube = new CustomCube(gl);
		//	sceneContainer.AddChild(cube);
		//	cubes.Add(cube);
		//}


		public void InitializeObjects()
		{
			CustomCube cube = new CustomCube(gl);
			cubes.Add(cube);
		}

		public void UpdateObject(CustomCube cube)
		{
			for(int i = 0; i < cube.Mesh.Facets.Length; i++)
			{
				cube.Mesh.Facets[i].IsSelected = false;
			}
		}

		public void SelectElement(int x, int y, OpenGL gl)
		{
			// Convert the mouse coordinates to world coordinates
			Vector near = new Vector(gl.UnProject(x, y, 0));
			Vector far = new Vector(gl.UnProject(x, y, 1));
			Vector direction = Vector.Normalize(far - near);

			ray = new Ray(near, direction, gl);
			lineRay = new Ray(gl);
			lineRay.Origin = near;

			// Iterate over each object in the scene

			Console.WriteLine("------------------------------");

			foreach (CustomCube cube in cubes)
			{
				UpdateObject(cube);

				// Perform a more precise intersection test (e.g., with the object's mesh)
				// TODO: Implement this part
				SelectFacet(cube, ray);
			}
		}

		public void SelectFacet(CustomCube cube, Ray ray)
		{
			double? minDistance = null;
			for(int i = 0; i < cube.Mesh.Facets.Length; i++)
			{
				double? currentDistance = ray.RayIntersectsPlane(cube.Mesh.Facets[i], lineRay);
				if (minDistance != null)
				{
					if(currentDistance < minDistance)
					{
						UpdateObject(cube);
						cube.Mesh.Facets[i].IsSelected = true;
						minDistance = currentDistance;
					}
				}
				else
				{
					UpdateObject(cube);
					minDistance = currentDistance;
					cube.Mesh.Facets[i].IsSelected = true;
				}
			}
		}
	}
}
