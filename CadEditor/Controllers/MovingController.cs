using CadEditor.MeshObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CadEditor.Controllers
{
    public class MovingController
    {
        private Point3D _startMovingPoint;
        private Point3D _endMovingPoint;
        public static Vector MovingVector;
        public static CoordinateAxis ActiveMovingAxis;

        public MovingController() { }

        public Point3D GetStartMovingPoint() { return _startMovingPoint; }

        public Point3D GetEndMovingPoint() { return  _endMovingPoint; }

        public void SetStartMovingPoint(Point3D p) { _startMovingPoint = p; }

        public void SetEndMovingPoint(Point3D p) { _endMovingPoint = p; }
    }
}
