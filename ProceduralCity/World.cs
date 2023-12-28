using System;
using System.Collections.Generic;
using System.Linq;
using ProceduralCity.GameObjects;
using ProceduralCity.Generators;
using ProceduralCity.Renderer;
using Serilog;

namespace ProceduralCity
{
    class World : IDisposable, IWorld
    {
        private readonly List<IRenderable> _renderables = [];
        private readonly List<TrafficLight> _trafficLights = [];
        private readonly IGroundGenerator _groundGenerator;
        private readonly IBuildingGenerator _buildingGenerator;
        private readonly ILogger _logger;
        private readonly GroundNodeTree _groundNodeTree;

        public GroundNodeTree BspTree => _groundNodeTree;

        public IEnumerable<TrafficLight> Traffic => _trafficLights;

        public IEnumerable<IRenderable> Renderables => _renderables;

        public World(IGroundGenerator groundGenerator, IBuildingGenerator buildingGenerator, ILogger logger)
        {
            _logger = logger;
            _groundGenerator = groundGenerator;
            _buildingGenerator = buildingGenerator;

            _groundNodeTree = _groundGenerator.GenerateSites();
            var sites = _groundNodeTree.GetLeaves();
            _logger.Information("Number of sites: {siteCount}", sites.Count());
            var groundPlane = _groundGenerator.CreateGroundPlane();
            _renderables.Add(groundPlane);
            var streetLights = _groundGenerator.CreateStreetLights(sites);
            _renderables.AddRange(streetLights);

            var trafficLights = _groundGenerator.CreateTrafficLights(sites);
            _trafficLights.AddRange(trafficLights);
            _logger.Information("Number of traffic lights: {trafficLightCount}", trafficLights.Count());

            var buildings = _buildingGenerator.GenerateBuildings(sites);
            _renderables.AddRange(buildings);
        }

        private bool disposedValue = false;

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    _buildingGenerator.Dispose();
                    _groundGenerator.Dispose();
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
