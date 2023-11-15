using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using SharpGL;
using CadEditor.Maths;
using CadEditor.MeshObjects;
using MathNet.Spatial.Euclidean;
using System.Linq.Expressions;
using CadEditor.Graphics;
using System.Linq;
using System.Runtime.InteropServices;

namespace CadEditor
{
    public class ComplexCube: MeshObject3D
    {
        private const int OUTER_VERTICES_AMOUNT = 20;
        private const int VERTICES_ON_FACET_AMOUNT = 8;
        public Point3D[] bigCubePoints;
        public LocalSystem localSystem;

        public class LocalSystem
        {
            public Mesh localMesh;
            public Point3D[] OuterVertices = new Point3D[20];
            public List<int> outerVerticesIndices = new List<int>();

            public void InitLocalSystem(ComplexCube cubeToDivide, Vector nValues)
            {

                double feAmountX = nValues[0];
                double feAmountY = nValues[1];
                double feAmountZ = nValues[2];

                double VerticesAmountX = feAmountX * 2 + 1;
                double VerticesAmountY = feAmountY * 2 + 1;
                double VerticesAmountZ = feAmountZ * 2 + 1;

                //size of smaller cube
                double feSizeX = cubeToDivide.Size[0] / feAmountX;
                double feSizeY = cubeToDivide.Size[1] / feAmountY;
                double feSizeZ = cubeToDivide.Size[2] / feAmountZ;

                //dividing into small cubes
                List<ComplexCube> FeCubeList = new List<ComplexCube>();
                for (int i_z = 1; i_z < VerticesAmountZ; i_z += 2)
                {
                    double feCenterPointZ = -cubeToDivide.Size[2] + i_z * feSizeZ;

                    for (int i_y = 1; i_y < VerticesAmountY; i_y += 2)
                    {
                        double feCenterPointY = -cubeToDivide.Size[1] + i_y * feSizeY;

                        for (int i_x = 1; i_x < VerticesAmountX; i_x += 2)
                        {
                            double feCenterPointX = -cubeToDivide.Size[0] + i_x * feSizeX;

                            Point3D feCenterPoint = new Point3D(feCenterPointX + cubeToDivide.CenterPoint[0], feCenterPointY + cubeToDivide.CenterPoint[1], feCenterPointZ + cubeToDivide.CenterPoint[2]);  //create a center point of finite element (cube)
                            ComplexCube feCube = new ComplexCube(feCenterPoint, new Vector(feSizeX, feSizeY, feSizeZ)); //create a finite element (cube) with sizes
                            feCube.ParentObject = cubeToDivide;
                            FeCubeList.Add(feCube);                                                             //add it to list of finite elements
                        }
                    }
                }

                //convert all cubes into big one mesh

                List<Point3D> uniquePoints = new List<Point3D>();
                List<Line> uniqueLines = new List<Line>();

                for (int i = 0; i < FeCubeList.Count; i++)
                {
                    for (int j = 0; j < FeCubeList[i].Mesh.Vertices.Count; j++)
                    {
                        Point3D p = FeCubeList[i].Mesh.Vertices[j];

                        if (uniquePoints.Count > 0)
                        {
                            bool isEqual = false;
                            for (int m = 0; m < uniquePoints.Count; m++)
                            {
                                Point3D pt = uniquePoints[m];
                                if (pt == p)
                                {
                                    isEqual = true;
                                    break;
                                }
                            }

                            if (!isEqual)
                            {
                                p.ParentCube = cubeToDivide;
                                p.PositionInCube = j;
                                p.ParentObject = cubeToDivide;
                                outerVerticesIndices.Add(j);
                                uniquePoints.Add(p.Clone());
                            }
                        }
                        else
                        {
                            p.ParentCube = cubeToDivide;
                            p.PositionInCube = j;
                            p.ParentObject = cubeToDivide;
                            outerVerticesIndices.Add(j);
                            uniquePoints.Add(p.Clone());
                        }
                    }
                }

                //new line will be created in such way: line1 and line2 are different by some size
                for (int i = 0; i < FeCubeList.Count; i++)
                {
                    ComplexCube feCube = FeCubeList[i];
                    for (int j = 0; j < feCube.Mesh.Edges.Count; j++)
                    {
                        Line line = feCube.Mesh.Edges[j];

                        //if line is not found then create another one base on unique points
                        Point3D p1 = null;
                        Point3D p2 = null;
                        foreach (Point3D p in uniquePoints)
                        {
                            if (line.P1 == p)
                            {
                                p1 = p;
                            }
                        }
                        foreach (Point3D p in uniquePoints)
                        {
                            if (line.P2 == p)
                            {
                                p2 = p;
                            }
                        }

                        Line newLine = new Line(p1, p2);
                        newLine.ParentObject = cubeToDivide;
                        uniqueLines.Add(newLine);
                    }
                }

                Mesh resultMesh = new Mesh();
                resultMesh.Vertices = uniquePoints;
                resultMesh.Edges = uniqueLines;
                localMesh = resultMesh;


            }

