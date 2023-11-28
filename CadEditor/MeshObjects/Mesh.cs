using CadEditor.Graphics;
using CadEditor.MeshObjects;
using System;
using System.Collections.Generic;
using System.Drawing;

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

	public class Point3D: ISceneObject
    {
        public double X { get; set; }
        public double Y { get; set; }
        public double Z { get; set; }
        public List<Line> EdgeParents { get; set; }
		public List<Plane> FacetParents { get; set; }
		public MeshObject3D ParentCube { get; set; }
        public int PositionInCube { get; set; }
		public bool IsSelected { get; set; }
		public Color SelectedColor { get; set; } = Color.Pink;
		public Color NonSelectedColor { get; set; } = Color.Black;
        public ISceneObject ParentObject { get; set; }

        public Point3D(double _x, double _y, double _z)
        {
            X = _x;
            Y = _y;
            Z = _z;
            IsSelected = false;
            EdgeParents = new List<Line>();
            FacetParents = new List<Plane>();
        }

        public Point3D(double[] values): this(values[0], values[1], values[2]) { }

        public Point3D(Vector v): this(v[0], v[1], v[2]) { }

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

        public Point3D GetCenterPoint()
        {
            return this.Clone();
        }

        public Point3D GetWorldCoordinates()
        {
            return new Point3D(GraphicsGL.GL.UnProject(X, Y, Z));
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
            Point3D clonePoint = new Point3D(X, Y, Z);
            clonePoint.ParentCube = ParentCube;
            clonePoint.PositionInCube = PositionInCube;
            return clonePoint;
        }

        public void Draw()
		{
            if (IsSelected)
            {
                GraphicsGL.GL.Color(SelectedColor.R, SelectedColor.G, SelectedColor.B);
            }
            else
            {
                GraphicsGL.GL.Color(NonSelectedColor.R, NonSelectedColor.G, NonSelectedColor.B);
            }

            GraphicsGL.GL.Vertex(X, Y, Z);
		}

        public void Move(Vector vector)
        {
			X += vector[0];
			Y += vector[1];
			Z += vector[2];

            if (ParentCube != null && ParentCube is ComplexCube && ((ComplexCube)ParentCube).IsDivided)
            {
                ((ComplexCube)ParentCube).Update(this);
                ((ComplexCube)ParentCube).Transform();
            }

            //if(ParentCube != null && ParentCube is ComplexCube)
            //{
            //    ParentCube.CenterPoint.X += vector[0];
            //    ParentCube.CenterPoint.Y += vector[1];
            //    ParentCube.CenterPoint.Z += vector[2];
            //}
        }

        public void Select()
		{
			IsSelected = true;
		}

        public void Deselect()
        {
			IsSelected = false;
        }

        public ISceneObject CheckSelected(Ray ray)
        {
            throw new NotImplementedException();
        }
    }


	public class Line: ISceneObject, IEquatable<Line>
	{
		public Point3D P1 { get; set; }
		public Point3D P2 { get; set; }
		public List<Plane> FacetParents { get; set; }
		public float LineWidth { get; set; } = 3.0f;
		public bool IsSelected { get; set; }
		public Color SelectedColor { get; set; } = Color.Red;
		public Color NonSelectedColor { get; set; } = Color.Black;
        public ISceneObject ParentObject { get; set; }

        public Line(Point3D _v1, Point3D _v2)
		{
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
            return new Line(P1.Clone(), P2.Clone());
        }

        public void Draw()
		{

            if (IsSelected)
            {
                GraphicsGL.GL.Color(SelectedColor.R, SelectedColor.G, SelectedColor.B);
            }
            else
            {
                GraphicsGL.GL.Color(NonSelectedColor.R, NonSelectedColor.G, NonSelectedColor.B);
            }

            LineWidth = (float)LineWidth;
			if (IsSelected)
			{
				LineWidth += 1.0f;
			}

            GraphicsGL.GL.LineWidth((float)LineWidth);
            GraphicsGL.GL.Vertex(P1.X, P1.Y, P1.Z);
            GraphicsGL.GL.Vertex(P2.X, P2.Y, P2.Z);
		}

		public void Move(Vector vector)
		{
			P1.Move(vector);
			P2.Move(vector);
		}

		public void Select()
		{
			IsSelected = true;
		}

		public void Deselect()
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

        public ISceneObject CheckSelected(Ray ray)
        {
            throw new NotImplementedException();
        }
    }

    public class Plane : ISceneObject
    {
        public List<Point3D> Points { get; set; }
        public bool IsSelected { get; set; }
        public Color SelectedColor { get; set; } = Color.Brown;
        public Color NonSelectedColor { get; set; } = Color.LightGray;
        public ISceneObject ParentObject { get; set; }

        public Plane(List<Point3D> _vertices)
        {
            Points = _vertices;
            IsSelected = false;
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
            if (plane != null)
            {
                for (int i = 0; i < plane.Points.Count; i++)
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
            if (Points.Count == 8)
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
            foreach (Point3D p in Points)
            {
                result += "Vertex: " + p.ToString() + "\n";
            }
            return result;
        }

        public void Draw()
        {
            if (IsSelected)
            {
                GraphicsGL.GL.Color(SelectedColor.R, SelectedColor.G, SelectedColor.B);
            }
            else
            {
                GraphicsGL.GL.Color(NonSelectedColor.R, NonSelectedColor.G, NonSelectedColor.B);
            }


            foreach (Point3D v in Points)
            {
                GraphicsGL.GL.Vertex(v.X, v.Y, v.Z);
            }
        }

        public void Move(Vector vector)
        {
            foreach (Point3D v in Points)
            {
                v.Move(vector);
            }
        }

        public void Select()
        {
            IsSelected = true;
        }

        public void Deselect()
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

            return new Plane(newPoints);
        }

        public Plane GetClickableArea(double tolerance)
        {
            Plane resultFacet = this.Clone();

            for (int i = 0; i < resultFacet.Points.Count; i++)
            {
                Point3D currentVertex = resultFacet.Points[i].Clone();
                Vector directionToCenter = currentVertex - GetCenterPoint();

                resultFacet[i] = new Point3D(new Vector(currentVertex) - directionToCenter * tolerance);
            }

            return resultFacet;
        }

        public ISceneObject CheckSelected(Ray ray)
        {
            throw new NotImplementedException();
        }
    }


    public class Axis: Line
	{
		public CoordinateAxis CoordinateAxis { get; set; }

		public Axis(Point3D v1, Point3D v2, CoordinateAxis axis): base(v1, v2)
		{
			CoordinateAxis = axis;
		}
	}
}
