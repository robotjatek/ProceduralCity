using System;
using System.Collections.Generic;
using System.Linq;
using OpenTK;
using ProceduralCity.Config;
using ProceduralCity.GameObjects;
using ProceduralCity.Renderer;
using ProceduralCity.Renderer.Uniform;
using ProceduralCity.Utils;
using Serilog;

namespace ProceduralCity.Generators
{
    class GroundGenerator : IGroundGenerator, IDisposable
    {
        private Vector2 _worldSize;
        private readonly Random _random = new Random();
        private readonly ILogger _logger;
        private readonly IAppConfig _config;
        private readonly Shader _planeShader = new Shader("vs.vert", "FlatColored.frag");
        private readonly Shader _lightShader = new Shader("vs.vert", "street_light.frag");
        private readonly List<Vector3> _lightColors = new List<Vector3>()
        {
            new Vector3(1f, 0.82f, 0.698f), //Sodium vapor
            new Vector3(0.847f, 0.969f, 1f), //Mercury Vapor
            new Vector3(0.949f, 0.988f, 1f), //Metal Halide
            new Vector3(1f, 0.718f, 0.298f), //High Pressure Sodium
        };

        public GroundGenerator(IAppConfig config, ILogger logger)
        {
            _config = config;
            _logger = logger;
            _worldSize = new Vector2(config.WorldSize);
        }

        public IRenderable CreateGroundPlane()
        {
            _planeShader.SetUniformValue("u_color", new Vector3Uniform
            {
                Value = new Vector3(0.025f, 0.025f, 0.025f)
            });

            return new GroundPlane(new Vector3(0, 0, 0), new Vector2(_config.WorldSize), _planeShader);
        }

        private readonly Shader _headLightShader = new Shader("instanced.vert", "street_light.frag"); //TODO: maybe one shader only, and set uniforms before render?
        private readonly Shader _rearLightShader = new Shader("instanced.vert", "street_light.frag");

        public IEnumerable<TrafficLight> CreateTrafficLights(IEnumerable<GroundNode> sites)
        {
            _headLightShader.SetUniformValue("u_color", new Vector3Uniform
            {
                Value = new Vector3(1f, 0.945f, 0.878f)
            });
            _rearLightShader.SetUniformValue("u_color", new Vector3Uniform
            {
                Value = new Vector3(1, 0, 0)
            });

            var areaBorder = new Vector2(_config.AreaBorderSize - 10);
            foreach (var site in sites)
            {
                var corners = new[]
                {
                    new Vector3(site.StartPosition.X + areaBorder.X, 0, site.StartPosition.Y + areaBorder.Y),
                    new Vector3(site.EndPosition.X - areaBorder.X, 0, site.StartPosition.Y + areaBorder.Y),
                    new Vector3(site.EndPosition.X - areaBorder.X, 0, site.EndPosition.Y - areaBorder.Y),
                    new Vector3(site.StartPosition.X + areaBorder.X, 0, site.EndPosition.Y - areaBorder.Y)
                };

                var maxSpeed = 10.00f;
                var minSpeed = 5.00f;
                var speed = (float)_random.NextDouble() * (maxSpeed - minSpeed) + minSpeed;

                var firstWaypoint = Waypoint.CreateCircle(corners);
                yield return new TrafficLight(corners.First(), firstWaypoint, _headLightShader, _rearLightShader, speed);
            }
        }

        public IEnumerable<IRenderable> CreateStreetLights(IEnumerable<GroundNode> sites)
        {
            var lightColor = _lightColors[_random.Next(_lightColors.Count)];
            _lightShader.SetUniformValue("u_color", new Vector3Uniform
            {
                Value = lightColor
            });
            _logger.Information($"Set streetlight color to: {lightColor}");

            var areaBorder = new Vector2(_config.AreaBorderSize - 4);
            var lightSize = new Vector2(2, 2);
            foreach (var site in sites)
            {
                var position = new Vector3(site.StartPosition.X + areaBorder.X, 0, site.StartPosition.Y + areaBorder.Y);
                var area = site.EndPosition - site.StartPosition - (areaBorder * 2);

                for (int i = (int)position.X + 2 + 1; i <= position.X + area.X; i += 12)
                {
                    var northPosition = new Vector3(i, position.Y, position.Z);
                    var northVertices = PrimitiveUtils.CreateTopVertices(northPosition, lightSize, 0.2f);
                    var northUvs = PrimitiveUtils.CreateTopUVs(1, 1);

                    yield return new StreetLightStrip(northVertices, northUvs, _lightShader);

                    var southPosition = new Vector3(i, position.Y, position.Z + area.Y + 2);
                    var southVertices = PrimitiveUtils.CreateTopVertices(southPosition, lightSize, 0.2f);
                    var southUvs = PrimitiveUtils.CreateTopUVs(1, 1);

                    yield return new StreetLightStrip(southVertices, southUvs, _lightShader);
                }

                for (int i = (int)position.Z + 2 + 1; i <= position.Z + area.Y; i += 12)
                {
                    var westPosition = new Vector3(position.X, position.Y, i);
                    var westVertices = PrimitiveUtils.CreateTopVertices(westPosition, lightSize, 0.2f);
                    var westUvs = PrimitiveUtils.CreateTopUVs(1, 1);

                    yield return new StreetLightStrip(westVertices, westUvs, _lightShader);

                    var eastPosition = new Vector3(position.X + area.X + 2, position.Y, i);
                    var eastVertices = PrimitiveUtils.CreateTopVertices(eastPosition, lightSize, 0.2f);
                    var eastUvs = PrimitiveUtils.CreateTopUVs(1, 1);

                    yield return new StreetLightStrip(eastVertices, eastUvs, _lightShader);
                }
            }
        }

        public IEnumerable<GroundNode> GenerateSites()
        {
            _logger.Information("Generating ground 2D tree");
            var tree = new BspTree(_worldSize, _config);
            SplitNode(tree.Root, maxLevel: 10);
            return tree.GetLeaves();
        }

        private void SplitNode(GroundNode node, int maxLevel)
        {
            SplitNodes(new[] { node }, 0, maxLevel);
        }

        private void SplitNodes(IEnumerable<GroundNode> nodes, int currentLevel, int maxLevel)
        {
            currentLevel++;
            if (currentLevel >= maxLevel)
            {
                return;
            }

            foreach (var node in nodes)
            {
                SplitNodes(node.Split(_random), currentLevel, maxLevel);
            }
        }

        private bool disposedValue = false; // To detect redundant calls

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    _planeShader.Dispose();
                    _lightShader.Dispose();
                    _headLightShader.Dispose();
                    _rearLightShader.Dispose();
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
