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

        public static void GetValuesOf(List<ISceneObject> objects)
        {
            ClearAll();

            foreach (ISceneObject obj in objects)
            {
                if (obj is ComplexCube)
                {
                    cubeCount++;
                }
                else if (obj is ComplexStructure)
                {
                     structureCount++;

                    foreach (ComplexCube cube in ((ComplexStructure)obj).GetCubes())
                    {
                        cubeCount++;
                    }
                }
            }
        }

        public static string GetNextCubeName()
        {
            return "Cube_" + cubeCount++;
        }

        public static string GetNextStructureName()
        {
            return "ComplexStructure_" + structureCount++;
        }

        public static void ClearAll()
        {
            cubeCount = 1;
            structureCount = 1;
        }
    }
}
