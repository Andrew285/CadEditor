using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;

using System.Runtime.InteropServices;
using SharpGL.SceneGraph;
using SharpGL;
using SharpGL.SceneGraph.Core;
using System.Linq.Expressions;

namespace CadEditor
{
    public class CustomCube: ISelectable
    {
        protected OpenGL gl;
		protected Mesh mesh;
		private string cubeName;
		private const int FACETS_AMOUNT = 6;
		private const int EDGES_AMOUNT = 12;
		private const int VERTICES_AMOUNT = 8;
		private float size = 1.0f;
		private Vertex centerPoint;

		public Mesh Mesh
		{
			get { return mesh; }
		}

		public string CubeName
		{
			get { return cubeName; }
		}

		public bool IsSelected { get; set; }
		public Color SelectedColor { get; set; }
		public Color NonSelectedColor { get; set; }

		public CustomCube(OpenGL _gl, Vertex _centerPoint, float? _size = null, string _cubeName = null)
		{
			gl = _gl;
			mesh = new Mesh();
			cubeName = _cubeName;
			centerPoint = _centerPoint;

			if(_size != null)
			{
				size = (float)_size;
			}
			
			if(_cubeName != null)
			{
				cubeName = _cubeName;
			}

			//Initializing Vertices
			mesh.Vertices = new Vertex[]
			{
				new Vertex(gl, -size + centerPoint.X, -size + centerPoint.Y, -size + centerPoint.Z),
				new Vertex(gl, -size + centerPoint.X, -size + centerPoint.Y, size + centerPoint.Z),
				new Vertex(gl, -size + centerPoint.X, size + centerPoint.Y, -size + centerPoint.Z),
				new Vertex(gl, -size + centerPoint.X, size + centerPoint.Y, size + centerPoint.Z),
				new Vertex(gl, size + centerPoint.X, -size + centerPoint.Y, -size + centerPoint.Z),
				new Vertex(gl, size + centerPoint.X, -size + centerPoint.Y, size + centerPoint.Z),
				new Vertex(gl, size + centerPoint.X, size + centerPoint.Y, -size + centerPoint.Z),
				new Vertex(gl, size + centerPoint.X, size + centerPoint.Y, size + centerPoint.Z)
			};

			//Initializing Facets
			mesh.Facets = new Facet[FACETS_AMOUNT]
			{
				new Facet(gl, new Vertex[]
				{
					mesh.Vertices[1],
					mesh.Vertices[5],
					mesh.Vertices[7],
					mesh.Vertices[3]
				}),
				new Facet(gl, new Vertex[]
				{
					mesh.Vertices[0],
					mesh.Vertices[2],
					mesh.Vertices[6],
					mesh.Vertices[4]
				}),
				new Facet(gl, new Vertex[]
				{
					mesh.Vertices[2],
					mesh.Vertices[3],
					mesh.Vertices[7],
					mesh.Vertices[6]
				}),
				new Facet(gl, new Vertex[]
				{
					mesh.Vertices[0],
					mesh.Vertices[4],
					mesh.Vertices[5],
					mesh.Vertices[1]
				}),
				new Facet(gl, new Vertex[]
				{
					mesh.Vertices[4],
					mesh.Vertices[6],
					mesh.Vertices[7],
					mesh.Vertices[5]
				}),
				new Facet(gl, new Vertex[]
				{
					mesh.Vertices[0],
					mesh.Vertices[1],
					mesh.Vertices[3],
					mesh.Vertices[2]
				})
			};

			//Defining facet - vertex relationships
			mesh.Vertices[0].FacetParents = new List<Facet>(){ mesh.Facets[1], mesh.Facets[3], mesh.Facets[5]};
			mesh.Vertices[1].FacetParents = new List<Facet>(){ mesh.Facets[0], mesh.Facets[3], mesh.Facets[5]};
			mesh.Vertices[2].FacetParents = new List<Facet>(){ mesh.Facets[1], mesh.Facets[2], mesh.Facets[5]};
			mesh.Vertices[3].FacetParents = new List<Facet>(){ mesh.Facets[0], mesh.Facets[2], mesh.Facets[5]};
			mesh.Vertices[4].FacetParents = new List<Facet>(){ mesh.Facets[1], mesh.Facets[3], mesh.Facets[4]};
			mesh.Vertices[5].FacetParents = new List<Facet>(){ mesh.Facets[0], mesh.Facets[3], mesh.Facets[4]};
			mesh.Vertices[6].FacetParents = new List<Facet>(){ mesh.Facets[1], mesh.Facets[2], mesh.Facets[4]};
			mesh.Vertices[7].FacetParents = new List<Facet>(){ mesh.Facets[0], mesh.Facets[2], mesh.Facets[4]};


			//Initializing Edges
			List<Edge> edges = new List<Edge>();
			for(int i = 0; i < mesh.Facets.Length; i++)
			{
				Facet currentFacet = mesh.Facets[i];
				for(int j = 0; j < 4; j++)
				{
					Edge newEdge = new Edge(gl, currentFacet.Vertices[j], currentFacet.Vertices[(j+1)%4]);
					if(edges.Count != 0)
					{
						if (!newEdge.Exists(edges))
						{
							edges.Add(newEdge);

							//Defining Edge - Vertex relationship
							mesh.Vertices[j].EdgeParents.Add(newEdge);
							mesh.Vertices[(j + 1) % 4].EdgeParents.Add(newEdge);
						}
					}
					else
					{
						edges.Add(newEdge);

						//Defining Edge - Vertex relationship
						mesh.Vertices[j].EdgeParents.Add(newEdge);
						mesh.Vertices[(j + 1) % 4].EdgeParents.Add(newEdge);
					}
				}
			}
			mesh.Edges = edges.ToArray();


		}

