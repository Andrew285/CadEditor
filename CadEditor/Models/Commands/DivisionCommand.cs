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
                    ComplexStructure structure = ComplexStructureController.GetInstance().GetStructureOf(divideableObject as ComplexCube);
                    if (structure != null)
                    {
                        List<ComplexCube> attachedCubesByX = structure.GetAttachedCubesByAxis(divideableObject as ComplexCube, MeshObjects.CoordinateAxis.X);
                        Divide(attachedCubesByX, new Vector(1, divisionDimensions[0], 1));

                        List<ComplexCube> attachedCubesByY = structure.GetAttachedCubesByAxis(divideableObject as ComplexCube, MeshObjects.CoordinateAxis.Y);
                        Divide(attachedCubesByY, new Vector(divisionDimensions[1], 1, 1));

                        List<ComplexCube> attachedCubesByZ = structure.GetAttachedCubesByAxis(divideableObject as ComplexCube, MeshObjects.CoordinateAxis.Z);
                        Divide(attachedCubesByZ, new Vector(1, 1, divisionDimensions[2]));
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
                    cubes[i].Divide(divDimensions);
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
