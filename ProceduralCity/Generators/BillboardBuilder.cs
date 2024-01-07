using System;
using System.Linq;
using OpenTK.Mathematics;

using ProceduralCity.Config;
using ProceduralCity.GameObjects;
using ProceduralCity.Renderer;
using ProceduralCity.Renderer.Uniform;
using ProceduralCity.Utils;

namespace ProceduralCity.Generators
{
    class BillboardBuilder : IBillboardBuilder, IDisposable
    {
        //TODO: generate billboard coordinates in model space, then transform them with a model matrix
        private readonly Texture[] _billboardTextures;
        private readonly IAppConfig _config;
        private readonly Shader _shader = new("vs.vert", new[] { "billboard.frag", "colorTools.frag" });
        private readonly ColorGenerator _colorGenerator;
        private readonly RandomService _randomService;

        public BillboardBuilder(IAppConfig config, IBillboardTextureGenerator billboardTextureGenerator, ColorGenerator colorGenerator, RandomService randomService)
        {
            _config = config;
            _randomService = randomService;
            _colorGenerator = colorGenerator;
            _colorGenerator.OnColorChanged += () =>
            {
                SetBillboardColor(CreateBillboardColor());
            };
            SetBillboardColor(CreateBillboardColor());

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
            var texture = _billboardTextures.ElementAt(_randomService.Next(_billboardTextures.Length));
            var width = CalculateBillboardWidth(height);
            area.X = width;
            position.Z += 0.05f;

            return new Billboard(
                texture,
                _shader,
                PrimitiveUtils.CreateFrontVertices(position, area, height),
                PrimitiveUtils.CreateFrontUvs(new Vector2(0), 1, 1));
        }

        public Billboard CreateSouthFacingBillboard(Vector3 position, Vector2 area, float height)
        {
            var texture = _billboardTextures.ElementAt(_randomService.Next(_billboardTextures.Length));
            var width = CalculateBillboardWidth(height);
            position.X = position.X + area.X - width; //compensate position because of the worldspace coordinates
            area.X = width;
            position.Z -= 0.05f;

            return new Billboard(
                texture,
                _shader,
                PrimitiveUtils.CreateBacksideVertices(position, area, height),
                PrimitiveUtils.CreateBackUVs(new Vector2(), 1, 1));
        }

        public Billboard CreateWestFacingBillboard(Vector3 position, Vector2 area, float height)
        {
            var texture = _billboardTextures.ElementAt(_randomService.Next(_billboardTextures.Length));
            var width = CalculateBillboardWidth(height);
            area.Y = width;
            position.X -= 0.05f;

            return new Billboard(
                texture,
                _shader,
                PrimitiveUtils.CreateLeftVertices(position, area, height),
                PrimitiveUtils.CreateLeftUVs(1, 1));
        }

        public Billboard CreateEastFacingBillboard(Vector3 position, Vector2 area, float height)
        {
            var texture = _billboardTextures.ElementAt(_randomService.Next(_billboardTextures.Length));
            var width = CalculateBillboardWidth(height);
            position.Z = position.Z + area.Y - width; //compensate position because of the worldspace coordinates
            area.Y = width;
            position.X += 0.05f;

            return new Billboard(
                texture,
                _shader,
                PrimitiveUtils.CreateRightVertices(position, area, height),
                PrimitiveUtils.CreateRightUVs(1, 1));
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

        public bool HasBillboards()
        {
            return _billboardTextures.Length > 0;
        }

        private Vector4 CreateBillboardColor()
        {
            var billboardColorHsv = Color4.ToHsv(_colorGenerator.Mixed);
            billboardColorHsv.Y = 0.2f; // Saturation
            billboardColorHsv.Z = 0.85f; // Light
            return billboardColorHsv;
        }

        private void SetBillboardColor(Vector4 billboardColorHsv)
        {
            _shader.SetUniformValue("billboardColor", new Vector3Uniform
            {
                Value = billboardColorHsv.Xyz
            });
        }
    }
}
