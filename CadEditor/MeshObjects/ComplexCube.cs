using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using SharpGL;
using CadEditor.Maths;
using CadEditor.MeshObjects;

namespace CadEditor
{
    public class ComplexCube: MeshObject3D
    {
        private const int OUTER_VERTICES = 20;
        private const int VERTICES_ON_FACET = 8;
        private Mesh localSystem;
        public Point3D[] bigCubePoints;           //points used for approximation formulas
        private List<Point3D> externalPoints;

        public ComplexCube(OpenGL _gl, Point3D _centerPoint, Vector _size, string _cubeName = null):
            base(_gl, _centerPoint, _size, _cubeName)
		{
			//Initializing Mesh
			Mesh.Vertices = InitPoints(CenterPoint, size, GL);
            Mesh.Facets = InitFacets(Mesh);
            Mesh.Edges = InitEdges(Mesh);

            bigCubePoints = new Point3D[OUTER_VERTICES];
            localSystem = this.Mesh.Clone();
            bigCubePoints = GetOuterCubeVertices(Mesh, Mesh);
        }

        public ComplexCube(Mesh mesh, OpenGL _gl = null) : base(mesh, _gl) 
        {
            bigCubePoints = new Point3D[20];
        }

        private List<Point3D> InitPoints(Point3D CenterPoint, Vector size, OpenGL GL)
        {
            double sizeX = size[0];
            double sizeY = size[1];
            double sizeZ = size[2];

            List<Point3D> points = new List<Point3D>
            {
                new Point3D(-sizeX + CenterPoint.X, -sizeY + CenterPoint.Y, sizeZ + CenterPoint.Z, GL),
                new Point3D(sizeX + CenterPoint.X, -sizeY + CenterPoint.Y, sizeZ + CenterPoint.Z, GL),
                new Point3D(sizeX + CenterPoint.X, -sizeY + CenterPoint.Y, -sizeZ + CenterPoint.Z, GL),
                new Point3D(-sizeX + CenterPoint.X, -sizeY + CenterPoint.Y, -sizeZ + CenterPoint.Z, GL),
                new Point3D(-sizeX + CenterPoint.X, sizeY + CenterPoint.Y, sizeZ + CenterPoint.Z, GL),
                new Point3D(sizeX + CenterPoint.X, sizeY + CenterPoint.Y, sizeZ + CenterPoint.Z, GL),
                new Point3D(sizeX + CenterPoint.X, sizeY + CenterPoint.Y, -sizeZ + CenterPoint.Z, GL),
                new Point3D(-sizeX + CenterPoint.X, sizeY + CenterPoint.Y, -sizeZ + CenterPoint.Z, GL),

                new Point3D(0 + CenterPoint.X, -sizeY + CenterPoint.Y, sizeZ + CenterPoint.Z, GL),
                new Point3D(sizeX + CenterPoint.X, -sizeY + CenterPoint.Y, 0 + CenterPoint.Z, GL),
                new Point3D(0 + CenterPoint.X, -sizeY + CenterPoint.Y, -sizeZ + CenterPoint.Z, GL),
                new Point3D(-sizeX + CenterPoint.X, -sizeY + CenterPoint.Y, 0 + CenterPoint.Z, GL),
                new Point3D(-sizeX + CenterPoint.X, 0 + CenterPoint.Y, sizeZ + CenterPoint.Z, GL),
                new Point3D(sizeX + CenterPoint.X, 0 + CenterPoint.Y, sizeZ + CenterPoint.Z, GL),
                new Point3D(sizeX + CenterPoint.X, 0 + CenterPoint.Y, -sizeZ + CenterPoint.Z, GL),
                new Point3D(-sizeX + CenterPoint.X, 0 + CenterPoint.Y, -sizeZ + CenterPoint.Z, GL),
                new Point3D(0 + CenterPoint.X, sizeY + CenterPoint.Y, sizeZ + CenterPoint.Z, GL),
                new Point3D(sizeX + CenterPoint.X, sizeY + CenterPoint.Y, 0 + CenterPoint.Z, GL),
                new Point3D(0 + CenterPoint.X, sizeY + CenterPoint.Y, -sizeZ + CenterPoint.Z, GL),
                new Point3D(-sizeX + CenterPoint.X, sizeY + CenterPoint.Y, 0 + CenterPoint.Z, GL),
            };

            foreach (Point3D p in points)
            {
                p.ParentCube = this;
            }
            CenterPoint.ParentCube = this;

            return points;
        }

