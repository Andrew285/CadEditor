using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace CadEditor.Settings
{
    public class SceneSettings: MainSettings
    {
        private static SceneSettings instance;
        public static bool AxisCubeDrawFacets = true;
        public static bool AxisCubeDrawEdges = true;
        public static bool AxisCubeDrawVertices = false;

        public static SceneSettings GetInstance()
        {
            if (instance == null)
            {
                instance = new SceneSettings();
            }

            return instance;
        }
    }
}
