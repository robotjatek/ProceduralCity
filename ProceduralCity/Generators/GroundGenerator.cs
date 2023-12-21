using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

using OpenTK.Mathematics;

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
        private readonly RandomService _randomService;
        private readonly ILogger _logger;
        private readonly IAppConfig _config;
        private readonly Shader _planeShader = new("vs.vert", new string[] { "FlatColored.frag", "fog.frag" });
        // "Light sources" like traffic lights, street ligths or billboards are not affected by the fog by design. (For now at least...)
        private readonly Shader _lightShader = new("vs.vert", "street_light.frag");
        private readonly ColorGenerator _colorGenerator;
        private readonly List<Vector3> _lightColors =
        [
            new Vector3(1f, 0.82f, 0.698f), //Sodium vapor
            new Vector3(0.847f, 0.969f, 1f), //Mercury Vapor
            new Vector3(0.949f, 0.988f, 1f), //Metal Halide
            new Vector3(1f, 0.718f, 0.298f), //High Pressure Sodium
        ];

        public GroundGenerator(IAppConfig config, ILogger logger, ColorGenerator colorGenerator, RandomService randomService)
        {
            _config = config;
            _logger = logger;
            _randomService = randomService;
            _colorGenerator = colorGenerator;
            _worldSize = new Vector2(config.WorldSize);

            _planeShader.SetUniformValue("u_color", new Vector3Uniform
            {
                Value = new Vector3(0.025f, 0.025f, 0.025f)
            });
            SetFogColor(_colorGenerator.Mixed);
            colorGenerator.OnColorChanged += () => SetFogColor(colorGenerator.Mixed);
        }

        public IRenderable CreateGroundPlane()
        {
            return new GroundPlane(new Vector3(0, 0, 0), new Vector2(_config.WorldSize), _planeShader);
        }

        private void SetFogColor(Color4 color)
        {
            _planeShader.SetUniformValue("fogColor", new Vector3Uniform
            {
                Value = new Vector3(color.R, color.G, color.B)
            });
            _planeShader.SetUniformValue("fogDensity", new FloatUniform
            {
                Value = 4.0f
            });
        }

        private readonly Shader _headLightShader = new("instanced.vert", "street_light.frag"); //TODO: maybe one shader only, and set uniforms before render?
        private readonly Shader _rearLightShader = new("instanced.vert", "street_light.frag"); //TODO: do not use the street_light shader

        public IEnumerable<TrafficLight> CreateTrafficLights(IEnumerable<GroundNode> sites)
        {
            var traffic = new List<TrafficLight>();
            _headLightShader.SetUniformValue("u_color", new Vector3Uniform
            {
                Value = new Vector3(1f, 0.945f, 0.878f)
            });
            _rearLightShader.SetUniformValue("u_color", new Vector3Uniform
            {
                Value = new Vector3(0.9f, 0.2f, 0.1f)
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

                var firstWaypoint = Waypoint.CreateCircle(corners);
                var trafficOnCircuit = CreateTrafficOnCircuit(5, firstWaypoint);
                traffic.AddRange(trafficOnCircuit);
                site.AddTrafficLights(trafficOnCircuit);
            }

            return traffic;
        }

        private ReadOnlyCollection<TrafficLight> CreateTrafficOnCircuit(int maxPerSegment, Waypoint firstWaypoint)
        {
            var traffic = new List<TrafficLight>();
            var current = firstWaypoint;

            do
            {
                var target = current.Next;
                traffic.AddRange(CreateTrafficOnSegment(current, target, maxPerSegment));

                current = current.Next;
            } while (current != firstWaypoint);

            return traffic.AsReadOnly();
        }

        private IEnumerable<TrafficLight> CreateTrafficOnSegment(Waypoint start, Waypoint finish, int maxPerSegment)
        {
            for (int i = 0; i < maxPerSegment; i++)
            {
                var maxSpeed = 10.00f;
                var minSpeed = 5.00f;
                var speed = (float)_randomService.NextDouble() * (maxSpeed - minSpeed) + minSpeed;

                var x = (float)_randomService.NextDouble() * (finish.Position.X - start.Position.X) + start.Position.X;
                var y = (float)_randomService.NextDouble() * (finish.Position.Y - start.Position.Y) + start.Position.Y;
                var z = (float)_randomService.NextDouble() * (finish.Position.Z - start.Position.Z) + start.Position.Z;

                var position = new Vector3(x, y, z);
                yield return new TrafficLight(position, finish, _headLightShader, _rearLightShader, speed);
            }
        }

        public IEnumerable<IRenderable> CreateStreetLights(IEnumerable<GroundNode> sites)
        {
            var lightColor = _lightColors[_randomService.Next(_lightColors.Count)];
            _lightShader.SetUniformValue("u_color", new Vector3Uniform
            {
                Value = lightColor
            });            
            _logger.Information("Set streetlight color to: {lightColor}", lightColor);

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

        public GroundNodeTree GenerateSites()
        {
            _logger.Information("Generating ground 2D tree");
            var tree = new GroundNodeTree(_worldSize, _config);
            SplitNode(tree.Root, maxLevel: 10);
            return tree;
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
                SplitNodes(node.Split(_randomService), currentLevel, maxLevel);
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
