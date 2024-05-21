using SharpGL;
using System;
using System.Collections.Generic;
using System.Drawing;

namespace CadEditor
{
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

        public Line(Line lineToClone)
        {
            P1 = (Point3D)lineToClone.P1.Clone();
            P2 = (Point3D)lineToClone.P2.Clone();

            FacetParents = new List<Plane>();
            for (int i = 0; i < lineToClone.FacetParents.Count; i++)
            {
                FacetParents[i] = (Plane)lineToClone.FacetParents[i].Clone();
            }
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
            if (line != null)
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

            float tempWidth = LineWidth;

            if (IsSelected)
            {
                tempWidth = 5.0f;
            }

            GraphicsGL.GL.LineWidth(tempWidth);
            GraphicsGL.GL.Begin(OpenGL.GL_LINES);
            GraphicsGL.GL.Vertex(P1.X, P1.Y, P1.Z);
            GraphicsGL.GL.Vertex(P2.X, P2.Y, P2.Z);
            GraphicsGL.GL.End();
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

        public (ISceneObject, double) CheckSelected()
        {
            throw new NotImplementedException();
        }

        public ISceneObject Clone()
        {
            return new Line(this);
        }

        public bool IsEqual(ISceneObject obj)
        {
            if (obj != null && obj is Line)
            {
                Line l = (Line)obj;
                return this.P1.IsEqual(l.P1) && this.P2.IsEqual(l.P2);
            }

            return false;
        }
    }
}
