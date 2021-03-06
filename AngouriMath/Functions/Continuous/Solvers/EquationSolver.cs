/*
 * Copyright (c) 2019-2020 Angourisoft
 * 
 * Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated documentation files (the "Software"), to deal in the Software without restriction, including without limitation the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons to whom the Software is furnished to do so, subject to the following conditions:
 * 
 * The above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software.
 * 
 * THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
 */
using AngouriMath.Core;
using AngouriMath.Core.Exceptions;
using AngouriMath.Extensions;
using AngouriMath.Functions.Algebra.AnalyticalSolving;
using System;
using System.Collections.Generic;
using System.Linq;

namespace AngouriMath.Functions.Algebra
{
    using static AngouriMath.Entity.Set;
    using static Entity;
    internal static class EquationSolver
    {
        /// <summary>Solves one equation</summary>
        internal static Set Solve(Entity equation, Variable x)
        {
            var solutions = MathS.Settings.PrecisionErrorZeroRange.As(1e-12m, () =>
                MathS.Settings.FloatToRationalIterCount.As(0, () =>
                    AnalyticalEquationSolver.Solve(equation, x)
                ));

            static Entity simplifier(Entity entity) => entity.InnerSimplified;
            static Entity evaluator(Entity entity) => entity.Evaled;
            var factorizer = equation.Vars.Count() == 1 ? (Func<Entity, Entity>)evaluator : simplifier;


            if (solutions is FiniteSet finiteSet)
            {
                return finiteSet.Select(simplifier)
                    .Where(elem => elem.IsFinite && factorizer(equation.Substitute(x, elem)).IsFinite).ToSet();
            }
            else
                return solutions;
        }

        /// <summary>
        /// Solves a system of equations by solving one after another with substitution, e.g. <br/>
        /// let { x - y + a = 0, y + 2a = 0 } be a system of equations for variables { x, y } <br/>
        /// Then we first find y from the first equation, <br/>
        /// y = x + a <br/>
        /// then we substitute it to all others <br/>
        /// x + a + 2a = 0 <br/>
        /// then we find x <br/>
        /// x = -3a <br/>
        /// Then we substitute back <br/>
        /// y = -3a + a = -2a <br/>
        /// </summary>
        internal static Tensor? SolveSystem(IEnumerable<Entity> inputEquations, ReadOnlySpan<Variable> vars)
        {
            var equations = new List<Entity>(inputEquations.Select(equation => equation.InnerSimplified));
            if (equations.Count != vars.Length)
                throw new MathSException("Amount of equations must be equal to that of vars");
            int initVarCount = vars.Length;

            var res = InSolveSystem(equations, vars);
            foreach (var tuple in res)
                if (tuple.Count != initVarCount)
                    throw new AngouriBugException("InSolveSystem incorrect output");
            if (res.Count == 0)
                return null;
            var tb = new TensorBuilder(res, initVarCount);
            return tb.ToTensor();
        }

        /// <summary>Solves system of equations</summary>
        /// <param name="equations"><see cref="List{T}"/> of <see cref="Entity"/></param>
        /// <param name="vars">
        /// <see cref="List{T}"/> of <see cref="Variable"/>s,
        /// where each of them must be mentioned in at least one entity from equations
        /// </param>
        internal static List<List<Entity>> InSolveSystem(List<Entity> equations, ReadOnlySpan<Variable> vars)
        {
            var var = vars[vars.Length - 1];
            if (equations.Count == 1)
                return equations[0].InnerSimplified.SolveEquation(var) is FiniteSet els 
                       ? els.Select(sol => new List<Entity> { sol }).ToList()
                       : new();
            var result = new List<List<Entity>>();
            var replacements = new Dictionary<Variable, Entity>();
            for (int i = 0; i < equations.Count; i++)
                if (equations[i].ContainsNode(var))
                {
                    var solutionsOverVar = equations[i].SolveEquation(var);
                    equations.RemoveAt(i);
                    vars = vars.Slice(0, vars.Length - 1);

                    if (solutionsOverVar is FiniteSet sols)
                        foreach (var sol in sols)
                        foreach (var j in InSolveSystem(equations.Select(eq => eq.Substitute(var, sol)).ToList(), vars))
                        {
                            replacements.Clear();
                            for (int varid = 0; varid < vars.Length; varid++)
                                replacements.Add(vars[varid], j[varid]);
                            j.Add(sol.Substitute(replacements).InnerSimplified);
                            result.Add(j);
                        }
                    break;
                }
            return result;
        }
    }
}