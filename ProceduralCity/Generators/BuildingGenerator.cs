using System;
using System.Collections.Generic;
using OpenTK;
using ProceduralCity.Renderer;
using Serilog;

namespace ProceduralCity.Generators
{
    class BuildingGenerator : IDisposable
    {
        private readonly Shader _buildingShader;
        private readonly Texture _buildingTexture;
        private readonly Vector2 _areaBorder = new Vector2(3.5f, 3.5f);

        public BuildingGenerator()
        {
            _buildingShader = new Shader("vs.vert", "fs.frag");
            _buildingTexture = new Texture("building/1.jpg");
        }

        public IEnumerable<Building> GenerateBuildings(IEnumerable<GroundNode> sites)
        {
            Log.Information("Generating buildings");

            var buildings = new List<Building>();
            foreach (var site in sites)
            {
                var position = new Vector3(site.StartPosition.X, 0, site.StartPosition.Y);
                var area = site.EndPosition - site.StartPosition - _areaBorder;

                var building = new Building(position, area, _buildingTexture, _buildingShader);
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
                    _buildingTexture.Dispose();
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
