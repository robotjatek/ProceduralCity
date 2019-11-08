using System;
using System.Collections.Generic;
using OpenTK.Graphics.OpenGL;
using System.Drawing;
using Serilog;

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
                using (var image = new Bitmap($"{defaultFolder}/{fileNames[i]}"))
                {
                    var bitmapData = image.LockBits(
                    new Rectangle(0, 0, image.Width, image.Height),
                    System.Drawing.Imaging.ImageLockMode.ReadOnly,
                    image.PixelFormat);

                    GL.TexImage2D(TextureTarget.TextureCubeMapPositiveX + i, 0, PixelInternalFormat.Rgb, image.Width, image.Height, 0, PixelFormat.Rgb, PixelType.UnsignedByte, bitmapData.Scan0);

                    image.UnlockBits(bitmapData);
                }
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
    }
}
