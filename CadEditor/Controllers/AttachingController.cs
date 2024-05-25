using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CadEditor.Controllers
{
    public class AttachingController
    {
        private ComplexCube _attachingCube;
        private ComplexCube _targetCube;

        public void SetAttachingCube(ISceneObject attachingCube)
        {
            _attachingCube = (ComplexCube)attachingCube;
        }

        public void SetTargetCube(ISceneObject targetCube)
        {
            _targetCube = (ComplexCube)targetCube;
        }

        public ComplexCube GetAttachingCube()
        {
            return _attachingCube;
        }

        public ComplexCube GetTargetCube()
        {
            return _targetCube;
        }

        public void ClearAttachingCube()
        {
            _attachingCube = null;
        }

        public void ClearTargetCube()
        {
            _targetCube = null;
        }
    }
}
