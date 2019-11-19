using System;
using System.Linq;
using OpenTK;
using ProceduralCity.Config;
using ProceduralCity.Renderer;
using ProceduralCity.Utils;

namespace ProceduralCity.Generators
{
    class BillboardBuilder : IBillboardBuilder, IDisposable
    {
        //TODO: do not add billboards to small buildings
        //TODO: generate billboard coordinates in model space, then transform them with a model matrix
        private readonly Random _random = new Random();
        private readonly Texture[] _billboardTextures;
        private readonly IAppConfig _config;
        private readonly Shader _shader = new Shader("vs.vert", "fs.frag");

        public BillboardBuilder(IAppConfig config, IBillboardTextureGenerator billboardTextureGenerator)
        {
            _config = config;
            _billboardTextures = new Texture[_config.BillboardTextureNumber];
            _billboardTextures = Enumerable.Range(0, _config.BillboardTextureNumber)
                .Select(n => new Texture(_config.BillboardTextureWidth, _config.BillboardTextureHeight)).ToArray();
            billboardTextureGenerator.GenerateBillboardTextures(_billboardTextures);
        }

        public float CalculateBillboardWidth(float height)
        {
            return (_config.BillboardTextureWidth / _config.BillboardTextureHeight) * height;
        }

        public Billboard CreateNorthFacingBillboard(Vector3 position, Vector2 area, float height)
        {
            var texture = _billboardTextures.ElementAt(_random.Next(_billboardTextures.Length));
            var width = CalculateBillboardWidth(height);
            area.X = width;
            position.Z += 0.05f;

            return new Billboard(
                texture,
                _shader,
                PrimitiveUtils.CreateFrontVertices(position, area, height),
                PrimitiveUtils.CreateFrontUvs());
        }

        public Billboard CreateSouthFacingBillboard(Vector3 position, Vector2 area, float height)
        {
            var texture = _billboardTextures.ElementAt(_random.Next(_billboardTextures.Length));
            var width = CalculateBillboardWidth(height);
            position.X = position.X + area.X - width; //compensate position because of the worldspace coordinates
            area.X = width;
            position.Z -= 0.05f;

            return new Billboard(
                texture,
                _shader,
                PrimitiveUtils.CreateBacksideVertices(position, area, height),
                PrimitiveUtils.CreateBackUVs());
        }

        public Billboard CreateWestFacingBillboard(Vector3 position, Vector2 area, float height)
        {
            var texture = _billboardTextures.ElementAt(_random.Next(_billboardTextures.Length));
            var width = CalculateBillboardWidth(height);
            area.Y = width;
            position.X -= 0.05f;

            return new Billboard(
                texture,
                _shader,
                PrimitiveUtils.CreateLeftVertices(position, area, height),
                PrimitiveUtils.CreateLeftUVs());
        }

        public Billboard CreateEastFacingBillboard(Vector3 position, Vector2 area, float height)
        {
            var texture = _billboardTextures.ElementAt(_random.Next(_billboardTextures.Length));
            var width = CalculateBillboardWidth(height);
            position.Z = position.Z + area.Y - width; //compensate position because of the worldspace coordinates
            area.Y = width;
            position.X += 1.05f;

            return new Billboard(
                texture,
                _shader,
                PrimitiveUtils.CreateRightVertices(position, area, height),
                PrimitiveUtils.CreateRightUVs());
        }

        private bool disposedValue = false;

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    Array.ForEach(_billboardTextures, t => t.Dispose());
                    _shader.Dispose();
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
