using System;
using System.Collections.Generic;
using System.Linq;

using OpenTK.Mathematics;

using ProceduralCity.Buildings;
using ProceduralCity.Config;
using ProceduralCity.Extensions;
using ProceduralCity.Renderer;
using ProceduralCity.Renderer.Uniform;
using ProceduralCity.Utils;

using Serilog;

namespace ProceduralCity.Generators
{
    class BuildingGenerator : IBuildingGenerator
    {
        enum BuildingType
        {
            Simple,
            Tower,
            //  Blocky
        }

        internal static readonly string[] fragmentShaders = ["building.frag", "fog.frag"];
        private readonly Shader[] _buildingShaders;
        private readonly Vector2 _areaBorder;
        private readonly RandomService _randomService;
        private readonly ILogger _logger;
        private readonly IAppConfig _config;
        private readonly IBillboardBuilder _billboardBuilder;
        private readonly BuildingTextureInfo _buildingTexture;

        private static readonly Vector3[] buildingColors = 
            [
                new Vector3(1f, 0.82f, 0.698f), //Sodium vapor
                new Vector3(0.847f, 0.969f, 1f), //Mercury Vapor
                new Vector3(0.949f, 0.988f, 1f), //Metal Halide
                new Vector3(252 / 256f, 237 / 256f, 206 / 256f),
                new Vector3(229 / 256f, 255 / 256f, 226 / 256f),
            ];

        public BuildingGenerator(
            ILogger logger,
            IAppConfig config,
            IBillboardBuilder billboardBuilder,
            ColorGenerator colorGenerator,
            RandomService randomService,
            BuildingTextureGenerator buildingTextureGenerator)
        {
            _config = config;
            _randomService = randomService;

            // TODO: implement material system: instead of changeing shader objects only uniforms should be updated
            _buildingShaders = buildingColors.Select(color =>
            {
                var shader = new Shader("vs.vert", fragmentShaders);

                shader.SetUniformValue("tex", new IntUniform
                {
                    Value = 0
                });

                shader.SetUniformValue("buildingColor", new Vector3Uniform
                {
                    Value = color
                });

                return shader;
            }).ToArray();

            SetFogColor(colorGenerator.Mixed);
            colorGenerator.OnColorChanged += () => SetFogColor(colorGenerator.Mixed);

            _areaBorder = new Vector2(_config.AreaBorderSize);
            _logger = logger;
            _billboardBuilder = billboardBuilder;
            _buildingTexture = buildingTextureGenerator.GenerateTexture();
        }

        public IEnumerable<IBuilding> GenerateBuildings(IEnumerable<GroundNode> sites)
        {
            _logger.Information("Generating buildings");
            var buildings = new List<IBuilding>();
            foreach (var site in sites)
            {
                var position = new Vector3(site.StartPosition.X + _areaBorder.X, 0, site.StartPosition.Y + _areaBorder.Y);
                var area = site.EndPosition - site.StartPosition - (_areaBorder * 2);
                var shader = _buildingShaders[_randomService.Next(0, _buildingShaders.Length)];
                var building = CreateRandomBuilding(position, area, _buildingTexture, shader);
                buildings.Add(building);
            }

            _logger.Information("Number of buildings: {buildingCount}", buildings.Count);

            return buildings;
        }

        private IBuilding CreateRandomBuilding(Vector3 position, Vector2 area, BuildingTextureInfo texture, Shader shader)
        {
            var type = (BuildingType)_randomService.Next(Enum.GetValues(typeof(BuildingType)).Length);
            var height = _randomService.Next(_config.MinBuildingHeight, _config.MaxBuildingHeight);

            return type switch
            {
                BuildingType.Simple => new Building(position, area, texture, shader, height, _randomService),
                BuildingType.Tower => new TowerBuilding(position, area, texture, shader, height, _billboardBuilder, _randomService),
                _ => throw new NotImplementedException(),
            };
        }

        // TODO: put fogcolor into an UBO
        private void SetFogColor(Color4 color)
        {
            _buildingShaders.ForEach(shader =>
            {
                shader.SetUniformValue("fogColor", new Vector3Uniform
                {
                    Value = new Vector3(color.R, color.G, color.B)
                });
                shader.SetUniformValue("fogDensity", new FloatUniform
                {
                    Value = 4.0f
                });
            });
        }

        private bool disposedValue = false;

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    _logger.Information("Disposing building generator");

                    _buildingShaders.ForEach(shader => shader.Dispose());
                    _buildingTexture.Texture.Dispose();
                    _billboardBuilder.Dispose();
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
