using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace CadEditor.Tools
{
    public class UITools
    {
        public static int WORKSPACE_SCALE_WIDTH = 80;
        public static int WORKSPACE_SCALE_HEIGHT = 80;
        public static int COLLECTION_SCALE_WIDTH = 24;
        public static int COLLECTION_SCALE_HEIGHT = 40;
        public static int PROPERTIES_SCALE_WIDTH = 24;
        public static int PROPERTIES_SCALE_HEIGHT = 38;
        public static int MENU_TOOLS_HEIGHT = 50;
        public static int GAP = 15;

        public static double ScaleBy(double value, double percentage)
        {
            return (value * percentage) / 100;
        }
    }
}
