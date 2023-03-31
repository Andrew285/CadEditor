using SharpGL;
using SharpGL.SceneGraph;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml.Linq;

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


	public class Vertex: ISelectable
	{
		private OpenGL gl;
		public double X { get; set; }
		public double Y { get; set; }
		public double Z { get; set; }

		public List<Edge> EdgeParents { get; set; }
		public List<Facet> FacetParents { get; set; }

		public bool IsSelected { get; set; }
		public Color SelectedColor { get; set; } = Color.Pink;
		public Color NonSelectedColor { get; set; } = Color.Green;

		public Vertex(double[] values)
		{
			X = (float)values[0];
			Y = (float)values[1];
			Z = (float)values[2];
			IsSelected = false;
		}

		public Vertex(Vector v)
		{
			X = v[0];
			Y = v[1];
			Z = v[2];
			IsSelected = false;
		}

		public Vertex(OpenGL _gl, double _x, double _y, double _z)
		{
			X = _x;
			Y = _y;
			Z = _z;
			gl = _gl;
			IsSelected = false;
			EdgeParents = new List<Edge>();
			FacetParents = new List<Facet>();
		}

		public static bool operator ==(Vertex a, Vertex b)
		{
			if (object.ReferenceEquals(a, b))
			{
				return true;
			}
			if (object.ReferenceEquals(a, null) ||
			object.ReferenceEquals(b, null))
			{
				return false;
			}

			return a.X == b.X && a.Y == b.Y && a.Z == b.Z;
		}

		public static bool operator !=(Vertex a, Vertex b)
		{
			return !(a == b);
		}

		public override bool Equals(object obj)
		{
			return Equals(obj as Vertex);
		}

		public bool Equals(Vertex other)
		{
			return this == other;
		}

		public override int GetHashCode()
		{
			return (int)X + (int)Y + (int)Z;
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

		public void Draw(double[] color = null)
		{
			if (color != null && color.Length == 3)
			{
				gl.Color(color[0], color[1], color[2]);
			}
			else
			{
				if (IsSelected)
				{
					gl.Color(SelectedColor.R, SelectedColor.G, SelectedColor.B);
				}
				else
				{
					gl.Color(NonSelectedColor.R, NonSelectedColor.G, NonSelectedColor.B);
				}
			}

			gl.Vertex(X, Y, Z);
		}

		public void Move(double x, double y, double z)
		{
			X += x;
			Y += y;
			Z += z;
		}
	}

	public class Facet: ISelectable
	{
		private OpenGL gl;
		public Vertex[] Vertices { get; set; }
		public bool IsSelected { get; set; }
		public Color SelectedColor { get; set; } = Color.Brown;
		public Color NonSelectedColor { get; set; } = Color.Yellow;

		public Facet(OpenGL _gl, Vertex[] _vertices)
		{
			Vertices = _vertices;
			IsSelected = false;
			gl = _gl;
		}

		public bool Contains(Vertex point)
		{
			Vertex fp1 = this[0];
			Vertex fp2 = this[2];
			if (point.X >= fp1.X && point.Y >= fp1.Y && point.Z >= fp1.Z &&
				point.X <= fp2.X && point.Y <= fp2.Y && point.Z <= fp2.Z)
			{
				return true;
			}
			else
			{
				return false;
			}
		}

		public Vector CalculateNormal()
		{
			// Calculate two vectors lying on the plane
			Vector p1 = new Vector(Vertices[0]);
			Vector p2 = new Vector(Vertices[1]);
			Vector p3 = new Vector(Vertices[2]);

			Vector v1 = p2 - p1;
			Vector v2 = p3 - p1;

			// Calculate the cross product of the two vectors
			Vector normal = v1.Cross(v2);

			// Normalize the normal vector
			normal = normal.Normalize();

			return normal;
		}

		public Vertex GetCenterPoint()
		{
			Vertex fp1 = this[0];
			Vertex fp2 = this[2];

			double x = (fp1.X + fp2.X) / 2;
			double y = (fp1.Y + fp2.Y) / 2;
			double z = (fp1.Z + fp2.Z) / 2;
			return new Vertex(gl, x, y, z);
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

		public void Draw(double[] color = null)
		{
			if (color != null && color.Length == 3)
			{
				gl.Color(color[0], color[1], color[2]);
			}
			else
			{
				if (IsSelected)
				{
					gl.Color(SelectedColor.R, SelectedColor.G, SelectedColor.B);
				}
				else
				{
					gl.Color(NonSelectedColor.R, NonSelectedColor.G, NonSelectedColor.B);
				}
			}


			foreach (Vertex v in Vertices)
			{
				gl.Vertex(v.X, v.Y, v.Z);
			}
		}
	}

	public class Edge: IEquatable<Edge>, ISelectable
	{
		private OpenGL gl;

		public Vertex V1 { get; set; }
		public Vertex V2 { get; set; }

		public List<Facet> FacetParents { get; set; }


		public float LineWidth { get; set; } = 3.0f;
		public bool IsSelected { get; set; }
		public Color SelectedColor { get; set; } = Color.Red;
		public Color NonSelectedColor { get; set; } = Color.Black;

		public Edge(OpenGL _gl, Vertex _v1, Vertex _v2)
		{
			V1 = _v1;
			V2 = _v2;
			gl = _gl;
			FacetParents = new List<Facet>();
		}

		public static bool operator ==(Edge a, Edge b)
		{
			if (object.ReferenceEquals(a, b))
			{
				return true;
			}
			if (object.ReferenceEquals(a, null) ||
			object.ReferenceEquals(b, null))
			{
				return false;
			}
			return (a.V1 == b.V1 && a.V2 == b.V2) || (a.V1 == b.V2 && a.V2 == b.V1);
		}

		public static bool operator !=(Edge a, Edge b)
		{
			return !(a == b);
		}

		//Overriding Equals() methods
		public override bool Equals(object obj)
		{
			return Equals(obj as Edge);
		}

		public bool Equals(Edge other)
		{
			return this == other;
		}

		public override int GetHashCode()
		{
			return 1;
		}

		//Check if edge exist in list of edges
		public bool Exists(List<Edge> edges)
		{
			foreach(Edge edge in edges)
			{
				if(this == edge)
				{
					return true;
				}
			}
			return false;
		}

		//Check if point contains edge
		public bool Contains(Vertex point)
		{
			double accuracy = 0.05;
			if ((Math.Abs(point.X - V1.X) <= accuracy && Math.Abs(point.Y - V1.Y) <= accuracy && Math.Abs(point.Z - V1.Z) <= accuracy) ||
				(Math.Abs(point.X - V2.X) <= accuracy && Math.Abs(point.Y - V2.Y) <= accuracy && Math.Abs(point.Z - V2.Z) <= accuracy))
			{
				return true;
			}
			else
			{
				return false;
			}
		}

		public void Draw(double[] color = null, float lineWidth = 3.0f)
		{
			if(color != null && color.Length == 3)
			{
				gl.Color(color[0], color[1], color[2]);
			}
			else
			{
				if (IsSelected)
				{
					gl.Color(SelectedColor.R, SelectedColor.G, SelectedColor.B);
				}
				else
				{
					gl.Color(NonSelectedColor.R, NonSelectedColor.G, NonSelectedColor.B);
				}
			}

			LineWidth = (float)lineWidth;
			if (IsSelected)
			{
				LineWidth += 1.0f;
			}

			gl.LineWidth((float)LineWidth);
			gl.Vertex(V1.X, V1.Y, V1.Z);
			gl.Vertex(V2.X, V2.Y, V2.Z);
		}

		public void Move(double x, double y, double z)
		{
			V1.Move(x, y, z);
			V2.Move(x, y, z);
		}
	}

	public class Axis: Edge
	{
		public CoordinateAxis CoordinateAxis { get; set; }

		public Axis(OpenGL openGL, Vertex v1, Vertex v2, CoordinateAxis axis): base(openGL, v1, v2)
		{
			CoordinateAxis = axis;
		}
	}

	public interface ISelectable
	{
		bool IsSelected { get; set; }
		Color SelectedColor { get; set; }
		Color NonSelectedColor { get; set; }
	}

}
