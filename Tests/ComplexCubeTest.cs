using CadEditor;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SharpGL.VertexBuffers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tests
{
    [TestClass]
    public class ComplexCubeTest
    {
        [TestMethod]
        public void VertexChangeTest()
        {
            ComplexCube c1 = new ComplexCube(new Point3D(0, 0, 0), new Vector(1, 1, 1), "cube1");
            ComplexCube c2 = new ComplexCube(new Point3D(5, 0, 0), new Vector(1, 1, 1), "cube2");

            c2.Mesh.Vertices[0] = c1.Mesh.Vertices[1];
            c2.UpdateMesh();

            Assert.AreEqual(c2.Mesh.Edges[0].P1, c1.Mesh.Vertices[1]);
        }
    }
}