            public void InitLocalOuterVertices(Mesh prevMesh)
            {
                for (int i = 0; i < prevMesh.Vertices.Count; i++)
                {
                    OuterVertices[i] = prevMesh.Vertices[i].Clone();

                    if (outerVerticesIndices.Count == 0)
                    {
                        OuterVertices[i].PositionInCube = prevMesh.Vertices[i].PositionInCube;
                    }
                    else
                    {
                        OuterVertices[i].PositionInCube = outerVerticesIndices[i];
                    }
                }

            }

            private List<Point3D> InitPoints(Point3D CenterPoint, Vector size)
                {
                    double sizeX = size[0];
                    double sizeY = size[1];
                    double sizeZ = size[2];

                    List<Point3D> points = new List<Point3D>
                {
                    new Point3D(-sizeX + CenterPoint.X, -sizeY + CenterPoint.Y, sizeZ + CenterPoint.Z),
                    new Point3D(sizeX + CenterPoint.X, -sizeY + CenterPoint.Y, sizeZ + CenterPoint.Z),
                    new Point3D(sizeX + CenterPoint.X, -sizeY + CenterPoint.Y, -sizeZ + CenterPoint.Z),
                    new Point3D(-sizeX + CenterPoint.X, -sizeY + CenterPoint.Y, -sizeZ + CenterPoint.Z),
                    new Point3D(-sizeX + CenterPoint.X, sizeY + CenterPoint.Y, sizeZ + CenterPoint.Z),
                    new Point3D(sizeX + CenterPoint.X, sizeY + CenterPoint.Y, sizeZ + CenterPoint.Z),
                    new Point3D(sizeX + CenterPoint.X, sizeY + CenterPoint.Y, -sizeZ + CenterPoint.Z),
                    new Point3D(-sizeX + CenterPoint.X, sizeY + CenterPoint.Y, -sizeZ + CenterPoint.Z),

                    new Point3D(0 + CenterPoint.X, -sizeY + CenterPoint.Y, sizeZ + CenterPoint.Z),
                    new Point3D(sizeX + CenterPoint.X, -sizeY + CenterPoint.Y, 0 + CenterPoint.Z),
                    new Point3D(0 + CenterPoint.X, -sizeY + CenterPoint.Y, -sizeZ + CenterPoint.Z),
                    new Point3D(-sizeX + CenterPoint.X, -sizeY + CenterPoint.Y, 0 + CenterPoint.Z),
                    new Point3D(-sizeX + CenterPoint.X, 0 + CenterPoint.Y, sizeZ + CenterPoint.Z),
                    new Point3D(sizeX + CenterPoint.X, 0 + CenterPoint.Y, sizeZ + CenterPoint.Z),
                    new Point3D(sizeX + CenterPoint.X, 0 + CenterPoint.Y, -sizeZ + CenterPoint.Z),
                    new Point3D(-sizeX + CenterPoint.X, 0 + CenterPoint.Y, -sizeZ + CenterPoint.Z),
                    new Point3D(0 + CenterPoint.X, sizeY + CenterPoint.Y, sizeZ + CenterPoint.Z),
                    new Point3D(sizeX + CenterPoint.X, sizeY + CenterPoint.Y, 0 + CenterPoint.Z),
                    new Point3D(0 + CenterPoint.X, sizeY + CenterPoint.Y, -sizeZ + CenterPoint.Z),
                    new Point3D(-sizeX + CenterPoint.X, sizeY + CenterPoint.Y, 0 + CenterPoint.Z),
                };


                return points;
            }

