using CadEditor.Models.Scene;
using GeometRi;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CadEditor.Controllers
{
    public class CubeController: ICubeController
    {
        private readonly ApplicationController _applicationControler;
        private readonly INameProvider _nameProvider;
        private readonly ModelTypes _modelType = ModelTypes.COMPLEX_CUBE;

        public CubeController(ApplicationController appController, INameProvider nameProvider)
        {
            _applicationControler = appController;
            _nameProvider = nameProvider;
        }

        public ComplexCube Create(Point3D position, Vector size = null, string name = null)
        {
            Point3D cubePosition = new Point3D(0, 0, 0);
            if (position != null) cubePosition = position;

            Vector cubeSize = new Vector(1, 1, 1);
            if (size != null) cubeSize = size;

            string cubeName = "";
            if (name == null) cubeName = _nameProvider.GetNextName(_modelType);

            return new ComplexCube(cubePosition, cubeSize, cubeName);
        }

        public void UpdateRotation(ComplexCube cube, double horizontalAngle, double verticalAngle)
        {
            float xDelta = (float)horizontalAngle;
            float yDelta = (float)verticalAngle;

            cube.xRotation += xDelta * 1f;
            cube.yRotation += yDelta * 1f;

            GraphicsGL.Control.Invalidate();
        }
    }
}
