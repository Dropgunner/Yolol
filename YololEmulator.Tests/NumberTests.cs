﻿using Microsoft.VisualStudio.TestTools.UnitTesting;
using Yolol.Execution;

namespace YololEmulator.Tests
{
    [TestClass]
    public class NumberTests
    {
        [TestMethod]
        public void TruncateOnConstruction()
        {
            var n = new Number(1.234567m);

            Assert.AreEqual(1.234m, n.Value);
            Assert.AreEqual("1.234", n.ToString());
        }

        [TestMethod]
        public void Equal()
        {
            var a = new Number(3.1415m);

            // ReSharper disable EqualExpressionComparison
#pragma warning disable CS1718 // Comparison made to same variable
            Assert.IsTrue(a == a);
            Assert.IsFalse(a != a);
#pragma warning restore CS1718 // Comparison made to same variable
            // ReSharper restore EqualExpressionComparison

            Assert.IsTrue(a.Equals(a));
            Assert.IsTrue(a.Equals((object)a));

            Assert.AreEqual(a.GetHashCode(), a.GetHashCode());
        }

        [TestMethod]
        public void NotEqual()
        {
            var a = new Number(3.1415m);
            var b = new Number(1);

            // ReSharper disable EqualExpressionComparison
#pragma warning disable CS1718 // Comparison made to same variable
            Assert.IsFalse(a == b);
            Assert.IsTrue(a != b);
#pragma warning restore CS1718 // Comparison made to same variable
            // ReSharper restore EqualExpressionComparison

            Assert.IsFalse(a.Equals(b));
            Assert.IsFalse(a.Equals((object)b));

            Assert.AreNotEqual(a.GetHashCode(), b.GetHashCode());
        }
    }
}
