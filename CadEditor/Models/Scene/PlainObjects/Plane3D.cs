﻿using CadEditor.MeshObjects;
using SharpGL;
using System;
using System.Collections.Generic;
using System.Drawing;

namespace CadEditor
{
    public class Plane : ISceneObject
    {
        public List<Point3D> Points { get; set; }
        public bool IsSelected { get; set; }
        public bool IsAttached { get; set; }
        public Color SelectedColor { get; set; } = Color.Brown;
        public Color NonSelectedColor { get; set; } = Color.LightGray;
        public ISceneObject ParentObject { get; set; }
        public CoordinateAxisType AxisType { get; set; }

        public Plane(List<Point3D> _vertices)
        {
            Points = _vertices;
            IsSelected = false;
        }

        public Plane(Plane planeToClone)
        {
            Points = new List<Point3D>();
            foreach (Point3D p in planeToClone.Points)
            {
                Points.Add((Point3D)p.Clone());
            }
            IsAttached = planeToClone.IsAttached;
            IsSelected = planeToClone.IsSelected;
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

        //public bool Equals(Plane plane)
        //{
        //    if (plane != null)
        //    {
        //        for (int i = 0; i < plane.Points.Count; i++)
        //        {
        //            if (!this.Points[i].Equals(plane.Points[i]))
        //            {
        //                return false;
        //            }
        //        }

        //        return true;
        //    }

        //    return false;
        //}

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

            GraphicsGL.GL.Begin(OpenGL.GL_POLYGON);
            foreach (Point3D v in Points)
            {
                GraphicsGL.GL.Vertex(v.X, v.Y, v.Z);
            }
            GraphicsGL.GL.End();
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

        //public Plane Clone()
        //{
        //    List<Point3D> newPoints = new List<Point3D>();

        //    for (int i = 0; i < this.Points.Count; i++)
        //    {
        //        newPoints.Add(this.Points[i].Clone());
        //    }

        //    return new Plane(newPoints);
        //}

        public Plane GetClickableArea(double tolerance)
        {
            Plane resultFacet = (Plane)this.Clone();

            for (int i = 0; i < resultFacet.Points.Count; i++)
            {
                Point3D currentVertex = (Point3D)resultFacet.Points[i].Clone();
                Vector directionToCenter = currentVertex - GetCenterPoint();

                resultFacet[i] = new Point3D(new Vector(currentVertex) - directionToCenter * tolerance);
            }

            return resultFacet;
        }

        public ISceneObject CheckSelected()
        {
            throw new NotImplementedException();
        }

        public bool Equals(Plane b)
        {
            if (b == null)
                return false;

            for (int i = 0; i < this.Points.Count; i++)
            {
                if (this[i] != b[i])
                {
                    return false;
                }
            }

            return true;
        }

        public ISceneObject Clone()
        {
            return new Plane(this);
        }

        public bool IsEqual(ISceneObject obj)
        {
            if (obj != null && obj is Plane)
            {
                Plane plane = (Plane)obj;
                
                for (int i = 0; i < plane.Points.Count; i++)
                {
                    if (!plane[i].IsEqual(this[i]))
                    {
                        return false;
                    }
                }

                return true;
            }

            return false;
        }

        (ISceneObject, double) ISceneObject.CheckSelected(int x, int y)
        {
            throw new NotImplementedException();
        }

        public static bool operator ==(Plane a, Plane b)
        {
            if (ReferenceEquals(a, b))
                return true;

            if (a is null || b is null)
                return false;

            return a.Equals(b);
        }

        public static bool operator !=(Plane a, Plane b)
        {
            return !(a == b);
        }
    }
}
