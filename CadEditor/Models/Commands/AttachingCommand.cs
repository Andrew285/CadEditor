using CadEditor.Controllers;
using CadEditor.MeshObjects;
using System;

namespace CadEditor.Models.Commands
{
    public class AttachingCommand : BinaryCommand, ICommand
    {
        CoordinateAxisType targetAxisType;
        CoordinateAxisType attachingAxisType;
        ComplexStructure targetComplexStructure;
        ComplexStructure attachingComplexStructure;

        public AttachingCommand(ApplicationController appController,
                               ISceneObject target,
                               ISceneObject attaching,
                               CoordinateAxisType targetAxis,
                               CoordinateAxisType attachingAxis) : base(appController, target, attaching)
        {
            targetAxisType = targetAxis;
            attachingAxisType = attachingAxis;
        }

        public bool Execute()
        {
            ComplexCube targetCube, attachingCube;
            if (sceneObject is ComplexCube && extraObject is ComplexCube)
            {
                targetCube = (ComplexCube)sceneObject;
                attachingCube = (ComplexCube)extraObject;
            }
            else
            {
                throw new NotImplementedException();
            }

            targetComplexStructure = _applicationController.SceneController.Scene.GetComplexStructureByCube(targetCube);
            attachingComplexStructure = _applicationController.SceneController.Scene.GetComplexStructureByCube(attachingCube);
            if (targetComplexStructure != null && attachingComplexStructure != null)
            {
                targetComplexStructure.AttachStructure(targetCube, targetAxisType, attachingCube, attachingAxisType, attachingComplexStructure);
            }
            if (targetComplexStructure != null)
            {
                targetComplexStructure.AttachCubes(targetCube, targetAxisType, attachingCube, attachingAxisType);
            }
            else
            {
                targetComplexStructure = new ComplexStructure(targetCube, targetAxisType, attachingCube, attachingAxisType);
            }
            return true;
        }

        public void Redo()
        {
            Execute();
        }

        public void Undo()
        {

        }

        public ComplexStructure GetComplexStructure()
        {
            return targetComplexStructure;
        }
    }
}
