﻿using Microsoft.VisualStudio.TestTools.UnitTesting;
using Yolol.Grammar.AST.Expressions;
using Yolol.Grammar.AST.Expressions.Unary;

namespace YololEmulator.Tests.Expressions.Str
{
    [TestClass]
    public class ConstantStringTests
    {
        [TestMethod]
        public void IsConstant()
        {
            var str = new ConstantString("str");
            Assert.IsTrue(str.IsConstant);
        }
    }
}
