

using CadEditor.Controllers;

namespace CadEditor.Models.Commands
{
    public class DeselectionCommand : UnaryCommand, ICommand
    {
        public DeselectionCommand(ApplicationController appController, ISceneObject obj) : base(appController, obj)
        {
        }

        public bool Execute()
        {
            if (sceneObject != null)
            {
                return _applicationController.SceneController.Scene.Deselect(sceneObject);
            }
            else
            {
                throw new System.Exception("There is no object to desselect");
            }
        }

        public void Undo()
        {
            _applicationController.SceneController.Scene.Select(sceneObject);
        }

        public void SetSelectedObject(ISceneObject obj)
        {
            sceneObject = obj;
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
