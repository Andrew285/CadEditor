using CadEditor.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CadEditor.Models.Commands
{
    public class InitAxesCommand : UnaryCommand, ICommand
    {
        public InitAxesCommand(ApplicationController appController, ISceneObject obj) : base(appController, obj)
        {
        }

        public bool Execute()
        {
            if (sceneObject != null)
            {
                return _applicationController.SceneController.CreateAxesAround(sceneObject);
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
            _applicationController.SceneController.DeleteSelectingCoordAxes();
        }
    }
}
