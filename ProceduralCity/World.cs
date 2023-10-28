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
        private readonly List<IRenderable> _renderables = new();
        private readonly List<TrafficLight> _trafficLights = new();
        private readonly IGroundGenerator _groundGenerator;
        private readonly IBuildingGenerator _buildingGenerator;
        private readonly ILogger _logger;
        private readonly BspTree _bspTree;

        public IEnumerable<TrafficLight> Traffic
        {
            get
            {
                return _trafficLights;
            }
        }

        public BspTree SitesBsp
        {
            get { return _bspTree; }
        }

        public World(IGroundGenerator groundGenerator, IBuildingGenerator buildingGenerator, ILogger logger)
        {
            _logger = logger;
            _groundGenerator = groundGenerator;
            _buildingGenerator = buildingGenerator;

            _bspTree = _groundGenerator.GenerateSites();
            var siteLeafs = _bspTree.GetLeaves();
            _logger.Information("Number of sites: {siteCount}", siteLeafs.Count());
            var groundPlane = _groundGenerator.CreateGroundPlane();
            _renderables.Add(groundPlane);
            var streetLights = _groundGenerator.CreateStreetLights(siteLeafs);
            _renderables.AddRange(streetLights);

            var trafficLights = _groundGenerator.CreateTrafficLights(siteLeafs);
            _trafficLights.AddRange(trafficLights);
            _logger.Information("Number of traffic lights: {trafficLightCount}", trafficLights.Count());

            var buildings = _buildingGenerator.GenerateBuildings(siteLeafs);
            _renderables.AddRange(buildings);
        }

        public IEnumerable<IRenderable> Renderables
        {
            get
            {
                return _renderables;
            }
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
