using System;

namespace CadEditor.Models.Commands
{
    public class DeleteAxesCommand : UnaryCommand, ICommand
    {
        public DeleteAxesCommand(CadEditor.Scene _scene, ISceneObject obj) : base(_scene, obj)
        {
        }

        public bool Execute()
        {
            if (sceneObject != null)
            {
                return scene.DeleteSelectingCoordAxes();
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
            scene.CreateAxes(sceneObject);
        }
    }
}
