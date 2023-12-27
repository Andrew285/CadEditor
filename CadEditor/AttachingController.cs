using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CadEditor
{
    public class AttachingController
    {
        public ISceneObject[] attachingObjects;

        public AttachingController()
        {
            attachingObjects = new ISceneObject[2];
        }

        public void DoAttach(ISceneObject obj)
        {
            attachingObjects[0] = obj;
        }

        public void DoDetach()
        {
            attachingObjects[0] = null;
        }

        public void DoSetTarget(ISceneObject obj)
        {
            attachingObjects[1] = obj;
        }

        public void DoNotSetTarget()
        {
            attachingObjects[1] = null;
        }

        public bool IsAttaching(ISceneObject obj)
        {
            return attachingObjects[0] == obj;
        }

        public bool IsTarget(ISceneObject obj)
        {
            return attachingObjects[1] == obj;
        }

        public bool IsEmpty()
        {
            return attachingObjects[0] == null && attachingObjects[1] == null;
        }

        public ISceneObject GetAttachingObject()
        {
            return attachingObjects[0];
        }

        public ISceneObject GetTargetObject()
        {
            return attachingObjects[1];
        }
    }
}
