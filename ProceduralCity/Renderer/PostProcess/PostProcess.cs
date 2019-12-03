using System;
using OpenTK;
using ProceduralCity.Renderer.Utils;
using Serilog;

namespace ProceduralCity.Renderer.PostProcess
{
    class PostProcess : IDisposable
    {
        private readonly ILogger _logger;
        private readonly IBackBufferRenderer _backbufferRenderer;
        private readonly IRenderer _renderer;
        private readonly Matrix4 _proj = Matrix4.CreateOrthographicOffCenter(-1, 1, -1, 1, -1, 1);

        public PostProcess(ILogger logger, Texture outputTexture)
        {
            _logger = logger;
            _renderer = new Renderer();
            _backbufferRenderer = new BackBufferRenderer(_logger, outputTexture, outputTexture.Width, outputTexture.Height, false);
        }

        public void DoPostProcess(Texture inputTexture, Shader effect)
        {
            //TODO: consider to move everything to the constructor except the _backbufferRenderer.RenderToTexture
            var quad = new FullScreenQuad(inputTexture, effect);
            _renderer.AddToScene(quad);
            _backbufferRenderer.RenderToTexture(_renderer, _proj, Matrix4.Identity, Matrix4.Identity);
            _renderer.Clear();
        }

        public void Resize(int width, int height, float scale)
        {
            _backbufferRenderer.Resize(width, height, scale);
        }

        private bool disposedValue = false;

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    _renderer.Dispose();
                    _backbufferRenderer.Dispose();
                }

                disposedValue = true;
            }
        }

        public void Dispose()
        {
            Dispose(true);
        }
    }
}