            public void UpdateOuterVerticesPositions()
            {
                List<Point3D> sampleList = InitPoints(new Point3D(0, 0, 0), new Vector(1, 1, 1));

                for (int i = 0; i < localMesh.Vertices.Count; i++)
                {
                    Point3D p1 = OuterVertices[i];
                    Point3D p2 = localMesh.Vertices[i];

                    if (p1 == p2)
                    {
                        OuterVertices[i].PositionInCube = p2.PositionInCube;
                    }
                }
            }
        }


        public bool IsDivided { get; private set; } = false;

        public ComplexCube(Point3D _centerPoint, Vector _size, string _cubeName = null):
            base(_centerPoint, _size, _cubeName)
		{
			//Initializing Mesh
			InitPoints(CenterPoint, Size);
            InitFacets(Mesh);
            InitEdges(Mesh);

            bigCubePoints = new Point3D[OUTER_VERTICES_AMOUNT];
            bigCubePoints = GetOuterCubeVertices(Mesh);

            localSystem = new LocalSystem();
            localSystem.InitLocalOuterVertices(Mesh);
        }

        public ComplexCube(Mesh mesh) : base(mesh) 
        {
            bigCubePoints = GetOuterCubeVertices(Mesh);
        }

        private void InitPoints(Point3D CenterPoint, Vector size)
        {
            double sizeX = size[0];
            double sizeY = size[1];
            double sizeZ = size[2];

            List<Point3D> points = new List<Point3D>
            {
                new Point3D(-sizeX + CenterPoint.X, -sizeY + CenterPoint.Y, sizeZ + CenterPoint.Z),
                new Point3D(sizeX + CenterPoint.X, -sizeY + CenterPoint.Y, sizeZ + CenterPoint.Z),
                new Point3D(sizeX + CenterPoint.X, -sizeY + CenterPoint.Y, -sizeZ + CenterPoint.Z),
                new Point3D(-sizeX + CenterPoint.X, -sizeY + CenterPoint.Y, -sizeZ + CenterPoint.Z),
                new Point3D(-sizeX + CenterPoint.X, sizeY + CenterPoint.Y, sizeZ + CenterPoint.Z),
                new Point3D(sizeX + CenterPoint.X, sizeY + CenterPoint.Y, sizeZ + CenterPoint.Z),
                new Point3D(sizeX + CenterPoint.X, sizeY + CenterPoint.Y, -sizeZ + CenterPoint.Z),
                new Point3D(-sizeX + CenterPoint.X, sizeY + CenterPoint.Y, -sizeZ + CenterPoint.Z),

                new Point3D(0 + CenterPoint.X, -sizeY + CenterPoint.Y, sizeZ + CenterPoint.Z),
                new Point3D(sizeX + CenterPoint.X, -sizeY + CenterPoint.Y, 0 + CenterPoint.Z),
                new Point3D(0 + CenterPoint.X, -sizeY + CenterPoint.Y, -sizeZ + CenterPoint.Z),
                new Point3D(-sizeX + CenterPoint.X, -sizeY + CenterPoint.Y, 0 + CenterPoint.Z),
                new Point3D(-sizeX + CenterPoint.X, 0 + CenterPoint.Y, sizeZ + CenterPoint.Z),
                new Point3D(sizeX + CenterPoint.X, 0 + CenterPoint.Y, sizeZ + CenterPoint.Z),
                new Point3D(sizeX + CenterPoint.X, 0 + CenterPoint.Y, -sizeZ + CenterPoint.Z),
                new Point3D(-sizeX + CenterPoint.X, 0 + CenterPoint.Y, -sizeZ + CenterPoint.Z),
                new Point3D(0 + CenterPoint.X, sizeY + CenterPoint.Y, sizeZ + CenterPoint.Z),
                new Point3D(sizeX + CenterPoint.X, sizeY + CenterPoint.Y, 0 + CenterPoint.Z),
                new Point3D(0 + CenterPoint.X, sizeY + CenterPoint.Y, -sizeZ + CenterPoint.Z),
                new Point3D(-sizeX + CenterPoint.X, sizeY + CenterPoint.Y, 0 + CenterPoint.Z),
            };

            Mesh.Vertices = points;

            for (int i = 0; i < Mesh.Vertices.Count; i++)
            {
                Mesh.Vertices[i].PositionInCube = i;
                Mesh.Vertices[i].ParentCube = this;
                Mesh.Vertices[i].ParentObject = this;
            }
            CenterPoint.ParentCube = this;
        }

