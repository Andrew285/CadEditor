using CadEditor.Controllers;
using System;

namespace CadEditor.Models.Commands
{
    public class DeleteAxesCommand : UnaryCommand, ICommand
    {
        public DeleteAxesCommand(ApplicationController appController, ISceneObject obj) : base(appController, obj)
        {
        }

        public bool Execute()
        {
            if (sceneObject != null)
            {
                return _applicationController.SceneController.DeleteSelectingCoordAxes();
            }
            else
            {
                throw new Exception("There is no object to create axes around");
            }
        }

        public void Redo()
        {
            Execute();
        }

        public void Undo()
        {
            _applicationController.SceneController.CreateAxesAround(sceneObject);
        }
    }
}
