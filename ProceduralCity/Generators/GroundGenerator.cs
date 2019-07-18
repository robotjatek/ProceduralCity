using System;
using System.Collections.Generic;
using OpenTK;
using ProceduralCity.Config;
using Serilog;

namespace ProceduralCity.Generators
{
    class GroundGenerator : IGroundGenerator
    {
        private Vector2 _worldSize;
        private readonly Random _random = new Random();
        private readonly ILogger _logger;
        private readonly IAppConfig _config;

        public GroundGenerator(IAppConfig config, ILogger logger)
        {
            _config = config;
            _logger = logger;
            _worldSize = new Vector2(config.WorldSize);
        }

        public IEnumerable<GroundNode> Generate()
        {
            _logger.Information("Generating ground 2D tree");
            var tree = new BspTree(_worldSize, _config);
            SplitNode(tree.Root, maxLevel: 10);
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
                SplitNodes(node.Split(_random), currentLevel, maxLevel);
            }
        }
    }
}
