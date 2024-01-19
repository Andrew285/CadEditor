using CadEditor.Settings;
using SharpGL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CadEditor.MeshObjects
{
	public class AxisCube : MeshObject3D
	{
        public CoordinateAxis Axis { get; set; }

        private const int VERTICES_ON_FACET = 4;

		public AxisCube(Point3D _centerPoint, CoordinateAxis _axis,
						Vector _size, string _cubeName = null) : base(_centerPoint, _size, _cubeName)
		{
			Axis = _axis;
			DrawFacets = SceneSettings.AxisCubeDrawFacets;
			DrawEdges = SceneSettings.AxisCubeDrawEdges;
			DrawVertices = SceneSettings.AxisCubeDrawVertices;
            double sizeX = Size[0];
            double sizeY = Size[1];
            double sizeZ = Size[2];

			//Initializing Vertices
			Point3D centerPoint = GetCenterPoint();
            Mesh.Vertices = new List<Point3D>
			{
				new Point3D(-sizeX + centerPoint.X, -sizeY + centerPoint.Y, sizeZ + centerPoint.Z),
				new Point3D(sizeX + centerPoint.X, -sizeY + centerPoint.Y, sizeZ + centerPoint.Z),
				new Point3D(sizeX + centerPoint.X, -sizeY + centerPoint.Y, -sizeZ + centerPoint.Z),
				new Point3D(-sizeX + centerPoint.X, -sizeY + centerPoint.Y, -sizeZ + centerPoint.Z),
				new Point3D(-sizeX + centerPoint.X, sizeY + centerPoint.Y, sizeZ + centerPoint.Z),
				new Point3D(sizeX + centerPoint.X, sizeY + centerPoint.Y, sizeZ + centerPoint.Z),
				new Point3D(sizeX + centerPoint.X, sizeY + centerPoint.Y, -sizeZ + centerPoint.Z),
				new Point3D(-sizeX + centerPoint.X, sizeY + centerPoint.Y, -sizeZ + centerPoint.Z),
			};

			foreach (Point3D p in Mesh.Vertices)
			{
				p.ParentCube = this;
				p.ParentObject = this;
			}
			centerPoint.ParentCube = this;

			//Initializing Facets
			Mesh.Facets = new List<Plane>
			{
				//FRONT
				new Plane(new List<Point3D>
				{
					Mesh.Vertices[0],
					Mesh.Vertices[1],
					Mesh.Vertices[5],
					Mesh.Vertices[4]
				}),

				//RIGHT
				new Plane(new List<Point3D>
				{
					Mesh.Vertices[1],
					Mesh.Vertices[2],
					Mesh.Vertices[6],
					Mesh.Vertices[5]
				}),

				//BACK
				new Plane(new List<Point3D>
				{
					Mesh.Vertices[2],
					Mesh.Vertices[3],
					Mesh.Vertices[7],
					Mesh.Vertices[6]
				}),

				//LEFT
				new Plane(new List<Point3D>
				{
					Mesh.Vertices[3],
					Mesh.Vertices[0],
					Mesh.Vertices[4],
					Mesh.Vertices[7]
				}),

				//TOP
				new Plane(new List<Point3D>
				{
					Mesh.Vertices[4],
					Mesh.Vertices[5],
					Mesh.Vertices[6],
					Mesh.Vertices[7]
				}),

				//BOTTOM
				new Plane(new List<Point3D>
				{
					Mesh.Vertices[1],
					Mesh.Vertices[0],
					Mesh.Vertices[3],
					Mesh.Vertices[2]
				})
			};

            foreach (Plane plane in Mesh.Facets)
            {
                plane.ParentObject = this;
            }

            //Initializing Edges
            List<Line> edges = new List<Line>();
			for (int i = 0; i < Mesh.Facets.Count; i++)
			{
				Plane currentFacet = Mesh.Facets[i];
				for (int j = 0; j < VERTICES_ON_FACET; j++)
				{
					Line newEdge = new Line(currentFacet.Points[j], currentFacet.Points[(j + 1) % VERTICES_ON_FACET]);
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

            foreach (Line line in Mesh.Edges)
            {
                line.ParentObject = this;
            }

            Mesh.Edges = edges;
		}
	}
}
