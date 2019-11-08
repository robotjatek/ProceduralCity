using System.Collections.Generic;
using OpenTK;

namespace ProceduralCity.Renderer
{
    interface IRenderable
    {
        IEnumerable<Vector3> Vertices { get; }
        IEnumerable<Vector2> UVs { get; }
        ITexture Texture { get; }
        Shader Shader { get; }
    }
}
