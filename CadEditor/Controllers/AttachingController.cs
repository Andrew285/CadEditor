using CadEditor.MeshObjects;

namespace CadEditor.Controllers
{
    public class AttachingController
    {
        private ComplexCube _attachingCube;
        private ComplexCube _targetCube;
        private CoordinateAxisType _attachingAxisType;
        private CoordinateAxisType _targetAxisType;
        public AxisSystem AttachingAxisSystem { get; set; }

        public void SetAttachingCube(ISceneObject attachingCube)
        {
            _attachingCube = (ComplexCube)attachingCube;
        }

        public void SetTargetCube(ISceneObject targetCube)
        {
            _targetCube = (ComplexCube)targetCube;
        }

        public void SetAttachingAxisType(CoordinateAxisType type)
        {
            _attachingAxisType = type;
        }

        public void SetTargetAxisType(CoordinateAxisType type)
        {
            _targetAxisType = type;
        }

        public ComplexCube GetAttachingCube()
        {
            return _attachingCube;
        }

        public ComplexCube GetTargetCube()
        {
            return _targetCube;
        }

        public CoordinateAxisType GetAttachingAxisType()
        {
            return _attachingAxisType;
        }

        public CoordinateAxisType GetTargetAxisType()
        {
            return _targetAxisType;
        }

        public void ClearAttachingCube()
        {
            _attachingCube = null;
        }

        public void ClearTargetCube()
        {
            _targetCube = null;
        }

        public AxisSystem InitializeAttachingAxes(MeshObject3D obj)
        {
            AttachingAxisSystem = new AxisSystem();
            AttachingAxisSystem.AxisLength = 5.0f;
            for (int i = 0; i < obj.Mesh.Facets.Count; i++)
            {
                if (!obj.Mesh.attachedFacets.Contains(i))
                {
                    AttachingAxisSystem.CreateAxis(obj.Mesh.Facets[i].AxisType, obj.Mesh.Facets[i].GetCenterPoint());
                }
            }

            return AttachingAxisSystem;
        }

    }
}
