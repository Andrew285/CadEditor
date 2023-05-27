using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;

using System.Runtime.InteropServices;
using SharpGL.SceneGraph;
using SharpGL;
using SharpGL.SceneGraph.Core;
using System.Linq.Expressions;
using CadEditor.Graphics;
using System.Windows.Forms.VisualStyles;
using MathNet.Numerics.Distributions;
using SharpGL.SceneGraph.Primitives;
using System.Linq;
using MathNet.Numerics.Statistics.Mcmc;

namespace CadEditor
{
    public interface IObject3D: IGraphics
	{
        string Name { get; set; }
        Mesh Mesh { get; set; }
        Point CenterPoint { get; set; }
    }

    public class CustomCube: IObject3D
	{
        public OpenGL GL { get; set; }
        public string Name { get; set; }
        public Mesh Mesh { get; set; }
		public Point CenterPoint { set; get; }
        protected double sizeX { get; set; }
        protected double sizeY { get; set; }
        protected double sizeZ { get; set; }

        public bool DrawFacets { get; set; }
        public CustomCube ParentCube;

        public CoordinateAxis Axis { get; set; }
        public bool IsSelected { get; set; }
        public Color VertexSelectedColor = Color.Red;
        public Color EdgeSelectedColor = Color.Red;
        public Color FacetSelectedColor = Color.LightGray;
        public Color VertexNonSelectedColor = Color.Black;
        public Color EdgeNonSelectedColor = Color.Black;
        public Color FacetNonSelectedColor = Color.LightGray;

        public CustomCube(OpenGL _Gl, Point _centerPoint, double? _sizeX, double? _sizeY, double? _sizeZ, string _cubeName)
        {
            GL = _Gl;
            Name = _cubeName;
            Mesh = new Mesh();
            CenterPoint = _centerPoint;

            sizeX = (_sizeX != null) ? (double)_sizeX : 1.0;
            sizeY = (_sizeY != null) ? (double)_sizeY : 1.0;
            sizeZ = (_sizeZ != null) ? (double)_sizeZ : 1.0;
        }

		public CustomCube(Mesh mesh, OpenGL _GL = null)
		{
            GL = _GL;
            Name = "CubeName";
			Mesh = mesh;
		}

		public void Draw()
        {

            //Draw Vertexes
            GL.PointSize(10.0f);
            GL.Begin(OpenGL.GL_POINTS);
            for (int i = 0; i < Mesh.Vertices.Count; i++)
            {
                if (IsSelected)
                {
                    Mesh.Vertices[i].SelectedColor = VertexSelectedColor;
                }
                else
                {
                    Mesh.Vertices[i].NonSelectedColor = VertexNonSelectedColor;
                }

                Mesh.Vertices[i].Draw();
            }
            GL.End();
            GL.Flush();
            

            //Draw Edges
            GL.Begin(OpenGL.GL_LINES);
            for (int i = 0; i < Mesh.Edges.Count; i++)
            {
                if (IsSelected)
                {
                    Mesh.Edges[i].SelectedColor = EdgeSelectedColor;
                }
                else
                {
                    Mesh.Edges[i].NonSelectedColor = EdgeNonSelectedColor;
                }

                Mesh.Edges[i].Draw();
            }
            GL.End();
            GL.Flush();


            //Draw Facets
            if (DrawFacets)
            {
                GL.Begin(OpenGL.GL_POLYGON);
                for (int i = 0; i < Mesh.Facets.Count; i++)
                {
                    if (IsSelected)
                    {
                        Mesh.Facets[i].SelectedColor = FacetSelectedColor;
                    }
                    else
                    {
                        Mesh.Facets[i].NonSelectedColor = FacetNonSelectedColor;
                    }

                    Mesh.Facets[i].Draw();
                }
                GL.End();
                GL.Flush();
            }
        }

        public void Move(double x, double y, double z)
        {
            for (int i = 0; i < Mesh.Vertices.Count; i++)
            {
                Mesh.Vertices[i].Move(x, y, z);
            }
            CenterPoint.Move(x, y, z);
        }

        public void Select()
        {
            foreach (Line edge in Mesh.Edges)
            {
                edge.IsSelected = true;
            }
        }

        public virtual void Deselect()
        {
            foreach (Plane facet in Mesh.Facets)
            {
                facet.IsSelected = false;
            }

            foreach (Line edge in Mesh.Edges)
            {
                edge.IsSelected = false;
            }

            foreach (Point vertex in Mesh.Vertices)
            {
                vertex.IsSelected = false;
            }
        }
    }


    public class ComplexCube: CustomCube
    {
        private const int VERTICES_ON_FACET = 8;
        public List<Point> bigCubePoints;
        //public List<ComplexCube> FeCubeList;
        //public ComplexCube ParentCube { get; private set; }
        //public ComplexCube InnerCube { get; private set; }

