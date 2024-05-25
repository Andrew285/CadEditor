
using CadEditor.MeshObjects;

namespace CadEditor.Controllers
{
    public class ApplicationController
    {
        private CubeController _cubeController;
        private Scene _scene;
        private SceneCollection _sceneCollection;

        public ApplicationController()
        {
            INameProvider nameProvider = new ModelNameProvider();
            _cubeController = new CubeController(nameProvider);
        }

        public void SetScene(Scene scene)
        {
            _scene = scene;
        }

        public void SetSceneCollection(SceneCollection sceneCollection)
        {
            _sceneCollection = sceneCollection;
        }

        public ComplexCube AddNewCubeElement()
        {
            ComplexCube cube = _cubeController.Create(new Point3D(0, 0, 0), new Vector(1, 1, 1));
            _scene.AddObject(cube);
            _sceneCollection.AddCube(cube);
            return cube;
        }
    }
}
