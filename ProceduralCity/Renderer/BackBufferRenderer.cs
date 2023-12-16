using System;
using OpenTK.Graphics.OpenGL;
using OpenTK.Mathematics;

using Serilog;

namespace ProceduralCity.Renderer
{
    class BackBufferRenderer : IBackBufferRenderer
    {
        private int _frameBufferId;
        private int _depthRboId;
        private readonly ILogger _logger;

        public Texture Texture
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

        public bool IsDepthBuffered
        {
            get;
            private set;
        }

        public BackBufferRenderer(ILogger logger, Texture texture, int width, int height, bool useDepthBuffer)
        {
            _logger = logger;
            Texture = texture;
            Width = width;
            Height = height;
            IsDepthBuffered = useDepthBuffer;

            CreateFramebuffer();
        }

        private void CreateFramebuffer()
        {
            _logger.Information("Creating framebuffer");
            Texture.Resize(Width, Height);

            _frameBufferId = GL.GenFramebuffer();
            GL.BindFramebuffer(FramebufferTarget.Framebuffer, _frameBufferId);
            GL.FramebufferTexture2D(FramebufferTarget.Framebuffer, FramebufferAttachment.ColorAttachment0, TextureTarget.Texture2D, Texture.Id, 0);

            if (IsDepthBuffered)
            {
                _depthRboId = GL.GenRenderbuffer();
                GL.BindRenderbuffer(RenderbufferTarget.Renderbuffer, _depthRboId);
                GL.RenderbufferStorage(RenderbufferTarget.Renderbuffer, RenderbufferStorage.DepthComponent, Width, Height);
                GL.FramebufferRenderbuffer(FramebufferTarget.Framebuffer, FramebufferAttachment.DepthAttachment, RenderbufferTarget.Renderbuffer, _depthRboId);
                GL.BindRenderbuffer(RenderbufferTarget.Renderbuffer, 0);
            }

            var frameBufferStatus = GL.CheckFramebufferStatus(FramebufferTarget.Framebuffer);
            if (frameBufferStatus != FramebufferErrorCode.FramebufferComplete)
            {
                _logger.Error("Error while creating framebuffer: {frameBufferStatus}", frameBufferStatus);
            }

            Clear();
            GL.BindFramebuffer(FramebufferTarget.Framebuffer, 0);
        }

        /// <summary>
        /// Uses the passed renderer to render an image to a framebuffer. The result of the rendering is in the <see cref="Texture"/> property
        /// </summary>
        /// <param name="renderer">The used renderer</param>
        /// <param name="projection">The projection matrix used in rendering</param>
        /// <param name="view">The view matrix used in rendering</param>
        /// <remarks>The per-model model matrix is inside the meshes rendered by the <paramref name="renderer"/></remarks>
        public void RenderToTexture(IRenderer renderer, Matrix4 projection, Matrix4 view)
        {
            GL.BindFramebuffer(FramebufferTarget.Framebuffer, _frameBufferId);
            GL.Viewport(0, 0, Width, Height);
            renderer.RenderScene(projection, view);
            Texture.CreateMipmaps();
            GL.BindFramebuffer(FramebufferTarget.Framebuffer, 0);
        }

        /// <summary>
        /// Clears the currently bound framebuffer
        /// </summary>
        public void Clear()
        {
            GL.BindFramebuffer(FramebufferTarget.Framebuffer, _frameBufferId);
            GL.Viewport(0, 0, Width, Height);
            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);
        }

        /// <summary>
        /// Clears the currently bound framebuffer to a given color.
        /// </summary>
        /// <param name="color"></param>
        public void Clear(Color4 color)
        {
            GL.ClearColor(color);
            this.Clear();
        }

        public void Resize(int width, int height, float scale)
        {
            Width = (int)(width * scale);
            Height = (int)(height * scale);

            GL.DeleteRenderbuffer(_depthRboId);
            GL.DeleteFramebuffer(_frameBufferId);
            CreateFramebuffer();
        }

        private bool disposedValue = false;

        protected virtual void Dispose(bool disposing)
        {
            _logger.Information("Disposing framebuffer: {_frameBufferId}", _frameBufferId);
            if (!disposedValue)
            {
                if (disposing)
                {
                }

                GL.DeleteFramebuffer(_frameBufferId);
                GL.DeleteRenderbuffer(_depthRboId);

                disposedValue = true;
            }
        }

        ~BackBufferRenderer()
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