        private List<Plane> InitFacets(Mesh mesh)
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
                }, GL),

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
                }, GL),

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
                }, GL),

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
                }, GL),

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
                }, GL),

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
                }, GL)
            };

            return facets;
        }

        private List<Line> InitEdges(Mesh mesh)
        {
            List<Line> meshEdges = new List<Line>();
            for (int i = 0; i < mesh.Facets.Count; i++)
            {
                Plane currentFacet = mesh.Facets[i];
                for (int j = 0; j < VERTICES_ON_FACET; j++)
                {
                    Line newEdge = new Line(currentFacet.Points[j], currentFacet.Points[(j + 1) % VERTICES_ON_FACET], GL);
                    if (meshEdges.Count != 0)
                    {
                        if (!newEdge.Exists(meshEdges))
                        {
                            meshEdges.Add(newEdge);

                            //Defining Edge - Vertex relationship
                            mesh.Vertices[j].EdgeParents.Add(newEdge);
                            mesh.Vertices[(j + 1) % VERTICES_ON_FACET].EdgeParents.Add(newEdge);
                        }
                    }
                    else
                    {
                        meshEdges.Add(newEdge);

                        //Defining Edge - Vertex relationship
                        mesh.Vertices[j].EdgeParents.Add(newEdge);
                        mesh.Vertices[(j + 1) % VERTICES_ON_FACET].EdgeParents.Add(newEdge);
                    }
                }
            }
            
            return meshEdges;
        }

        public new void Draw()
        {
            base.Draw();

            if(bigCubePoints.Length > 0)
            {
				//Draw Vertexes
				GL.PointSize(7.0f);
				GL.Begin(OpenGL.GL_POINTS);
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
				GL.End();
				GL.Flush();
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
            localSystem = GetDividedLocalSystem(nValues);
            bigCubePoints = GetOuterCubeVertices(localSystem, prevMesh);
        }

        private Mesh GetDividedLocalSystem(Vector nValues)
        {
            double feAmountX = nValues[0];
            double feAmountY = nValues[1];
            double feAmountZ = nValues[2];

            double VerticesAmountX = feAmountX * 2 + 1;
            double VerticesAmountY = feAmountY * 2 + 1;
            double VerticesAmountZ = feAmountZ * 2 + 1;

            //size of smaller cube
            double feSizeX = size[0] / feAmountX;
            double feSizeY = size[1] / feAmountY;
            double feSizeZ = size[2] / feAmountZ;

            //dividing into small cubes
            List<ComplexCube> FeCubeList = new List<ComplexCube>();
            for (int i_z = 1; i_z < VerticesAmountZ; i_z += 2)
            {
                double feCenterPointZ = -size[2] + i_z * feSizeZ;

                for (int i_y = 1; i_y < VerticesAmountY; i_y += 2)
                {
                    double feCenterPointY = -size[1] + i_y * feSizeY;

                    for (int i_x = 1; i_x < VerticesAmountX; i_x += 2)
                    {
                        double feCenterPointX = -size[0] + i_x * feSizeX;

                        Point3D feCenterPoint = new Point3D(feCenterPointX + CenterPoint[0], feCenterPointY + CenterPoint[1], feCenterPointZ + CenterPoint[2], GL);  //create a center point of finite element (cube)
                        ComplexCube feCube = new ComplexCube(GL, feCenterPoint, new Vector(feSizeX, feSizeY, feSizeZ)); //create a finite element (cube) with sizes
                        feCube.ParentObject = this;
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

                    if ((uniquePoints.Count > 0 && !uniquePoints.Contains(p)) || uniquePoints.Count == 0)
                    {
                        p.ParentCube = this;
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
                        if (line.P1.Equals(p))
                        {
                            p1 = p;
                        }
                        else if (line.P2.Equals(p))
                        {
                            p2 = p;
                        }
                    }

                    Line newLine = new Line(p1, p2, GL);

                    uniqueLines.Add(newLine);
                }
            }


            this.Mesh.Vertices = uniquePoints;
            this.Mesh.Edges = uniqueLines;
            return this.Mesh.Clone();
        }

        private Point3D[] GetOuterCubeVertices(Mesh currentMesh, Mesh prevMesh)
        {
            Point3D[] resultVertices = new Point3D[OUTER_VERTICES];
            //point of big cube
            for (int i = 0; i < currentMesh.Vertices.Count; i++)
            {
                Point3D p = currentMesh.Vertices[i];

                for (int j = 0; j < prevMesh.Vertices.Count; j++)
                {
                    Point3D meshP = prevMesh.Vertices[j];
                    if (p.Equals(meshP))
                    {
                        resultVertices[j] = p;
                        break;
                    }
                }
            }

            return resultVertices;
        }

        /// <summary>
        /// Transform method used to transform each point of this cube using approximation formulas
        /// </summary>
        /// <param name="index">defines which coordinate would be operated on</param>
        /// <param name="cube">defines a cube from which all points will be transformed</param>
        public void Transform(Vector vector, Point3D point)
        {
            int index = -1;
            if (vector[0] != 0) index = 0;
            else if (vector[1] != 0) index = 1;
            else if (vector[2] != 0) index = 2;

            if(index != -1)
            {
                int indexOfPoint = this.Mesh.GetIndexOfPoint(point);
                Point3D p = localSystem.Vertices[indexOfPoint];
                p.X += vector[0];
                p.Y += vector[1];
                p.Z += vector[2];

                for (int i = 0; i < localSystem.Vertices.Count; i++)
                {
                    Point3D point1 = localSystem.Vertices[i];
                    double sum = 0;
                    for (int m = 0; m < this.bigCubePoints.Length; m++)
                    {
                        Point3D point2 = this.bigCubePoints[m];

                        Func<Point3D, Point3D, double> phiFunc = null;
                        if (m >= 0 && m < 8)
                        {
                            phiFunc = TransformMesh.PhiAngle;
                        }
                        else if (m >= 8 && m < this.bigCubePoints.Length)
                        {
                            phiFunc = TransformMesh.PhiEdge;
                        }

                        double funcResult = phiFunc(point1, point2);
                        sum += point2[index] * funcResult;
                    }

                    this.Mesh.Vertices[i][index] = sum;
                }
            }
        }

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
