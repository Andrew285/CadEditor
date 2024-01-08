using System.Collections.Generic;

namespace CadEditor
{
    public class SceneGrid : IDrawable
    {
        private List<Line> grid;
        private int lineAmount;
        private int lineLength;
        private float lineWidth;


        public SceneGrid(int _lineAmount, int _lineLength, float _lineWidth)
        {
            lineAmount = _lineAmount;
            lineLength = _lineLength;
            lineWidth = _lineWidth;

            InitLines();
        }

        private void InitLines()
        {
            grid = new List<Line>();
            for (int i = 0; i < lineAmount; i++)
            {
                grid.Add(new Line(new Point3D(lineLength, 0, 1 * i), new Point3D(-lineLength, 0, 1 * i)));
                grid.Add(new Line(new Point3D(lineLength, 0, -1 * i), new Point3D(-lineLength, 0, -1 * i)));
                grid.Add(new Line(new Point3D(1 * i, 0, lineLength), new Point3D(1 * i, 0, -lineLength)));
                grid.Add(new Line(new Point3D(-1 * i, 0, lineLength), new Point3D(-1 * i, 0, -lineLength)));
            }

            foreach (Line line in grid)
            {
                line.LineWidth = lineWidth;
            }
        }

        public void Draw()
        {
            for (int i = 0; i < lineAmount; i++)
            {
                grid[i].Draw();
            }
        }
    }
}
