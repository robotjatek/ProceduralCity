using System;
using System.Collections.Generic;
using System.Linq;
using ProceduralCity.Generators;
using ProceduralCity.Renderer;
using Serilog;

namespace ProceduralCity
{
    class World : IDisposable
    {
        private readonly List<IRenderable> _renderables = new List<IRenderable>();
        private readonly GroundGenerator _groundGenerator;
        private readonly BuildingGenerator _buildingGenerator;

        public World(GroundGenerator groundGenerator, BuildingGenerator buildingGenerator)
        {
            _groundGenerator = groundGenerator;
            _buildingGenerator = buildingGenerator;

            var sites = _groundGenerator.Generate();
            Log.Information($"Number of sites: {sites.Count()}");

            var buildings = _buildingGenerator.GenerateBuildings(sites);
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