        public ComplexCube(OpenGL _gl, Point _centerPoint, double? _sizeX = null, double? _sizeZ = null, double? _sizeY = null, string _cubeName = null):
            base(_gl, _centerPoint, _sizeX, _sizeY, _sizeZ, _cubeName)
		{
			//FeCubeList = new List<ComplexCube>();

			//Initializing Vertices
			Mesh.Vertices = InitPoints(CenterPoint, sizeX, sizeY, sizeZ, GL);
            //ChangebaleMesh.Vertices = InitPoints(CenterPoint, sizeX, sizeY, sizeZ, GL);

            //Initializing Facets
            Mesh.Facets = InitFacets(Mesh);
            //ChangebaleMesh.Facets = InitFacets(ChangebaleMesh);

            //Initializing Edges
            Mesh.Edges = InitEdges(Mesh);
            //ChangebaleMesh.Edges = InitEdges(ChangebaleMesh);

            bigCubePoints = new List<Point>();

            //DrawableMesh.Vertices = NonDrawableMesh.Vertices;
            //DrawableMesh.Edges = NonDrawableMesh.Edges;
            //DrawableMesh.Facets = NonDrawableMesh.Facets;
        }

        public ComplexCube(Mesh mesh) : base(mesh) 
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

        //public new void Draw()
        //{
        //    //if(FeCubeList.Count > 0)
        //    //{
        //    //    foreach(ComplexCube cube in FeCubeList)
        //    //    {
        //    //        cube.Draw();
        //    //    }
        //    //}
        //    if(InnerCube != null)
        //    {
        //        InnerCube.Draw();
        //    }
        //    else
        //    {
        //        this.Draw();
        //    }
        //}

        //public new void Move(double x, double y, double z)
        //{
        //    for (int i = 0; i < ChangebaleMesh.Vertices.Count; i++)
        //    {
        //        ChangebaleMesh.Vertices[i].Move(x, y, z);
        //    }
        //    CenterPoint.Move(x, y, z);
        //}

        //public new void Draw()
        //{
        //    if(FeCubeList.Count > 0)
        //    {
        //        foreach(ComplexCube feCube in FeCubeList)
        //        {
        //            feCube.Draw();
        //        }
        //    }
        //    else
        //    {
        //        //Draw Vertexes
        //        GL.PointSize(10.0f);
        //        GL.Begin(OpenGL.GL_POINTS);
        //        for (int i = 0; i < ChangebaleMesh.Vertices.Count; i++)
        //        {
        //            if (IsSelected)
        //            {
        //                ChangebaleMesh.Vertices[i].SelectedColor = VertexSelectedColor;
        //            }
        //            else
        //            {
        //                ChangebaleMesh.Vertices[i].NonSelectedColor = VertexNonSelectedColor;
        //            }

        //            ChangebaleMesh.Vertices[i].Draw();
        //        }
        //        GL.End();
        //        GL.Flush();


        //        //Draw Edges
        //        GL.Begin(OpenGL.GL_LINES);
        //        for (int i = 0; i < ChangebaleMesh.Edges.Count; i++)
        //        {
        //            if (IsSelected)
        //            {
        //                ChangebaleMesh.Edges[i].SelectedColor = EdgeSelectedColor;
        //            }
        //            else
        //            {
        //                ChangebaleMesh.Edges[i].NonSelectedColor = EdgeNonSelectedColor;
        //            }

        //            ChangebaleMesh.Edges[i].Draw();
        //        }
        //        GL.End();
        //        GL.Flush();


        //        //Draw Facets
        //        if (DrawFacets)
        //        {
        //            GL.Begin(OpenGL.GL_POLYGON);
        //            for (int i = 0; i < ChangebaleMesh.Facets.Count; i++)
        //            {
        //                if (IsSelected)
        //                {
        //                    ChangebaleMesh.Facets[i].SelectedColor = FacetSelectedColor;
        //                }
        //                else
        //                {
        //                    ChangebaleMesh.Facets[i].NonSelectedColor = FacetNonSelectedColor;
        //                }

        //                ChangebaleMesh.Facets[i].Draw();
        //            }
        //            GL.End();
        //            GL.Flush();
        //        }
        //    }
        //}

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

            //for (int i = 0; i < FeCubeList.Count - 1; i++)
            //{
            //    ComplexCube firstCube = FeCubeList[i];
            //    for (int m = 0; m < firstCube.Mesh.Vertices.Count; m++)
            //    {
            //        Point p1 = firstCube.ChangebaleMesh.Vertices[m];
            //        for (int j = i + 1; j < FeCubeList.Count; j++)
            //        {
            //            ComplexCube secondCube = FeCubeList[j];

            //            for (int p = 0; p < secondCube.Mesh.Vertices.Count; p++)
            //            {
            //                Point p2 = secondCube.ChangebaleMesh.Vertices[p];
            //                if (p1.X == p2.X && p1.Y == p2.Y && p1.Z == p2.Z)
            //                {
            //                    p1 = p2;
            //                }
            //            }
            //        }
            //    }
            //}


            //dividing into cubes
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

                        Point feCenterPoint = new Point(feCenterPointX, feCenterPointY, feCenterPointZ, GL);  //create a center point of finite element (cube)
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

