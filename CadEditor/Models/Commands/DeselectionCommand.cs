

namespace CadEditor.Models.Commands
{
    public class DeselectionCommand : UnaryCommand, ICommand
    {
        public DeselectionCommand(CadEditor.Scene _scene, ISceneObject obj) : base(_scene, obj)
        {
        }

        public bool Execute()
        {
            if (sceneObject != null)
            {
                return scene.Deselect(sceneObject);
            }
            else
            {
                throw new System.Exception("There is no object to desselect");
            }
        }

        public void Undo()
        {
            scene.Select(sceneObject);
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
