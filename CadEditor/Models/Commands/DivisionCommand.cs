using CadEditor.Models.Scene.MeshObjects;
using System;
using System.Collections.Generic;
using System.Xml.Linq;


namespace CadEditor.Models.Commands
{
    public class DivisionCommand : SceneCommand, ICommand
    {
        private ISceneObject divideableObject;
        private ISceneObject prevObject;
        private Vector divisionDimensions;

        public DivisionCommand(CadEditor.Scene _scene, ISceneObject obj, Vector _divDimensions) : base(_scene)
        {
            divisionDimensions = _divDimensions;
            divideableObject = obj;
        }

        public bool Execute()
        {
            if (divideableObject is IDivideable)
            {
                (divideableObject as IDivideable).Divide(divisionDimensions);
                if (divideableObject is ComplexCube)
                {
                    ComplexStructure structure = scene.GetComplexStructureByCube(divideableObject as ComplexCube);
                    if (structure != null)
                    {
                        structure.Divide(divideableObject as ComplexCube, divisionDimensions);
                    }
                }
                return true;
            }
            else
            {
                return false;
            }
        }

        public void Divide(List<ComplexCube> cubes, Vector divDimensions)
        {
            if (cubes == null || cubes.Count == 0)
            {
                return;
            }
            else
            {
                for (int i = 0; i < cubes.Count; i++)
                {
                    //if (cubes[i].IsDivided)
                    //{
                    //    cubes[i].Unite();
                    //}

                    // TODO: If cube is united local system is not appropriate

                    if (!cubes[i].IsDivided)
                    {
                        cubes[i].Divide(divDimensions);
                    }
                }
            }
        }

        public void Redo()
        {
            Execute();
        }

        public void Undo()
        {
            (divideableObject as IDivideable).Unite();
        }
    }
}
