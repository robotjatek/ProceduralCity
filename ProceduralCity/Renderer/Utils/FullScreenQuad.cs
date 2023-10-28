using System.Collections.Generic;
using ProceduralCity.Utils;

namespace ProceduralCity.Renderer.Utils
{
    class FullScreenQuad : IRenderable
    {
        private readonly List<Mesh> _meshes = new();
        private readonly Shader _shader;
        private readonly IEnumerable<ITexture> _textures;

        public IEnumerable<Mesh> Meshes
        {
            get
            {
                return _meshes;
            }
        }

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
