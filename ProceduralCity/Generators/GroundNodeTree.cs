using System;
using System.Collections.Generic;

using OpenTK.Mathematics;

using ProceduralCity.Camera;
using ProceduralCity.Config;

namespace ProceduralCity.Generators
{
    /// <summary>
    /// A custom quadtree-like datastructure containing GroundNodes
    /// </summary>
    public class GroundNodeTree
    {
        public GroundNode Root { get; private set; }

        public GroundNodeTree(Vector2 quadSize, IAppConfig config)
        {
             Root = new GroundNode(Vector2.Zero, quadSize, config);
        }

        /// <summary>
        /// Get all leaves of the tree
        /// </summary>
        /// <returns>All leaves of the tree. (Nodes that do not have any children)</returns>
        public IEnumerable<GroundNode> GetLeaves()
        {
           return TraverseTree(Root);
        }

        /// <summary>
        /// Walk through the tree in a recursive manner and return only those leaves that are in the camera's view frustum.
        /// </summary>
        /// <param name="camera"></param>
        /// <returns>The leaves that are visible by the camera</returns>
        public IEnumerable<GroundNode> GetLeavesInFrustum(ICamera camera)
        {
            return TraverseTree(Root, node => camera.IsInViewFrustum(node.BoundingBox));
        }

        /// <summary>
        /// Traverses the tree starting from the specified node, applying the specified filter.
        /// </summary>
        /// <param name="startNode">The starting node of the traversal.</param>
        /// <param name="nodeFilter">A filter to determine whether a node should be included.</param>
        /// <returns>The nodes that satisfy the filter condition.</returns>
        private static IEnumerable<GroundNode> TraverseTree(GroundNode startNode, Func<GroundNode, bool> filter = null)
        {
            var stack = new Stack<GroundNode>();
            stack.Push(startNode);

            while (stack.Count > 0)
            {
                var node = stack.Pop();

                if (filter != null && !filter(node))
                {
                    continue;
                }

                if (node.Children.Count == 0)
                {
                    yield return node;
                }
                else
                {
                    foreach (var child in node.Children)
                    {
                        stack.Push(child);
                    }
                }
            }
        }
    }
}
