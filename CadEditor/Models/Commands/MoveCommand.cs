using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CadEditor.Models.Commands
{
    public class MoveCommand : UnaryCommand, ICommand
    {
        private Vector movingVector;

        public MoveCommand(CadEditor.Scene _scene, ISceneObject obj, Vector _movingVector) : base(_scene, obj)
        {
            movingVector = _movingVector;
        }

        public bool Execute()
        {
            sceneObject.Move(movingVector);
            return true;
        }

        public void Redo()
        {
            Execute();
        }

        public void Undo()
        {
            sceneObject.Move((-1) * movingVector);
        }
    }
}
