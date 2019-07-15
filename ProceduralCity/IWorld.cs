using System;
using System.Collections.Generic;
using ProceduralCity.Renderer;

namespace ProceduralCity
{
    public interface IWorld : IDisposable
    {
        IEnumerable<IRenderable> Renderables { get; }
    }
}