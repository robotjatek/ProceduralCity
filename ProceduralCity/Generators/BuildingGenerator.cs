using System;
using System.Collections.Generic;
using OpenTK;
using ProceduralCity.Renderer;
using Serilog;

namespace ProceduralCity.Generators
{
    class BuildingGenerator : IDisposable, IBuildingGenerator
    {
        private readonly Shader _buildingShader;
        private readonly Vector2 _areaBorder = new Vector2(3.5f, 3.5f);
        private readonly Random _random = new Random();
        private readonly Texture[] _buildingTextures;
        private readonly ILogger _logger;

        public BuildingGenerator(ILogger logger)
        {
            _buildingShader = new Shader("vs.vert", "fs.frag");

            _buildingTextures = new[]
            {
                new Texture("building/1.jpg"),
                new Texture("building/2.jpg"),
                new Texture("building/3.jpg"),
                new Texture("building/4.jpg"),
                new Texture("building/5.jpg"),
                new Texture("building/6.jpg"),
                new Texture("building/7.jpg"),
            };

            _logger = logger;
        }

        public IEnumerable<Building> GenerateBuildings(IEnumerable<GroundNode> sites)
        {
            _logger.Information("Generating buildings");

            var buildings = new List<Building>();
            foreach (var site in sites)
            {
                var position = new Vector3(site.StartPosition.X, 0, site.StartPosition.Y);
                var area = site.EndPosition - site.StartPosition - _areaBorder;
                var texture = _buildingTextures[_random.Next(_buildingTextures.Length)];

                var building = new Building(position, area, texture, _buildingShader, _random.Next(10, 40));
                buildings.Add(building);
            }

            Log.Information($"Number of buildings: {buildings.Count}");

            return buildings;
        }

        private bool disposedValue = false;
        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    _buildingShader.Dispose();
                    Array.ForEach(_buildingTextures, t => t.Dispose());
                }

                disposedValue = true;
            }
        }

        public void Dispose()
        {
            Dispose(true);
        }
    }
}
