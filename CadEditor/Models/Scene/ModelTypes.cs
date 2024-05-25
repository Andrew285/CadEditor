using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CadEditor.Models.Scene
{
    public enum ModelTypes
    {
        COMPLEX_CUBE,
        COMPLEX_STRUCTURE,
    }

    public enum ModelPartTypes 
    {
        VERTEX,
        EDGE,
        FACET
    }

    public enum ModelColorMode
    {
        SELECTED,
        NON_SELECTED
    }
}
