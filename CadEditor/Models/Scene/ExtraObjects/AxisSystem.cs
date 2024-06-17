using SharpGL;
using System;
using System.Collections.Generic;
using System.Drawing;

namespace CadEditor.MeshObjects
{
    public enum CoordinateAxis { X, Y, Z }
    public enum CoordinateAxisType { PlusX, MinusX, PlusY, MinusY, PlusZ, MinusZ }

    public class Axis : Line
    {
        public CoordinateAxis CoordinateAxis { get; set; }

        public Axis(Point3D v1, Point3D v2, CoordinateAxis axis) : base(v1, v2)
        {
            CoordinateAxis = axis;
            if(CoordinateAxis == CoordinateAxis.X)
            {
                NonSelectedColor = Color.Red;
            }
            else if (CoordinateAxis == CoordinateAxis.Y)
            {
                NonSelectedColor = Color.Green;
            }
            else if (CoordinateAxis == CoordinateAxis.Z)
            {
                NonSelectedColor = Color.Blue;
            }
        }
    }

    public class AxisSystem: ISceneObject
    {
        private List<Axis> axes = new List<Axis>();
        private List<AxisCube> axisCubes = new List<AxisCube>();
        public double AxisLength = 3.0f;
        public float AxisWidth = 2.8f;

        private Point3D centerPoint;
        public ISceneObject ParentObject { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public bool IsSelected { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

        public bool X;
        public bool MinusX;

        public AxisSystem() 
        {
            axes = new List<Axis>();
        }

        public AxisSystem(Point3D centerPoint, Ray ray) 
        {

            Vector distance = ray.Origin - new Vector(centerPoint);
            AxisLength += 1.0f;

            double xCoord = distance[0] - centerPoint.X > 0 ? AxisLength + centerPoint.X : centerPoint.X - AxisLength;
            double yCoord = distance[1] - centerPoint.Y > 0 ? AxisLength + centerPoint.Y : centerPoint.Y - AxisLength;
            double zCoord = distance[2] - centerPoint.Z > 0 ? AxisLength + centerPoint.Z : centerPoint.Z - AxisLength;

            Axis axisX = new Axis((Point3D)centerPoint.Clone(), new Point3D(xCoord, centerPoint.Y, centerPoint.Z), CoordinateAxis.X);
            Axis axisY = new Axis((Point3D)centerPoint.Clone(), new Point3D(centerPoint.X, yCoord, centerPoint.Z), CoordinateAxis.Y);
            Axis axisZ = new Axis((Point3D)centerPoint.Clone(), new Point3D(centerPoint.X, centerPoint.Y, zCoord), CoordinateAxis.Z);

            axisX.NonSelectedColor = Color.Red;
            axisY.NonSelectedColor = Color.Green;
            axisZ.NonSelectedColor = Color.Blue;

            axisX.LineWidth = AxisWidth;
            axisY.LineWidth = AxisWidth;
            axisZ.LineWidth = AxisWidth;
            axes = new List<Axis> { axisX, axisY, axisZ };

            //Create Axis Cubes
            AxisCube cubeX = new AxisCube(new Point3D(xCoord, centerPoint.Y, centerPoint.Z), CoordinateAxis.X, new Vector(0.1, 0.1, 0.1), "cubeAxisX");
            AxisCube cubeY = new AxisCube(new Point3D(centerPoint.X, yCoord, centerPoint.Z), CoordinateAxis.Y, new Vector(0.1, 0.1, 0.1), "cubeAxisY");
            AxisCube cubeZ = new AxisCube(new Point3D(centerPoint.X, centerPoint.Y, zCoord), CoordinateAxis.Z, new Vector(0.1, 0.1, 0.1), "cubeAxisZ");

            //Set colors
            cubeX.FacetSelectedColor = Color.Red;
            cubeY.FacetSelectedColor = Color.Green;
            cubeZ.FacetSelectedColor = Color.Blue;

            cubeX.FacetNonSelectedColor = Color.Red;
            cubeY.FacetNonSelectedColor = Color.Green;
            cubeZ.FacetNonSelectedColor = Color.Blue;

            axisCubes = new List<AxisCube> { cubeX, cubeY, cubeZ };
        }

        public Point3D GetCenterPoint()
        {
            return centerPoint;
        }

        public void Draw()
        {

            foreach (AxisCube cube in axisCubes)
            {
                cube.Draw();
            }


            GraphicsGL.GL.Begin(OpenGL.GL_LINES);

            foreach (Axis axis in axes)
            {
                axis.Draw();
            }

            GraphicsGL.GL.End();
            GraphicsGL.GL.Flush();
        }

        public void Move(Vector v)
        {
            for (int i = 0; i < axisCubes.Count; i++)
            {
                axisCubes[i].Move(v);
                axes[i].Move(v);
            }
        }

        public void Select()
        {
            foreach (AxisCube cube in axisCubes)
            {
                cube.Deselect();
            }
        }

        public void Deselect()
        {
            foreach (AxisCube cube in axisCubes)
            {
                cube.Deselect();
            }
        }

        public (ISceneObject, double) CheckSelected(int x, int y)
        {
            Ray ray = GraphicsGL.InitializeRay(x, GraphicsGL.GetHeight() - y);
            foreach (AxisCube cube in axisCubes)
            {
                cube.Deselect();
            }

            for (int i = 0; i < axisCubes.Count; i++)
            {

                (Plane, double) selectedFacet = axisCubes[i].CheckSelectedFacet(ray);
                if (selectedFacet.Item1 != null)
                {
                    axisCubes[i].IsSelected = true;
                    return (axisCubes[i], selectedFacet.Item2);
                }
            }

            return (null, 0);
        }

        public void CreateAxis(CoordinateAxisType axis, Point3D centerPoint)
        {
            Point3D endPoint = (Point3D)centerPoint.Clone();
            CoordinateAxis coordinateAxis = new CoordinateAxis();

            if(axis == CoordinateAxisType.PlusX)
            {
                endPoint.X += AxisLength;
                coordinateAxis = CoordinateAxis.X;
            }
            else if (axis == CoordinateAxisType.MinusX)
            {
                endPoint.X -= AxisLength;
                coordinateAxis = CoordinateAxis.X;
            }
            else if(axis == CoordinateAxisType.PlusY)
            {
                endPoint.Y += AxisLength;
                coordinateAxis = CoordinateAxis.Y;
            }
            else if (axis == CoordinateAxisType.MinusY)
            {
                endPoint.Y -= AxisLength;
                coordinateAxis = CoordinateAxis.Y;
            }
            else if (axis == CoordinateAxisType.PlusZ)
            {
                endPoint.Z += AxisLength;
                coordinateAxis = CoordinateAxis.Z;
            }
            else if (axis == CoordinateAxisType.MinusZ)
            {
                endPoint.Z -= AxisLength;
                coordinateAxis = CoordinateAxis.Z;
            }


            Axis axisLine = new Axis((Point3D)centerPoint.Clone(), endPoint, coordinateAxis);
            axisLine.LineWidth = AxisWidth;
            axes.Add(axisLine);
        }

        public List<Axis> GetAxes(CoordinateAxis axis)
        {
            List<Axis> resultAxes = new List<Axis>();

            foreach(Axis axisLine in axes)
            {
                if(axisLine.CoordinateAxis == axis)
                {
                    resultAxes.Add(axisLine);
                }
            }

            return resultAxes;
        }

        public static CoordinateAxisType GetOppositeAxisType(CoordinateAxisType type)
        {
            switch(type)
            {
                case CoordinateAxisType.PlusX: return CoordinateAxisType.MinusX;
                case CoordinateAxisType.MinusX: return CoordinateAxisType.PlusX;
                case CoordinateAxisType.PlusY: return CoordinateAxisType.MinusY;
                case CoordinateAxisType.MinusY: return CoordinateAxisType.PlusY;
                case CoordinateAxisType.PlusZ: return CoordinateAxisType.MinusZ;
                case CoordinateAxisType.MinusZ: return CoordinateAxisType.PlusZ;
                default: return CoordinateAxisType.PlusX;
            }
        }

        public static CoordinateAxis GetAxisFromType(CoordinateAxisType type)
        {
            switch (type)
            {
                case CoordinateAxisType.PlusX: 
                case CoordinateAxisType.MinusX: return CoordinateAxis.X;
                case CoordinateAxisType.PlusY: 
                case CoordinateAxisType.MinusY: return CoordinateAxis.Y;
                case CoordinateAxisType.PlusZ: 
                case CoordinateAxisType.MinusZ: return CoordinateAxis.Z;
                default: return CoordinateAxis.X;
            }
        }

        public static (CoordinateAxisType, CoordinateAxisType) GetAxisTypesFromAxis(CoordinateAxis axis)
        {
            switch (axis)
            {
                case CoordinateAxis.X: return (CoordinateAxisType.PlusX, CoordinateAxisType.MinusX);
                case CoordinateAxis.Y: return (CoordinateAxisType.PlusY, CoordinateAxisType.MinusY);
                case CoordinateAxis.Z: return (CoordinateAxisType.PlusZ, CoordinateAxisType.MinusZ);
                default: return (CoordinateAxisType.PlusX, CoordinateAxisType.MinusX);
            }
        }

        public ISceneObject Clone()
        {
            List<Axis> axes = new List<Axis>();
            List<AxisCube> axisCubes = new List<AxisCube>();

            for (int i = 0; i < this.axes.Count; i++)
            {
                axes.Add(this.axes[i]);
                axisCubes.Add(this.axisCubes[i]);
            }

            AxisSystem axisSystem = new AxisSystem();
            axisSystem.axes = axes;
            axisSystem.axisCubes = axisCubes;
            return axisSystem;
        }

        public bool IsEqual(ISceneObject obj)
        {
            if (obj != null && obj is AxisSystem)
            {
                AxisSystem axisSystem = obj as AxisSystem;

                for (int i = 0; i < axisCubes.Count; i++)
                {
                    if (!this.axisCubes[i].IsEqual(axisSystem.axisCubes[i]))
                    {
                        return false;
                    }
                }

                return true;
            }

            return false;
        }
    }
}
