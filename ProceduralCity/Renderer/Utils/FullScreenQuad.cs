using System;
using System.Collections.Generic;
using ProceduralCity.Renderer.Uniform;
using ProceduralCity.Utils;

namespace ProceduralCity.Renderer.Utils
{
    class FullScreenQuad : IRenderable, IDisposable
    {
        private readonly List<Mesh> _meshes = new List<Mesh>();
        private readonly Shader _shader;
        private readonly ITexture _texture;

        public IEnumerable<Mesh> Meshes
        {
            get
            {
                return _meshes;
            }
        }

        public FullScreenQuad(Texture texture)
        {
            _texture = texture;
            _shader = new Shader("vs.vert", "fs.frag");
            _shader.SetUniformValue("tex", new IntUniform
            {
                Value = 0
            });

            _meshes.Add(new Mesh(
                PrimitiveUtils.CreateNDCFullscreenGuiVertices(),
                PrimitiveUtils.CreateNDCFullscreenUVs(),
                _texture,
                _shader));
        }

        public void Dispose()
        {
            _shader.Dispose();
        }
    }
}
