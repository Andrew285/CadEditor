using CadEditor.Models.Scene;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CadEditor.Controllers
{
    public interface INameProvider
    {
        string GetNextName(ModelTypes type);
    }
}
