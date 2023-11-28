using CadEditor.Graphics;
using SharpGL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CadEditor
{
    public class SceneGrid : IDrawable
    {
        private List<Line> grid;
        public int LineAmount { get; set; } = 200;
        public int LineLength { get; set; } = 50;


        public SceneGrid()
        {
            grid = new List<Line>();
            for (int i = 0; i < LineAmount; i++)
            {
                grid.Add(new Line(new Point3D(LineLength, 0, 1 * i), new Point3D(-LineLength, 0, 1 * i)));
                grid.Add(new Line(new Point3D(LineLength, 0, -1 * i), new Point3D(-LineLength, 0, -1 * i)));
                grid.Add(new Line(new Point3D(1 * i, 0, LineLength), new Point3D(1 * i, 0, -LineLength)));
                grid.Add(new Line(new Point3D(-1 * i, 0, LineLength), new Point3D(-1 * i, 0, -LineLength)));
            }

            foreach(Line line in grid)
            {
                line.LineWidth = 0.1f;
            }
        }

        public void Draw()
        {
            OpenGL gl = GraphicsGL.GL;

            for (int i = 0; i < LineAmount; i++)
            {
                grid[i].Draw();
            }
        }
    }
}
