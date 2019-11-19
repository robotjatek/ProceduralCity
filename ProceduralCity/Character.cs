using System.Collections.Generic;
using OpenTK;
using ProceduralCity.Config;
using ProceduralCity.Renderer;
using ProceduralCity.Utils;

namespace ProceduralCity
{
    class Character : IRenderable
    {
        public IEnumerable<Vector3> Vertices { get; private set; }

        public IEnumerable<Vector2> UVs { get; private set; }

        public ITexture Texture { get; private set; }

        public Shader Shader { get; private set; }

        public float Advance
        {
            get;
            private set;
        }

        public Character(Shader shader, Texture texture, CharProperties charConfig, Vector2 originPosition, float scale)
        {
            Texture = texture;
            Shader = shader;
            Advance = charConfig.Advance * scale;

            var height = charConfig.Height * scale;
            var width = charConfig.Width * scale;
            var originX = charConfig.OriginX * scale;
            var originY = charConfig.OriginY * scale;

            var originYOffset = height - originY;
            var bottomY = originPosition.Y + originYOffset;
            var topY = bottomY - height;
            var left = originPosition.X - originX;

            Vertices = PrimitiveUtils.CreateSpriteVertices(new Vector2(left, topY), width, height);

            var s = (float)charConfig.X / texture.Width;
            var t = (float)charConfig.Y / texture.Height;
            UVs = PrimitiveUtils.CreateSpriteUVs(new Vector2(s, t), (float)charConfig.Width / texture.Width, (float)charConfig.Height / texture.Height);
        }
    }
}
