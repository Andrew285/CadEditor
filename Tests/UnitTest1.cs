using CadEditor;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using SharpGL;
using System.Collections.Generic;

namespace Tests
{
    [TestClass]
    public class UnitTest1
    {
        [TestMethod]
        public void IsPointOnLine()
        {
            Point3D firstVertex = new Point3D(-1, -1, 1);
            Point3D secondVertex = new Point3D(1, -1, 1);
            Line edge = new Line(firstVertex, secondVertex);

            Assert.IsTrue(edge.Contains(new Point3D(-0.9, -1, 1)));
            Assert.IsTrue(edge.Contains(new Point3D(0, -1, 1)));
            Assert.IsTrue(edge.Contains(new Point3D(0.9, -1, 1)));
            Assert.IsTrue(edge.Contains(new Point3D(0.9, -1, 1)));

        }

        [TestMethod]
        public void IsPointNotOnLine()
        {
            Point3D firstVertex = new Point3D(-1, -1, 1);
            Point3D secondVertex = new Point3D(1, -1, 1);
            Line edge = new Line(firstVertex, secondVertex);

            Assert.IsFalse(edge.Contains(new Point3D(1.9, -1, 1)));
            Assert.IsFalse(edge.Contains(new Point3D(2, -1, 1)));
            Assert.IsFalse(edge.Contains(new Point3D(-1.1, -1, 1)));
        }

        [TestMethod]
        public void IsPointOnFacet()
        {
            Point3D firstVertex = new Point3D(-1, -1, 1);
            Point3D secondVertex = new Point3D(1, -1, 1);
            Point3D thirdVertex = new Point3D(1, 1, 1);
            Point3D fouthVertex = new Point3D(-1, 1, 1);
            Plane facet = new Plane(new List<Point3D> { firstVertex, secondVertex, thirdVertex, fouthVertex });

            Assert.IsTrue(facet.Contains(new Point3D(0, 0, 1)));
            Assert.IsTrue(facet.Contains(new Point3D(0.1, -0.1, 1)));
            Assert.IsTrue(facet.Contains(new Point3D(-0.9, 0, 1)));
            Assert.IsTrue(facet.Contains(new Point3D(-0.9, 0.9, 1)));
        }

        [TestMethod]
        public void IsPointNotOnFacet()
        {
            Point3D firstVertex = new Point3D(-1, -1, 1);
            Point3D secondVertex = new Point3D(1, -1, 1);
            Point3D thirdVertex = new Point3D(1, 1, 1);
            Point3D fouthVertex = new Point3D(-1, 1, 1);
            Plane facet = new Plane(new List<Point3D> { firstVertex, secondVertex, thirdVertex, fouthVertex });

            Assert.IsFalse(facet.Contains(new Point3D(2, 0, 1)));
            Assert.IsFalse(facet.Contains(new Point3D(0, -2, 1)));
            Assert.IsFalse(facet.Contains(new Point3D(-1, 0, 1.1)));
            Assert.IsFalse(facet.Contains(new Point3D(-1.1, 0, 1)));
            Assert.IsFalse(facet.Contains(new Point3D(-0.9, 1.9, 1)));
        }
    }
}
