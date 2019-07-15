using System;
using System.Collections.Generic;
using OpenTK;

namespace ProceduralCity.Renderer
{
    public interface IRenderer : IDisposable
    {
        void RenderScene(Matrix4 projection, Matrix4 view, Matrix4 model);

        void AddToScene(IEnumerable<IRenderable> renderables);
    }
}
