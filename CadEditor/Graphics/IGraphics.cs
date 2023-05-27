using SharpGL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CadEditor.Graphics
{
    public interface ISelectable
    {
        bool IsSelected { get; set; }
        //Color SelectedColor { get; set; }
        //Color NonSelectedColor { get; set; }

        void Select();
        void Deselect();
    }

    public interface IGraphics: ISelectable
    {
        OpenGL GL { get; }

        void Draw();
        void Move(double x, double y, double z);
    }
}
