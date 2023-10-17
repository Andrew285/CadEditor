using CadEditor.Graphics;
using SharpGL;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CadEditor.MeshObjects
{
	public interface IObject3D : IGraphics
	{
		string Name { get; set; }
		Mesh Mesh { get; set; }
		Point CenterPoint { get; set; }
	}

	public class CustomCube : IObject3D
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
			GL.PointSize(7.0f);
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

		public void Move(Vector vector)
		{
			for (int i = 0; i < Mesh.Vertices.Count; i++)
			{
				Mesh.Vertices[i].Move(vector);
			}
			CenterPoint.Move(vector);
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
}
