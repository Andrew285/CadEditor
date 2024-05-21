

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
        protected Scene scene;

        public SceneCommand(Scene _scene)
        {
            scene = _scene;
        }
    }
}
