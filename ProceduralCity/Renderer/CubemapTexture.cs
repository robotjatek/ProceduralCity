using System;
using System.Collections.Generic;
using System.IO;

using OpenTK.Graphics.OpenGL;

using Serilog;

using StbImageSharp;

namespace ProceduralCity.Renderer
{
    class CubemapTexture : ITexture
    {
        public int Id { get; private set; }
        public int Width { get; private set; }
        public int Height { get; private set; }

        public CubemapTexture(List<string> fileNames, string defaultFolder = "Textures")
        {
            if (fileNames.Count == 6)
            {
                LoadCubeMap(fileNames, defaultFolder);
            }
            else
            {
                Log.Error("Filenames count is not 6.");
                throw new ArgumentException("Filenames count must be 6.");
            }
        }

        public void Bind(TextureUnit textureUnit)
        {
            GL.ActiveTexture(textureUnit);
            GL.BindTexture(TextureTarget.TextureCubeMap, Id);
        }

        public void CreateMipmaps()
        {
            GL.BindTexture(TextureTarget.TextureCubeMap, Id);
            GL.GenerateMipmap(GenerateMipmapTarget.TextureCubeMap);
        }

        private void LoadCubeMap(List<string> fileNames, string defaultFolder)
        {
            Id = GL.GenTexture();
            GL.BindTexture(TextureTarget.TextureCubeMap, Id);
            GL.TexParameter(TextureTarget.TextureCubeMap, TextureParameterName.TextureMagFilter, (int)TextureMagFilter.Linear);
            GL.TexParameter(TextureTarget.TextureCubeMap, TextureParameterName.TextureMinFilter, (int)TextureMinFilter.Linear);
            GL.TexParameter(TextureTarget.TextureCubeMap, TextureParameterName.TextureWrapS, (int)TextureWrapMode.ClampToEdge);
            GL.TexParameter(TextureTarget.TextureCubeMap, TextureParameterName.TextureWrapT, (int)TextureWrapMode.ClampToEdge);
            GL.TexParameter(TextureTarget.TextureCubeMap, TextureParameterName.TextureWrapR, (int)TextureWrapMode.ClampToEdge);
            GL.TexParameter(TextureTarget.TextureCubeMap, TextureParameterName.TextureBaseLevel, 0);
            GL.TexParameter(TextureTarget.TextureCubeMap, TextureParameterName.TextureMaxLevel, 0);

            for (var i = 0; i < fileNames.Count; i++)
            {
                var image = ImageResult.FromStream(File.OpenRead($"{defaultFolder}/{fileNames[i]}"), ColorComponents.RedGreenBlueAlpha);
                GL.TexImage2D(TextureTarget.TextureCubeMapPositiveX + i, 0, PixelInternalFormat.Rgba, image.Width, image.Height, 0, PixelFormat.Rgba, PixelType.UnsignedByte, image.Data);
            }
        }

        private bool disposedValue = false;

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                }

                GL.DeleteTexture(Id);

                disposedValue = true;
            }
        }

        ~CubemapTexture()
        {
            Dispose(false);
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        public void Resize(int width, int height)
        {
            throw new NotImplementedException();
        }
    }
}
