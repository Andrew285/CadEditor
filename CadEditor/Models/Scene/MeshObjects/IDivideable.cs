﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CadEditor
{
    public interface IDivideable: ISceneObject
    {
        bool IsDivided { get; set; }

        void Divide(Vector v);

        void Unite();
    }
}
