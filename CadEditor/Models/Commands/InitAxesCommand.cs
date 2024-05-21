using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CadEditor.Models.Commands
{
    public class InitAxesCommand : UnaryCommand, ICommand
    {
        public InitAxesCommand(CadEditor.Scene _scene, ISceneObject obj) : base(_scene, obj)
        {
        }

        public bool Execute()
        {
            if (sceneObject != null)
            {
                return scene.CreateAxes(sceneObject);
            }
            else
            {
                throw new Exception("There is no object to create axes around");
            }
        }

        public void Redo()
        {
            throw new NotImplementedException();
        }

        public void Undo()
        {
            scene.DeleteSelectingCoordAxes();
        }
    }
}
