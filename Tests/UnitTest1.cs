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
            Point firstVertex = new Point(-1, -1, 1);
            Point secondVertex = new Point(1, -1, 1);
            Line edge = new Line(firstVertex, secondVertex);

            Assert.IsTrue(edge.Contains(new Point(-0.9, -1, 1)));
            Assert.IsTrue(edge.Contains(new Point(0, -1, 1)));
            Assert.IsTrue(edge.Contains(new Point(0.9, -1, 1)));
            Assert.IsTrue(edge.Contains(new Point(0.9, -1, 1)));

        }

        [TestMethod]
        public void IsPointNotOnLine()
        {
            Point firstVertex = new Point(-1, -1, 1);
            Point secondVertex = new Point(1, -1, 1);
            Line edge = new Line(firstVertex, secondVertex);

            Assert.IsFalse(edge.Contains(new Point(1.9, -1, 1)));
            Assert.IsFalse(edge.Contains(new Point(2, -1, 1)));
            Assert.IsFalse(edge.Contains(new Point(-1.1, -1, 1)));
        }

        [TestMethod]
        public void IsPointOnFacet()
        {
            Point firstVertex = new Point(-1, -1, 1);
            Point secondVertex = new Point(1, -1, 1);
            Point thirdVertex = new Point(1, 1, 1);
            Point fouthVertex = new Point(-1, 1, 1);
            Plane facet = new Plane(new List<Point> { firstVertex, secondVertex, thirdVertex, fouthVertex });

            Assert.IsTrue(facet.Contains(new Point(0, 0, 1)));
            Assert.IsTrue(facet.Contains(new Point(0.1, -0.1, 1)));
            Assert.IsTrue(facet.Contains(new Point(-0.9, 0, 1)));
            Assert.IsTrue(facet.Contains(new Point(-0.9, 0.9, 1)));
        }

        [TestMethod]
        public void IsPointNotOnFacet()
        {
            Point firstVertex = new Point(-1, -1, 1);
            Point secondVertex = new Point(1, -1, 1);
            Point thirdVertex = new Point(1, 1, 1);
            Point fouthVertex = new Point(-1, 1, 1);
            Plane facet = new Plane(new List<Point> { firstVertex, secondVertex, thirdVertex, fouthVertex });

            Assert.IsFalse(facet.Contains(new Point(2, 0, 1)));
            Assert.IsFalse(facet.Contains(new Point(0, -2, 1)));
            Assert.IsFalse(facet.Contains(new Point(-1, 0, 1.1)));
            Assert.IsFalse(facet.Contains(new Point(-1.1, 0, 1)));
            Assert.IsFalse(facet.Contains(new Point(-0.9, 1.9, 1)));
        }
    }
}
