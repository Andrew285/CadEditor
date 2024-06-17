using CadEditor;
using CadEditor.Controllers;
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
        ComplexCube cube1;

        [TestInitialize]
        public void Setup()
        {
            cube1 = new ComplexCube(new Point3D(0, 0, 0), new Vector(1, 1, 1), "cube1");
        }

        [TestMethod]
        public void VertexChangeTest()
        {
            ComplexCube c1 = new ComplexCube(new Point3D(0, 0, 0), new Vector(1, 1, 1), "cube1");
            ComplexCube c2 = new ComplexCube(new Point3D(5, 0, 0), new Vector(1, 1, 1), "cube2");

            c2.Mesh.Vertices[0] = c1.Mesh.Vertices[1];
            c2.UpdateObject();

            Assert.AreEqual(c2.Mesh.Edges[0].P1, c1.Mesh.Vertices[1]);
        }

        [TestMethod]
        [DataRow(1, 1, 1, 100, 200, 300)]
        public void DivideTest(int valueX, int valueY, int valueZ, int verticesAmount, int edgesAmount, int facetsAmount)
        {
            cube1.Divide(new Vector(valueX, valueY, valueZ));
            Assert.AreEqual(verticesAmount, cube1.Mesh.Vertices.Count);
            Assert.AreEqual(edgesAmount, cube1.Mesh.Edges.Count);
            Assert.AreEqual(facetsAmount, cube1.Mesh.Facets.Count);
        }
    }
}
