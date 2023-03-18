using SharpGL;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CadEditor
{
	public class Mesh
	{
		private Facet[] facets;
		private Vertex[] vertices;
		private Edge[] edges;

		public Vertex[] Vertices 
		{
			get { return vertices; }
			set { vertices = value; }
		}

		public Facet[] Facets
		{
			get { return facets; }
			set { facets = value; }
		}

		public Edge[] Edges
		{
			get { return edges; }
			set { edges = value; }
		}
	}


	public struct Vertex
	{
		public float X { get; set; }
		public float Y { get; set; }
		public float Z { get; set; }
		public bool IsSelected { get; set; }

		public Vertex(double[] values)
		{
			X = (float)values[0];
			Y = (float)values[1];
			Z = (float)values[2];
			IsSelected = false;
		}

		public Vertex(Vector v)
		{
			X = (float)v[0];
			Y = (float)v[1];
			Z = (float)v[2];
			IsSelected = false;
		}

		public Vertex(float _x, float _y, float _z)
		{
			X = _x;
			Y = _y;
			Z = _z;
			IsSelected = false;
		}

		public Vertex GetWorldCoordinates(OpenGL gl) 
		{
			return new Vertex(gl.UnProject(X, Y, Z));
		}

		public bool IsLower(Vertex v)
		{
			return this.X < v.X && this.Y < v.Y && this.Z < v.Z;
		}

		public override string ToString()
		{
			return String.Format("({0}, {1}, {2})", X, Y, Z);
		}
	}

	public struct Facet
	{
		public Vertex[] Vertices { get; set; }
		public bool IsSelected { get; set; }

		public Facet(Vertex[] _vertices)
		{
			Vertices = _vertices;
			IsSelected = false;
		}

		public override string ToString()
		{
			string result = "";
			foreach(Vertex v in Vertices)
			{
				result += "Vertex: " + v.ToString() + "\n";
			}
			return result;
		}

		public Vertex this[int index]
		{
			get
			{
				if (index < Vertices.Length && index >= 0)
				{
					return Vertices[index];
				}
				else
				{
					throw new ArgumentException("Wrong index!");
				}
			}
			set
			{
				if (index < Vertices.Length && index >= 0)
				{
					Vertices[index] = value;
				}
				else
				{
					throw new ArgumentException("Wrong index!");
				}
			}
		}
	}

	public struct Edge
	{
		public Vertex V1 { get; set; }
		public Vertex V2 { get; set; }
		public bool IsSelected { get; set; }
	}
}