        private void InitFacets(Mesh mesh)
        {
            List<Plane> facets = new List<Plane>
            {
				//FRONT
				new Plane(new List<Point3D>
                {
                    mesh.Vertices[0],
                    mesh.Vertices[8],
                    mesh.Vertices[1],
                    mesh.Vertices[13],
                    mesh.Vertices[5],
                    mesh.Vertices[16],
                    mesh.Vertices[4],
                    mesh.Vertices[12]
                }),

				//RIGHT
				new Plane(new List<Point3D>
                {
                    mesh.Vertices[1],
                    mesh.Vertices[9],
                    mesh.Vertices[2],
                    mesh.Vertices[14],
                    mesh.Vertices[6],
                    mesh.Vertices[17],
                    mesh.Vertices[5],
                    mesh.Vertices[13]
                }),

				//BACK
				new Plane(new List<Point3D>
                {
                    mesh.Vertices[2],
                    mesh.Vertices[10],
                    mesh.Vertices[3],
                    mesh.Vertices[15],
                    mesh.Vertices[7],
                    mesh.Vertices[18],
                    mesh.Vertices[6],
                    mesh.Vertices[14]
                }),

				//LEFT
				new Plane(new List<Point3D>
                {
                    mesh.Vertices[3],
                    mesh.Vertices[11],
                    mesh.Vertices[0],
                    mesh.Vertices[12],
                    mesh.Vertices[4],
                    mesh.Vertices[19],
                    mesh.Vertices[7],
                    mesh.Vertices[15]
                }),

				//TOP
				new Plane(new List<Point3D>
                {
                    mesh.Vertices[4],
                    mesh.Vertices[16],
                    mesh.Vertices[5],
                    mesh.Vertices[17],
                    mesh.Vertices[6],
                    mesh.Vertices[18],
                    mesh.Vertices[7],
                    mesh.Vertices[19]
                }),

				//BOTTOM
				new Plane(new List<Point3D>
                {
                    mesh.Vertices[1],
                    mesh.Vertices[8],
                    mesh.Vertices[0],
                    mesh.Vertices[11],
                    mesh.Vertices[3],
                    mesh.Vertices[10],
                    mesh.Vertices[2],
                    mesh.Vertices[9]
                })
            };

            foreach(Plane plane in facets)
            {
                plane.ParentObject = this;
            }

            Mesh.Facets = facets;
        }

        private void InitEdges(Mesh mesh)
        {
            List<Line> meshEdges = new List<Line>();
            for (int i = 0; i < mesh.Facets.Count; i++)
            {
                Plane currentFacet = mesh.Facets[i];
                for (int j = 0; j < VERTICES_ON_FACET_AMOUNT; j++)
                {
                    Line newEdge = new Line(currentFacet.Points[j], currentFacet.Points[(j + 1) % VERTICES_ON_FACET_AMOUNT]);
                    if (meshEdges.Count != 0)
                    {
                        if (!newEdge.Exists(meshEdges))
                        {
                            meshEdges.Add(newEdge);

                            //Defining Edge - Vertex relationship
                            mesh.Vertices[j].EdgeParents.Add(newEdge);
                            mesh.Vertices[(j + 1) % VERTICES_ON_FACET_AMOUNT].EdgeParents.Add(newEdge);
                        }
                    }
                    else
                    {
                        meshEdges.Add(newEdge);

                        //Defining Edge - Vertex relationship
                        mesh.Vertices[j].EdgeParents.Add(newEdge);
                        mesh.Vertices[(j + 1) % VERTICES_ON_FACET_AMOUNT].EdgeParents.Add(newEdge);
                    }
                }
            }

            foreach (Line line in meshEdges)
            {
                line.ParentObject = this;
            }

            Mesh.Edges = meshEdges;
        }

