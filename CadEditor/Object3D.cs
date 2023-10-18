using SharpGL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CadEditor
{
    public abstract class Object3D
    {
        protected OpenGL GL { get; set; }
        public bool IsSelected { get; set; }
        public bool Movable { get; set; } = true;

        public Point3D CenterPoint { get; set; }

        public Object3D ParentObject { get; set; }

        public abstract void Draw();

        public abstract void Select();
        public abstract void Deselect();
        public abstract void Move(Vector moveVector);
    }
}