		public void Draw(double[] vertexColor = null, double[] edgeColor = null, double[] facetColor = null)
		{

			//Draw Vertexes
			gl.PointSize(10.0f);
			gl.Begin(OpenGL.GL_POINTS);
			for (int i = 0; i < mesh.Vertices.Length; i++)
			{
				mesh.Vertices[i].Draw(vertexColor);
			}
			gl.End();
			gl.Flush();

			//Draw Edges
			gl.Begin(OpenGL.GL_LINES);
			for (int i = 0; i < mesh.Edges.Length; i++)
			{
				mesh.Edges[i].Draw(edgeColor);
			}
			gl.End();
			gl.Flush();


			//Draw Facets
			gl.Begin(OpenGL.GL_QUADS);
			for (int i = 0; i < mesh.Facets.Length; i++)
			{
				mesh.Facets[i].Draw(facetColor);
			}
			gl.End();
			gl.Flush();
		}

		public void Select()
		{
			foreach (Edge edge in Mesh.Edges)
			{
				edge.IsSelected = true;
			}
		}

		public virtual void Deselect()
		{
			foreach (Facet facet in Mesh.Facets)
			{
				facet.IsSelected = false;
			}

			foreach (Edge edge in Mesh.Edges)
			{
				edge.IsSelected = false;
			}

			foreach (Vertex vertex in Mesh.Vertices)
			{
				vertex.IsSelected = false;
			}
		}

		public void Move(double x, double y, double z)
		{
			for (int i = 0; i < mesh.Vertices.Length; i++)
			{
				mesh.Vertices[i].Move(x, y, z);
			}
			centerPoint.Move(x, y, z);
		}

		public Vertex GetCenterPoint()
		{
			return centerPoint;
		}

	}


	public class AxisCube : CustomCube, ISelectable
	{
		public CoordinateAxis Axis { get; set; }
		public bool IsSelected { get; set; }
		public Color SelectedColor { get; set; }
		public Color NonSelectedColor { get; set; }

		public AxisCube(OpenGL _gl, Vertex _centerPoint, CoordinateAxis _axis, float? _size = null, string _cubeName = null) : base(_gl, _centerPoint, _size, _cubeName)
		{
			Axis = _axis;
		}

		public void Draw()
		{
			if (IsSelected)
			{
				base.Draw(null, new double[] { SelectedColor.R, SelectedColor.G, SelectedColor.B }, null);
			}
			else
			{
				base.Draw(null, null, new double[] { NonSelectedColor.R, NonSelectedColor.G, NonSelectedColor.B });
			}
		}

		public override void Deselect()
		{
			IsSelected = false;
		}
	}
}
