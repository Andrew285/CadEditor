using CadEditor.MeshObjects;

namespace CadEditor.Controllers
{
    public class SceneController
    {
        public Scene Scene { get; set; }
        private ISceneObject _previousSelectedObject;
        private ISceneObject _selectedObject;
        private AxisCube _selectedAxisCube;
        private AxisSystem AxisSystem;

        public SceneController()
        {
            Scene = new Scene();
        }

        public ISceneObject GetPreviousSelectedObject()
        {
            return _previousSelectedObject;
        }

        public void SetPreviousSelectedObject(ISceneObject previousSelectedObject)
        {
            _previousSelectedObject = previousSelectedObject;
        }

        public ISceneObject GetSelectedObject()
        {
            return _selectedObject;
        }

        public void SetSelectedObject(ISceneObject selectedObject)
        {
            _selectedObject = selectedObject;
        }

        public AxisCube GetAxisCube()
        {
            return _selectedAxisCube;
        }

        public void SetAxisCube(AxisCube axisCube)
        {
            _selectedAxisCube = axisCube;
        }

        public bool CreateAxesAround(ISceneObject obj)
        {
            if (AxisSystem != null) Scene.Remove(AxisSystem);
            AxisSystem = new AxisSystem(obj.GetCenterPoint(), RenderController.selectingRay);
            Scene.Insert(0, AxisSystem);
            return true;
        }

        public bool DeleteSelectingCoordAxes()
        {
            if (AxisSystem != null) Scene.Remove(AxisSystem);
            return true;
        }

        public void UpdateScene()
        {
            Scene.DeselectAll();
            DeleteSelectingCoordAxes();
        }

        public void DeleteCompletely(ISceneObject obj)
        {
            Scene.Remove(obj);
            DeleteSelectingCoordAxes();
        }

        public void MoveCoordinateAxes(Vector vector)
        {
            AxisSystem.Move(vector);
        }
    }
}
