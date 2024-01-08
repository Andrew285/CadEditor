using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CadEditor
{
    public class ComplexStructureController
    {
        public static ComplexStructureController Instance { get; private set; }
        private static List<ComplexStructure> structures;

        public static ComplexStructureController GetInstance()
        {
            if (Instance == null)
            {
                Instance = new ComplexStructureController();
                structures = new List<ComplexStructure>();
            }

            return Instance;
        }

        public ComplexStructure Add(ComplexCube attachingCube, ComplexCube targetCube)
        {
            ComplexStructure targetStructure = GetStructureOf(targetCube);

            if (targetStructure != null)
            {
                targetStructure.AddCube(attachingCube);
                return targetStructure;
            }
            else
            {
                ComplexStructure newStructure = new ComplexStructure();
                structures.Add(newStructure);
                newStructure.AddCube(targetCube);
                newStructure.AddCube(attachingCube);
                return newStructure;
            }
        }

        public ComplexStructure GetStructureOf(ComplexCube cube)
        {
            foreach (ComplexStructure structure in structures)
            {
                if (structure.Contains(cube))
                {
                    return structure;
                }
            }

            return null;
        }

        public bool Contains(ComplexStructure structure)
        {
            return structures.Contains(structure);
        }
    }
}
