using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;

using System.Runtime.InteropServices;
using SharpGL.SceneGraph;
using SharpGL;
using SharpGL.SceneGraph.Core;

namespace CadEditor
{
    public class CustomCube: SceneElement
    {
        private OpenGL gl;
		private Mesh mesh;
		private const int facetsAmount = 6;
		private const int verticesAmount = 8;

		public Mesh Mesh
		{
			get { return mesh; }
		}

		public CustomCube(OpenGL _gl)
		{
			gl = _gl;
			mesh = new Mesh();
			mesh.Facets = new Facet[facetsAmount]
			{
				new Facet(new Vertex[]
				{
					new Vertex(-1.0f, -1.0f, 1.0f),
					new Vertex(1.0f, -1.0f, 1.0f),
					new Vertex(1.0f, 1.0f, 1.0f),
					new Vertex(-1.0f, 1.0f, 1.0f)
				}),
				new Facet(new Vertex[]
				{
					new Vertex(-1.0f, -1.0f, -1.0f),
					new Vertex(-1.0f, 1.0f, -1.0f),
					new Vertex(1.0f, 1.0f, -1.0f),
					new Vertex(1.0f, -1.0f, -1.0f)
				}),
				new Facet(new Vertex[]
				{
					new Vertex(-1.0f, 1.0f, -1.0f),
					new Vertex(-1.0f, 1.0f, 1.0f),
					new Vertex(1.0f, 1.0f, 1.0f),
					new Vertex(1.0f, 1.0f, -1.0f)
				}),
				new Facet(new Vertex[]
				{
					new Vertex(-1.0f, -1.0f, -1.0f),
					new Vertex(1.0f, -1.0f, -1.0f),
					new Vertex(1.0f, -1.0f, 1.0f),
					new Vertex(-1.0f, -1.0f, 1.0f)
				}),
				new Facet(new Vertex[]
				{
					new Vertex(1.0f, -1.0f, -1.0f),
					new Vertex(1.0f, 1.0f, -1.0f),
					new Vertex(1.0f, 1.0f, 1.0f),
					new Vertex(1.0f, -1.0f, 1.0f)
				}),
				new Facet(new Vertex[]
				{
					new Vertex(-1.0f, -1.0f, -1.0f),
					new Vertex(-1.0f, -1.0f, 1.0f),
					new Vertex(-1.0f, 1.0f, 1.0f),
					new Vertex(-1.0f, 1.0f, -1.0f)
				})
			};
		}

		public void Draw()
		{
			gl.Begin(OpenGL.GL_QUADS);
			for (int i = 0; i < mesh.Facets.Length; i++)
			{
				Facet currectFacet = mesh.Facets[i];
				if (currectFacet.IsSelected)
				{
					gl.Color(currectFacet.SelectedColor);
				}
				else
				{
					gl.Color(currectFacet.NonSelectedColor);
				}

				gl.Vertex(currectFacet.Vertices[0].X, currectFacet.Vertices[0].Y, currectFacet.Vertices[0].Z);
				gl.Vertex(currectFacet.Vertices[1].X, currectFacet.Vertices[1].Y, currectFacet.Vertices[1].Z);
				gl.Vertex(currectFacet.Vertices[2].X, currectFacet.Vertices[2].Y, currectFacet.Vertices[2].Z);
				gl.Vertex(currectFacet.Vertices[3].X, currectFacet.Vertices[3].Y, currectFacet.Vertices[3].Z);
			}
			gl.End();
			gl.Flush();
		}
    }
}
