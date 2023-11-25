using System.Collections.Generic;

using OpenTK.Mathematics;

using ProceduralCity.Camera;
using ProceduralCity.Config;

namespace ProceduralCity.Generators
{
    /// <summary>
    /// A custom class for making generating the building sites easier.
    /// This class should only be used with the generator classes. For rendering a custumised faster datastructure is needed
    /// </summary>
    /// <param name="quadSize">The dimensions of the full area. This area will be split to 2by2 children recursively.</param>
    /// <param name="config"></param>
    class BspTree(Vector2 quadSize, IAppConfig config)
    {
        public GroundNode Root { get; private set; } = new GroundNode(Vector2.Zero, quadSize, config);

        /// <summary>
        /// Get all leafs of the tree
        /// </summary>
        /// <returns>All leafs of the tree. (Nodes that do not have any children)</returns>
        public IEnumerable<GroundNode> GetLeafs()
        {
            return GetChildren(Root);
        }

        private static IEnumerable<GroundNode> GetChildren(GroundNode node)
        {
            foreach (var child in node.Children)
            {
                if (child.Children.Count == 0)
                {
                    yield return child;
                }
                else
                {
                    foreach (var n in GetChildren(child))
                    {
                        yield return n;
                    }
                }
            }
        }

        /// <summary>
        /// Walk through the tree in a recursive manner and return only those leafs that are in the camera's view frustum.
        /// </summary>
        /// <param name="camera"></param>
        /// <returns>The leafs that are visible by the camera</returns>
        public IEnumerable<GroundNode> GetLeafsInFrustum(ICamera camera)
        {
            return GetChildrenInCameraFrustum(Root, camera);
        }

        private static IEnumerable<GroundNode> GetChildrenInCameraFrustum(GroundNode node, ICamera camera)
        {
            // Check if the current node's bounding box is outside the view frustum
            if (!camera.IsInViewFrustum(node.BoundingBox))
            {
                yield break;
            }

            foreach (var child in node.Children)
            {
                // If the child's bounding box is outside the frustum, skip it
                if (!camera.IsInViewFrustum(child.BoundingBox))
                {
                    continue;
                }

                // TODO: extract this to a method
                if (child.Children.Count == 0)
                {
                    yield return child;
                }
                else
                {
                    foreach (var n in GetChildrenInCameraFrustum(child, camera))
                    {
                        yield return n;
                    }
                }
            }
        }
    }
}
