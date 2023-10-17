using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using SharpGL;
using CadEditor.Graphics;
using CadEditor.Maths;
using CadEditor.MeshObjects;

namespace CadEditor
{
    public class ComplexCube: CustomCube
    {
        private const int VERTICES_ON_FACET = 8;
        private Mesh divideLocalSystem;
        public List<Point> bigCubePoints;           //points used for approximation formulas

        public ComplexCube(OpenGL _gl, Point _centerPoint, double? _sizeX = null, double? _sizeZ = null, double? _sizeY = null, string _cubeName = null):
            base(_gl, _centerPoint, _sizeX, _sizeY, _sizeZ, _cubeName)
		{
			//Initializing Mesh
			Mesh.Vertices = InitPoints(CenterPoint, sizeX, sizeY, sizeZ, GL);
            Mesh.Facets = InitFacets(Mesh);
            Mesh.Edges = InitEdges(Mesh);

            bigCubePoints = new List<Point>();
        }

        public ComplexCube(Mesh mesh, OpenGL _gl = null) : base(mesh, _gl) 
        {
            bigCubePoints = new List<Point>();
        }

        private List<Point> InitPoints(Point CenterPoint, double sizeX, double sizeY, double sizeZ, OpenGL GL)
        {
            List<Point> points = new List<Point>
            {
                new Point(-sizeX + CenterPoint.X, -sizeY + CenterPoint.Y, sizeZ + CenterPoint.Z, GL),
                new Point(sizeX + CenterPoint.X, -sizeY + CenterPoint.Y, sizeZ + CenterPoint.Z, GL),
                new Point(sizeX + CenterPoint.X, -sizeY + CenterPoint.Y, -sizeZ + CenterPoint.Z, GL),
                new Point(-sizeX + CenterPoint.X, -sizeY + CenterPoint.Y, -sizeZ + CenterPoint.Z, GL),
                new Point(-sizeX + CenterPoint.X, sizeY + CenterPoint.Y, sizeZ + CenterPoint.Z, GL),
                new Point(sizeX + CenterPoint.X, sizeY + CenterPoint.Y, sizeZ + CenterPoint.Z, GL),
                new Point(sizeX + CenterPoint.X, sizeY + CenterPoint.Y, -sizeZ + CenterPoint.Z, GL),
                new Point(-sizeX + CenterPoint.X, sizeY + CenterPoint.Y, -sizeZ + CenterPoint.Z, GL),

                new Point(0 + CenterPoint.X, -sizeY + CenterPoint.Y, sizeZ + CenterPoint.Z, GL),
                new Point(sizeX + CenterPoint.X, -sizeY + CenterPoint.Y, 0 + CenterPoint.Z, GL),
                new Point(0 + CenterPoint.X, -sizeY + CenterPoint.Y, -sizeZ + CenterPoint.Z, GL),
                new Point(-sizeX + CenterPoint.X, -sizeY + CenterPoint.Y, 0 + CenterPoint.Z, GL),
                new Point(-sizeX + CenterPoint.X, 0 + CenterPoint.Y, sizeZ + CenterPoint.Z, GL),
                new Point(sizeX + CenterPoint.X, 0 + CenterPoint.Y, sizeZ + CenterPoint.Z, GL),
                new Point(sizeX + CenterPoint.X, 0 + CenterPoint.Y, -sizeZ + CenterPoint.Z, GL),
                new Point(-sizeX + CenterPoint.X, 0 + CenterPoint.Y, -sizeZ + CenterPoint.Z, GL),
                new Point(0 + CenterPoint.X, sizeY + CenterPoint.Y, sizeZ + CenterPoint.Z, GL),
                new Point(sizeX + CenterPoint.X, sizeY + CenterPoint.Y, 0 + CenterPoint.Z, GL),
                new Point(0 + CenterPoint.X, sizeY + CenterPoint.Y, -sizeZ + CenterPoint.Z, GL),
                new Point(-sizeX + CenterPoint.X, sizeY + CenterPoint.Y, 0 + CenterPoint.Z, GL),
            };

            foreach (Point p in points)
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
				new Plane(new List<Point>
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
				new Plane(new List<Point>
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
				new Plane(new List<Point>
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
				new Plane(new List<Point>
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
				new Plane(new List<Point>
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
				new Plane(new List<Point>
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

            if(bigCubePoints.Count > 0)
            {
				//Draw Vertexes
				GL.PointSize(7.0f);
				GL.Begin(OpenGL.GL_POINTS);
				for (int i = 0; i < bigCubePoints.Count; i++)
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
        public void Divide(int[] nValues)
        {

            double feAmountX = nValues[0];
            double feAmountY = nValues[1];
            double feAmountZ = nValues[2];

            double VerticesAmountX = feAmountX * 2 + 1;
            double VerticesAmountY = feAmountY * 2 + 1;
            double VerticesAmountZ = feAmountZ * 2 + 1;

            //size of smaller cube
            double feSizeX = sizeX / feAmountX;
            double feSizeY = sizeY / feAmountY;
            double feSizeZ = sizeZ / feAmountZ;

            //dividing into small cubes
            List<ComplexCube> FeCubeList = new List<ComplexCube>();
            for (int i_z = 1; i_z < VerticesAmountZ; i_z += 2)
            {
                double feCenterPointZ = -sizeZ + i_z * feSizeZ;

                for (int i_y = 1; i_y < VerticesAmountY; i_y += 2)
                {
                    double feCenterPointY = -sizeY + i_y * feSizeY;

                    for (int i_x = 1; i_x < VerticesAmountX; i_x += 2)
                    {
                        double feCenterPointX = -sizeX + i_x * feSizeX;

                        Point feCenterPoint = new Point(feCenterPointX + CenterPoint[0], feCenterPointY + CenterPoint[1], feCenterPointZ + CenterPoint[2], GL);  //create a center point of finite element (cube)
                        ComplexCube feCube = new ComplexCube(GL, feCenterPoint, feSizeX, feSizeY, feSizeZ); //create a finite element (cube) with sizes
                        feCube.ParentCube = this;
                        FeCubeList.Add(feCube);                                                             //add it to list of finite elements
                    }
                }
            }

            //convert all cubes into big one mesh

            List<Point> uniquePoints = new List<Point>();
            List<Line> uniqueLines = new List<Line>();
            bigCubePoints = new List<Point>();
            for (int i = 0; i < this.Mesh.Vertices.Count; i++)
            {
                bigCubePoints.Add(null);
            }

            for (int i = 0; i < FeCubeList.Count; i++)
            {
                for (int j = 0; j < FeCubeList[i].Mesh.Vertices.Count; j++)
                {
                    Point p = FeCubeList[i].Mesh.Vertices[j];

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
                    Point p1 = null;
                    Point p2 = null;
                    foreach(Point p in uniquePoints)
                    {
                        if(line.P1.Equals(p))
                        {
                            p1 = p;
                        }
                        else if(line.P2.Equals(p))
                        {
                            p2 = p;
                        }
                    }

                    Line newLine = new Line(p1, p2, GL);

					uniqueLines.Add(newLine);
                }
            }

			//point of big cube
			for (int i = 0; i < uniquePoints.Count; i++)
            {
                Point p = uniquePoints[i];

                for (int j = 0; j < Mesh.Vertices.Count; j++)
                {
                    Point meshP = Mesh.Vertices[j];
                    if (p.Equals(meshP))
                    {
                        bigCubePoints[j] = p;
                        break;
                    }
                }
            }

            this.Mesh.Vertices = uniquePoints;
            this.Mesh.Edges = uniqueLines;
            divideLocalSystem = this.Mesh.Clone();
        }

        /// <summary>
        /// Transform method used to transform each point of this cube using approximation formulas
        /// </summary>
        /// <param name="index">defines which coordinate would be operated on</param>
        /// <param name="cube">defines a cube from which all points will be transformed</param>
        public void Transform(Vector vector, Point point)
        {
            int index = -1;
            if (vector[0] != 0) index = 0;
            else if (vector[1] != 0) index = 1;
            else if (vector[2] != 0) index = 2;

            if(index != -1)
            {
                int indexOfPoint = this.Mesh.GetIndexOfPoint(point);
                Point p = divideLocalSystem.Vertices[indexOfPoint];
                p.Move(vector);

                for (int i = 0; i < divideLocalSystem.Vertices.Count; i++)
                {
                    Point point1 = divideLocalSystem.Vertices[i];
                    double sum = 0;
                    for (int m = 0; m < this.bigCubePoints.Count; m++)
                    {
                        Point point2 = this.bigCubePoints[m];

                        Func<Point, Point, double> phiFunc = null;
                        if (m >= 0 && m < 8)
                        {
                            phiFunc = TransformMesh.PhiAngle;
                        }
                        else if (m >= 8 && m < this.bigCubePoints.Count)
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

            foreach(Point p in this.bigCubePoints)
            {
                int indexOfPoint = this.Mesh.GetIndexOfPoint(p);
                if(indexOfPoint != -1)
                {
                    Point point = cube.Mesh.Vertices[indexOfPoint];
                    cube.bigCubePoints.Add(point);
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
