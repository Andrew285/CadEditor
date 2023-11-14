using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CadEditor
{
    public static class WindowHandler
    {
        public static double MouseX { get; set; } = 0;
        public static double MouseY { get; set; } = 0;
        public static bool IsMiddleButtonPressed { get; set; } = false;
    }
}
