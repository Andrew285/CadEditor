using CadEditor.MeshObjects;
using SharpGL;
using SharpGL.SceneGraph.Primitives;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Windows.Forms;

namespace CadEditor
{
	public class Mesh
	{
		private List<Plane> facets;
		private List<Point3D> vertices;
		private List<Line> edges;

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
        }

        public Mesh(List<Point3D> vertices, List<Line> edges, List<Plane> facets)
        {
            Facets = (facets != null) ? facets : new List<Plane>();
            Edges = (edges != null) ? edges : new List<Line>();
            Vertices = (vertices != null) ? vertices : new List<Point3D>();
        }

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

        public Mesh Clone()
        {
            Mesh cloneMesh = new Mesh();

			for (int i = 0; i < this.Vertices.Count; i++)
			{
                cloneMesh.Vertices.Add(Vertices[i].Clone());
			}

			for (int i = 0; i < this.Edges.Count; i++)
			{
				cloneMesh.Edges.Add(Edges[i].Clone());
			}

			for (int i = 0; i < this.Facets.Count; i++)
			{
				cloneMesh.Facets.Add(Facets[i].Clone());
			}

            return cloneMesh;
		}
	}

	public class Point3D: Object3D
    {
		public OpenGL GL { get; set; }
        public double X { get; set; }
        public double Y { get; set; }
        public double Z { get; set; }
        public List<Line> EdgeParents { get; set; }
		public List<Plane> FacetParents { get; set; }
		public MeshObject3D ParentCube { get; set; }
		public bool IsSelected { get; set; }
		public Color SelectedColor { get; set; } = Color.Pink;
		public Color NonSelectedColor { get; set; } = Color.Black;

        public Point3D(double[] values, OpenGL _gl = null)
		{
            GL = _gl;
            X = values[0];
            Y = values[1];
            Z = values[2];
            IsSelected = false;
		}

		public Point3D(Vector v)
		{
            X = v[0];
            Y = v[1];
            Z = v[2];
            IsSelected = false;
		}

		public Point3D(double _x, double _y, double _z, OpenGL _gl=null)
		{
			GL = _gl;
			X = _x;
			Y = _y;
			Z = _z;
			IsSelected = false;
			EdgeParents = new List<Line>();
			FacetParents = new List<Plane>();
		}

        public bool Equals(Point3D p)
        {
            if(p != null)
            {
                return Math.Abs(this.X - p.X) < 0.00001 && Math.Abs(this.Y - p.Y) < 0.00001 && Math.Abs(this.Z - p.Z) < 0.00001;
            }

            return false;
        }

		public static bool operator ==(Point3D a, Point3D b)
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

		public static bool operator !=(Point3D a, Point3D b)
		{
			return !(a == b);
		}

		public static Vector operator -(Point3D a, Point3D b)
		{
			return new Vector(a.X - b.X, a.Y - b.Y, a.Z - b.Z);
		}

		public static Vector operator +(Point3D a, Point3D b)
		{
			return new Vector(a.X + b.X, a.Y + b.Y, a.Z + b.Z);
		}

		public override int GetHashCode()
		{
			return (int)X + (int)Y + (int)Z;
		}

		public override string ToString()
		{
			return String.Format("({0},{1},{2})", X, Y, Z);
		}

        public Point3D GetWorldCoordinates(OpenGL gl)
        {
            return new Point3D(gl.UnProject(X, Y, Z));
        }

        public bool IsLower(Point3D v)
        {
            return this.X < v.X && this.Y < v.Y && this.Z < v.Z;
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

        public Point3D Clone()
        {
            Point3D clonePoint = new Point3D(X, Y, Z, GL);
            clonePoint.ParentCube = ParentCube;
            return clonePoint;
        }

        public override void Draw()
		{
            if (IsSelected)
            {
                GL.Color(SelectedColor.R, SelectedColor.G, SelectedColor.B);
            }
            else
            {
                GL.Color(NonSelectedColor.R, NonSelectedColor.G, NonSelectedColor.B);
            }

            GL.Vertex(X, Y, Z);
		}

        public override void Move(Vector vector)
        {
			X += vector[0];
			Y += vector[1];
			Z += vector[2];

            if(ParentCube != null && ParentCube is ComplexCube)
            {
                ((ComplexCube)ParentCube).Transform(vector, this);
            }
        }

        public override void Select()
		{
			IsSelected = true;
		}

        public override void Deselect()
        {
			IsSelected = false;
        }
    }

	public class Plane: Object3D
	{
		public OpenGL GL { get; set; }
		public List<Point3D> Points { get; set; }
		public bool IsSelected { get; set; }
		public Color SelectedColor { get; set; } = Color.Brown;
		public Color NonSelectedColor { get; set; } = Color.LightGray;

		public Plane(List<Point3D> _vertices, OpenGL _gl=null)
		{
            Points = _vertices;
            IsSelected = false;
			GL = _gl;
        }

        public Point3D this[int index]
        {
            get
            {
                if (index < Points.Count && index >= 0)
                {
                    return Points[index];
                }
                else
                {
                    throw new ArgumentException("Wrong index!");
                }
            }
            set
            {
                if (index < Points.Count && index >= 0)
                {
                    Points[index] = value;
                }
                else
                {
                    throw new ArgumentException("Wrong index!");
                }
            }
        }

        public bool Equals(Plane plane)
        {
            if(plane != null)
            {
                for(int i = 0; i < plane.Points.Count; i++)
                {
                    if (!this.Points[i].Equals(plane.Points[i]))
                    {
                        return false;
                    }
                }

                return true;
            }

            return false;
        }

        public bool Contains(Point3D point)
        {
            Line centerToPointEdge = new Line(GetCenterPoint(), point);

            for (int i = 0; i < this.Points.Count; i++)
            {
                int j = i;
                Line cubeEdge = new Line(Points[i], Points[(j + 1) % Points.Count]);

                if (centerToPointEdge.IntersectsLine(cubeEdge))
                {
                    return false;
                }
            }

            return true;
        }

        public Vector CalculateNormal()
        {
            // Calculate two vectors lying on the plane
            Vector p1 = null, p2 = null, p3 = null;
            if(Points.Count == 8)
            {
				p1 = new Vector(Points[0]);
				p2 = new Vector(Points[2]);
				p3 = new Vector(Points[4]);
			}
            else if (Points.Count == 4)
            {
				p1 = new Vector(Points[0]);
				p2 = new Vector(Points[1]);
				p3 = new Vector(Points[2]);
			}


            Vector v1 = p2 - p1;
            Vector v2 = p3 - p1;

            // Calculate the cross product of the two vectors
            Vector normal = v1.Cross(v2);

            // Normalize the normal vector
            //normal = normal.Normalize();

            return normal;
        }

        public Point3D GetCenterPoint()
        {
            double x = 0;
            double y = 0;
            double z = 0;

            for (int j = 0; j < Points.Count; j++)
            {
                x += Points[j].X;
                y += Points[j].Y;
                z += Points[j].Z;
            }

            x /= Points.Count;
            y /= Points.Count;
            z /= Points.Count;

            return new Point3D(x, y, z);
        }

        public override string ToString()
		{
			string result = "";
			foreach(Point3D p in Points)
			{
				result += "Vertex: " + p.ToString() + "\n";
			}
			return result;
		}

		public override void Draw()
		{
            if (IsSelected)
            {
                GL.Color(SelectedColor.R, SelectedColor.G, SelectedColor.B);
            }
            else
            {
                GL.Color(NonSelectedColor.R, NonSelectedColor.G, NonSelectedColor.B);
            }


            foreach (Point3D v in Points)
			{
				GL.Vertex(v.X, v.Y, v.Z);
			}
		}

		public override void Move(Vector vector)
		{
			foreach(Point3D v in Points)
			{
				v.Move(vector);
			}
		}

		public override void Select()
		{
			IsSelected = true;
		}

        public override void Deselect()
        {
            IsSelected = false;
        }

        public Plane Clone()
        {
			List<Point3D> newPoints = new List<Point3D>();

            for (int i = 0; i < this.Points.Count; i++)
            {
                newPoints.Add(this.Points[i].Clone());
            }

            return new Plane(newPoints, GL);
        }

        public Plane GetClickableArea(double tolerance)
		{
			Plane resultFacet = this.Clone();

			for(int i = 0; i < resultFacet.Points.Count; i++)
			{
				Point3D currentVertex = resultFacet.Points[i].Clone();
				Vector directionToCenter = currentVertex - GetCenterPoint();

				resultFacet[i] = new Point3D(new Vector(currentVertex) - directionToCenter * tolerance);
			}

			return resultFacet;
		}
	}

	public class Line: Object3D, IEquatable<Line>
	{
		public OpenGL GL { get; set; }
		public Point3D P1 { get; set; }
		public Point3D P2 { get; set; }
		public List<Plane> FacetParents { get; set; }
		public float LineWidth { get; set; } = 3.0f;
		public bool IsSelected { get; set; }
		public Color SelectedColor { get; set; } = Color.Red;
		public Color NonSelectedColor { get; set; } = Color.Black;

		public Line(Point3D _v1, Point3D _v2, OpenGL _gl=null)
		{
			GL = _gl;
			P1 = _v1;
			P2 = _v2;
			FacetParents = new List<Plane>();
		}

		public static bool operator ==(Line a, Line b)
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
			return (a.P1 == b.P1 && a.P2 == b.P2) || (a.P1 == b.P2 && a.P2 == b.P1);
		}

		public static bool operator !=(Line a, Line b)
		{
			return !(a == b);
		}

		//Overriding Equals() methods
        public bool Equals(Line line)
        {
            if(line != null)
            {
                return this.P1.Equals(line.P1) && this.P2.Equals(line.P2);
            }

            return false;
        }

		public override int GetHashCode()
		{
			return 1;
		}

        //Check if edge exist in list of edges
        public bool Exists(List<Line> edges)
        {
            foreach (Line edge in edges)
            {
                if (this == edge)
                {
                    return true;
                }
            }
            return false;
        }

        //Check if point contains edge
        public bool Contains(Point3D point)
        {
            double lengthErrorThreshold = 1e-3;

            // See if this lies on the segment
            if ((new Vector(point) - new Vector(P1)).LengthSquared() + (new Vector(point) - new Vector(P2)).LengthSquared()
            <= new Vector(new Point3D(P2 - P1)).LengthSquared() + lengthErrorThreshold)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public Point3D GetCenterPoint()
        {
            double x = (P1.X + P2.X) / 2;
            double y = (P1.Y + P2.Y) / 2;
            double z = (P1.Z + P2.Z) / 2;

            return new Point3D(x, y, z);
        }

        public bool IntersectsLine(Line line)
        {
            Vector line1Direction = P2 - P1;
            Vector line2Direction = line.P2 - line.P1;

            Vector intersectionDirection = line1Direction.Cross(line2Direction);
            double intersectionMagSqr = intersectionDirection.LengthSquared();

            if (intersectionMagSqr < 0.0001f) // lines are parallel
            {
                return false;
            }

            Vector toLine2Start = line.P1 - P1;
            double t = (toLine2Start.Cross(line2Direction) * intersectionDirection) / intersectionMagSqr;
            double u = (toLine2Start.Cross(line1Direction) * intersectionDirection) / intersectionMagSqr;

            if (t < 0f || t > 1f || u < 0f || u > 1f) // intersection point is outside of both line segments
            {
                return false;
            }

            return true;
        }

        public Line Clone()
        {
            return new Line(P1.Clone(), P2.Clone(), GL);
        }

        public override void Draw()
		{

            if (IsSelected)
            {
                GL.Color(SelectedColor.R, SelectedColor.G, SelectedColor.B);
            }
            else
            {
                GL.Color(NonSelectedColor.R, NonSelectedColor.G, NonSelectedColor.B);
            }

            LineWidth = (float)LineWidth;
			if (IsSelected)
			{
				LineWidth += 1.0f;
			}

			GL.LineWidth((float)LineWidth);
			GL.Vertex(P1.X, P1.Y, P1.Z);
			GL.Vertex(P2.X, P2.Y, P2.Z);
		}

		public override void Move(Vector vector)
		{
			P1.Move(vector);
			P2.Move(vector);
		}

		public override void Select()
		{
			IsSelected = true;
		}

		public override void Deselect()
        {
			IsSelected = false;
        }

        public Line GetClickableArea(double tolerance)
        {
            Line resultEdge = (Line)this.Clone();
			Point3D[] edgeVertices = new Point3D[] { resultEdge.P1, resultEdge.P2 };
            for (int i = 0; i < edgeVertices.Length; i++)
            {
                Point3D currentVertex = edgeVertices[i];
                Vector directionToCenter = currentVertex - GetCenterPoint();

				edgeVertices[i] = new Point3D(new Vector(currentVertex) - directionToCenter * tolerance);
            }

			resultEdge.P1 = edgeVertices[0];
			resultEdge.P2 = edgeVertices[1];

            return resultEdge;
        }
    }

	public class Axis: Line
	{
		public CoordinateAxis CoordinateAxis { get; set; }

		public Axis(Point3D v1, Point3D v2, CoordinateAxis axis, OpenGL openGL = null): base(v1, v2, openGL)
		{
			CoordinateAxis = axis;
		}
	}
}
