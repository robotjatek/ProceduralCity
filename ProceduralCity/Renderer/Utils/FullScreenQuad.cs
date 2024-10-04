using System.Collections.Generic;
using ProceduralCity.Utils;

namespace ProceduralCity.Renderer.Utils
{
    class FullScreenQuad : IRenderable
    {
        private readonly List<Mesh> _meshes = [];
        private readonly Shader _shader;
        private readonly IEnumerable<ITexture> _textures;

        public IReadOnlyCollection<Mesh> Meshes => _meshes.AsReadOnly();

        public FullScreenQuad(IEnumerable<Texture> textures, Shader shader)
        {
            _textures = textures;
            _shader = shader;

            _meshes.Add(new Mesh(
                PrimitiveUtils.CreateNDCFullscreenGuiVertices(),
                PrimitiveUtils.CreateNDCFullscreenUVs(),
                _textures,
                _shader));
        }
    }
}
