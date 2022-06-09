using System;
using OpenTK.Graphics.OpenGL;
using Serilog;

using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Processing;

namespace ProceduralCity.Renderer
{
    class Texture : ITexture, IDisposable
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
            GL.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.Rgb, width, height, 0, PixelFormat.Rgb, PixelType.UnsignedByte, IntPtr.Zero);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)TextureMagFilter.Nearest);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)TextureMinFilter.Nearest);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapS, (int)TextureWrapMode.ClampToEdge);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapT, (int)TextureWrapMode.ClampToEdge);
        }

        public Texture(string fileName, string defaultFolder = "Textures")
        {
            LoadImage(fileName, defaultFolder);
        }

        private void LoadImage(string fileName, string defaultFolder)
        {
            Id = GL.GenTexture();
            GL.BindTexture(TextureTarget.Texture2D, Id);
            Rgba32 transparent = Color.Transparent;

            using (var image = Image.Load<Rgba32>($"{defaultFolder}/{fileName}"))
            {
                image.Mutate(i => i.Flip(FlipMode.Vertical));
                image.ProcessPixelRows(a =>
                {
                    for (int i = 0; i < image.Height; i++)
                    {
                        var row = a.GetRowSpan(i);
                        foreach(ref var pixel in row)
                        {
                            if(pixel.A == 0)
                            {
                                pixel = Color.Transparent;
                            }
                        }
                    }
                });

                var rawData = new byte[image.Width * image.Height * 4];
                image.CopyPixelDataTo(rawData); 

                GL.TexImage2D(
                        TextureTarget.Texture2D,
                        0,
                        PixelInternalFormat.Rgba,
                        image.Width,
                        image.Height,
                        0,
                        PixelFormat.Rgba,
                        PixelType.UnsignedByte,
                        rawData);
            }

            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapS, (int)TextureWrapMode.Repeat);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapT, (int)TextureWrapMode.Repeat);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)TextureMagFilter.Linear);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)TextureMinFilter.LinearMipmapLinear);

            GL.GetFloat((GetPName)ExtTextureFilterAnisotropic.MaxTextureMaxAnisotropyExt, out float maxAniso);
            GL.TexParameter(TextureTarget.Texture2D, (TextureParameterName)ExtTextureFilterAnisotropic.TextureMaxAnisotropyExt, maxAniso);

            GL.GenerateMipmap(GenerateMipmapTarget.Texture2D);
        }

        public void Bind(TextureUnit textureUnit)
        {
            GL.ActiveTexture(textureUnit);
            GL.BindTexture(TextureTarget.Texture2D, Id);
        }

        public void Resize(int width, int height)
        {
            Width = width;
            Height = height;
            GL.BindTexture(TextureTarget.Texture2D, Id);
            GL.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.Rgba, width, height, 0, PixelFormat.Rgba, PixelType.UnsignedByte, IntPtr.Zero);
        }

        public void CreateMipmaps()
        {
            GL.BindTexture(TextureTarget.Texture2D, Id);
            GL.GenerateMipmap(GenerateMipmapTarget.Texture2D);
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
