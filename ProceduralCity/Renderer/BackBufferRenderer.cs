using System;
using OpenTK;
using OpenTK.Graphics.OpenGL;
using Serilog;

namespace ProceduralCity.Renderer
{
    class BackBufferRenderer : IDisposable
    {
        private int _frameBufferId;
        private int _rboId;
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

        public BackBufferRenderer(ILogger logger, int width, int height, bool useDepthBuffer)
        {
            _logger = logger;
            Width = width;
            Height = height;
            IsDepthBuffered = useDepthBuffer;

            CreateFramebuffer();
        }

        private void CreateFramebuffer()
        {
            _logger.Information("Creating framebuffer");
            Texture = new Texture(Width, Height);

            _frameBufferId = GL.GenFramebuffer();
            GL.BindFramebuffer(FramebufferTarget.Framebuffer, _frameBufferId);
            GL.FramebufferTexture2D(FramebufferTarget.Framebuffer, FramebufferAttachment.ColorAttachment0, TextureTarget.Texture2D, Texture.Id, 0);

            if (IsDepthBuffered)
            {
                _rboId = GL.GenRenderbuffer();
                GL.BindRenderbuffer(RenderbufferTarget.Renderbuffer, _rboId);
                GL.RenderbufferStorage(RenderbufferTarget.Renderbuffer, RenderbufferStorage.DepthComponent, Width, Height);
                GL.FramebufferRenderbuffer(FramebufferTarget.Framebuffer, FramebufferAttachment.DepthAttachment, RenderbufferTarget.Renderbuffer, _rboId);
                GL.BindRenderbuffer(RenderbufferTarget.Renderbuffer, 0);
            }

            var frameBufferStatus = GL.CheckFramebufferStatus(FramebufferTarget.Framebuffer);
            if (frameBufferStatus != FramebufferErrorCode.FramebufferComplete)
            {
                _logger.Error($"Error while creating framebuffer: {frameBufferStatus.ToString()}");
            }

            Clear();
            GL.BindFramebuffer(FramebufferTarget.Framebuffer, 0);
        }

        public void RenderToTexture(IRenderer renderer, Matrix4 projection, Matrix4 view, Matrix4 model)
        {
            GL.BindFramebuffer(FramebufferTarget.Framebuffer, _frameBufferId);
            GL.Viewport(0, 0, Width, Height);
            renderer.RenderScene(projection, view, model);
            GL.BindFramebuffer(FramebufferTarget.Framebuffer, 0);
        }

        public void Clear()
        {
            GL.BindFramebuffer(FramebufferTarget.Framebuffer, _frameBufferId);
            GL.Viewport(0, 0, Width, Height);
            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);
        }

        public void Resize(int width, int height)
        {
            Width = width;
            Height = height;

            GL.DeleteFramebuffer(_frameBufferId);
            GL.DeleteRenderbuffer(_rboId);
            Texture.Dispose();
            CreateFramebuffer();
        }

        private bool disposedValue = false;

        protected virtual void Dispose(bool disposing)
        {
            _logger.Information($"Disposing framebuffer: {_frameBufferId}");
            if (!disposedValue)
            {
                if (disposing)
                {
                    Texture.Dispose();
                }

                GL.DeleteFramebuffer(_frameBufferId);
                GL.DeleteRenderbuffer(_rboId);

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