                        //for (int m = 0; m < this.NonDrawableMesh.Vertices.Count; m++)
                        //{
                        //    Point tempPoint = this.NonDrawableMesh.Vertices[m];
                        //    if (tempPoint.X == p.X && tempPoint.Y == p.Y && tempPoint.Z == p.Z)
                        //    {
                        //        bigCubePoints[m] = p;
                        //        break;
                        //    }
                        //}
                    }
                }

                   

                    //DrawableMesh.Vertices = uniqueDrawablePoints;
                    //DrawableMesh.Edges = uniqueDrawableLines;
                    //DrawableMesh.Facets = new List<Plane>();

                    //NonDrawableMesh.Vertices = uniqueNonDrawablePoints;
                    //NonDrawableMesh.Edges = uniqueNonDrawableLines;
                    //NonDrawableMesh.Facets = new List<Plane>();


                    //const int VertAmount = 20;
                    //for(int c = 0; c < FeCubeList.Count; c++)
                    //{
                    //    for(int p = 0; p < VertAmount; p++)
                    //    {
                    //        if (CubeChangebaleMeshPoints.Count > 0)
                    //        {
                    //            if (CubeChangebaleMeshPoints.ContainsKey(FeCubeList[c].Mesh.Vertices[p]))
                    //            {
                    //                CubeChangebaleMeshPoints[FeCubeList[c].ChangebaleMesh.Vertices[p]].Add(FeCubeList[c]);
                    //                CubeMeshPoints[FeCubeList[c].Mesh.Vertices[p]].Add(FeCubeList[c]);
                    //            }
                    //            else
                    //            {
                    //                CubeChangebaleMeshPoints.Add(FeCubeList[c].ChangebaleMesh.Vertices[p], null);
                    //                CubeChangebaleMeshPoints[FeCubeList[c].ChangebaleMesh.Vertices[p]] = new List<ComplexCube>() { FeCubeList[c] };

                    //                CubeMeshPoints.Add(FeCubeList[c].Mesh.Vertices[p], null);
                    //                CubeMeshPoints[FeCubeList[c].Mesh.Vertices[p]] = new List<ComplexCube>() { FeCubeList[c] };
                    //            }
                    //        }
                    //        else
                    //        {
                    //            CubeChangebaleMeshPoints.Add(FeCubeList[c].ChangebaleMesh.Vertices[p], null);
                    //            CubeChangebaleMeshPoints[FeCubeList[c].ChangebaleMesh.Vertices[p]] = new List<ComplexCube>() { FeCubeList[c] };

                    //            CubeMeshPoints.Add(FeCubeList[c].Mesh.Vertices[p], null);
                    //            CubeMeshPoints[FeCubeList[c].Mesh.Vertices[p]] = new List<ComplexCube>() { FeCubeList[c] };
                    //        }
                    //    }
                    //}


                    //for (int i = 0; i < FeCubeList.Count; i++)
                    //{
                    //    for (int j = 0; j < FeCubeList[i].ChangebaleMesh.Vertices.Count; j++)
                    //    {
                    //        Point pFeCube = FeCubeList[i].ChangebaleMesh.Vertices[j];
                    //        Point pCube = this.ChangebaleMesh.Vertices[j];

                    //        if (pFeCube.X == pCube.X && pFeCube.Y == pCube.Y && pFeCube.Z == pCube.Z)
                    //        {
                    //            this.ChangebaleMesh.Vertices[j] = pFeCube;
                    //        }
                    //    }
                    //}
            }

            //for (int i = 0; i < FeCubeList.Count; i++)
            //{
            //    for (int j = 0; j < FeCubeList[i].DrawableMesh.Vertices.Count; j++)
            //    {
            //        Point p = FeCubeList[i].DrawableMesh.Vertices[j];

            //        if ((uniquePoints2.Count > 0 && !uniquePoints2.Contains(p)) || uniquePoints2.Count == 0)
            //        {
            //            p.ParentCube = this;
            //            uniquePoints2.Add(p.Clone());

            //            //for (int m = 0; m < this.NonDrawableMesh.Vertices.Count; m++)
            //            //{
            //            //    Point tempPoint = this.NonDrawableMesh.Vertices[m];
            //            //    if (tempPoint.X == p.X && tempPoint.Y == p.Y && tempPoint.Z == p.Z)
            //            //    {
            //            //        bigCubePoints[m] = p;
            //            //        break;
            //            //    }
            //            //}
            //        }
            //    }
            //}

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
                        if(ArePointsEqual(line.P1, p))
                        {
                            p1 = p;
                        }
                        else if(ArePointsEqual(line.P2, p))
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
                    if (p.X == meshP.X && p.Y == meshP.Y && p.Z == meshP.Z)
                    {
                        bigCubePoints[j] = p;
                        break;
                    }
                }
            }

            this.Mesh.Vertices = uniquePoints;
            this.Mesh.Edges = uniqueLines;


            //public void Transform()
            //{
            //    if (ParentCube.FeCubeList.Count > 0)
            //    {
            //        Dictionary<int, Point> verticesDictionary = new Dictionary<int, Point>();
            //        foreach (ComplexCube cube in ParentCube.FeCubeList)
            //        {
            //            List<Point> transformedVertices = new List<Point>();
            //            for (int i = 0; i < cube.ChangebaleMesh.Vertices.Count; i++)
            //            {
            //                Point point1 = cube.ChangebaleMesh.Vertices[i].Clone();
            //                double sumX = 0, sumY = 0, sumZ = 0;

            //                for (int j = 0; j < cube.ParentCube.ChangebaleMesh.Vertices.Count; j++)
            //                {
            //                    Point point2 = cube.ParentCube.ChangebaleMesh.Vertices[j].Clone();

            //                    Func<Point, Point, double> phiFunc = null;
            //                    if (j >= 0 && j < 8)
            //                    {
            //                        phiFunc = PhiAngle;
            //                    }
            //                    else if (j >= 8 && j < cube.ParentCube.ChangebaleMesh.Vertices.Count)
            //                    {
            //                        phiFunc = PhiEdge;
            //                    }

            //                    double funcResult = phiFunc(point1, point2);
            //                    sumX += point2.X * funcResult;
            //                    sumY += point2.Y * funcResult;
            //                    sumZ += point2.Z * funcResult;
            //                }
            //                cube.Mesh.Vertices[i].X = sumX;
            //                cube.Mesh.Vertices[i].Y = sumY;
            //                cube.Mesh.Vertices[i].Z = sumZ;
            //            }
            //        }
            //    }
            //}


            //public void Transform(int index)
            //{
            //    if (ParentCube.FeCubeList.Count > 0)
            //    {
            //        Dictionary<int, Point> verticesDictionary = new Dictionary<int, Point>();
            //        foreach (ComplexCube cube in ParentCube.FeCubeList)
            //        {
            //            List<Point> transformedVertices = new List<Point>();
            //            for (int i = 0; i < cube.ChangebaleMesh.Vertices.Count; i++)
            //            {
            //                Point point1 = cube.ChangebaleMesh.Vertices[i].Clone();
            //                //double sumX = 0, sumY = 0, sumZ = 0;
            //                double sum = 0;

            //                for (int j = 0; j < cube.ParentCube.ChangebaleMesh.Vertices.Count; j++)
            //                {
            //                    Point point2 = cube.ParentCube.ChangebaleMesh.Vertices[j].Clone();

            //                    Func<Point, Point, double> phiFunc = null;
            //                    if (j >= 0 && j < 8)
            //                    {
            //                        phiFunc = PhiAngle;
            //                    }
            //                    else if (j >= 8 && j < cube.ParentCube.ChangebaleMesh.Vertices.Count)
            //                    {
            //                        phiFunc = PhiEdge;
            //                    }

            //                    double funcResult = phiFunc(point1, point2);
            //                    sum += point2[index] * funcResult;
            //                    //sumX += point2.X * funcResult;
            //                    //sumY += point2.Y * funcResult;
            //                    //sumZ += point2.Z * funcResult;
            //                }
            //                cube.Mesh.Vertices[i][index] = sum;
            //                //cube.Mesh.Vertices[i].X = sumX;
            //                //cube.Mesh.Vertices[i].Y = sumY;
            //                //cube.Mesh.Vertices[i].Z = sumZ;
            //            }
            //        }
            //    }
            //}
        }

        private bool ArePointsEqual(Point p1, Point p2)
        {
            if(p1.X == p2.X && p1.Y == p2.Y && p1.Z == p2.Z)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        private bool AreLinesEqual(Line l1, Line l2)
        {
            if(l1.P1.X == l2.P1.X && l1.P1.Y == l2.P2.Y && l1.P1.Z == l2.P2.Z)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public void Transform2(Point p)
        {
            Point meshPoint = this.Mesh.ContainsPoint(p);
            if(meshPoint != null)
            {
                meshPoint.X = p.X;
                meshPoint.Y = p.Y;
                meshPoint.Z = p.Z;
            }
        }

        public bool Equals(ComplexCube cube)
        {
            if(cube != null)
            {
                return this.Mesh.Equals(cube.Mesh);
            }

            return false;
		}

        public void Transform(int index, ComplexCube cube)
        {
            for (int i = 0; i < cube.Mesh.Vertices.Count; i++)
            {
                Point point1 = cube.Mesh.Vertices[i];
                double sum = 0;
                for (int m = 0; m < cube.bigCubePoints.Count; m++)
                {
                    Point point2 = cube.bigCubePoints[m];

                    Func<Point, Point, double> phiFunc = null;
                    if (m >= 0 && m < 8)
                    {
                        phiFunc = PhiAngle;
                    }
                    else if (m >= 8 && m < cube.bigCubePoints.Count)
                    {
                        phiFunc = PhiEdge;
                    }

                    double funcResult = phiFunc(point1, point2);
                    sum += point2[index] * funcResult;
                }

                this.Mesh.Vertices[i][index] = sum;
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

        //public void Transform(int index)
        //{
        //    if(ParentCube.CubeChangebaleMeshPoints.Count > 0)
        //    {
        //        for(int i = 0; i < ParentCube.CubeChangebaleMeshPoints.Count; i++)
        //        {
        //            Point point1 = ParentCube.CubeChangebaleMeshPoints.ElementAt(i).Key;
        //            Point point1Clone = ParentCube.CubeChangebaleMeshPoints.ElementAt(i).Key.Clone();
        //            double sum = 0;
        //            for(int m = 0; m < 20; m++)
        //            {
        //                Point point2 = this.ParentCube.ChangebaleMesh.Vertices[m].Clone();

        //                Func<Point, Point, double> phiFunc = null;
        //                if (m >= 0 && m < 8)
        //                {
        //                    phiFunc = PhiAngle;
        //                }
        //                else if (m >= 8 && m < this.ParentCube.ChangebaleMesh.Vertices.Count)
        //                {
        //                    phiFunc = PhiEdge;
        //                }

        //                double funcResult = phiFunc(point1, point2);
        //                sum += point2[index] * funcResult;
        //            }


        //            for (int j = 0; j < ParentCube.CubeChangebaleMeshPoints.ElementAt(i).Value.Count; j++)
        //            {
        //                ComplexCube cube = ParentCube.CubeChangebaleMeshPoints.ElementAt(i).Value[j];
        //                int indexOfPoint = cube.ChangebaleMesh.GetIndexOfPoint(point1Clone);
        //                Point p = ParentCube.CubeMeshPoints.ElementAt(i).Value[j].Mesh.Vertices[indexOfPoint];
        //                p[index] = sum;
        //            }
        //        }
        //    }
        //}

        //public void Transform()
        //{
        //    if (ParentCube.FeCubeList.Count > 0)
        //    {
        //        Dictionary<int, Point> verticesDictionary = new Dictionary<int, Point>();
        //        foreach (ComplexCube cube in ParentCube.FeCubeList)
        //        {
        //            List<Point> transformedVertices = new List<Point>();
        //            for (int i = 0; i < cube.ChangebaleMesh.Vertices.Count; i++)
        //            {
        //                Point point1 = cube.ChangebaleMesh.Vertices[i].Clone();
        //                double sumX = 0, sumY = 0, sumZ = 0;

        //                for (int j = 0; j < cube.ParentCube.ChangebaleMesh.Vertices.Count; j++)
        //                {
        //                    Point point2 = cube.ParentCube.ChangebaleMesh.Vertices[j].Clone();

        //                    Func<Point, Point, double> phiFunc = null;
        //                    if (j >= 0 && j < 8)
        //                    {
        //                        phiFunc = PhiAngle;
        //                    }
        //                    else if (j >= 8 && j < cube.ParentCube.ChangebaleMesh.Vertices.Count)
        //                    {
        //                        phiFunc = PhiEdge;
        //                    }

        //                    double funcResult = phiFunc(point1, point2);
        //                    sumX += point2.X * funcResult;
        //                    sumY += point2.Y * funcResult;
        //                    sumZ += point2.Z * funcResult;
        //                }

        //                //point2.X = sumX;
        //                //point2.Y = sumY;
        //                //point2.Z = sumZ;


        //                //foreach(Point cubePoint in cube.ParentCube.ChangebaleMesh.Vertices)
        //                //{
        //                //    if(cubePoint == point1)
        //                //    {
        //                //        cubePoint.X = sumX;
        //                //        cubePoint.Y = sumY;
        //                //        cubePoint.Z = sumZ;

        //                //        break;
        //                //    }
        //                //}

        //                for(int m = 0; m < cube.ParentCube.cubePoints.Count; m++)
        //                {
        //                    Point valuePoint = cube.ParentCube.cubePoints.ElementAt(m).Value;
        //                    if (valuePoint.X == point1.X && valuePoint.Y == point1.Y && valuePoint.Z == point1.Z)
        //                    {
        //                        Point keyPoint = cube.ParentCube.cubePoints.ElementAt(m).Key;
        //                        keyPoint.X = sumX;
        //                        keyPoint.Y = sumY;
        //                        keyPoint.Z = sumZ;
        //                    }
        //                }

        //                //if (cube.ParentCube.cubePoints.ContainsValue(point1))
        //                //{
        //                //    Point p = cube.ParentCube.cubePoints.FirstOrDefault(x => x.Value == cube.ParentCube.ChangebaleMesh.Vertices[i]).Key;
        //                //    p.X = sumX;
        //                //    p.Y = sumY;
        //                //    p.Z = sumZ;
        //                //}


        //                cube.Mesh.Vertices[i].X = sumX;
        //                cube.Mesh.Vertices[i].Y = sumY;
        //                cube.Mesh.Vertices[i].Z = sumZ;

        //                //Point newPoint = new Point(sumX, sumY, sumZ);
        //                //if (cube.ChangebaleMesh.Vertices[i] == cube.ParentCube.ChangebaleMesh.Vertices[i] && this.ParentCube.cubePoints.ContainsValue(cube.ChangebaleMesh.Vertices[i]))
        //                //{
        //                //    Point p = this.ParentCube.cubePoints.FirstOrDefault(x => x.Value == cube.ChangebaleMesh.Vertices[i]).Key;
        //                //    p.X = sumX;
        //                //    p.Y = sumY;
        //                //    p.Z = sumZ;
        //                //}


        //                //int indexOfPoint = cube.ParentCube.ChangebaleMesh.GetIndexOfPoint(point1);
        //                //if (indexOfPoint != -1 && !verticesDictionary.ContainsKey(indexOfPoint))
        //                //{
        //                //    Point newPoint = new Point(sumX, sumY, sumZ);
        //                //    verticesDictionary.Add(indexOfPoint, newPoint);
        //                //}




        //                //Point newPoint = new Point(sumX, sumY, sumZ, GL);
        //                //transformedVertices.Add(newPoint.Clone());
        //            }

        //            //for (int i = 0; i < transformedVertices.Count; i++)
        //            //{
        //            //    cube.ParentCube.Mesh.Vertices[i].X = transformedVertices[i].X;
        //            //    cube.ParentCube.Mesh.Vertices[i].Y = transformedVertices[i].Y;
        //            //    cube.ParentCube.Mesh.Vertices[i].Z = transformedVertices[i].Z;
        //            //}


        //        }
        //        //foreach (KeyValuePair<int, Point> pair in verticesDictionary)
        //        //{
        //        //    this.ParentCube.Mesh.Vertices[pair.Key].X = pair.Value.X;
        //        //    this.ParentCube.Mesh.Vertices[pair.Key].Y = pair.Value.Y;
        //        //    this.ParentCube.Mesh.Vertices[pair.Key].Z = pair.Value.Z;
        //        //}
        //    }
        //}

        //public void TransformVertices(double? x, double? y, double? z)
        //{
        //    int coordIndex = 1;
        //    if (x != null) coordIndex = 0;
        //    else if (y != null) coordIndex = 1;
        //    else if (z != null) coordIndex = 2;

        //    //Console.WriteLine(String.Format("({0}, {1}, {2})", x, y, z));

        //    if (FeCubeList.Count > 0)
        //    {
        //        for (int m = 0; m < FeCubeList.Count; m++)
        //        {
        //            List<Point> transformedVertices = new List<Point>();
        //            for (int v1 = 0; v1 < FeCubeList[m].DrawableMesh.Vertices.Count; v1++)
        //            {
        //                Point vertex1 = FeCubeList[m].DrawableMesh.Vertices[v1].Clone();
        //                Func<Point, Point, double> phiFunc = null;
        //                if (v1 >= 0 && v1 < 8)
        //                {
        //                    phiFunc = PhiAngle;
        //                }
        //                else if (v1 >= 8 && v1 < FeCubeList[m].DrawableMesh.Vertices.Count)
        //                {
        //                    phiFunc = PhiEdge;
        //                }

        //                double sum = 0;
        //                for (int c = 0; c < FeCubeList.Count; c++)
        //                {
        //                    for (int v2 = 0; v2 < FeCubeList[c].DrawableMesh.Vertices.Count; v2++)
        //                    {
        //                        Point vertex2 = FeCubeList[c].DrawableMesh.Vertices[v2].Clone();
        //                        double funcResult = phiFunc(vertex2, vertex1);
        //                        sum += vertex1[coordIndex] * funcResult;
        //                    }
        //                }
        //                vertex1[coordIndex] = sum;
        //                transformedVertices.Add(vertex1.Clone());
        //            }


        //            for (int i = 0; i < transformedVertices.Count; i++)
        //            {
        //                FeCubeList[m].ChangebaleMesh.Vertices[i].X = transformedVertices[i].X;
        //                FeCubeList[m].ChangebaleMesh.Vertices[i].Y = transformedVertices[i].Y;
        //                FeCubeList[m].ChangebaleMesh.Vertices[i].Z = transformedVertices[i].Z;
        //            }
        //        }
        //    }
        //    else
        //    {
        //        List<Point> transformedVertices = new List<Point>();
        //        for (int v1 = 0; v1 < this.DrawableMesh.Vertices.Count; v1++)
        //        {
        //            Point vertex1 = this.DrawableMesh.Vertices[v1].Clone();
        //            Func<Point, Point, double> phiFunc = null;
        //            if (v1 >= 0 && v1 < 8)
        //            {
        //                phiFunc = PhiAngle;
        //            }
        //            else if (v1 >= 8 && v1 < this.DrawableMesh.Vertices.Count)
        //            {
        //                phiFunc = PhiEdge;
        //            }

        //            double sum = 0;
        //            for (int v2 = 0; v2 < this.DrawableMesh.Vertices.Count; v2++)
        //            {
        //                Point vertex2 = this.DrawableMesh.Vertices[v2].Clone();
        //                double funcResult = phiFunc(vertex2, vertex1);
        //                sum += vertex1[coordIndex] * funcResult;
        //                //sumY += vertex1[1] * funcResult;
        //                //sumZ += vertex1[2] * funcResult;
        //            }
        //            vertex1[coordIndex] = sum;
        //            //vertex1.Y = sumY;
        //            //vertex1.Z = sumZ;

        //            transformedVertices.Add(vertex1.Clone());
        //        }


        //        for (int i = 0; i < transformedVertices.Count; i++)
        //        {
        //            this.ChangebaleMesh.Vertices[i].X = transformedVertices[i].X;
        //            this.ChangebaleMesh.Vertices[i].Y = transformedVertices[i].Y;
        //            this.ChangebaleMesh.Vertices[i].Z = transformedVertices[i].Z;
        //        }
        //    }



            //if (FeCubeList.Count > 0)
            //{
            //    for (int m = 0; m < FeCubeList.Count; m++)
            //    {
            //        List<Point> transformedVertices = new List<Point>();
            //        for (int v1 = 0; v1 < FeCubeList[m].Mesh.Vertices.Count; v1++)
            //        {
            //            Point vertex1 = FeCubeList[m].Mesh.Vertices[v1].Clone();
            //            Func<Point, Point, double> phiFunc = null;
            //            if (v1 >= 0 && v1 < 8)
            //            {
            //                phiFunc = PhiAngle;
            //            }
            //            else if (v1 >= 8 && v1 < FeCubeList[m].Mesh.Vertices.Count)
            //            {
            //                phiFunc = PhiEdge;
            //            }

            //            double sumX = 0, sumY = 0, sumZ = 0;
            //            for (int c = 0; c < FeCubeList.Count; c++)
            //            {
            //                for (int v2 = 0; v2 < FeCubeList[c].Mesh.Vertices.Count; v2++)
            //                {
            //                    Point vertex2 = FeCubeList[c].Mesh.Vertices[v2].Clone();
            //                    double funcResult = phiFunc(vertex2, vertex1);
            //                    sumX += vertex1[0] * funcResult;
            //                    sumY += vertex1[1] * funcResult;
            //                    sumZ += vertex1[2] * funcResult;
            //                }
            //            }
            //            vertex1[0] = sumX;
            //            vertex1[1] = sumY;
            //            vertex1[2] = sumZ;
            //            transformedVertices.Add(vertex1.Clone());
            //        }


            //        for (int i = 0; i < transformedVertices.Count; i++)
            //        {
            //            FeCubeList[m].ChangebaleMesh.Vertices[i].X = transformedVertices[i].X;
            //            FeCubeList[m].ChangebaleMesh.Vertices[i].Y = transformedVertices[i].Y;
            //            FeCubeList[m].ChangebaleMesh.Vertices[i].Z = transformedVertices[i].Z;
            //        }
            //    }
            //}
            //else
            //{
            //    List<Point> transformedVertices = new List<Point>();
            //    for (int v1 = 0; v1 < this.Mesh.Vertices.Count; v1++)
            //    {
            //        Point vertex1 = this.Mesh.Vertices[v1].Clone();
            //        Func<Point, Point, double> phiFunc = null;
            //        if (v1 >= 0 && v1 < 8)
            //        {
            //            phiFunc = PhiAngle;
            //        }
            //        else if (v1 >= 8 && v1 < this.Mesh.Vertices.Count)
            //        {
            //            phiFunc = PhiEdge;
            //        }

            //        double sumX = 0, sumY = 0, sumZ = 0;
            //        for (int v2 = 0; v2 < this.Mesh.Vertices.Count; v2++)
            //        {
            //            Point vertex2 = this.Mesh.Vertices[v2].Clone();
            //            double funcResult = phiFunc(vertex2, vertex1);
            //            sumX += vertex2[0] * funcResult;
            //            sumY += vertex2[1] * funcResult;
            //            sumZ += vertex2[2] * funcResult;
            //        }
            //        vertex1[0] = sumX;
            //        vertex1[1] = sumY;
            //        vertex1[2] = sumZ;

            //        transformedVertices.Add(vertex1.Clone());
            //    }


            //    for (int i = 0; i < transformedVertices.Count; i++)
            //    {
            //        this.ChangebaleMesh.Vertices[i].X = transformedVertices[i].X;
            //        this.ChangebaleMesh.Vertices[i].Y = transformedVertices[i].Y;
            //        this.ChangebaleMesh.Vertices[i].Z = transformedVertices[i].Z;
            //    }
            //}
        //}

        //public void TransformVertices(Point p)
        //{
        //    List<Point> transformedVertices = new List<Point>();
        //    Point vertex1 = p.Clone();
        //    Func<Point, Point, double> phiFunc = null;
        //    int v1 = 0;
        //    for(int i = 0; i < this.DrawableMesh.Vertices.Count; i++)
        //    {
        //        Point meshPoint = this.DrawableMesh.Vertices[i];
        //        if(p.X == meshPoint.X && p.Y == meshPoint.Y && p.Z == meshPoint.Z)
        //        {
        //            v1 = i;
        //            break;
        //        }
        //    }


        //    if (v1 >= 0 && v1 < 8)
        //    {
        //        phiFunc = PhiAngle;
        //    }
        //    else if (v1 >= 8 && v1 < this.DrawableMesh.Vertices.Count)
        //    {
        //        phiFunc = PhiEdge;
        //    }

        //    double sum = 0;
        //    for (int v2 = 0; v2 < this.DrawableMesh.Vertices.Count; v2++)
        //    {
        //        Point vertex2 = this.DrawableMesh.Vertices[v2].Clone();
        //        double funcResult = phiFunc(vertex2, vertex1);
        //        sum += vertex1[0] * funcResult;
        //        //sumY += vertex1[1] * funcResult;
        //        //sumZ += vertex1[2] * funcResult;
        //    }
        //    vertex1[0] = sum;
        //    //vertex1.Y = sumY;
        //    //vertex1.Z = sumZ;

        //    this.ChangebaleMesh.Vertices[v1].X = vertex1.X;
        //    this.ChangebaleMesh.Vertices[v1].Y = vertex1.Y;
        //    this.ChangebaleMesh.Vertices[v1].Z = vertex1.Z;
        //}


        private double PhiAngle(Point v1, Point v2)
        {
            return (double)1 / 8 * (1 + v1[0] * v2[0]) * (1 + v1[1] * v2[1]) * (1 + v1[2] * v2[2]) *
                (v1[0] * v2[0] + v1[1] * v2[1] + v1[2] * v2[2] - 2);
        }

        private double PhiEdge(Point v1, Point v2)
        {
            return (double)1 / 4 * (1 + v1[0] * v2[0]) * (1 + v1[1] * v2[1]) * (1 + v1[2] * v2[2]) *
                (1 - Math.Pow((v1[0] * v2[1] * v2[2]), 2) - Math.Pow((v1[1] * v2[0] * v2[2]), 2) - Math.Pow((v1[2] * v2[0] * v2[1]), 2));
        }
    }


	public class AxisCube : CustomCube
	{
        private const int VERTICES_ON_FACET = 4;

        public AxisCube(OpenGL _gl, Point _centerPoint, CoordinateAxis _axis,
						double? _sizeX = null, double? _sizeY = null, double?
						_sizeZ = null, string _cubeName = null): base(_gl, _centerPoint, _sizeX, _sizeY, _sizeZ, _cubeName)
		{
			Axis = _axis;
            DrawFacets = true;

            //Initializing Vertices
            Mesh.Vertices = new List<Point>
            {
                new Point(-sizeX + CenterPoint.X, -sizeY + CenterPoint.Y, sizeZ + CenterPoint.Z, GL),
                new Point(sizeX + CenterPoint.X, -sizeY + CenterPoint.Y, sizeZ + CenterPoint.Z, GL),
                new Point(sizeX + CenterPoint.X, -sizeY + CenterPoint.Y, -sizeZ + CenterPoint.Z, GL),
                new Point(-sizeX + CenterPoint.X, -sizeY + CenterPoint.Y, -sizeZ + CenterPoint.Z, GL),
                new Point(-sizeX + CenterPoint.X, sizeY + CenterPoint.Y, sizeZ + CenterPoint.Z, GL),
                new Point(sizeX + CenterPoint.X, sizeY + CenterPoint.Y, sizeZ + CenterPoint.Z, GL),
                new Point(sizeX + CenterPoint.X, sizeY + CenterPoint.Y, -sizeZ + CenterPoint.Z, GL),
                new Point(-sizeX + CenterPoint.X, sizeY + CenterPoint.Y, -sizeZ + CenterPoint.Z, GL),
            };

            foreach(Point p in Mesh.Vertices)
            {
                p.ParentCube = this;
            }
            CenterPoint.ParentCube = this;

            //Initializing Facets
            Mesh.Facets = new List<Plane>
            {
				//FRONT
				new Plane(new List<Point>
                {
                    Mesh.Vertices[0],
                    Mesh.Vertices[1],
                    Mesh.Vertices[5],
                    Mesh.Vertices[4]
                }, GL),

				//RIGHT
				new Plane(new List<Point>
                {
                    Mesh.Vertices[1],
                    Mesh.Vertices[2],
                    Mesh.Vertices[6],
                    Mesh.Vertices[5]
                }, GL),

				//BACK
				new Plane(new List<Point>
                {
                    Mesh.Vertices[2],
                    Mesh.Vertices[3],
                    Mesh.Vertices[7],
                    Mesh.Vertices[6]
                }, GL),

				//LEFT
				new Plane(new List<Point>
                {
                    Mesh.Vertices[3],
                    Mesh.Vertices[0],
                    Mesh.Vertices[4],
                    Mesh.Vertices[7]
                }, GL),

				//TOP
				new Plane(new List<Point>
                {
                    Mesh.Vertices[4],
                    Mesh.Vertices[5],
                    Mesh.Vertices[6],
                    Mesh.Vertices[7]
                }, GL),

				//BOTTOM
				new Plane(new List<Point>
                {
                    Mesh.Vertices[1],
                    Mesh.Vertices[0],
                    Mesh.Vertices[3],
                    Mesh.Vertices[2]
                }, GL)
            };

            //Defining facet - vertex relationships
            //mesh.Vertices[0].FacetParents = new List<Facet>(){ mesh.Facets[1], mesh.Facets[3], mesh.Facets[5]};
            //mesh.Vertices[1].FacetParents = new List<Facet>(){ mesh.Facets[0], mesh.Facets[3], mesh.Facets[5]};
            //mesh.Vertices[2].FacetParents = new List<Facet>(){ mesh.Facets[1], mesh.Facets[2], mesh.Facets[5]};
            //mesh.Vertices[3].FacetParents = new List<Facet>(){ mesh.Facets[0], mesh.Facets[2], mesh.Facets[5]};
            //mesh.Vertices[4].FacetParents = new List<Facet>(){ mesh.Facets[1], mesh.Facets[3], mesh.Facets[4]};
            //mesh.Vertices[5].FacetParents = new List<Facet>(){ mesh.Facets[0], mesh.Facets[3], mesh.Facets[4]};
            //mesh.Vertices[6].FacetParents = new List<Facet>(){ mesh.Facets[1], mesh.Facets[2], mesh.Facets[4]};
            //mesh.Vertices[7].FacetParents = new List<Facet>(){ mesh.Facets[0], mesh.Facets[2], mesh.Facets[4]};


            //Initializing Edges
            List<Line> edges = new List<Line>();
            for (int i = 0; i < Mesh.Facets.Count; i++)
            {
                Plane currentFacet = Mesh.Facets[i];
                for (int j = 0; j < VERTICES_ON_FACET; j++)
                {
                    Line newEdge = new Line(currentFacet.Points[j], currentFacet.Points[(j + 1) % VERTICES_ON_FACET], GL);
                    if (edges.Count != 0)
                    {
                        if (!newEdge.Exists(edges))
                        {
                            edges.Add(newEdge);

                            //Defining Edge - Vertex relationship
                            Mesh.Vertices[j].EdgeParents.Add(newEdge);
                            Mesh.Vertices[(j + 1) % VERTICES_ON_FACET].EdgeParents.Add(newEdge);
                        }
                    }
                    else
                    {
                        edges.Add(newEdge);

                        //Defining Edge - Vertex relationship
                        Mesh.Vertices[j].EdgeParents.Add(newEdge);
                        Mesh.Vertices[(j + 1) % VERTICES_ON_FACET].EdgeParents.Add(newEdge);
                    }
                }
            }
            Mesh.Edges = edges;
        }
    }
}
