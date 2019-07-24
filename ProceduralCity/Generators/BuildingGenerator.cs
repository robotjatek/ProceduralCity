using System;
using System.Collections.Generic;
using System.Linq;
using OpenTK;
using ProceduralCity.Buildings;
using ProceduralCity.Config;
using ProceduralCity.Renderer;
using Serilog;

namespace ProceduralCity.Generators
{
    class BuildingGenerator : IBuildingGenerator
    {
        enum BuildingType
        {
            Simple,
            Tower,
            //  Blocky
        }

        private readonly Shader _buildingShader;
        private readonly Vector2 _areaBorder;
        private readonly Random _random = new Random();
        private readonly Texture[] _buildingTextures;
        private readonly ILogger _logger;
        private readonly IAppConfig _config;

        public BuildingGenerator(ILogger logger, IAppConfig config)
        {
            _config = config;
            _buildingShader = new Shader("vs.vert", "fs.frag");
            _buildingTextures = _config.BuildingTextures.Select(c => new Texture(c)).ToArray();
            _areaBorder = new Vector2(_config.AreaBorderSize);
            _logger = logger;
        }

        public IEnumerable<IBuilding> GenerateBuildings(IEnumerable<GroundNode> sites)
        {
            _logger.Information("Generating buildings");

            var buildings = new List<IBuilding>();
            foreach (var site in sites)
            {
                var position = new Vector3(site.StartPosition.X + _areaBorder.X, 0, site.StartPosition.Y + _areaBorder.Y);
                var area = site.EndPosition - site.StartPosition - _areaBorder;
                var texture = _buildingTextures[_random.Next(_buildingTextures.Length)];
                var building = CreateRandomBuilding(position, area, texture);
                buildings.Add(building);
            }

            _logger.Information($"Number of buildings: {buildings.Count}");

            return buildings;
        }

        private IBuilding CreateRandomBuilding(Vector3 position, Vector2 area, Texture texture)
        {
            var type = (BuildingType)_random.Next(Enum.GetValues(typeof(BuildingType)).Length);
            var height = _random.Next(_config.MinBuildingHeight, _config.MaxBuildingHeight);

            switch (type)
            {
                case BuildingType.Simple:
                    return new Building(position, area, texture, _buildingShader, height);
                case BuildingType.Tower:
                    return new TowerBuilding(position, area, texture, _buildingShader, height);
                /*  case BuildingType.Blocky:
                      return new BlockyBuilding(position, area, texture, _buildingShader, height); */
                default:
                    throw new NotImplementedException();
            }

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
