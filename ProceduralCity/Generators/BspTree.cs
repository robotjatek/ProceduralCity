using System.Collections.Generic;
using OpenTK.Mathematics;

using ProceduralCity.Config;

namespace ProceduralCity.Generators
{
    class BspTree
    {
        public GroundNode Root { get; private set; }

        public BspTree(Vector2 quadSize, IAppConfig config)
        {
            Root = new GroundNode(Vector2.Zero, quadSize, config);
        }

        public IEnumerable<GroundNode> GetLeaves()
        {
            return GetChildren(Root);
        }

        private IEnumerable<GroundNode> GetChildren(GroundNode node)
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
    }
}
