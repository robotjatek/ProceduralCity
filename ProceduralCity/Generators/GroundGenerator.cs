using System.Collections.Generic;
using OpenTK;
using Serilog;

namespace ProceduralCity.Generators
{
    class GroundGenerator
    {
        private Vector2 _worldSize;

        public GroundGenerator(Vector2 worldSize)
        {
            _worldSize = worldSize;
        }

        public IEnumerable<GroundNode> Generate()
        {
            Log.Information("Generating ground 2D tree");
            var tree = new BspTree(_worldSize);
            SplitNode(tree.Root, maxLevel: 4);
            return tree.GetLeaves();
        }

        private void SplitNode(GroundNode node, int maxLevel)
        {
            SplitNodes(new[] { node }, 0, maxLevel);
        }

        private void SplitNodes(IEnumerable<GroundNode> nodes, int currentLevel, int maxLevel)
        {
            currentLevel++;
            if (currentLevel >= maxLevel)
            {
                return;
            }

            foreach (var node in nodes)
            {
                SplitNodes(node.Split(), currentLevel, maxLevel);
            }
        }
    }
}
