using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using OpenTK.Graphics.OpenGL;
using Serilog;

namespace ProceduralCity.Renderer
{
    public class Texture : IDisposable
    {
        public int Id
        {
            get;
            private set;
        }

        public int Width
        {
            get;
            private set;
        }

        public int Height
        {
            get;
            private set;
        }

        public Texture(int width, int height)
        {
            Width = width;
            Height = height;

            Id = GL.GenTexture();
            GL.BindTexture(TextureTarget.Texture2D, Id);
            GL.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.Rgba, width, height, 0, PixelFormat.Rgba, PixelType.UnsignedByte, IntPtr.Zero);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)TextureMagFilter.Linear);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)TextureMinFilter.Linear);
        }

        public Texture(string fileName, string defaultFolder = "Textures")
        {
            LoadImage(fileName, defaultFolder);
        }

        public Texture(List<string> fileNames, string defaultFolder = "Textures")
        {
            if (fileNames.Count == 1)
            {
                LoadImage(fileNames.First(), defaultFolder);
            }
            else if (fileNames.Count == 6)
            {
                LoadCubeMap(fileNames);
            }
            else
            {
                Log.Error("Filenames count is neither 1 nor 6.");
                throw new ArgumentException("Filenames count is neither 1 nor 6.");
            }
        }

        private void LoadCubeMap(List<string> fileNames)
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
                using (var image = new Bitmap($"Textures/{fileNames[i]}"))
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

        private void LoadImage(string fileName, string defaultFolder)
        {
            Id = GL.GenTexture();
            GL.BindTexture(TextureTarget.Texture2D, Id);
            using (var image = new Bitmap($"{defaultFolder}/{fileName}"))
            {
                Width = image.Width;
                Height = image.Height;
                image.MakeTransparent();
                var bitmapData = image.LockBits(
                    new Rectangle(0, 0, image.Width, image.Height),
                    System.Drawing.Imaging.ImageLockMode.ReadOnly,
                    image.PixelFormat);

                GL.TexImage2D(
                    TextureTarget.Texture2D,
                    0,
                    PixelInternalFormat.Rgba,
                    image.Width,
                    image.Height,
                    0,
                    PixelFormat.Bgra,
                    PixelType.UnsignedByte,
                    bitmapData.Scan0);
                image.UnlockBits(bitmapData);

                GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapS, (int)TextureWrapMode.Repeat);
                GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapT, (int)TextureWrapMode.Repeat);
                GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)TextureMagFilter.Linear);
                GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)TextureMinFilter.LinearMipmapLinear);

                GL.GetFloat((GetPName)ExtTextureFilterAnisotropic.MaxTextureMaxAnisotropyExt, out float maxAniso);
                GL.TexParameter(TextureTarget.Texture2D, (TextureParameterName)ExtTextureFilterAnisotropic.TextureMaxAnisotropyExt, maxAniso);

                GL.GenerateMipmap(GenerateMipmapTarget.Texture2D);
            }
        }

        private bool disposedValue = false; // To detect redundant calls

        protected virtual void Dispose(bool disposing)
        {
            Log.Information($"Disposing Texture {Id}");
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
