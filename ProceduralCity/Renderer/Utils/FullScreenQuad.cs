using System.Collections.Generic;
using ProceduralCity.Utils;

namespace ProceduralCity.Renderer.Utils
{
    class FullScreenQuad : IRenderable
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

        public FullScreenQuad(Texture texture, Shader shader)
        {
            _texture = texture;
            _shader = shader;

            _meshes.Add(new Mesh(
                PrimitiveUtils.CreateNDCFullscreenGuiVertices(),
                PrimitiveUtils.CreateNDCFullscreenUVs(),
                _texture,
                _shader));
        }
    }
}
