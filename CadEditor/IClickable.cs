using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CadEditor
{
    public interface IClickable
    {
        bool IsClicked { get; set; }
        Object3D GetClickableArea(double tolerance);
    }
}
