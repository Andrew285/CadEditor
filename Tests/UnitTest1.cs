using CadEditor;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using SharpGL;

namespace Tests
{
    [TestClass]
    public class UnitTest1
    {
        [TestMethod]
        public void IsPointOnLine()
        {
            Vertex firstVertex = new Vertex(-1, -1, 1);
            Vertex secondVertex = new Vertex(1, -1, 1);
            Edge edge = new Edge(firstVertex, secondVertex);

            Assert.IsTrue(edge.Contains(new Vertex(-0.9, -1, 1)));
            Assert.IsTrue(edge.Contains(new Vertex(0, -1, 1)));
            Assert.IsTrue(edge.Contains(new Vertex(0.9, -1, 1)));
            Assert.IsTrue(edge.Contains(new Vertex(0.9, -1, 1)));

        }

        [TestMethod]
        public void IsPointNotOnLine()
        {
            Vertex firstVertex = new Vertex(-1, -1, 1);
            Vertex secondVertex = new Vertex(1, -1, 1);
            Edge edge = new Edge(firstVertex, secondVertex);

            Assert.IsFalse(edge.Contains(new Vertex(1.9, -1, 1)));
            Assert.IsFalse(edge.Contains(new Vertex(2, -1, 1)));
            Assert.IsFalse(edge.Contains(new Vertex(-1.1, -1, 1)));
        }

        [TestMethod]
        public void IsPointOnFacet()
        {
            Vertex firstVertex = new Vertex(-1, -1, 1);
            Vertex secondVertex = new Vertex(1, -1, 1);
            Vertex thirdVertex = new Vertex(1, 1, 1);
            Vertex fouthVertex = new Vertex(-1, 1, 1);
            Facet facet = new Facet(new Vertex[] { firstVertex, secondVertex, thirdVertex, fouthVertex });

            Assert.IsTrue(facet.Contains(new Vertex(0, 0, 1)));
            Assert.IsTrue(facet.Contains(new Vertex(0.1, -0.1, 1)));
            Assert.IsTrue(facet.Contains(new Vertex(-0.9, 0, 1)));
            Assert.IsTrue(facet.Contains(new Vertex(-0.9, 0.9, 1)));
        }

        [TestMethod]
        public void IsPointNotOnFacet()
        {
            Vertex firstVertex = new Vertex(-1, -1, 1);
            Vertex secondVertex = new Vertex(1, -1, 1);
            Vertex thirdVertex = new Vertex(1, 1, 1);
            Vertex fouthVertex = new Vertex(-1, 1, 1);
            Facet facet = new Facet(new Vertex[] { firstVertex, secondVertex, thirdVertex, fouthVertex });

            Assert.IsFalse(facet.Contains(new Vertex(2, 0, 1)));
            Assert.IsFalse(facet.Contains(new Vertex(0, -2, 1)));
            Assert.IsFalse(facet.Contains(new Vertex(-1, 0, 1.1)));
            Assert.IsFalse(facet.Contains(new Vertex(-1.1, 0, 1)));
            Assert.IsFalse(facet.Contains(new Vertex(-0.9, 1.9, 1)));
        }
    }
}
