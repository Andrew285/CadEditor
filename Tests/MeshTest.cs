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
    public class MeshTest
    {
        [TestMethod]
        public void MeshClone()
        {
            Mesh mesh1 = new Mesh();
            mesh1.Vertices = new List<Point3D>() { new Point3D(new Vector(0, 0, 1)),
                                                   new Point3D(new Vector(1, 0, 1)),
                                                   new Point3D(new Vector(0, 1, 1)),
                                                   new Point3D(new Vector(1, 1, 0))};

            mesh1.Edges = new List<Line3D>() { new Line3D(mesh1.Vertices[0], mesh1.Vertices[1]),
                                             new Line3D(mesh1.Vertices[1], mesh1.Vertices[2]),
                                             new Line3D(mesh1.Vertices[2], mesh1.Vertices[3]),
                                             new Line3D(mesh1.Vertices[3], mesh1.Vertices[0]),};

            Mesh mesh2 = mesh1.Clone();

            mesh1.Vertices[0][0] += 4;

            Point3D p1 = mesh1.Vertices[0];
            Point3D p2 = mesh2.Vertices[0];
            Assert.IsTrue(p1 != p2);
            Assert.IsTrue(p1[0] == 4 && p2[0] == 0);

            Line3D l1 = mesh1.Edges[0];
            Line3D l2 = mesh2.Edges[0];
            Assert.IsTrue(l1 != l2);
            Assert.IsTrue(l1.P1[0] == 4);
            Assert.IsTrue(l2.P2[0] == 0);
        }
    }
}
