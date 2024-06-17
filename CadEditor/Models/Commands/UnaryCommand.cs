using CadEditor.Controllers;

namespace CadEditor.Models.Commands
{
    public abstract class UnaryCommand : SceneCommand
    {
        protected ISceneObject sceneObject;

        protected UnaryCommand(ApplicationController appController, ISceneObject sceneObject) : base(appController)
        {
            this.sceneObject = sceneObject;
        }

        public ISceneObject GetSceneObject()
        {
            return sceneObject;
        }
    }
}
