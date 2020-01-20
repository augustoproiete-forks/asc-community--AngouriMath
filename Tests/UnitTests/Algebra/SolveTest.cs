﻿using AngouriMath;
using AngouriMath.Core;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace UnitTests
{
    [TestClass]
    public class SolveTest
    {
        public static VariableEntity x = "x";
        [TestMethod]
        public void Test1()
        {
            var eq = (x - 1) * (x - 2);
            var roots = eq.SolveNt(x);
            Assert.IsTrue(roots.Count == 2);
            var s = roots[0] + roots[1];
            Assert.IsTrue(s == 3);
        }
        [TestMethod]
        public void Test2()
        {
            var eq = MathS.Sqr(x) + 1;
            var roots = eq.SolveNt(x);
            Assert.IsTrue(roots.Count == 2);
            Assert.IsTrue(roots[0] == MathS.i);
            Assert.IsTrue(roots[1] == new Number(0, -1));
        }
        [TestMethod]
        public void Test3()
        {
            var eq = new NumberEntity(1);
            var roots = eq.SolveNt(x);
            Assert.IsTrue(roots.Count == 0);
        }
        [TestMethod]
        public void Test4()
        {
            var eq = x.Pow(2) + 2 * x + 1;
            MathS.EQUALITY_THRESHOLD = 1.0e-6;
            var roots = eq.SolveNt(x, precision: 100);
            MathS.EQUALITY_THRESHOLD = 1.0e-11;
            Assert.IsTrue(roots.Count == 1);
        }

        [TestMethod]
        public void Test5()
        {
            // solve x2 + 2x + 2
            var eq = x.Pow(2) + 2 * x + 2;
            var roots = eq.Solve("x");
            var r1 = MathS.FromString("-1 + 1i").Simplify();
            var r2 = MathS.FromString("-1 - 1i").Simplify();

            Assert.IsTrue(roots.Count == 2 &&
                ((roots[0] == r1 && roots[1] == r2) || (roots[0] == r2 && roots[1] == r1)),
            string.Format("roots: {0}, expected: [-1 - 1i, -1 + 1i]", roots));
        }

        [TestMethod]
        public void Test6()
        {
            // solve 2x2 + 4x + 2
            var eq = 2 * x.Pow(2) + 4 * x + 2;
            var roots = eq.Solve("x");
            Assert.IsTrue(roots.Count == 1 && roots[0] == -1, string.Format("roots: {0}, expected: [-1]", roots));
        }

        [TestMethod]
        public void Test7()
        {
            // solve x2 - 3x + 2
            var eq = x.Pow(2) - 3 * x + 2;
            var roots = eq.Solve("x");

            Assert.IsTrue(roots.Count == 2 &&
                ((roots[0] == 1 && roots[1] == 2) || (roots[0] == 2 && roots[1] == 1)),
                 string.Format("roots: {0}, expected: [1, 2]", roots));
        }

        [TestMethod]
        public void Test8()
        {
            // solve x3 + 3x2 + 3x + 1
            var eq = x.Pow(3) + 3 * x.Pow(2) + 3 * x + 1;
            var roots = eq.Solve("x");
            Assert.IsTrue(roots.Count == 1 && roots[0] == -1, string.Format("roots: {0}, expected: [-1]", roots));
        }

        [TestMethod]
        public void Test9()
        {
            // solve x3 - 6x2 + 11x - 6
            var eq = x.Pow(3) - 6 * x.Pow(2) + 11 * x - 6;
            var roots = eq.Solve("x");
            Assert.IsTrue(roots.Count == 3 &&
                ((roots[0] == 1 && roots[1] == 2 && roots[2] == 3) ||
                 (roots[0] == 1 && roots[1] == 3 && roots[2] == 2) ||
                 (roots[0] == 2 && roots[1] == 1 && roots[2] == 3) ||
                 (roots[0] == 2 && roots[1] == 3 && roots[2] == 1) ||
                 (roots[0] == 3 && roots[1] == 2 && roots[2] == 1) ||
                 (roots[0] == 3 && roots[1] == 1 && roots[2] == 2)),
                string.Format("roots: {0}, expected: [1, 2, 3]", roots));
        }
        [TestMethod]
        public void Test10()
        {
            
        }
    }
}
