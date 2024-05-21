

namespace CadEditor.Models.Commands
{
    public class SelectionCommand : UnaryCommand, ICommand
    {
        public SelectionCommand(CadEditor.Scene _scene, ISceneObject obj) : base(_scene, obj)
        {
        }

        public bool Execute()
        {
            if (sceneObject != null)
            {
                return scene.Select(sceneObject);
            }
            else
            {
                throw new System.Exception("There is no object to select");
            }
        }

        public void Undo()
        {
            scene.Deselect(sceneObject);
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
