using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CadEditor.Controllers
{
    public interface IObjectController
    {
    }

    public interface ICubeController: IObjectController
    {
        ComplexCube Create(Point3D position, Vector size, string name = null);
    }
}
