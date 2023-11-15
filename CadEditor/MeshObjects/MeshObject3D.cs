﻿using CadEditor.Graphics;
using SharpGL;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CadEditor.MeshObjects
{
	public class MeshObject3D : Object3D
	{
		public string Name { get; set; }
		public Mesh Mesh { get; set; }

		public Vector Size { get; set; }
		public bool DrawFacets { get; set; }

		public Color VertexSelectedColor = Color.Red;
		public Color EdgeSelectedColor = Color.Red;
		public Color FacetSelectedColor = Color.LightGray;
		public Color VertexNonSelectedColor = Color.Black;
		public Color EdgeNonSelectedColor = Color.Black;
		public Color FacetNonSelectedColor = Color.LightGray;

		public MeshObject3D(Point3D _centerPoint, Vector _size, string _cubeName)
		{
			Name = _cubeName;
			Mesh = new Mesh();
			CenterPoint = _centerPoint;

			if(_size != null)
			{
                Size = new Vector(_size.Size);
                Size[0] = (_size[0] != 0) ? (double)_size[0] : 1.0;
                Size[1] = (_size[1] != 0) ? (double)_size[1] : 1.0;
                Size[2] = (_size[2] != 0) ? (double)_size[2] : 1.0;
            }
		}

		public MeshObject3D(Mesh mesh, OpenGL _GL = null)
		{
			Name = "CubeName";
			Mesh = mesh;
		}

		public override void Draw()
		{

			//Draw Vertexes
			GraphicsGL.GL.PointSize(7.0f);
            GraphicsGL.GL.Begin(OpenGL.GL_POINTS);
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
            GraphicsGL.GL.End();
            GraphicsGL.GL.Flush();


            //Draw Edges
            GraphicsGL.GL.Begin(OpenGL.GL_LINES);
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
            GraphicsGL.GL.End();
            GraphicsGL.GL.Flush();


			//Draw Facets
			if (DrawFacets)
			{
                GraphicsGL.GL.Begin(OpenGL.GL_POLYGON);
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
                GraphicsGL.GL.End();
                GraphicsGL.GL.Flush();
			}
		}

		public override void Move(Vector vector)
		{
			for (int i = 0; i < Mesh.Vertices.Count; i++)
			{
				Mesh.Vertices[i].Move(vector);
			}
			CenterPoint.Move(vector);
		}

		public override void Select()
		{
			foreach (Line edge in Mesh.Edges)
			{
				edge.IsSelected = true;
			}
		}

		public override void Deselect()
		{
			foreach (Plane facet in Mesh.Facets)
			{
				facet.IsSelected = false;
			}

			foreach (Line edge in Mesh.Edges)
			{
				edge.IsSelected = false;
			}

			foreach (Point3D vertex in Mesh.Vertices)
			{
				vertex.IsSelected = false;
			}

		}
	}
}
