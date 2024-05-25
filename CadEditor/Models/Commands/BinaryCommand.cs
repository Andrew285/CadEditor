using CadEditor.Controllers;

namespace CadEditor.Models.Commands
{
    public class BinaryCommand : UnaryCommand
    {
        protected ISceneObject extraObject;
        public BinaryCommand(ApplicationController appController, ISceneObject firstObject, ISceneObject secondObject) : base(appController, firstObject)
        {
            extraObject = secondObject;
        }

        public ISceneObject GetExtraObject()
        {
            return extraObject;
        }
    }
}
