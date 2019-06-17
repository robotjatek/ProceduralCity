using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using OpenTK.Graphics.OpenGL;

namespace ProceduralCity.Renderer
{
    class Texture : IDisposable
    {
        public int Id
        {
            get;
            private set;
        }

        public Texture(string fileName)
        {
            LoadImage(fileName);
        }

        public Texture(List<string> fileNames)
        {
            if (fileNames.Count == 1)
            {
                LoadImage(fileNames.First());
            }
            else if (fileNames.Count == 6)
            {
                LoadCubeMap(fileNames);
            }
            else
            {
                throw new ArgumentException("Filenames count is neither 1 nor 6.");
            }
        }

        private void LoadCubeMap(List<string> fileNames)
        {
            throw new NotImplementedException();
        }

        private void LoadImage(string fileName)
        {
            Id = GL.GenTexture();
            GL.BindTexture(TextureTarget.Texture2D, Id);
            using (var image = new Bitmap(fileName))
            {
                var bitmapData = image.LockBits(
                    new Rectangle(0, 0, image.Width, image.Height),
                    System.Drawing.Imaging.ImageLockMode.ReadOnly,
                    System.Drawing.Imaging.PixelFormat.Format32bppArgb);

                GL.TexImage2D(
                    TextureTarget.Texture2D,
                    0,
                    PixelInternalFormat.Rgb,
                    image.Width,
                    image.Height,
                    0,
                    PixelFormat.Rgb,
                    PixelType.UnsignedByte,
                    bitmapData.Scan0);
                image.UnlockBits(bitmapData);

                GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapS, (int)TextureWrapMode.Repeat);
                GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapT, (int)TextureWrapMode.Repeat);
                GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)TextureMagFilter.Linear);
                GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)TextureMinFilter.LinearMipmapLinear);

                var aniso = GL.GetFloat((GetPName)All.MaxTextureMaxAnisotropy);
                GL.TexParameter(TextureTarget.Texture2D, (TextureParameterName)All.MaxTextureMaxAnisotropyExt, aniso);
                GL.GenerateMipmap(GenerateMipmapTarget.Texture2D);
            }
        }

        private bool disposedValue = false; // To detect redundant calls

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                //if (disposing)
                //{
                //}

                GL.DeleteTexture(Id);

                disposedValue = true;
            }
        }
        ~Texture()
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
