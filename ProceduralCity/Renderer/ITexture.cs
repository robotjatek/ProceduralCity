using OpenTK.Graphics.OpenGL;
using System;

namespace ProceduralCity.Renderer
{
    interface ITexture : IDisposable
    {
        int Id { get; }

        int Width { get; }

        int Height { get; }

        void Bind(TextureUnit textureUnit);
    }
}