using CadEditor.Graphics;
using SharpGL;
using SharpGL.SceneGraph.Primitives;
using SharpGL.SceneGraph.Raytracing;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CadEditor.MeshObjects
{
	public class MeshObject3D : ISceneObject
	{
		public string Name { get; set; }
		public Mesh Mesh { get; set; }

		public Vector Size { get; set; }
		public bool DrawFacets { get; set; }
        private Point3D centerPoint { get; set; }
        public ISceneObject ParentObject { get; set; }
        public bool IsSelected { get; set; }
        private readonly double facetTolerance = 0.08;


        public Color VertexSelectedColor = Color.Red;
		public Color EdgeSelectedColor = Color.Red;
		public Color FacetSelectedColor = Color.LightGray;
		public Color VertexNonSelectedColor = Color.Black;
		public Color EdgeNonSelectedColor = Color.Black;
		public Color FacetNonSelectedColor = Color.LightGray;

		public MeshObject3D(Point3D _centerPoint, Vector _size, string _cubeName)
		{
			Name = _cubeName;
			Mesh = new Mesh();
			centerPoint = _centerPoint;

			if(_size != null)
			{
                Size = new Vector(_size.Size);
                Size[0] = (_size[0] != 0) ? (double)_size[0] : 1.0;
                Size[1] = (_size[1] != 0) ? (double)_size[1] : 1.0;
                Size[2] = (_size[2] != 0) ? (double)_size[2] : 1.0;
            }
		}

		public MeshObject3D(Mesh mesh, OpenGL _GL = null)
		{
			Name = "CubeName";
			Mesh = mesh;
		}

		public void Draw()
		{

			//Draw Vertexes
			GraphicsGL.GL.PointSize(7.0f);
            GraphicsGL.GL.Begin(OpenGL.GL_POINTS);
			for (int i = 0; i < Mesh.Vertices.Count; i++)
			{
				if (IsSelected)
				{
					Mesh.Vertices[i].SelectedColor = VertexSelectedColor;
				}
				else
				{
					Mesh.Vertices[i].NonSelectedColor = VertexNonSelectedColor;
				}

				Mesh.Vertices[i].Draw();
			}
            GraphicsGL.GL.End();
            GraphicsGL.GL.Flush();


            //Draw Edges
            GraphicsGL.GL.Begin(OpenGL.GL_LINES);
			for (int i = 0; i < Mesh.Edges.Count; i++)
			{
				if (IsSelected)
				{
					Mesh.Edges[i].SelectedColor = EdgeSelectedColor;
				}
				else
				{
					Mesh.Edges[i].NonSelectedColor = EdgeNonSelectedColor;
				}

				Mesh.Edges[i].Draw();
			}
            GraphicsGL.GL.End();
            GraphicsGL.GL.Flush();


			//Draw Facets
			if (DrawFacets)
			{
                GraphicsGL.GL.Begin(OpenGL.GL_POLYGON);
				for (int i = 0; i < Mesh.Facets.Count; i++)
				{
					if (IsSelected)
					{
						Mesh.Facets[i].SelectedColor = FacetSelectedColor;
					}
					else
					{
						Mesh.Facets[i].NonSelectedColor = FacetNonSelectedColor;
					}

					Mesh.Facets[i].Draw();
				}
                GraphicsGL.GL.End();
                GraphicsGL.GL.Flush();
			}
		}

		public void Move(Vector vector)
		{
			for (int i = 0; i < Mesh.Vertices.Count; i++)
			{
				Mesh.Vertices[i].Move(vector);
			}
			centerPoint.Move(vector);
		}

		public void Select()
		{
			foreach (Line edge in Mesh.Edges)
			{
				edge.IsSelected = true;
			}
		}

		public void Deselect()
		{
			foreach (Plane facet in Mesh.Facets)
			{
				facet.IsSelected = false;
			}

			foreach (Line edge in Mesh.Edges)
			{
				edge.IsSelected = false;
			}

			foreach (Point3D vertex in Mesh.Vertices)
			{
				vertex.IsSelected = false;
			}

		}

        public ISceneObject CheckSelected(Ray ray)
        {
            //deselect all facets, edges and vertices before another selecting
            Deselect();

            //check if any vertex is selected
            Point3D selectedVertex = CheckSelectedVertex(ray);
            if (selectedVertex != null)
            {
                return selectedVertex;
            }

            //check if any edge is selected
            Line selectedEdge = CheckSelectedEdge(ray);
            if (selectedEdge != null)
            {
                return selectedEdge;
            }

            //check if any facet is selected
            Plane selectedFacet = CheckSelectedFacet(ray);
            if (selectedFacet != null)
            {
                return selectedFacet;
            }

            return null;
        }

        public Line CheckSelectedEdge(Ray ray)
        {
            double? minDistance = null; //minimal distance between edge and ray origin
            Line selectedEdge = null; //edge that is selected
            Point3D minIntersectionPoint = null;

            for (int i = 0; i < Mesh.Edges.Count; i++)
            {
                Line currentEdge = Mesh.Edges[i];
                Point3D intersectionPoint = ray.RayIntersectsLine(currentEdge);

                //check if edge contains intersection point
                if (intersectionPoint != null && currentEdge.Contains(intersectionPoint))
                {
                    //compare distances from ray origin to intersection point
                    double distanceToPoint = GetDistance(intersectionPoint, new Point3D(ray.Origin));

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

            if (!IsOnClickableArea(selectedEdge, minIntersectionPoint))
            {
                return null;
            }

            return selectedEdge;
        }

        public Point3D CheckSelectedVertex(Ray ray)
        {
            double? minDistance = null; //minimal distance between vertex and ray origin
            Point3D selectedVertex = null; //vertex that is selected
            Point3D minIntersectionPoint = null;

            for (int i = 0; i < Mesh.Vertices.Count; i++)
            {
                Point3D currentVertex = Mesh.Vertices[i];
                Point3D intersectionPoint = ray.RayIntersectsVertex(currentVertex);

                //check if edge contains intersection point
                if (intersectionPoint != null)
                {
                    //compare distances from ray origin to intersection point
                    double distanceToPoint = GetDistance(intersectionPoint, new Point3D(ray.Origin));

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

            //if(minIntersectionPoint != null)
            //{
            //             selectingRay.Direction = new Vector(minIntersectionPoint);
            //         }

            return selectedVertex;
        }

        private (ISceneObject, Point3D) GetIntersectionPoint(Ray ray)
        {
            double? minDistance = null; //minimal distance between facet and ray origin
            Plane selectedFacet = null; //facet that is selected
            Point3D minIntersectionPoint = null;

            for (int i = 0; i < Mesh.Facets.Count; i++)
            {
                Plane currentFacet = Mesh.Facets[i];
                Point3D intersectionPoint = ray.RayIntersectsPlane(currentFacet);

                //check if facet contains intersection point
                if (intersectionPoint != null && currentFacet.Contains(intersectionPoint))
                {
                    //compare distances from ray origin to intersection point
                    double distanceToPoint = GetDistance(intersectionPoint, new Point3D(ray.Origin));

                    if (minDistance != null)
                    {
                        if (distanceToPoint < minDistance)
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

            return (selectedFacet, minIntersectionPoint);
        }

        private bool IsOnClickableArea(ISceneObject element, Point3D intersection)
        {
            //check if clickable area of facet contains the intersection point
            if (element != null && intersection != null)
            {
                if (element is Line)
                {
                    Line line = ((Line)element).GetClickableArea(facetTolerance);
                    if (!line.Contains(intersection))
                    {
                        return false;
                    }
                }
                else if (element is Plane)
                {
                    Plane plane = ((Plane)element).GetClickableArea(facetTolerance);
                    if (!plane.Contains(intersection))
                    {
                        return false;
                    }
                }

                return true;

                //selectingRay.Direction = new Vector(minIntersectionPoint);
            }

            return false;
        }


        public Plane CheckSelectedFacet(Ray ray)
        {
            var result = GetIntersectionPoint(ray);

            if (!IsOnClickableArea(result.Item1, result.Item2))
            {
                return null;
            }

            return (Plane)result.Item1;
        }


        private double GetDistance(Point3D v1, Point3D v2)
        {
            return Math.Sqrt(Math.Pow((v2.X - v1.X), 2) + Math.Pow((v2.Y - v1.Y), 2) + Math.Pow((v2.Z - v1.Z), 2));
        }

        public Point3D GetCenterPoint()
        {
            return centerPoint;
        }
    }
}
