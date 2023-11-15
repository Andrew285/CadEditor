using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CadEditor.Graphics
{
    public class MouseController
    {
        public static double X { get; set; } = 0;
        public static double Y { get; set; } = 0;

        public static bool IsMiddleButtonPressed { get; set; } = false;

        public static float SENSITIVITY = 0.5f;

        public static double GetHorizontalAngle(int x)
        {
            return (x - MouseController.X) * MouseController.SENSITIVITY;

        }

        public static double GetVerticalAngle(int y)
        {
            return (y - MouseController.Y) * MouseController.SENSITIVITY;

        }
    }
}
