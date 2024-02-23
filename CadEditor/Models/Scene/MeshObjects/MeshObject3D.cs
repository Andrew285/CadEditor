using CadEditor.Models.Scene.MeshObjects;
using GeometRi;
using SharpGL;
using System;
using System.Collections.Generic;
using System.Drawing;

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

        public ISceneObject CheckSelected()
        {
            //deselect all facets, edges and vertices before another selecting
            Deselect();

            Ray ray = GraphicsGL.InitializeRay(MouseController.X, GraphicsGL.GetHeight() - MouseController.Y);
            Vector origin = ray.Origin;
            Vector dir = ray.Direction;

            Scene.selectingRay = new Ray3d(new Point3d(origin[0], origin[1], origin[2]), new Vector3d(dir[0], dir[1], dir[2]));


            //check if any vertex is selected
            //Point3D selectedVertex2 = CheckSelectedVertex(ray);

            //Vector3d vector = new Vector3d(MouseController.X, GraphicsGL.GetHeight() - MouseController.Y, 0);
            //Vector3d global = vector
            ISceneObject clickedObject;

            (Point3D, double) selectedVertex = CheckSelectedVertex(ray);
            //if (selectedVertex.Item1 != null)
            //{
            //    //minDistance = selectedVertex.Item2;
            //    //clickedObject = selectedVertex.Item1;

            //    return selectedVertex;
            //}

            //check if any edge is selected
            (Line, double) selectedEdge = CheckSelectedEdge(ray);

            //if (selectedEdge != null)
            //{
            //    return selectedEdge;
            //}

            //check if any facet is selected
            Plane selectedFacet = CheckSelectedFacet(ray);
            //if (selectedFacet != null)
            //{
            //    return selectedFacet;
            //}

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

            if (minDistance != 0)
            {
                if (minDistance == selectedVertex.Item2) return selectedVertex.Item1;
                if (minDistance == selectedEdge.Item2) return selectedEdge.Item1;
            }
            else
            {
                return selectedFacet;
            }

            return null;
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
                    double distanceToPoint = GetDistance(intersectionPoint, new Point3D(ray.Origin));

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

        //public (Line, double) CheckSelectedEdgeNEW(Ray3d ray)
        //{
        //    Line line = Mesh.Edges[0];
        //    Line3d line3D = new Line3d(new Point3d(line.P1.X, line.P1.Y, line.P1.Z),
        //                               new Point3d(line.P2.X, line.P2.Y, line.P2.Z));
        //    double minDistance = ray.DistanceTo(line3D);
        //    Line minLine = line;

        //    for (int i = 0; i < Mesh.Edges.Count; i++)
        //    {
        //        line = Mesh.Edges[i];
        //        line3D = new Line3d(new Point3d(line.P1.X, line.P1.Y, line.P1.Z),
        //                                   new Point3d(line.P2.X, line.P2.Y, line.P2.Z));
        //        double distance = ray.DistanceTo(line3D);
        //        var obj = ray.IntersectionWith(line3D);
        //        if (obj != null && obj is Point3d)
        //        {
        //            Console.WriteLine();
        //        }

        //        if (distance < minDistance)
        //        {
        //            minDistance = distance;
        //            minLine = Mesh.Edges[i];
        //        }
        //    }

        //    return (minLine, minDistance);
        //}

        public (Point3D, double) CheckSelectedVertex(Ray ray)
        {
            double minDistance = 0; //minimal distance between vertex and ray origin
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

                    if (minDistance != 0)
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

            return (selectedVertex, minDistance);
        }

        //public (Point3D, double) CheckSelectedVertexNEW(Ray3d ray)
        //{
        //    Point3D p = Mesh.Vertices[0];
        //    Point3d newP = new Point3d(p.X, p.Y, p.Z);
        //    double minDistance = ray.DistanceTo(newP);
        //    Point3D minPoint = p;

        //    for (int i = 0; i < Mesh.Vertices.Count; i++)
        //    {
        //        p = Mesh.Vertices[i];
        //        newP = new Point3d(p.X, p.Y, p.Z);
        //        double distance = ray.DistanceTo(newP);

        //        if (distance < minDistance) {
        //            minDistance = distance;
        //            minPoint = Mesh.Vertices[i];
        //        }
        //    }

        //    return (minPoint, minDistance);
        //}

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

        //public Plane CheckSelectedFacetNEW(Ray3d ray)
        //{
        //    double minDistance = 0;
        //    Plane minPlane = null;

        //    for (int i = 0; i < Mesh.Facets.Count; i++)
        //    {
        //        Plane plane = Mesh.Facets[i];
        //        var obj = ray.IntersectionWith(new Plane3d(new Point3d(plane[0].X, plane[0].Y, plane[0].Z),
        //                                                   new Point3d(plane[2].X, plane[2].Y, plane[2].Z),
        //                                                   new Point3d(plane[4].X, plane[4].Y, plane[4].Z)));

        //        var obj2 = ray.IntersectionWith(new Plane3d(new Point3d(plane[0].X, plane[0].Y, plane[0].Z),
        //                                                   new Point3d(plane[4].X, plane[4].Y, plane[4].Z),
        //                                                   new Point3d(plane[6].X, plane[6].Y, plane[6].Z)));
        //        if (obj != null)
        //        {
        //            double distance = ray.DistanceTo((Point3d)obj);

        //            if (minDistance != 0)
        //            {
        //                if (distance < minDistance)
        //                {
        //                    minPlane = Mesh.Facets[i];
        //                    minDistance = distance;
        //                }
        //            }
        //            else
        //            {
        //                minPlane = Mesh.Facets[i];
        //                minDistance = distance;
        //            }
        //        }

        //        if (obj2 != null)
        //        {
        //            double distance = ray.DistanceTo((Point3d)obj2);

        //            if (minDistance != 0)
        //            {
        //                if (distance < minDistance)
        //                {
        //                    minPlane = Mesh.Facets[i];
        //                    minDistance = distance;
        //                }
        //            }
        //            else
        //            {
        //                minPlane = Mesh.Facets[i];
        //                minDistance = distance;
        //            }
        //        }
        //    }

        //    return minPlane;
        //}

        private double GetDistance(Point3D v1, Point3D v2)
        {
            return Math.Sqrt(Math.Pow((v2.X - v1.X), 2) + Math.Pow((v2.Y - v1.Y), 2) + Math.Pow((v2.Z - v1.Z), 2));
        }

        public Point3D GetCenterPoint()
        {
            return centerPoint;
        }

        public object Clone()
        {
            return new MeshObject3D(Mesh.Clone());
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
    }
}
