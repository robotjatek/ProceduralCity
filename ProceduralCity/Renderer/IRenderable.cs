using System.Collections.Generic;

namespace ProceduralCity.Renderer
{
    interface IRenderable
    {
        IEnumerable<Mesh> Meshes { get; }
    }
}
