using System;
using System.Collections.Generic;
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
        private readonly Shader _effect;
        private readonly Matrix4 _proj = Matrix4.CreateOrthographicOffCenter(-1, 1, -1, 1, -1, 1);
        private readonly IEnumerable<Texture> _inputTextures;

        public PostProcess(ILogger logger, IEnumerable<Texture> inputTextures, Texture outputTexture, Shader effect)
        {
            _logger = logger;
            _inputTextures = inputTextures;
            _effect = effect;
            _renderer = new Renderer();
            _backbufferRenderer = new BackBufferRenderer(_logger, outputTexture, outputTexture.Width, outputTexture.Height, false);
        }

        public void DoPostProcess()
        {
            //TODO: consider to move everything to the constructor except the _backbufferRenderer.RenderToTexture
            var quad = new FullScreenQuad(_inputTextures, _effect);
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
