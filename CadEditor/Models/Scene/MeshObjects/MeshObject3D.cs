using CadEditor.Models.Scene.MeshObjects;
using GeometRi;
using SharpGL;
using System;
using System.Drawing;
using System.Numerics;

namespace CadEditor.MeshObjects
{
	public class MeshObject3D : ISceneObject, IUniqueable
	{
		public string Name { get; set; }
		public Mesh Mesh { get; set; }

		public Vector Size { get; set; }
        public bool DrawFacets { get; set; } = true;
        public bool DrawEdges { get; set; } = true;
        public bool DrawVertices { get; set; } = true;
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

            //center point
            double allX = 0;
            double allY = 0;
            double allZ = 0;
            int verticesCount = mesh.Vertices.Count;

            for (int i = 0; i < verticesCount; i++)
            {
                allX += mesh.Vertices[i].X;
                allY += mesh.Vertices[i].Y;
                allZ += mesh.Vertices[i].Z;
            }

            centerPoint = new Point3D(allX / verticesCount,
                                      allY / verticesCount,
                                      allZ / verticesCount);
		}

        public MeshObject3D(MeshObject3D meshObjectToClone)
        {
            Name = meshObjectToClone.Name;
            Mesh = meshObjectToClone.Mesh.Clone();

            centerPoint = (Point3D)meshObjectToClone.centerPoint.Clone();
        }

		public void Draw()
		{

			//Draw Vertexes
            if (DrawVertices)
            {
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
            }

            //Draw Edges
            if (DrawEdges)
            {
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
            }

            //Draw Facets
            if (DrawFacets)
			{
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
			}

		}

		public void Move(Vector vector)
		{
			for (int i = 0; i < Mesh.Vertices.Count; i++)
			{
				Mesh.Vertices[i].Move(vector);
			}
			centerPoint.Move(vector);

            GraphicsGL.Control.Invalidate();
		}

		public void Select()
		{
			foreach (Line edge in Mesh.Edges)
			{
				edge.IsSelected = true;
			}

            IsSelected = true;
		}

		public void Deselect()
		{
            IsSelected = false;

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

        public (ISceneObject, double) CheckSelected()
        {
            //deselect all facets, edges and vertices before another selecting
            //Deselect();

            Ray ray = GraphicsGL.InitializeRay(MouseController.X, GraphicsGL.GetHeight() - MouseController.Y);
            Scene.selectingRay = ray;
            (ISceneObject, double) selectedVertex = CheckSelectedVertex(ray);
            (ISceneObject, double) selectedEdge = CheckSelectedEdge(ray);
            (ISceneObject, double) selectedFacet = CheckSelectedFacet(ray);

            //Get the closest object
            double minDistance = 0;
            if (selectedVertex.Item1 != null)
            {
                minDistance = selectedVertex.Item2;
            }

            if (selectedEdge.Item1 != null)
            {
                if (minDistance != 0 && selectedEdge.Item2 < minDistance)
                {
                    minDistance = selectedEdge.Item2;
                }
                else if (minDistance == 0)
                {
                    minDistance = selectedEdge.Item2;
                }
            }

            if (selectedFacet.Item1 != null)
            {
                if (minDistance != 0 && selectedFacet.Item2 < minDistance)
                {
                    minDistance = selectedFacet.Item2;
                }
                else if (minDistance == 0)
                {
                    minDistance = selectedFacet.Item2;
                }
            }

            if (minDistance != 0)
            {
                if (minDistance == selectedVertex.Item2) return (selectedVertex.Item1, selectedVertex.Item2);
                if (minDistance == selectedEdge.Item2) return (selectedEdge.Item1, selectedEdge.Item2);
                if (minDistance == selectedFacet.Item2) return (selectedFacet.Item1, selectedFacet.Item2);
            }

            return (null, 0);
        }

        public (Line, double) CheckSelectedEdge(Ray ray)
        {
            double minDistance = 0; //minimal distance between edge and ray origin
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
                    double distanceToPoint = GetDistance(intersectionPoint, new Point3D(ray.Origin[0],
                                                                                        ray.Origin[1],
                                                                                        ray.Origin[2]));

                    if (minDistance != 0)
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
                return (null, 0);
            }

            return (selectedEdge, minDistance);
        }

