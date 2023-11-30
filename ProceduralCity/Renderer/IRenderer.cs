using System;
using System.Collections.Generic;
using OpenTK.Mathematics;

namespace ProceduralCity.Renderer
{
    interface IRenderer : IDisposable
    {
        Action BeforeRender { get; set; }

        Action AfterRender { get; set; }

        void RenderScene(Matrix4 projection, Matrix4 view);

        void AddToScene(IEnumerable<IRenderable> renderables);

        void AddToScene(IRenderable renderable);

        void Clear();
    }
}
