﻿using AngouriMath;
using System;
using System.Collections.Generic;
using System.Text;
using Xunit;
using static AngouriMath.Entity;
using static AngouriMath.Entity.Set;
using static AngouriMath.MathS;
using static AngouriMath.MathS.Sets;
namespace UnitTests.Core.Sets
{
    public class CSetAndCSet
    {
        private readonly Set A = new ConditionalSet("x", "x > 0");
        private readonly Set A1 = new ConditionalSet("y", "y > 0");
        private readonly Set B = new ConditionalSet("x", "x xor true");
        private readonly Set C = new ConditionalSet("x", "x5 - x - 1 = 0");
        private readonly Set D = new ConditionalSet("x", "x < 0");

        private void Test(Set actual, ConditionalSet expected)
        {
            var csetAct = Assert.IsType<ConditionalSet>(actual.InnerSimplified);
            Assert.Equal(expected, csetAct);
        }

        private void TestArb(Entity actual, Entity expected)
        {
            Assert.Equal(expected, actual);
        }

        [Fact] public void VarDoesntMatter1() => Test(A, new("y", "y > 0")); // { x | f(x) } == { y | f(y) }
        [Fact] public void VarDoesntMatter2() => Test(B, new("y", "y xor true"));

        [Fact] public void Union1() => Test(A.Unite(A1), new("x", "x > 0"));
        [Fact] public void Union2() => Test(A1.Unite(A), new("x", "x > 0"));
        [Fact] public void Union3() => Test(A.Unite(B), new("x", "x > 0 or x xor true"));
        [Fact] public void Union4() => Test(B.Unite(A), new("x", "x xor true or x > 0"));

        [Fact] public void Intersection1() => Test(A1.Intersect(A), new("x", "x > 0"));
        [Fact] public void Intersection2() => Test(A.Intersect(A1), new("x", "x > 0"));
        [Fact] public void Intersection3() => TestArb(A.Intersect(D).Simplify(), Set.Empty);
        [Fact] public void Intersection4() => TestArb(D.Intersect(A).Simplify(), Set.Empty);
    }
}
