using SharpGL;
using SharpGL.SceneGraph;
using SharpGL.SceneGraph.Raytracing;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml.Linq;
using static System.Windows.Forms.AxHost;

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
			//Vertex fp1 = this[0];
			//Vertex fp2 = this[2];
			//if (point.X >= fp1.X && point.Y >= fp1.Y && point.Z >= fp1.Z &&
			//	point.X <= fp2.X && point.Y <= fp2.Y && point.Z <= fp2.Z)
			//{
			//	return true;
			//}
			//else
			//{
			//	return false;
			//}

			Vertex minVertex = Vertices[0];
			Vertex maxVertex = Vertices[2];

			for (int i = 0; i < Vertices.Length; i++)
			{
				if (Vertices[i].X < minVertex.X)
				{
					minVertex.X = Vertices[i].X;
				}

				if (Vertices[i].Y < minVertex.Y)
				{
					minVertex.Y = Vertices[i].Y;
				}

				if (Vertices[i].Z < minVertex.Z)
				{
					minVertex.Z = Vertices[i].Z;
				}

				if (Vertices[i].X > maxVertex.X)
				{
					maxVertex.X = Vertices[i].X;
				}

				if (Vertices[i].Y > maxVertex.Y)
				{
					maxVertex.Y = Vertices[i].Y;
				}

				if (Vertices[i].Z > maxVertex.Z)
				{
					maxVertex.Z = Vertices[i].Z;
				}
			}
			if (point.X >= minVertex.X && point.Y >= minVertex.Y && point.Z >= minVertex.Z &&
				point.X <= maxVertex.X && point.Y <= maxVertex.Y && point.Z <= maxVertex.Z)
			{
				Console.WriteLine("\nThis point is in these bounderies: " + point);
				Console.WriteLine("\nMin: " + minVertex + " / Max: " + maxVertex);
				return true;
			}
			else
			{
				return false;
			}
		}

		//public bool Contains(Vertex p)
		//{
		//	Vector intersection;
		//	Vector direction1 = this.GetCenterPoint() - p;

		//	for(int i = 0; i < Vertices.Length; i++)
		//	{
		//		Vertex v1, v2;

		//		if (i == Vertices.Length - 1)
		//		{
		//			v1 = Vertices[i];
		//			v2 = Vertices[0];
		//		}
		//		else
		//		{
		//			v1 = Vertices[i];
		//			v2 = Vertices[i++];
		//		}

		//		Vector direction2 = v2 - v1;

		//		Vector cross = direction1.Cross(direction2);
		//		double crossLengthSquared = LengthSquared(cross);

		//		// Lines are parallel
		//		if (crossLengthSquared < float.Epsilon)
		//		{
		//			intersection = null;
		//			return false;
		//		}

		//		Vector vectorBetweenStarts = v2 - p;

		//		double u = (vectorBetweenStarts.Cross(direction2) * cross) / crossLengthSquared;
		//		double v = (vectorBetweenStarts.Cross(direction1) * cross) / crossLengthSquared;

		//		// Lines intersect
		//		if (u >= 0f && u <= 1f && v >= 0f && v <= 1f)
		//		{
		//			intersection = new Vector(this.GetCenterPoint()) + direction1 * u;
		//			return true;
		//		}

		//		// Lines do not intersect
		//		intersection = null;
		//		return false;
		//	}

		//	return false;
		//}

		//public bool Contains(Vertex v)
		//{
		//	Edge newEdge = new Edge(gl, this.GetCenterPoint(), v);

		//	for (int i = 0; i < Vertices.Length; i++)
		//	{
		//		Edge facetEdge;
		//		if(i == Vertices.Length - 1)
		//		{
		//			facetEdge = new Edge(gl, Vertices[i], Vertices[0]);
		//		}
		//		else
		//		{
		//			facetEdge = new Edge(gl, Vertices[i], Vertices[i++]);
		//		}

		//		if (newEdge.IntersectsEdge(facetEdge))
		//		{
		//			return true;
		//		}
		//	}

		//	return false;
		//}

		//public bool Contains(Vertex point)
		//{
		//	//int intersectCount = 0;

		//	//for (int i = 0; i < Vertices.Length; i++)
		//	//{
		//	//	int j = (i + 1) % Vertices.Length;

		//	//	if (((Vertices[i].Y <= point.Y) && (Vertices[j].Y > point.Y))
		//	//		|| ((Vertices[i].Y > point.Y) && (Vertices[j].Y <= point.Y)))
		//	//	{
		//	//		double vt = (point.Y - Vertices[i].Y) / (Vertices[j].Y - Vertices[i].Y);

		//	//		if (point.X < Vertices[i].X + vt * (Vertices[j].X - Vertices[i].X))
		//	//		{
		//	//			intersectCount++;
		//	//		}
		//	//	}
		//	//}

		//	//return (intersectCount % 2 == 1);



		//	Vector center = null;

		//	foreach (Vertex vertex in Vertices)
		//	{
		//		center = new Vertex(center) + vertex;
		//	}

		//	center /= Vertices.Length;

		//	Vector3 normal = Vector3.Cross(polygonVertices[1] - polygonVertices[0], polygonVertices[2] - polygonVertices[0]);
		//	normal.Normalize();

		//	Vector3 toPoint = point - center;

		//	if (Vector3.Dot(normal, toPoint) > 0)
		//	{
		//		return false;
		//	}

		//	for (int i = 0; i < polygonVertices.Count; i++)
		//	{
		//		Vector3 edge = polygonVertices[(i + 1) % polygonVertices.Count] - polygonVertices[i];
		//		Vector3 toEdge = point - polygonVertices[i];

		//		if (Vector3.Dot(normal, Vector3.Cross(edge, toEdge)) < 0)
		//		{
		//			return false;
		//		}
		//	}

		//	return true;
		//}

		public double LengthSquared(Vector vector)
		{
			return vector[0] * vector[0] + vector[1] * vector[1] + vector[2] * vector[2];
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
			double accuracy = 0.1;
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

		public Vertex GetCenterPoint()
		{
			double x = (V1.X + V2.X) / 2;
			double y = (V1.Y + V2.Y) / 2;
			double z = (V1.Z + V2.Z) / 2;

			return new Vertex(gl, x, y, z); 
		}

		public void Select()
		{
			IsSelected = true;
		}

		public bool IntersectsEdge(Edge edge)
		{
			// Calculate the direction vector of the line
			Vector lineDirection = new Vector(edge.V2) - new Vector(edge.V1);

			// Calculate the normal of the plane that contains the line and the ray
			Vector planeNormal = lineDirection.Cross(new Vector(V1) - new Vector(V2));

			// Calculate the distance between the line and the ray
			double denominator = planeNormal * planeNormal;
			double numerator = planeNormal * (new Vector(V1) - new Vector(edge.V1));
			double distance = -numerator / denominator;

			// Check if the intersection point lies on the line segment
			float epsilon = 0.0001f;
			if (distance < -epsilon || distance > 1 + epsilon)
			{
				return false;
			}

			// Calculate the intersection point
			Vertex intersectionPoint = new Vertex(new Vector(edge.V1) + lineDirection * distance);

			if (!edge.Contains(intersectionPoint))
			{
				return false;
			}

			return true;
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

		void Move(double x, double y, double z);
		void Select();
	}
}
