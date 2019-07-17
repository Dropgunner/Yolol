﻿using System;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using Yolol.Analysis.ControlFlowGraph.AST;
using Yolol.Analysis.Types;

namespace Yolol.Analysis.ControlFlowGraph.Extensions
{
    public static class ReductionExtensions
    {
        /// <summary>
        /// Remove blocks that cannot be reached from the entry node
        /// </summary>
        /// <param name="graph"></param>
        /// <returns></returns>
        [NotNull] public static IControlFlowGraph RemoveUnreachableBlocks([NotNull] this IControlFlowGraph graph)
        {
            var reachable = new HashSet<IBasicBlock>();
            var queue = new Queue<IBasicBlock>();

            void Mark(IBasicBlock block)
            {
                if (reachable.Add(block))
                    foreach (var edge in block.Outgoing)
                        queue.Enqueue(edge.End);
            }

            queue.Enqueue(graph.Vertices.Single(v => v.Type == BasicBlockType.Entry));
            while (queue.Count > 0)
                Mark(queue.Dequeue());

            return graph.Trim(reachable.Contains);
        }

        /// <summary>
        /// Bypass nodes which contain no statements
        /// </summary>
        /// <param name="graph"></param>
        /// <returns></returns>
        [NotNull] public static IControlFlowGraph RemoveEmptyNodes([NotNull] this IControlFlowGraph graph)
        {
            // Find all the vertices we can shortcut
            var emptyVertices = new HashSet<IBasicBlock>(
                from vertex in graph.Vertices
                where vertex.Type == BasicBlockType.Basic
                where !vertex.Statements.Any()
                where vertex.Outgoing.Count() == 1
                where vertex.Outgoing.Single().Type == EdgeType.Continue
                select vertex
            );

            // Select all edges to delete
            var edgesDelete = new HashSet<IEdge>(
                from vertex in emptyVertices
                from input in vertex.Incoming
                select input
            );

            // Select edges to create
            var edgesCreate = new HashSet<(Guid, Guid, EdgeType)>(
                from vertex in emptyVertices
                from input in vertex.Incoming
                let output = vertex.Outgoing.Single()
                select (input.Start.ID, output.End.ID, input.Type)
            );

            var g = graph;
            g = g.Trim(e => !edgesDelete.Contains(e));
            g = g.Add(edgesCreate);
            return g;
        }

        /// <summary>
        /// Removed error/continue edges which we know cannot happen due to type info
        /// </summary>
        /// <param name="graph"></param>
        /// <param name="types"></param>
        /// <returns></returns>
        [NotNull] public static IControlFlowGraph TypeDrivenEdgeTrimming([NotNull] this IControlFlowGraph graph, ITypeAssignments types)
        {
            return graph.Trim(edge => {

                // Find last statement in previous block
                var stmt = edge.Start.Statements.LastOrDefault();
                var tass = stmt as TypedAssignment;
                var err = stmt as ErrorStatement;

                // If type is unassigned we can't make a judgement
                if (tass?.Type == Execution.Type.Unassigned)
                    return true;

                if (edge.Type == EdgeType.RuntimeError)
                {
                    // If it's an error statement keep it
                    if (err != null)
                        return true;

                    // If there is no statement at all then it can't be an error
                    if (tass == null)
                        return false;

                    // Only keep edge if type has an error
                    return tass.Type.HasFlag(Execution.Type.Error);
                }
                else
                {
                    // If it's an error statement remove it
                    if (err != null)
                        return false;

                    // If there is no typed assignment we can't judge
                    if (tass == null)
                        return true;

                    return tass.Type != Execution.Type.Error;
                }
            });
        }

        /// <summary>
        /// Replaces nodes with an error statement and an error edge with an empty node and a continue edge
        /// </summary>
        /// <param name="cfg"></param>
        /// <returns></returns>
        [NotNull] public static IControlFlowGraph NormalizeErrors([NotNull] this IControlFlowGraph cfg)
        {
            var todo = new HashSet<IBasicBlock>();

            // Remove the error statements and save blocks to remove edges from
            cfg = cfg.Modify((a, b) => {

                if (!a.Statements.Any())
                    return;

                var toCopy = a.Statements;
                if ((a.Statements.Last() is ErrorStatement) && a.Outgoing.Count() == 1 && a.Outgoing.Single().Type == EdgeType.RuntimeError)
                {
                    toCopy = a.Statements.Take(a.Statements.Count() - 1);
                    todo.Add(b);
                }

                foreach (var stmt in toCopy)
                    b.Add(stmt);
            });

            // Copy graph, replacing edges as necessary
            return cfg.Modify((e, c) => {
                c(e.Start, e.End, todo.Contains(e.Start) ? EdgeType.Continue : e.Type);
            });
        }
    }
}