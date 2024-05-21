using System;
using System.Collections.Generic;
using CadEditor.Maths;
using CadEditor.MeshObjects;
using System.Text;
using CadEditor.Models.Scene.MeshObjects;
using CadEditor.Models.Scene;
using System.Reflection.Emit;

namespace CadEditor
{
    public class ComplexCube: MeshObject3D, IDivideable, IExportable, IRotateable
    {
        private const int OUTER_VERTICES_AMOUNT = 20;
        public Point3D[] OuterVertices { get; set; }
        private LocalSystem localSystem;
        public bool IsDivided { get; set; } = false;

        public float xRotation { get; set; } = 0.0f;
        public float yRotation { get; set; } = 0.0f;

        public class LocalSystem: ICloneable
        {
            public Mesh LocalMesh { get; set; }
            public Mesh TransformMesh { get; set; }
            public Point3D[] OuterLocalVertices = new Point3D[20];
            public List<int> OuterVerticesIndices;

            public LocalSystem()
            {
                OuterVerticesIndices = new List<int>();
            }

            public LocalSystem(LocalSystem systemToClone)
            {
                if (systemToClone.LocalMesh != null)
                {
                    LocalMesh = systemToClone.LocalMesh?.Clone();
                    TransformMesh = systemToClone.TransformMesh?.Clone();
                    OuterLocalVertices = new Point3D[20];
                    for (int i = 0; i < OuterLocalVertices.Length; i++)
                    {
                        OuterLocalVertices[i] = (Point3D)systemToClone.OuterLocalVertices[i].Clone();
                    }
                    OuterVerticesIndices = new List<int>(systemToClone.OuterVerticesIndices);
                }
                else
                {
                    OuterVerticesIndices = new List<int>();
                }
            }

            public void InitTransformMesh()
            {
                TransformMesh = LocalMesh.Clone();
            }

            public void InitLocalSystem(ComplexCube cubeToDivide, Vector nValues, ComplexCube globalCube)
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

