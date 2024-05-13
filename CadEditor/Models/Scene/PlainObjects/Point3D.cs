using CadEditor.MeshObjects;
using SharpGL;
using System;
using System.Collections.Generic;
using System.Drawing;

namespace CadEditor
{
    public class Point3D : ISceneObject
    {
        public double X { get; set; }
        public double Y { get; set; }
        public double Z { get; set; }
        public float PointSize { get; set; } = 7.0f;
        public List<Line> EdgeParents { get; set; }
        public List<Plane> FacetParents { get; set; }
        public MeshObject3D ParentCube { get; set; }
        public int PositionInCube { get; set; }
        public bool IsSelected { get; set; }
        public Color SelectedColor { get; set; } = Color.Pink;
        public Color NonSelectedColor { get; set; } = Color.Black;
        public ISceneObject ParentObject { get; set; }
        public int Coefficient { get; set; } = 1;

        public Point3D(double _x, double _y, double _z)
        {
            X = _x;
            Y = _y;
            Z = _z;
            IsSelected = false;
            EdgeParents = new List<Line>();
            FacetParents = new List<Plane>();
        }

        public Point3D(double[] values) : this(values[0], values[1], values[2]) { }

        public Point3D(Vector v) : this(v[0], v[1], v[2]) { }

        public Point3D(Point3D pointToClone): this(pointToClone.X, pointToClone.Y, pointToClone.Z)
        {
            ParentCube = pointToClone.ParentCube;
            //ParentObject = pointToClone.ParentObject.Clone();
            PositionInCube = pointToClone.PositionInCube;
            Coefficient = pointToClone.Coefficient;

            // TODO: Point3D doesn't cloneo ParentObject
        }

        public bool Equals(Point3D p)
        {
            if (p != null)
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
            return (Point3D)Clone();
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

        public ISceneObject Clone()
        {
            return new Point3D(this);
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

            GraphicsGL.GL.PointSize(PointSize);
            GraphicsGL.GL.Begin(OpenGL.GL_POINTS);
            GraphicsGL.GL.Vertex(X, Y, Z);
            GraphicsGL.GL.End();

        }

        public void Move(Vector vector)
        {
            X += vector[0] / Coefficient;
            Y += vector[1] / Coefficient;
            Z += vector[2] / Coefficient;

            if (ParentCube != null && ParentCube is ComplexCube && ((ComplexCube)ParentCube).IsDivided)
            {
                ((ComplexCube)ParentCube).Update(this);
                ((ComplexCube)ParentCube).Transform();
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

        public (ISceneObject, double) CheckSelected()
        {
            throw new NotImplementedException();
        }

        public bool IsEqual(ISceneObject obj)
        {
            if (obj != null && obj is Point3D)
            {
                Point3D p = (Point3D)obj;
                return this.X == p.X && this.Y == p.Y && this.Z == p.Z;
            }

            return false;
        }
    }
}
