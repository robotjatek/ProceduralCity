﻿using System;
using System.Collections.Generic;
using System.Linq;
using OpenTK.Mathematics;

using ProceduralCity.Buildings;
using ProceduralCity.Config;
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

        internal static readonly string[] fragmentShaders = ["fs.frag", "fog.frag"];
        private readonly Shader _buildingShader;
        private readonly Vector2 _areaBorder;
        private readonly RandomService _randomService;
        private readonly Texture[] _buildingTextures;
        private readonly ILogger _logger;
        private readonly IAppConfig _config;
        private readonly IBillboardBuilder _billboardBuilder;

        public BuildingGenerator(ILogger logger, IAppConfig config, IBillboardBuilder billboardBuilder, ColorGenerator colorGenerator, RandomService randomService)
        {
            _config = config;
            _randomService = randomService;

            _buildingShader = new Shader("vs.vert", fragmentShaders);
            _buildingShader.SetUniformValue("tex", new IntUniform
            {
                Value = 0
            });

            SetFogColor(colorGenerator.Mixed);
            colorGenerator.OnColorChanged += () => SetFogColor(colorGenerator.Mixed);

            _buildingTextures = _config.BuildingTextures.Select(c => new Texture(c)).ToArray();
            _areaBorder = new Vector2(_config.AreaBorderSize);
            _logger = logger;
            _billboardBuilder = billboardBuilder;
        }

        public IEnumerable<IBuilding> GenerateBuildings(IEnumerable<GroundNode> sites)
        {
            _logger.Information("Generating buildings");
            var buildings = new List<IBuilding>();
            foreach (var site in sites)
            {
                var position = new Vector3(site.StartPosition.X + _areaBorder.X, 0, site.StartPosition.Y + _areaBorder.Y);
                var area = site.EndPosition - site.StartPosition - (_areaBorder * 2);
                var texture = _buildingTextures[_randomService.Next(_buildingTextures.Length)];
                var building = CreateRandomBuilding(position, area, texture);
                buildings.Add(building);
            }

            _logger.Information("Number of buildings: {buildingCount}", buildings.Count);

            return buildings;
        }

        private IBuilding CreateRandomBuilding(Vector3 position, Vector2 area, Texture texture)
        {
            var type = (BuildingType)_randomService.Next(Enum.GetValues(typeof(BuildingType)).Length);
            var height = _randomService.Next(_config.MinBuildingHeight, _config.MaxBuildingHeight);

            return type switch
            {
                BuildingType.Simple => new Building(position, area, texture, _buildingShader, height),
                BuildingType.Tower => new TowerBuilding(position, area, texture, _buildingShader, height, _billboardBuilder, _randomService),
                _ => throw new NotImplementedException(),
            };
        }

        private void SetFogColor(Color4 color)
        {
            _buildingShader.SetUniformValue("fogColor", new Vector3Uniform
            {
                Value = new Vector3(color.R, color.G, color.B)
            });
            _buildingShader.SetUniformValue("fogDensity", new FloatUniform
            {
                Value = 4.0f
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

                    _buildingShader.Dispose();
                    _billboardBuilder.Dispose();
                    Array.ForEach(_buildingTextures, t => t.Dispose());
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
