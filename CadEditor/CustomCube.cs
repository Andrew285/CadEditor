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
    public class CustomCube: SceneElement
    {
        private OpenGL gl;
		private Mesh mesh;
		private string cubeName;
		private const int FACETS_AMOUNT = 6;
		private const int EDGES_AMOUNT = 12;
		private const int VERTICES_AMOUNT = 8;

		public Mesh Mesh
		{
			get { return mesh; }
		}

		public string CubeName
		{
			get { return cubeName; }
		}

		public CustomCube(OpenGL _gl, string _cubeName)
		{
			gl = _gl;
			mesh = new Mesh();
			cubeName = _cubeName;

			//Initializing Vertices
			mesh.Vertices = new Vertex[]
			{
				new Vertex(gl, -1.0f, -1.0f, -1.0f),
				new Vertex(gl, -1.0f, -1.0f, 1.0f),
				new Vertex(gl, -1.0f, 1.0f, -1.0f),
				new Vertex(gl, -1.0f, 1.0f, 1.0f),
				new Vertex(gl, 1.0f, -1.0f, -1.0f),
				new Vertex(gl, 1.0f, -1.0f, 1.0f),
				new Vertex(gl, 1.0f, 1.0f, -1.0f),
				new Vertex(gl, 1.0f, 1.0f, 1.0f)
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

		public void Draw()
		{

			//Draw Vertexes
			gl.PointSize(10.0f);
			gl.Begin(OpenGL.GL_POINTS);
			for (int i = 0; i < mesh.Vertices.Length; i++)
			{
				mesh.Vertices[i].Draw();
			}
			gl.End();
			gl.Flush();

			//Draw Edges
			gl.Begin(OpenGL.GL_LINES);
			for (int i = 0; i < mesh.Edges.Length; i++)
			{
				mesh.Edges[i].Draw();
			}
			gl.End();
			gl.Flush();

			


			//Draw Facets
			gl.Begin(OpenGL.GL_QUADS);
			for (int i = 0; i < mesh.Facets.Length; i++)
			{
				mesh.Facets[i].Draw();
				//gl.Vertex(currectFacet.Vertices[0].X, currectFacet.Vertices[0].Y, currectFacet.Vertices[0].Z);
				//gl.Vertex(currectFacet.Vertices[1].X, currectFacet.Vertices[1].Y, currectFacet.Vertices[1].Z);
				//gl.Vertex(currectFacet.Vertices[2].X, currectFacet.Vertices[2].Y, currectFacet.Vertices[2].Z);
				//gl.Vertex(currectFacet.Vertices[3].X, currectFacet.Vertices[3].Y, currectFacet.Vertices[3].Z);
			}
			gl.End();
			gl.Flush();
		}

		public void SelectAll()
		{
			foreach(Facet facet in Mesh.Facets)
			{
				facet.IsSelected = true;
			}

			foreach (Edge edge in Mesh.Edges)
			{
				edge.IsSelected = true;
			}

			foreach (Vertex vertex in Mesh.Vertices)
			{
				vertex.IsSelected = true;
			}
		}

		public void DeselectAll()
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
	}
}
