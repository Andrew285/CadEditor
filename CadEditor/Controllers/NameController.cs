using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CadEditor.Controllers
{
    public class NameController
    {
        static int cubeCount = 1;
        static int structureCount = 1;

        public static string GetNextCubeName()
        {
            return "Cube_" + cubeCount++;
        }

        public static string GetNextStructureName()
        {
            return "ComplexStructure_" + structureCount++;
        }
    }
}
