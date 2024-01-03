using CadEditor.Graphics;
using CadEditor.MeshObjects;
using System;
using System.Collections.Generic;
using System.Drawing;

namespace CadEditor
{
	public class Mesh
	{
        public const int FACET_VERTICES = 8;

        private List<Plane> facets;
		private List<Point3D> vertices;
		private List<Line> edges;
        public List<int> attachedFacets;

        public List<Point3D> Vertices 
		{
			get { return vertices; }
			set { vertices = value; }
		}

		public List<Plane> Facets
		{
			get { return facets; }
			set { facets = value; }
		}

		public List<Line> Edges
		{
			get { return edges; }
			set { edges = value; }
		}

        public Mesh()
        {
            facets = new List<Plane>();
            vertices = new List<Point3D>();
            edges = new List<Line>();
            attachedFacets = new List<int>();
        }

        //public Mesh(List<Point3D> vertices, List<Line> edges, List<Plane> facets)
        //{
        //    Facets = (facets != null) ? facets : new List<Plane>();
        //    Edges = (edges != null) ? edges : new List<Line>();
        //    Vertices = (vertices != null) ? vertices : new List<Point3D>();
        //}

        public bool Equals(Mesh mesh)
        {
			for (int i = 0; i < this.Vertices.Count; i++)
            {
				if (!this.Vertices[i].Equals(mesh.Vertices[i]))
				{
					return false;
				}
			}

			for (int i = 0; i < this.Edges.Count; i++)
			{
				if (!this.Edges[i].Equals(mesh.Edges[i]))
				{
					return false;
				}
			}

			for (int i = 0; i < this.Facets.Count; i++)
			{
				if (!this.Facets[i].Equals(mesh.Facets[i]))
				{
					return false;
				}
			}

            return true;
		}

        public Point3D ContainsPoint(Point3D point)
        {
            foreach(Point3D p in Vertices)
            {
                if(p == point)
                {
                    return p;
                }
            }
            return null;
        }

        public int GetIndexOfPoint(Point3D point)
        {
            for(int i = 0; i < Vertices.Count; i++)
            {
                if (Vertices[i] == point)
                {
                    return i;
                }
            }

            return -1;
        }

        public int GetIndexOfFacet(Plane plane)
        {
            for (int i = 0; i < Facets.Count; i++)
            {
                if (Facets[i] == plane)
                {
                    return i;
                }
            }

            return -1;
        }

        public static List<Point3D> CloneVertices(List<Point3D> listToClone)
        {
            List<Point3D> resultList = new List<Point3D>();

            for (int i = 0; i < listToClone.Count; i++)
            {
                resultList.Add(listToClone[i].Clone());
            }

            return resultList;
        }

        public static Point3D[] CloneVertices(Point3D[] listToClone)
        {
            Point3D[] resultList = new Point3D[listToClone.Length];

            for (int i = 0; i < listToClone.Length; i++)
            {
                resultList[i] = listToClone[i].Clone();
            }

            return resultList;
        }

        public Mesh Clone()
        {
            Mesh cloneMesh = new Mesh();

			for (int i = 0; i < this.Vertices.Count; i++)
			{
                cloneMesh.Vertices.Add(Vertices[i].Clone());
			}

			for (int i = 0; i < this.Edges.Count; i++)
			{
                Point3D p1 = cloneMesh.ContainsPoint(this.Edges[i].P1);
                Point3D p2 = cloneMesh.ContainsPoint(this.Edges[i].P2);
                cloneMesh.Edges.Add(new Line(p1, p2));
			}

			for (int i = 0; i < this.Facets.Count; i++)
			{
                Point3D p1 = cloneMesh.ContainsPoint(this.Facets[i][0]);
                Point3D p2 = cloneMesh.ContainsPoint(this.Facets[i][1]);
                Point3D p3 = cloneMesh.ContainsPoint(this.Facets[i][2]);
                Point3D p4 = cloneMesh.ContainsPoint(this.Facets[i][3]);
                cloneMesh.Facets.Add(new Plane(new List<Point3D> {p1, p2, p3, p4 }));
            }

            return cloneMesh;
		}
	}
}
