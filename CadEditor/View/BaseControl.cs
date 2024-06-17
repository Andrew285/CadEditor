using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CadEditor.View
{
    public interface BaseControl
    {
        void Initialize();
        void Resize(object sender, EventArgs e);
        void SetTextToControls();
    }
}
