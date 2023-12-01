using CadEditor.Graphics;
using SharpGL;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CadEditor.MeshObjects
{
    public class AxisSystem: ISceneObject
    {
        private List<Axis> axes = new List<Axis>();
        private List<AxisCube> axisCubes = new List<AxisCube>();
        public double AxisLength = 1.0f;
        public float AxisWidth = 2.8f;

        private Point3D centerPoint;
        public ISceneObject ParentObject { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public bool IsSelected { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

        public AxisSystem(ISceneObject obj) 
        {
            if (obj is Point3D)
            {
                centerPoint = ((Point3D)obj).Clone();
            }
            else
            {
                centerPoint = obj.GetCenterPoint();
            }

            if(obj is ComplexCube)
            {
                AxisLength += 1.0f;
            }


            //Create Axis Lines
            double[] axisXYZLengths = new double[3];   //axisXLength, axisYLength, axisZLength

            for (int i = 0; i < axisXYZLengths.Length; i++)
            {
                double multiplier = 1;

                if (centerPoint[i] < 0)
                {
                    multiplier = -1;
                }
                else if (centerPoint[i] > 0)
                {
                    multiplier = 1;
                }
                else
                {
                    axisXYZLengths[i] = AxisLength;
                }

                axisXYZLengths[i] = AxisLength * multiplier;
            }

            Axis axisX = new Axis(centerPoint.Clone(), new Point3D(axisXYZLengths[0] + centerPoint.X, centerPoint.Y, centerPoint.Z), CoordinateAxis.X);
            Axis axisY = new Axis(centerPoint.Clone(), new Point3D(centerPoint.X, axisXYZLengths[1] + centerPoint.Y, centerPoint.Z), CoordinateAxis.Y);
            Axis axisZ = new Axis(centerPoint.Clone(), new Point3D(centerPoint.X, centerPoint.Y, axisXYZLengths[2] + centerPoint.Z), CoordinateAxis.Z);


            axisX.NonSelectedColor = Color.Red;
            axisY.NonSelectedColor = Color.Green;
            axisZ.NonSelectedColor = Color.Blue;

            axisX.LineWidth = AxisWidth;
            axisY.LineWidth = AxisWidth;
            axisZ.LineWidth = AxisWidth;
            axes = new List<Axis> { axisX, axisY, axisZ };

            //Create Axis Cubes
            AxisCube cubeX = new AxisCube(new Point3D(axisXYZLengths[0] + centerPoint.X, centerPoint.Y, centerPoint.Z), CoordinateAxis.X, new Vector(0.1, 0.1, 0.1), "cubeAxisX");
            AxisCube cubeY = new AxisCube(new Point3D(centerPoint.X, axisXYZLengths[1] + centerPoint.Y, centerPoint.Z), CoordinateAxis.Y, new Vector(0.1, 0.1, 0.1), "cubeAxisY");
            AxisCube cubeZ = new AxisCube(new Point3D(centerPoint.X, centerPoint.Y, axisXYZLengths[2] + centerPoint.Z), CoordinateAxis.Z, new Vector(0.1, 0.1, 0.1), "cubeAxisZ");

            //Set colors
            cubeX.FacetSelectedColor = Color.Red;
            cubeY.FacetSelectedColor = Color.Green;
            cubeZ.FacetSelectedColor = Color.Blue;

            cubeX.FacetNonSelectedColor = Color.Red;
            cubeY.FacetNonSelectedColor = Color.Green;
            cubeZ.FacetNonSelectedColor = Color.Blue;

            axisCubes = new List<AxisCube> { cubeX, cubeY, cubeZ };
        }

        public void InitAxisCubes()
        {

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

        public ISceneObject CheckSelected()
        {
            Ray ray = GraphicsGL.InitializeRay(MouseController.X, GraphicsGL.GetHeight() - MouseController.Y);
            foreach (AxisCube cube in axisCubes)
            {
                cube.Deselect();
            }

            for (int i = 0; i < axisCubes.Count; i++)
            {

                Plane selectedFacet = axisCubes[i].CheckSelectedFacet(ray);
                if (selectedFacet != null)
                {
                    axisCubes[i].IsSelected = true;
                    return axisCubes[i];
                }
            }

            return null;
        }
    }
}
