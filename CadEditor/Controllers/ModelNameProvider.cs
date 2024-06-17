using CadEditor.Models.Scene;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CadEditor.Controllers
{
    public class ModelNameProvider: INameProvider
    {
        private static ModelNameProvider instance;
        private static int cubeCount = 1;
        private static int structureCount = 1;

        public static ModelNameProvider GetInstance()
        {
            if (instance == null)
            {
                instance = new ModelNameProvider();
            }
            return instance;
        }

        public void GetValuesOf(List<ISceneObject> objects)
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

        public string GetNextName(ModelTypes type)
        {
            int currentIndex = 0;
            string currentBaseName = "None";
            if (type == ModelTypes.COMPLEX_CUBE)
            {
                currentIndex = cubeCount;
                currentBaseName = "Cube_";
                cubeCount++;
            }
            else if (type == ModelTypes.COMPLEX_STRUCTURE)
            {
                currentIndex = structureCount;
                currentBaseName = "ComplexStructure_";
                structureCount++;
            }

            return currentBaseName + currentIndex;
        }

        public void ClearAll()
        {
            cubeCount = 1;
            structureCount = 1;
        }
    }
}
