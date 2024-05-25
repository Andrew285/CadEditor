using CadEditor.Models.Scene;
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
        private readonly INameProvider _nameProvider;
        private readonly ModelTypes _modelType = ModelTypes.COMPLEX_CUBE;

        public CubeController(INameProvider nameProvider)
        {
            _nameProvider = nameProvider;
        }

        public ComplexCube Create(Point3D position, Vector size, string name = null)
        {
            Point3D cubePosition = new Point3D(0, 0, 0);
            if (position != null) cubePosition = position;

            Vector cubeSize = new Vector(1, 1, 1);
            if (size != null) cubeSize = size;

            string cubeName = "";
            if (name == null) cubeName = _nameProvider.GetNextName(_modelType);

            return new ComplexCube(cubePosition, cubeSize, cubeName);
        }
    }
}
