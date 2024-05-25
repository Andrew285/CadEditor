using SharpGL.SceneGraph.Cameras;
using SharpGL.SceneGraph;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CadEditor.Controllers
{
    public class RenderController
    {
        private Camera _camera;
        public bool DrawFacets { get; set; }

        public void SetCamera(Camera camera)
        {
            _camera = camera;
        }

        public RenderController() 
        {
        }

        public void SetCameraTarget(ISceneObject obj)
        {
            Point3D centerPoint = obj.GetCenterPoint();
            _camera.SetTarget(centerPoint.X, centerPoint.Y, centerPoint.Z);
        }
    }
}
