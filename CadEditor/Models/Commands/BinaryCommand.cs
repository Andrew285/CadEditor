using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CadEditor.Models.Commands
{
    public class BinaryCommand : UnaryCommand
    {
        protected ISceneObject extraObject;
        public BinaryCommand(CadEditor.Scene _scene, ISceneObject firstObject, ISceneObject secondObject) : base(_scene, firstObject)
        {
            extraObject = secondObject;
        }

        public ISceneObject GetExtraObject()
        {
            return extraObject;
        }
    }
}
