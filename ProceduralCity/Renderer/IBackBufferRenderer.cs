using System;
using OpenTK.Mathematics;

namespace ProceduralCity.Renderer
{
    interface IBackBufferRenderer : IDisposable
    {
        int Height { get; }
        bool IsDepthBuffered { get; }
        Texture Texture { get; }
        int Width { get; }

        void Clear();
        void Clear(Color4 color);
        void RenderToTexture(IRenderer renderer, Matrix4 projection, Matrix4 view);
        void Resize(int width, int height, float scale);
    }
}