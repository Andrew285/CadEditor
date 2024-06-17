using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CadEditor
{
    public class MouseController
    {
        public int X { get; set; } = 0;
        public int Y { get; set; } = 0;

        public bool IsMiddleButtonPressed { get; set; } = false;

        public float Sensitivity = 0.9f;

        public double GetHorizontalAngle(int x)
        {
            return (x - X) * Sensitivity;

        }

        public double GetVerticalAngle(int y)
        {
            return (y - Y) * Sensitivity;

        }

        public void UpdateMousePosition(int x, int y)
        {
            X = x;
            Y = y;
        }
    }
}
