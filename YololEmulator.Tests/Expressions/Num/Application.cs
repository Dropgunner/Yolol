﻿using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace YololEmulator.Tests.Expressions.Num
{
    [TestClass]
    public class Application
    {
        [TestMethod]
        public void Constant()
        {
            var result = TestExecutor.Execute("a = ABS(-9)");

            var a = result.GetVariable("a");
            Assert.AreEqual(9, a.Value.Number);
        }

        [TestMethod]
        public void Variable()
        {
            var result = TestExecutor.Execute("a = -7 b = ABS(a)");

            var a = result.GetVariable("a");
            var b = result.GetVariable("b");

            Assert.AreEqual(-7, a.Value.Number);
            Assert.AreEqual(7, b.Value.Number);
        }

        [TestMethod]
        public void Abs()
        {
            var result = TestExecutor.Execute("a = ABS(-9)");

            var a = result.GetVariable("a");
            Assert.AreEqual(9, a.Value.Number);
        }

        [TestMethod]
        public void Sqrt()
        {
            var result = TestExecutor.Execute("a = SQRT(9)");

            var a = result.GetVariable("a");
            Assert.AreEqual(3, a.Value.Number);
        }

        [TestMethod]
        public void Sin()
        {
            var result = TestExecutor.Execute("a = SIN(90)");

            var a = result.GetVariable("a");
            Assert.AreEqual(1, a.Value.Number);
        }

        [TestMethod]
        public void ArcSin()
        {
            var result = TestExecutor.Execute("a = ARCSIN(1)");

            var a = result.GetVariable("a");
            Assert.AreEqual(90, a.Value.Number);
        }

        [TestMethod]
        public void Cos()
        {
            var result = TestExecutor.Execute("a = COS(90)");

            var a = result.GetVariable("a");
            Assert.AreEqual(0, a.Value.Number);
        }

        [TestMethod]
        public void ArcCos()
        {
            var result = TestExecutor.Execute("a = ARCCOS(0)");

            var a = result.GetVariable("a");
            Assert.AreEqual(90, a.Value.Number);
        }

        [TestMethod]
        public void Tan()
        {
            var result = TestExecutor.Execute("a = TAN(45)");

            var a = result.GetVariable("a");
            Assert.AreEqual(1, a.Value.Number);
        }

        [TestMethod]
        public void ArcTan()
        {
            var result = TestExecutor.Execute("a = ARCTAN(1)");

            var a = result.GetVariable("a");
            Assert.AreEqual(45, a.Value.Number);
        }
    }
}