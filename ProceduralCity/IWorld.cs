using System;
using System.Collections.Generic;
using ProceduralCity.Renderer;

namespace ProceduralCity
{
    interface IWorld : IDisposable
    {
        IEnumerable<IRenderable> Renderables { get; }
    }
}