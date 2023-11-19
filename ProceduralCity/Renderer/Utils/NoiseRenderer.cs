using System;
using OpenTK.Mathematics;

using Serilog;

namespace ProceduralCity.Renderer.Utils
{
    class NoiseRenderer : IDisposable
    {
        private readonly Renderer _renderer;
        private readonly BackBufferRenderer _backbufferRenderer;
        private readonly Shader _shader = new("vs.vert", "Noise/Noise.frag");
        private readonly Texture _texture;

        public NoiseRenderer(ILogger logger, Texture texture)
        {
            _texture = texture;
            _renderer = new Renderer();
            _backbufferRenderer = new BackBufferRenderer(logger, _texture, _texture.Width, _texture.Height, false);
            var fullscreenQuad = new FullScreenQuad(null, _shader);
            _renderer.AddToScene(fullscreenQuad);
        }

        public void Render()
        {
            var proj = Matrix4.CreateOrthographicOffCenter(-1, 1, -1, 1, -1, 1);
            _backbufferRenderer.Clear();
            _backbufferRenderer.RenderToTexture(_renderer, proj, Matrix4.Identity);
        }

        private bool disposedValue = false;

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    _shader.Dispose();
                    _backbufferRenderer.Dispose();
                    _renderer.Dispose();
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