                            Point3D feCenterPoint = new Point3D(feCenterPointX + cubeToDivide.GetCenterPoint()[0], feCenterPointY + cubeToDivide.GetCenterPoint()[1], feCenterPointZ + cubeToDivide.GetCenterPoint()[2]);  //create a center point of finite element (cube)
                            ComplexCube feCube = new ComplexCube(feCenterPoint, new Vector(feSizeX, feSizeY, feSizeZ)); //create a finite element (cube) with sizes
                            feCube.ParentObject = globalCube;
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
                                p.ParentCube = globalCube;
                                p.PositionInCube = j;
                                p.ParentObject = globalCube;
                                OuterVerticesIndices.Add(j);
                                uniquePoints.Add((Point3D)p.Clone());
                            }
                        }
                        else
                        {
                            p.ParentCube = globalCube;
                            p.PositionInCube = j;
                            p.ParentObject = globalCube;
                            OuterVerticesIndices.Add(j);
                            uniquePoints.Add((Point3D)p.Clone());
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
                        newLine.ParentObject = globalCube;
                        uniqueLines.Add(newLine);
                    }
                }

                Mesh resultMesh = new Mesh();
                resultMesh.Vertices = uniquePoints;
                resultMesh.Edges = uniqueLines;
                LocalMesh = resultMesh;


            }

            public void InitOuterVertices(Point3D[] OuterVertices, Mesh prevMesh)
            {
                for (int i = 0; i < prevMesh.Vertices.Count; i++)
                {
                    OuterVertices[i] = prevMesh.Vertices[i];

                    if (OuterVerticesIndices.Count == 0)
                    {
                        OuterVertices[i].PositionInCube = prevMesh.Vertices[i].PositionInCube;
                    }
                    else
                    {
                        OuterVertices[i].PositionInCube = OuterVerticesIndices[i];
                    }
                }

            }

            public void InitLocalOuterVertices()
            {
                ComplexCube sampleCube = new ComplexCube(new Point3D(0, 0, 0), new Vector(1, 1, 1));

                int count = 0;
                int i = 0;
                while (i < LocalMesh.Vertices.Count && count < 20)
                {
                    if (LocalMesh.Vertices[i] == sampleCube.Mesh.Vertices[count])
                    {
                        OuterLocalVertices[count] = (Point3D)LocalMesh.Vertices[i].Clone();
                        count++;
                        i = 0;
                        continue;
                    }

                    i++;
                }
            }

            public object Clone()
            {
                return new LocalSystem(this);
            }
        }

        public ComplexCube(Point3D _centerPoint, Vector _size, string _cubeName = null):
            base(_centerPoint, _size, _cubeName)
		{
			//Initializing Mesh
			InitPoints(GetCenterPoint(), Size);
            InitFacets(Mesh);
            InitEdges(Mesh);

            localSystem = new LocalSystem();
            OuterVertices = new Point3D[20];
            localSystem.InitOuterVertices(OuterVertices, Mesh);
        }

        public ComplexCube(Mesh mesh) : base(mesh) 
        {
            foreach(Point3D p in mesh.Vertices)
            {
                p.ParentObject = this;
            }

            foreach (Line l in mesh.Edges)
            {
                l.ParentObject = this;
            }

            foreach (Plane plane in mesh.Facets)
            {
                plane.ParentObject = this;
            }

            localSystem = new LocalSystem();

            if (OuterVertices == null)
            {
                OuterVertices = new Point3D[20];
                localSystem.InitOuterVertices(OuterVertices, Mesh);
            }
        }

        public ComplexCube(Mesh mesh, Point3D[] outer): base(mesh)
        {
            foreach (Point3D p in mesh.Vertices)
            {
                p.ParentObject = this;
            }

            foreach (Line l in mesh.Edges)
            {
                l.ParentObject = this;
            }

            foreach (Plane plane in mesh.Facets)
            {
                plane.ParentObject = this;
            }

            localSystem = new LocalSystem();
            OuterVertices = outer;
            //localSystem.InitOuterVertices(OuterVertices, Mesh);
        }

        public ComplexCube(ComplexCube cubeToClone): base(cubeToClone)
        {
            xRotation = cubeToClone.xRotation;
            yRotation = cubeToClone.yRotation;
            IsSelected = cubeToClone.IsSelected;
            IsDivided = cubeToClone.IsDivided;
            //localSystem = (LocalSystem)cubeToClone.localSystem.Clone();
            //localSystem = new LocalSystem();
            localSystem = (LocalSystem)cubeToClone.localSystem.Clone();
            OuterVertices = new Point3D[20];
            //localSystem.InitOuterVertices(OuterVertices, cubeToClone.Mesh);

            for (int i = 0; i < cubeToClone.OuterVertices.Length; i++)
            {
                OuterVertices[i] = (Point3D)cubeToClone.OuterVertices[i].Clone();
            }
        }

        public void UpdateRotation(int x, int y)
        {
            float xDelta = (float)MouseController.GetHorizontalAngle(x);
            float yDelta = (float)MouseController.GetVerticalAngle(y);

            xRotation += xDelta * 1f;
            yRotation += yDelta * 1f;

            GraphicsGL.Control.Invalidate();
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
                    mesh.Vertices[3],
                    mesh.Vertices[10],
                    mesh.Vertices[2],
                    mesh.Vertices[14],
                    mesh.Vertices[6],
                    mesh.Vertices[18],
                    mesh.Vertices[7],
                    mesh.Vertices[15]
                }),

				//LEFT
				new Plane(new List<Point3D>
                {
                    mesh.Vertices[0],
                    mesh.Vertices[11],
                    mesh.Vertices[3],
                    mesh.Vertices[15],
                    mesh.Vertices[7],
                    mesh.Vertices[19],
                    mesh.Vertices[4],
                    mesh.Vertices[12]
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
                    mesh.Vertices[0],
                    mesh.Vertices[8],
                    mesh.Vertices[1],
                    mesh.Vertices[9],
                    mesh.Vertices[2],
                    mesh.Vertices[10],
                    mesh.Vertices[3],
                    mesh.Vertices[11]
                })
            };

            foreach(Plane plane in facets)
            {
                plane.ParentObject = this;
            }


            facets[0].AxisType = CoordinateAxisType.PlusZ;
            facets[1].AxisType = CoordinateAxisType.PlusX;
            facets[2].AxisType = CoordinateAxisType.MinusZ;
            facets[3].AxisType = CoordinateAxisType.MinusX;
            facets[4].AxisType = CoordinateAxisType.PlusY;
            facets[5].AxisType = CoordinateAxisType.MinusY;

            Mesh.Facets = facets;
        }

        private void InitEdges(Mesh mesh)
        {
            List<Line> meshEdges = new List<Line>();
            for (int i = 0; i < mesh.Facets.Count; i++)
            {
                Plane currentFacet = mesh.Facets[i];
                for (int j = 0; j < Mesh.FACET_VERTICES; j++)
                {
                    Line newEdge = new Line(currentFacet.Points[j], currentFacet.Points[(j + 1) % Mesh.FACET_VERTICES]);
                    if (meshEdges.Count != 0)
                    {
                        if (!newEdge.Exists(meshEdges))
                        {
                            meshEdges.Add(newEdge);

                            //Defining Edge - Vertex relationship
                            mesh.Vertices[j].EdgeParents.Add(newEdge);
                            mesh.Vertices[(j + 1) % Mesh.FACET_VERTICES].EdgeParents.Add(newEdge);
                        }
                    }
                    else
                    {
                        meshEdges.Add(newEdge);

                        //Defining Edge - Vertex relationship
                        mesh.Vertices[j].EdgeParents.Add(newEdge);
                        mesh.Vertices[(j + 1) % Mesh.FACET_VERTICES].EdgeParents.Add(newEdge);
                    }
                }
            }

            foreach (Line line in meshEdges)
            {
                line.ParentObject = this;
            }

            Mesh.Edges = meshEdges;
        }

        //Main method to divide cube into small finite elements
        public void Divide(Vector nValues)
        {
            Mesh prevMesh = this.Mesh.Clone();

            //Initializing local system
            ComplexCube local = new ComplexCube(new Point3D(0, 0, 0), new Vector(1, 1, 1));
            localSystem.InitLocalSystem(local, nValues, this);
            localSystem.InitLocalOuterVertices();
            localSystem.InitOuterVertices(OuterVertices, prevMesh);
            localSystem.InitTransformMesh();

            //Transform cube in all axes
            Scene.ActiveMovingAxis = CoordinateAxis.X;
            Transform();

            Scene.ActiveMovingAxis = CoordinateAxis.Y;
            Transform();

            Scene.ActiveMovingAxis = CoordinateAxis.Z;
            Transform();

            IsDivided = true;

            //return new ComplexCube(this);
        }

        public void Update(Point3D p)
        {
            int index = p.PositionInCube;
            OuterVertices[index][0] += Scene.MovingVector[0];
            OuterVertices[index][1] += Scene.MovingVector[1];
            OuterVertices[index][2] += Scene.MovingVector[2];
        }

        public void UpdateObject()
        {
            Mesh cloneMesh = Mesh.Clone();

            InitFacets(Mesh);
            InitEdges(Mesh);

            UpdateMesh(Mesh, cloneMesh);
        }

        public void UpdateOuterVertices()
        {
            List<int> list = localSystem.OuterVerticesIndices;
        }

        public void UpdateMesh(Mesh original, Mesh clone)
        {
            for (int i = 0; i < clone.Facets.Count; i++)
            {
                original.Facets[i].IsSelected = clone.Facets[i].IsSelected;
                original.Facets[i].IsAttached = clone.Facets[i].IsAttached;
                
                foreach(int index in clone.attachedFacets)
                {
                    original.attachedFacets.Add(index);
                }
            }

            for (int i = 0; i < clone.Edges.Count; i++)
            {
                original.Edges[i].IsSelected = clone.Edges[i].IsSelected;
            }

            
        }

        //Main method to transform points after cube was divided into finite elements
        public void Transform()
        {
            Mesh currentMesh = this.Mesh;
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
                    currentMesh = localSystem.TransformMesh;
                }

                for (int i = 0; i < localSystem.LocalMesh.Vertices.Count; i++)
                {
                    //one of the local points of cube
                    Point3D localPoint = localSystem.LocalMesh.Vertices[i];

                    double sum = 0;
                    for (int m = 0; m < OUTER_VERTICES_AMOUNT; m++)
                    {
                        //One of the outer points of cube. There only 20 outer points in the cube
                        Point3D outerPoint = OuterVertices[m];

                        Func<Point3D, Point3D, double> phiFunc = null;
                        if (m >= 0 && m < 8)
                        {
                            //Specific function for points that are on ends (edges) of the cube
                            phiFunc = TransformMesh.PhiAngle;
                        }
                        else if (m >= 8 && m < OUTER_VERTICES_AMOUNT)
                        {
                            //Specific function for points that are in the middle of edges.
                            phiFunc = TransformMesh.PhiEdge;
                        }

                        //One of the outer LOCAL points. Point(a, b, c), where -1 <= a, b, c <= 1
                        Point3D localOuterPoint = localSystem.OuterLocalVertices[m];
                        double funcResult = phiFunc(localPoint, localOuterPoint);
                        sum += outerPoint[index] * funcResult;
                    }

                    currentMesh.Vertices[i][index] = sum;
                    currentMesh.Vertices[i].ParentObject = this;
                }
            }

            this.Mesh = currentMesh;
            GraphicsGL.Control.Invalidate();
        }

        public void Unite()
        {
            //Initializing Mesh
            InitPoints(GetCenterPoint(), Size);
            InitFacets(Mesh);
            InitEdges(Mesh);

            localSystem = new LocalSystem();
            OuterVertices = new Point3D[20];
            localSystem.InitOuterVertices(OuterVertices, Mesh);

            IsDivided = false;
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
            string exportString = Name + "\n";

            //Vertices
            exportString += "Vertices:\n";
            for(int i = 0; i < Mesh.Vertices.Count; i++)
            {
                exportString += String.Format("({0} {1} {2})", Mesh.Vertices[i].X, Mesh.Vertices[i].Y, Mesh.Vertices[i].Z);
                exportString += "\n";
            }
            exportString += "\n";

			//Edges
			for (int i = 0; i < Mesh.Edges.Count; i++)
			{
                int indexOfPoint1 = this.Mesh.GetIndexOfPoint(Mesh.Edges[i].P1);
                int indexOfPoint2 = this.Mesh.GetIndexOfPoint(Mesh.Edges[i].P2);
				exportString += String.Format("Edge_{0} {1}", indexOfPoint1, indexOfPoint2);
				exportString += "\n";
			}
            exportString += "\n";

            //Facets
            for (int i = 0; i < Mesh.Facets.Count; i++)
            {
                StringBuilder sb = new StringBuilder();
                sb.Append("Facet_");
                for (int j = 0; j < Mesh.Facets[i].Points.Count; j++)
                {
                    int indexOfPoint = this.Mesh.GetIndexOfPoint(Mesh.Facets[i][j]);
                    sb.Append(indexOfPoint + " ");
                }

                sb.Append(Mesh.Facets[i].AxisType + " ");
                sb.Append(this.Name);
                exportString += sb.ToString();
                exportString += "\n";
            }

            StringBuilder sb2 = new StringBuilder();
            sb2.Append("OuterVertices ");
            for (int i = 0; i < OuterVertices.Length; i++)
            {
                sb2.Append(this.Mesh.GetIndexOfPoint(OuterVertices[i]));
                sb2.Append(" ");
            }
            sb2.Append('\n');
            exportString += sb2.ToString();

            return exportString;
        }

        public override ISceneObject Clone()
        {
            return new ComplexCube(this);
            //return null;
        }
    }
}
