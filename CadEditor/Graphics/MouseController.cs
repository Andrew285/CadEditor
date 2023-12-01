using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CadEditor.Graphics
{
    public class MouseController
    {
        public static int X { get; set; } = 0;
        public static int Y { get; set; } = 0;

        public static bool IsMiddleButtonPressed { get; set; } = false;

        public static float Sensitivity = 0.9f;

        public static double GetHorizontalAngle(int x)
        {
            return (x - MouseController.X) * MouseController.Sensitivity;

        }

        public static double GetVerticalAngle(int y)
        {
            return (y - MouseController.Y) * MouseController.Sensitivity;

        }
    }
}
