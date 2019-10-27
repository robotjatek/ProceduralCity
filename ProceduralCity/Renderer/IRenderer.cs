using System;
using System.Collections.Generic;
using OpenTK;

namespace ProceduralCity.Renderer
{
    interface IRenderer : IDisposable
    {
        Action BeforeRender { get; set; }

        Action AfterRender { get; set; }

        void RenderScene(Matrix4 projection, Matrix4 view, Matrix4 model);

        void AddToScene(IEnumerable<IRenderable> renderables);
    }
}
