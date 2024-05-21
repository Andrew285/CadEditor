namespace CadEditor.Models.Commands
{
    public abstract class UnaryCommand : SceneCommand
    {
        protected ISceneObject sceneObject;

        protected UnaryCommand(CadEditor.Scene _scene, ISceneObject sceneObject) : base(_scene)
        {
            this.sceneObject = sceneObject;
        }

        public ISceneObject GetSceneObject()
        {
            return sceneObject;
        }
    }
}