        public new void Draw()
        {
            base.Draw();

            if (bigCubePoints.Length > 0)
            {
                //Draw Vertexes
                GraphicsGL.GL.PointSize(7.0f);
                GraphicsGL.GL.Begin(OpenGL.GL_POINTS);
                for (int i = 0; i < bigCubePoints.Length; i++)
                {
                    if (IsSelected)
                    {
                        bigCubePoints[i].SelectedColor = Color.Yellow;
                    }
                    else
                    {
                        bigCubePoints[i].NonSelectedColor = Color.Green;
                    }

                    bigCubePoints[i].Draw();
                }
                GraphicsGL.GL.End();
                GraphicsGL.GL.Flush();
            }
        }

        /// <summary>
        /// This method divide big Cube into some smaller ones. Each cube is called as finite element (feCube in code)
        /// After dividing into smaller cubes, the method Compress will be used to remove duplicated vertices and edges.
        /// </summary>
        /// <param name="nValues"> This array shows how many cubes should be the big one divided into in some axis</param>
        public void Divide(Vector nValues)
        {
            Mesh prevMesh = this.Mesh.Clone();
            //localSystem = GetDividedLocalSystem(nValues);
            localSystem.InitLocalSystem(this, nValues);
            localSystem.InitLocalOuterVertices(prevMesh);

            //this.Mesh.Vertices.Clear();
            //foreach(Point3D p in localSystem.Vertices)
            //{
            //    this.Mesh.Vertices.Add(p.Clone());
            //}

            //this.Mesh.Edges.Clear();
            //foreach (Line p in localSystem.Edges)
            //{
            //    this.Mesh.Edges.Add(p.Clone());
            //}

            Scene.ActiveMovingAxis = CoordinateAxis.X;
            Transform();

            Scene.ActiveMovingAxis = CoordinateAxis.Y;
            Transform();

            Scene.ActiveMovingAxis = CoordinateAxis.Z;
            Transform();

            //localSystem.InitLocalOuterVertices(prevMesh);
            //bigCubePoints = GetOuterCubeVertices(Mesh);
            UpdateBigCubePoints();
            //OuterVertices = GetOuterCubeVertices(this.Mesh, prevMesh);
            IsDivided = true;


        }

        private void UpdateBigCubePoints()
        {
            for(int i = 0; i < localSystem.OuterVertices.Length; i++)
            {
                int pos = localSystem.OuterVertices[i].PositionInCube;

                bigCubePoints[i] = Mesh.Vertices[pos];
            }
        }

        //private Mesh GetDividedLocalSystem(Vector nValues)
        //{
            
        //}

        private Point3D[] GetOuterCubeVertices(Mesh currentMesh)
        {
            Point3D[] resultVertices = new Point3D[OUTER_VERTICES_AMOUNT];
            //point of big cube
            int counter = 0;
            for (int i = 0; i < currentMesh.Vertices.Count; i++)
            {
                Point3D p = currentMesh.Vertices[i];
                int pos = p.PositionInCube;

                resultVertices[counter] = currentMesh.Vertices[pos];
                counter++;
            }

            return resultVertices;
        }

        public void Update(Point3D p)
        {
            int index = p.PositionInCube;
            localSystem.OuterVertices[index][0] += Scene.MovingVector[0];
            localSystem.OuterVertices[index][1] += Scene.MovingVector[1];
            localSystem.OuterVertices[index][2] += Scene.MovingVector[2];
        }

