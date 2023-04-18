using SharpGL;
using System;
using System.Collections.Generic;
using System.Drawing;

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
			X = values[0];
			Y = values[1];
			Z = values[2];
			IsSelected = false;
		}

		public Vertex(Vector v)
		{
			X = v[0];
			Y = v[1];
			Z = v[2];
			IsSelected = false;
		}

		public Vertex(double _x, double _y, double _z, OpenGL _gl = null)
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

		public static Vector operator -(Vertex a, Vertex b)
		{
			return new Vector(a.X - b.X, a.Y - b.Y, a.Z - b.Z);
		}

		public static Vector operator +(Vertex a, Vertex b)
		{
			return new Vector(a.X + b.X, a.Y + b.Y, a.Z + b.Z);
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

		public double this[int index]
		{
			get
			{
				switch (index)
				{
					case 0: return X;
					case 1: return Y;
					case 2: return Z;
					default: throw new Exception("Index is incorrect");
				}	
			}
			set
			{
				switch (index)
				{
					case 0: X = value; break;
					case 1: Y = value; break;
					case 2: Z = value; break;
					default: throw new Exception("Index is incorrect");
				}
			}
		}

		public void Select()
		{
			IsSelected = true;
		}

		public Vertex Clone()
		{
			return new Vertex(X, Y, Z);
		}
	}

	public class Facet: ISelectable
	{
		private OpenGL gl;
		public Vertex[] Vertices { get; set; }
		public bool IsSelected { get; set; }
		public Color SelectedColor { get; set; } = Color.Brown;
		public Color NonSelectedColor { get; set; } = Color.Yellow;

		public Facet(Vertex[] _vertices, OpenGL _gl = null)
		{
			Vertices = _vertices;
			IsSelected = false;
			gl = _gl;
        }

		public bool Contains(Vertex point)
		{
			Edge centerToPointEdge = new Edge(GetCenterPoint(), point, gl);

			for (int i = 0; i < this.Vertices.Length; i++)
			{
				int j = i;
				Edge cubeEdge = new Edge(Vertices[i], Vertices[(j + 1) % Vertices.Length], gl);

				if (centerToPointEdge.IntersectsEdge(cubeEdge))
				{
					return false;
				}
			}

			return true;
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
			double x = 0;
			double y = 0;
			double z = 0;

			for(int j = 0; j < Vertices.Length; j++)
			{
				x += Vertices[j].X;
				y += Vertices[j].Y;
				z += Vertices[j].Z;
			}

			x /= Vertices.Length;
			y /= Vertices.Length;
			z /= Vertices.Length;

			return new Vertex(x, y, z, gl);
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

		public void Move(double x, double y, double z)
		{
			foreach(Vertex v in Vertices)
			{
				v.Move(x, y, z);
			}
		}

		public void Select()
		{
			IsSelected = true;
		}

		public Facet GetClickableFacet(double tolerance)
		{
			Facet resultFacet = this.Clone();

			for(int i = 0; i < resultFacet.Vertices.Length; i++)
			{
				Vertex currentVertex = resultFacet.Vertices[i].Clone();
				Vector directionToCenter = currentVertex - GetCenterPoint();

				resultFacet[i] = new Vertex(new Vector(currentVertex) - directionToCenter * tolerance);
			}

			return resultFacet;
		}

		public Facet Clone()
		{
			Vertex[] vertices = new Vertex[4];

			for(int i = 0; i < this.Vertices.Length; i++)
			{
				vertices[i] = this.Vertices[i].Clone();
			}

			return new Facet(vertices);
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

		public Edge(Vertex _v1, Vertex _v2, OpenGL _gl = null)
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
            double lengthErrorThreshold = 1e-3;

            // See if this lies on the segment
            if ((new Vector(point) - new Vector(V1)).LengthSquared() + (new Vector(point) - new Vector(V2)).LengthSquared()
			<= new Vector(new Vertex(V2 - V1)).LengthSquared() + lengthErrorThreshold)
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

		public Vertex GetCenterPoint()
		{
			double x = (V1.X + V2.X) / 2;
			double y = (V1.Y + V2.Y) / 2;
			double z = (V1.Z + V2.Z) / 2;

			return new Vertex(x, y, z, gl); 
		}

		public void Select()
		{
			IsSelected = true;
		}

		public bool IntersectsEdge(Edge edge)
		{
            Vector line1Direction = V2 - V1;
            Vector line2Direction = edge.V2 - edge.V1;

            Vector intersectionDirection = line1Direction.Cross(line2Direction);
            double intersectionMagSqr = intersectionDirection.LengthSquared();

            if (intersectionMagSqr < 0.0001f) // lines are parallel
            {
                return false;
            }

            Vector toLine2Start = edge.V1 - V1;
            double t = (toLine2Start.Cross(line2Direction) * intersectionDirection) / intersectionMagSqr;
            double u = (toLine2Start.Cross(line1Direction) * intersectionDirection) / intersectionMagSqr;

            if (t < 0f || t > 1f || u < 0f || u > 1f) // intersection point is outside of both line segments
            {
                return false;
            }

            //Vector intersectionPoint = new Vector(V1) + line1Direction * t;
            return true;
        }

        public Edge Clone()
        {
            return new Edge(V1.Clone(), V2.Clone());
        }

        public Edge GetClickableEdge(double tolerance)
        {
            Edge resultEdge = this.Clone();
			Vertex[] edgeVertices = new Vertex[] { resultEdge.V1, resultEdge.V2 };
            for (int i = 0; i < edgeVertices.Length; i++)
            {
                Vertex currentVertex = edgeVertices[i];
                Vector directionToCenter = currentVertex - GetCenterPoint();

				edgeVertices[i] = new Vertex(new Vector(currentVertex) - directionToCenter * tolerance);
            }

			resultEdge.V1 = edgeVertices[0];
			resultEdge.V2 = edgeVertices[1];

            return resultEdge;
        }
    }

	public class Axis: Edge
	{
		public CoordinateAxis CoordinateAxis { get; set; }

		public Axis(Vertex v1, Vertex v2, CoordinateAxis axis, OpenGL openGL = null): base(v1, v2, openGL)
		{
			CoordinateAxis = axis;
		}
	}

	public interface ISelectable
	{
		bool IsSelected { get; set; }
		Color SelectedColor { get; set; }
		Color NonSelectedColor { get; set; }

		void Move(double x, double y, double z);
		void Select();
	}
}
