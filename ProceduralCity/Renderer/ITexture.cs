using System;
using OpenTK.Graphics.OpenGL;

namespace ProceduralCity.Renderer
{
    interface ITexture : IDisposable
    {
        int Id { get; }

        int Width { get; }

        int Height { get; }

        void Bind(TextureUnit textureUnit);

        void Resize(int width, int height);

        void CreateMipmaps();
    }
}