        public (Point3D, double) CheckSelectedVertex(Ray ray)
        {
            double minDistance = 0; //minimal distance between vertex and ray origin
            Point3D selectedVertex = null; //vertex that is selected

            for (int i = 0; i < Mesh.Vertices.Count; i++)
            {
                Point3D currentVertex = Mesh.Vertices[i];
                Sphere sphere = new Sphere(new Point3d(currentVertex.X, currentVertex.Y, currentVertex.Z), 0.2);

                Ray3d newRay = new Ray3d(new Point3d(ray.Origin[0], ray.Origin[1], ray.Origin[2]),
                                         new Vector3d(ray.Direction[0], ray.Direction[1], ray.Direction[2]));
                if (newRay.IntersectionWith(sphere) != null)
                {
                    selectedVertex = currentVertex;
                    double distanceToPoint = GetDistance(currentVertex, new Point3D(ray.Origin[0],
                                                                                    ray.Origin[1],
                                                                                    ray.Origin[2]));

                    if (minDistance != 0)
                    {
                        if (distanceToPoint < minDistance)
                        {
                            minDistance = distanceToPoint;
                            selectedVertex = currentVertex;
                        }
                    }
                    else
                    {
                        minDistance = distanceToPoint;
                        selectedVertex = currentVertex;
                    }
                }
            }

            return (selectedVertex, minDistance);
        }

        public (Plane, double) CheckSelectedFacet(Ray ray)
        {
            var result = GetPlaneIntersection(ray);

            if (!IsOnClickableArea(result.Item1, result.Item2))
            {
                return (null, 0);
            }

            //double distance = GetDistance(new Point3D(ray.Origin), result.Item2);

            //Vector3 cameraPos = Scene.GetInstance().Camera.Position;
            //double distance = GetDistance(result.Item2, new Point3D(cameraPos.X, cameraPos.Y, cameraPos.Z));

            double distance = GetDistance(result.Item2, new Point3D(Scene.selectingRay.Origin[0],
                                                                    Scene.selectingRay.Origin[1],
                                                                    Scene.selectingRay.Origin[2]));


            return (result.Item1, distance);
        }

        private (Plane, Point3D) GetPlaneIntersection(Ray ray)
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
                    double distanceToPoint = GetDistance(intersectionPoint, new Point3D(ray.Origin[0],
                                                                                        ray.Origin[1],
                                                                                        ray.Origin[2]));

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
            }            return false;
        }

        private double GetDistance(Point3D v1, Point3D v2)
        {            
            return Math.Sqrt(Math.Pow((v2.X - v1.X), 2) + Math.Pow((v2.Y - v1.Y), 2) + Math.Pow((v2.Z - v1.Z), 2));
        }

        public Point3D GetCenterPoint() 
        {
            return centerPoint;
        }


        public void SetDefaultColors()
        {
            EdgeSelectedColor = Color.Red;
            EdgeNonSelectedColor = Color.Black;
        }

        public bool IsEqual(ISceneObject obj)
        {
            if (obj != null && obj is MeshObject3D)
            {
                MeshObject3D meshObject = (MeshObject3D)obj;

                for (int i = 0; i < meshObject.Mesh.Vertices.Count; i++)
                {
                    if (!this.Mesh.Vertices[i].IsEqual(meshObject.Mesh.Vertices[i]))
                    {
                        return false;
                    }
                }

                for (int i = 0; i < meshObject.Mesh.Edges.Count; i++)
                {
                    if (!this.Mesh.Edges[i].IsEqual(meshObject.Mesh.Edges[i]))
                    {
                        return false;
                    }
                }

                for (int i = 0; i < meshObject.Mesh.Facets.Count; i++)
                {
                    if (!this.Mesh.Facets[i].IsEqual(meshObject.Mesh.Facets[i]))
                    {
                        return false;
                    }

                }

                return true;
            }

            return false;
        }

        public virtual ISceneObject Clone()
        {
            return new MeshObject3D(this);
        }
    }
}
