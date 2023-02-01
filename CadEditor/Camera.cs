using SharpGL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CadEditor
{
    public class Camera
    {
        public float rtri { get; set; }
        public float utri { get; set; }
        private OpenGL gl;

        public Camera(OpenGL _gl)
        {
            gl = _gl;
        }

        public void RotateAxisX()
        {
            gl.Rotate(utri, 1.0f, 0.0f, 0.0f);
        }

        public void RotateAxisY() 
        {
            gl.Rotate(rtri, 0.0f, 1.0f, 0.0f);
        }
    }
}
