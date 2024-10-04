using System.Collections.Generic;

namespace ProceduralCity.Renderer
{
    interface IRenderable
    {
        IReadOnlyCollection<Mesh> Meshes { get; }
    }
}
