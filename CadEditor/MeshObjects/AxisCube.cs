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

		public AxisCube(OpenGL _gl, Point3D _centerPoint, CoordinateAxis _axis,
						Vector _size, string _cubeName = null) : base(_gl, _centerPoint, _size, _cubeName)
		{
			Axis = _axis;
			DrawFacets = true;
            double sizeX = size[0];
            double sizeY = size[1];
            double sizeZ = size[2];

            //Initializing Vertices
            Mesh.Vertices = new List<Point3D>
			{
				new Point3D(-sizeX + CenterPoint.X, -sizeY + CenterPoint.Y, sizeZ + CenterPoint.Z, GL),
				new Point3D(sizeX + CenterPoint.X, -sizeY + CenterPoint.Y, sizeZ + CenterPoint.Z, GL),
				new Point3D(sizeX + CenterPoint.X, -sizeY + CenterPoint.Y, -sizeZ + CenterPoint.Z, GL),
				new Point3D(-sizeX + CenterPoint.X, -sizeY + CenterPoint.Y, -sizeZ + CenterPoint.Z, GL),
				new Point3D(-sizeX + CenterPoint.X, sizeY + CenterPoint.Y, sizeZ + CenterPoint.Z, GL),
				new Point3D(sizeX + CenterPoint.X, sizeY + CenterPoint.Y, sizeZ + CenterPoint.Z, GL),
				new Point3D(sizeX + CenterPoint.X, sizeY + CenterPoint.Y, -sizeZ + CenterPoint.Z, GL),
				new Point3D(-sizeX + CenterPoint.X, sizeY + CenterPoint.Y, -sizeZ + CenterPoint.Z, GL),
			};

			foreach (Point3D p in Mesh.Vertices)
			{
				p.ParentCube = this;
			}
			CenterPoint.ParentCube = this;

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
				}, GL),

				//RIGHT
				new Plane(new List<Point3D>
				{
					Mesh.Vertices[1],
					Mesh.Vertices[2],
					Mesh.Vertices[6],
					Mesh.Vertices[5]
				}, GL),

				//BACK
				new Plane(new List<Point3D>
				{
					Mesh.Vertices[2],
					Mesh.Vertices[3],
					Mesh.Vertices[7],
					Mesh.Vertices[6]
				}, GL),

				//LEFT
				new Plane(new List<Point3D>
				{
					Mesh.Vertices[3],
					Mesh.Vertices[0],
					Mesh.Vertices[4],
					Mesh.Vertices[7]
				}, GL),

				//TOP
				new Plane(new List<Point3D>
				{
					Mesh.Vertices[4],
					Mesh.Vertices[5],
					Mesh.Vertices[6],
					Mesh.Vertices[7]
				}, GL),

				//BOTTOM
				new Plane(new List<Point3D>
				{
					Mesh.Vertices[1],
					Mesh.Vertices[0],
					Mesh.Vertices[3],
					Mesh.Vertices[2]
				}, GL)
			};

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
