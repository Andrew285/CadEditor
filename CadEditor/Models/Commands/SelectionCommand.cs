

using CadEditor.Controllers;

namespace CadEditor.Models.Commands
{
    public class SelectionCommand : UnaryCommand, ICommand
    {
        public SelectionCommand(ApplicationController appController, ISceneObject obj) : base(appController, obj)
        {
        }

        public bool Execute()
        {
            if (sceneObject != null)
            {
                return _applicationController.SceneController.Scene.Select(sceneObject);
            }
            else
            {
                throw new System.Exception("There is no object to select");
            }
        }

        public void Undo()
        {
            _applicationController.SceneController.Scene.Deselect(sceneObject);
        }

        public ISceneObject GetSelectedObject()
        {
            return sceneObject;
        }

        public void Redo()
        {
            throw new System.NotImplementedException();
        }
    }
}
