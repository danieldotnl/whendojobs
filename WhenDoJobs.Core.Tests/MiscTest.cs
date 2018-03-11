using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WhenDoJobs.Core.Tests
{
    [TestClass]
    public class MiscTest
    {
        [TestMethod]
        public void SomeTestsWithLists()
        {
            var a = new List<int>() { 1, 2, 3 };
            Assert.AreEqual(3, a.Count);

            var b = a;
            Assert.AreEqual(3, b.Count);

            b.Add(4);
            Assert.AreEqual(4, b.Count);
            Assert.AreEqual(4, a.Count);

            var c = new List<int>();
            b.ForEach(x => c.Add(x));
            Assert.AreEqual(4, c.Count);
            Assert.AreEqual(4, b.Count);
            c.Add(8);
            Assert.AreEqual(5, c.Count);
            Assert.AreEqual(4, b.Count);

        }
    }
}
