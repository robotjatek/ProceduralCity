using System;
using System.Collections.Generic;
using OpenTK;

namespace ProceduralCity.Renderer
{
    interface IRenderable : IDisposable
    {
        IEnumerable<Vector3> GetVertices();
        IEnumerable<Vector2> GetUVs();
        Texture GetTexture();
        Shader GetShader();
    }
}
