

using CadEditor.Controllers;

namespace CadEditor
{
    public interface ICommand
    {
        bool Execute();
        void Undo();
        void Redo();
    }

    public abstract class SceneCommand
    {
        protected ApplicationController _applicationController;

        public SceneCommand(ApplicationController applicationController)
        {
            _applicationController = applicationController;
        }
    }
}
