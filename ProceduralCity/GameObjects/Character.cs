using System.Collections.Generic;
using OpenTK;
using ProceduralCity.Config;
using ProceduralCity.Renderer;
using ProceduralCity.Utils;

namespace ProceduralCity.GameObjects
{
    class Character : IRenderable
    {
        private readonly List<Mesh> _meshes = new List<Mesh>();
        private readonly ITexture _texture;
        private readonly Shader _shader;

        public IEnumerable<Mesh> Meshes
        {
            get
            {
                return _meshes;
            }
        }

        public float Advance
        {
            get;
            private set;
        }

        public Character(Shader shader, Texture texture, CharProperties charConfig, Vector2 originPosition, float scale)
        {
            _texture = texture;
            _shader = shader;
            Advance = charConfig.Advance * scale;

            var height = charConfig.Height * scale;
            var width = charConfig.Width * scale;
            var originX = charConfig.OriginX * scale;
            var originY = charConfig.OriginY * scale;

            var originYOffset = height - originY;
            var bottomY = originPosition.Y + originYOffset;
            var topY = bottomY - height;
            var left = originPosition.X - originX;

            var vertices = PrimitiveUtils.CreateSpriteVertices(new Vector2(left, topY), width, height);

            var s = (float)charConfig.X / texture.Width;
            var t = (float)charConfig.Y / texture.Height;
            var uvs = PrimitiveUtils.CreateSpriteUVs(new Vector2(s, t), (float)charConfig.Width / texture.Width, (float)charConfig.Height / texture.Height);

            _meshes.Add(new Mesh(vertices, uvs, new[] { _texture }, _shader));
        }
    }
}