        /// <summary>
        /// Transform method used to transform each point of this cube using approximation formulas
        /// </summary>
        /// <param name="index">defines which coordinate would be operated on</param>
        /// <param name="cube">defines a cube from which all points will be transformed</param>
        public void Transform()
        {
            //Vector vector = Scene.MovingVector;
            Mesh currentMesh = this.Mesh;
            //int index = -1;
            //if (vector[0] != 0) index = 0;
            //else if (vector[1] != 0) index = 1;
            //else if (vector[2] != 0) index = 2;

            int index = -1;

            switch(Scene.ActiveMovingAxis)
            {
                case CoordinateAxis.X: index = 0; break;
                case CoordinateAxis.Y: index = 1; break;
                case CoordinateAxis.Z: index = 2; break;
            }

            if (index != -1)
            {
                if(IsDivided == false)
                {
                    currentMesh = localSystem.localMesh.Clone();
                }

                for (int i = 0; i < localSystem.localMesh.Vertices.Count; i++)
                {
                    Point3D point1 = localSystem.localMesh.Vertices[i];
                    double sum = 0;
                    for (int m = 0; m < localSystem.OuterVertices.Length; m++)
                    {
                        Point3D point2 = localSystem.OuterVertices[m];

                        Func<Point3D, Point3D, double> phiFunc = null;
                        if (m >= 0 && m < 8)
                        {
                            phiFunc = TransformMesh.PhiAngle;
                        }
                        else if (m >= 8 && m < localSystem.OuterVertices.Length)
                        {
                            phiFunc = TransformMesh.PhiEdge;
                        }

                        double funcResult = phiFunc(point1, point2);
                        sum += point2[index] * funcResult;
                    }

                    currentMesh.Vertices[i][index] = sum;
                }
            }

            this.Mesh = currentMesh;
        }

        //public void Transform(Vector vector, Point3D point)
        //{


        //    if (vector[0] == 0 && vector[1] == 0 && vector[2] == 0)
        //    {
        //        return;
        //    }
        //    else
        //    {
        //        int index = 2;
        //        Point3D[] cloneList = new Point3D[20];
        //        for (int i = 0; i < bigCubePoints.Length; i++)
        //        {
        //            cloneList[i] = bigCubePoints[i].Clone();
        //        }

        //        for (int i = 0; i < localSystem.Vertices.Count; i++)
        //        {
        //            Point3D point1 = localSystem.Vertices[i];
        //            double sum = 0;
        //            for (int m = 0; m < cloneList.Length; m++)
        //            {
        //                Point3D point2 = cloneList[m];

        //                Func<Point3D, Point3D, double> phiFunc = null;
        //                if (m >= 0 && m < 8)
        //                {
        //                    phiFunc = TransformMesh.PhiAngle;
        //                }
        //                else if (m >= 8 && m < cloneList.Length)
        //                {
        //                    phiFunc = TransformMesh.PhiEdge;
        //                }

        //                double funcResult = phiFunc(point2, point1);
        //                sum += point1[index] * funcResult;
        //            }

        //            this.Mesh.Vertices[i][index] = sum;
        //        }
        //    }
        //}

        public ComplexCube Clone()
        {
            ComplexCube cube = new ComplexCube(Mesh.Clone());

            for(int i = 0; i < this.bigCubePoints.Length; i++)
            {
                int indexOfPoint = this.Mesh.GetIndexOfPoint(this.bigCubePoints[i]);
                if(indexOfPoint != -1)
                {
                    Point3D point = cube.Mesh.Vertices[indexOfPoint];
                    cube.bigCubePoints[i] = point;
                }
            }

			return cube;
        }

		public bool Equals(ComplexCube cube)
		{
			if (cube != null)
			{
				return this.Mesh.Equals(cube.Mesh);
			}

			return false;
		}

        public string Export()
        {
            string exportString = "";

            exportString += Name + "\n";
            //exportString += "Center_Point: " + CenterPoint.ToString() + "\n";
            exportString += "Vertices:\n";
            for(int i = 0; i < Mesh.Vertices.Count; i++)
            {
                exportString += String.Format("({0} {1} {2})", Mesh.Vertices[i].X, Mesh.Vertices[i].Y, Mesh.Vertices[i].Z);
                exportString += "\n";
            }

            exportString += "\n";
			exportString += "Edges:\n";
			for (int i = 0; i < Mesh.Edges.Count; i++)
			{
                int indexOfPoint1 = this.Mesh.GetIndexOfPoint(Mesh.Edges[i].P1);
                int indexOfPoint2 = this.Mesh.GetIndexOfPoint(Mesh.Edges[i].P2);
				exportString += String.Format("L_{0} {1}", indexOfPoint1, indexOfPoint2);
				exportString += "\n";
			}

			return exportString;
        }
    }
}
