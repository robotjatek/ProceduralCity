using System;
using System.Collections.Generic;
using System.Linq;
using ProceduralCity.Generators;
using ProceduralCity.Renderer;
using Serilog;

namespace ProceduralCity
{
    class World : IDisposable, IWorld
    {
        private readonly List<IRenderable> _renderables = new List<IRenderable>();
        private readonly IGroundGenerator _groundGenerator;
        private readonly IBuildingGenerator _buildingGenerator;
        private readonly ILogger _logger;

        public World(IGroundGenerator groundGenerator, IBuildingGenerator buildingGenerator, ILogger logger)
        {
            _logger = logger;
            _groundGenerator = groundGenerator;
            _buildingGenerator = buildingGenerator;

            var sites = _groundGenerator.Generate();
            _logger.Information($"Number of sites: {sites.Count()}");

            var buildings = _buildingGenerator.GenerateBuildings(sites);
            _renderables.AddRange(buildings);
            var billboards = buildings.Where(b => b.HasBillboard).Select(b => b.Billboard);
            _renderables.AddRange(billboards);
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
