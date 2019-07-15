using System;
using System.Collections.Generic;
using System.Linq;
using OpenTK;
using ProceduralCity.Config;
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
        private readonly IAppConfig _config;

        public BuildingGenerator(ILogger logger, IAppConfig config)
        {
            _config = config;
            _buildingShader = new Shader("vs.vert", "fs.frag");
            _buildingTextures = _config.BuildingTextures.Select(c => new Texture(c)).ToArray();

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

                var building = new Building(position, area, texture, _buildingShader, _random.Next(_config.MinBuildingHeight, _config.MaxBuildingHeight));
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
