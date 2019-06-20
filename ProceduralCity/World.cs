using System.Collections.Generic;
using System.Linq;
using OpenTK;
using ProceduralCity.Generators;
using ProceduralCity.Renderer;
using Serilog;

namespace ProceduralCity
{
    class World
    {
        private readonly List<IRenderable> _renderables = new List<IRenderable>();

        public IEnumerable<IRenderable> Renderables
        {
            get
            {
                return _renderables;
            }
        }

        public World()
        {
            var groundGenearator = new GroundGenerator(new Vector2(2048, 2048));
            var sites = groundGenearator.Generate();
            Log.Information($"Number of sites: {sites.ToArray().Length}");
        }
    }
}
